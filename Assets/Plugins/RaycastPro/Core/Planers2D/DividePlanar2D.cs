namespace RaycastPro.Planers2D
{
    using System.Collections.Generic;
    using RaySensors2D;
    using UnityEngine;

#if UNITY_EDITOR
    using System.Linq;
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Planers/" + nameof(DividePlanar2D))]
    abstract class DividePlanar2D : Planar2D
    {
        public float arcAngle = 0f;

        public int count = 5;

        // Temp Disable and no Supporting Feature..
        // public int Count
        // {
        //     get => count;
        //     set
        //     {
        //         if (value <= 0) return;
        //         if (value == count) return;
        //         count = value;
        //         foreach (var c in CloneProfile.Keys)
        //         {
        //             c.enabled = true;
        //             c.liner.enabled = true;
        //             foreach (var r in CloneProfile[c]) RaySensor2D.CloneDestroy(r);
        //             CloneProfile.Remove(c);
        //             AddDividedClonesToBaseClone(c);
        //         }
        //     }
        // }

        public readonly Dictionary<RaySensor2D, List<RaySensor2D>> CloneProfile =
            new Dictionary<RaySensor2D, List<RaySensor2D>>();
        public override void OnReceiveRay(RaySensor2D sensor)
        {
            if (!sensor.cloneRaySensor) return;

            var clone = sensor.cloneRaySensor;

            var forward = GetForward(sensor, transform.right);

            var point = sensor.hit.point;

            clone.transform.position = point;
            
            clone.transform.right = forward;

            ApplyLengthControl(sensor);

            if (!CloneProfile.ContainsKey(clone)) return;

            var cloneCount = CloneProfile[clone].Count;

            var step = arcAngle / cloneCount;

            var directions = new List<Vector2> {forward};

            for (var i = 1; i <= cloneCount / 2; i++)
            {
                directions.Add(Quaternion.AngleAxis(step * i, Vector3.forward) * forward);
            }

            for (var i = -1; i >= -cloneCount / 2; i--)
            {
                directions.Add(Quaternion.AngleAxis(step * i, Vector3.forward) * forward);
            }

            CloneProfile[clone].ForEach(c =>
            {
                c.transform.position = point;

                c.direction = clone.direction;
            });

            for (var i = 0; i < cloneCount; i++)
            {
                CloneProfile[clone][i].transform.right = directions[i];
            }
        }

        public override void OnBeginReceiveRay(RaySensor2D sensor)
        {
            base.OnBeginReceiveRay(sensor);

            var clone = sensor.cloneRaySensor;
            
            ApplyLengthControl(sensor);
            
            var clones = new List<RaySensor2D>();
            
            for (var i = 0; i < count; i++) clones.Add(Instantiate(clone));
            
            CloneProfile.Add(clone, clones);

            foreach (var c in clones)
            {
                c.transform.parent = clone.transform;

                if (sensor.stamp) //STAMP Reservation
                {
                    c.stamp = Instantiate(sensor.stamp);
                    c.stampOffset = sensor.stampOffset;
                    c.stampAutoHide = sensor.stampAutoHide;
                    c.stampOnHit = sensor.stampOnHit;
                }
            }

            OnReceiveRay(sensor);
            
            Destroy(clone.liner);
            
            clone.enabled = false;

#if UNITY_EDITOR
            clone.gizmosUpdate = GizmosMode.Off;
#endif
        }
        public override bool OnEndReceiveRay(RaySensor2D sensor)
        {
            if (base.OnEndReceiveRay(sensor))
            {
                if (sensor.stamp)
                {
                    foreach (var raySensor in CloneProfile[sensor.cloneRaySensor]) Destroy(raySensor.stamp.gameObject);
                }

                CloneProfile.Remove(sensor.cloneRaySensor);
            }

            return true;
        }
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Division of Planar Sensitive 2D Ray in angles and count entered.";
#pragma warning restore CS0414
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                PropertySliderField(_so.FindProperty(nameof(arcAngle)), 0f, 180f, CArcAngle.ToContent());
                PropertyMaxIntField(_so.FindProperty(nameof(count)), CCount.ToContent(), 1);
            }
            if (hasGeneral) GeneralField(_so);
            if (hasEvents) EventField(_so);
            if (hasInfo)
            {
                InformationField();
            }
        }
#endif
    }
}