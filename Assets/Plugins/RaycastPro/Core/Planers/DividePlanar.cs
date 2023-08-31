using System;

namespace RaycastPro.Planers
{
    using System.Collections.Generic;
    using RaySensors;

    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif
    [AddComponentMenu("RaycastPro/Planers/"+nameof(DividePlanar))]
    abstract class DividePlanar : Planar
    {
        public float radius= 1f;
        public float forward = 1f;
        public int count = 5;
        public readonly Dictionary<RaySensor, List<RaySensor>> CloneProfile = new Dictionary<RaySensor, List<RaySensor>>();
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Division of Planar Sensitive Ray in angles and count entered.";
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            points = CircularPoints(transform.position+transform.forward*forward, radius, transform.forward, transform.up, count, true);
            
            for (i = 0; i < points.Length-1; i++)
            {
                Gizmos.DrawLine(points[i], points[i+1]);
                Gizmos.DrawLine(transform.position,points[i]);
            }
            
            DrawPlanar();
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                RadiusField(_so);
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(forward)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(count)));
            }
            if (hasGeneral) GeneralField(_so);
            if (hasEvents)EventField(_so);
        }
#endif

        private RaySensor clone;
        private Vector3 _forward, point, inverseTransformDirection, cloneDirection, cross;
        private Transform _cloneT;
        private Vector3[] points = Array.Empty<Vector3>();
        private int cloneCount;
        internal override void OnReceiveRay(RaySensor sensor)
        {
            clone = sensor.cloneRaySensor;
            if (!clone) return;
            // // TEMP BASE RAY SENSOR DEBUG..
            if (!clone._baseRaySensor) RaySensor.CloneDestroy(clone);
            _forward = GetForward(sensor, transform.forward);
            point = sensor.Hit.point;
            _cloneT = clone.transform;
            _cloneT.position = point;
            inverseTransformDirection = transform.InverseTransformDirection(point - sensor.BasePoint);
            cloneDirection = transform.TransformDirection(inverseTransformDirection);
            _cloneT.rotation = Quaternion.LookRotation(cloneDirection);
            
            point += _cloneT.forward * offset; // Offset most Apply after Rotation Calculating
            
            _cloneT.localScale = Vector3.one;
            ApplyLengthControl(sensor);
            if (!CloneProfile.ContainsKey(clone)) return;
            cloneCount = CloneProfile[clone].Count;
            
            // Switch it to non Allocator Later
            points = CircularPoints(_forward*forward, radius, _forward, _cloneT.up, cloneCount);
            foreach (var c in CloneProfile[clone])
            {
                c.transform.position = point;
                c.direction = clone.direction;
                c.transform.localScale = sensor.transform.localScale;
                if (c.liner) c.liner.enabled = sensor.liner.enabled;
                if (c.stamp) c.stamp.gameObject.SetActive(sensor.stamp.gameObject.activeInHierarchy);
            }
            for (i = 0; i < cloneCount; i++)
            {
                CloneProfile[clone][i].transform.rotation = Quaternion.LookRotation(points[i]);
            }
        }

        /// <summary>
        /// This method is used for Bullet Transitions.
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal override TransitionData[] GetTransitionData(RaycastHit hit, Vector3 direction)
        {
            point = hit.point + direction * offset;
            cross = Vector3.Cross(transform.right, direction);
            points = CircularPoints(point + direction * forward, radius, direction, cross, count);
            var data = new TransitionData[count];
            for (i = 0; i < points.Length; i++)
            {
                data[i].position = point;
                data[i].rotation = Quaternion.LookRotation(points[i] - point, cross);
            }
            return data;
        }

        private readonly List<RaySensor> clones = new List<RaySensor>();
        private int i;
        internal override void OnBeginReceiveRay(RaySensor sensor)
        {
            base.OnBeginReceiveRay(sensor);
            clone = sensor.cloneRaySensor;
            ApplyLengthControl(sensor);
            clones.Clear();
            for (i = 0; i < count; i++) clones.Add(Instantiate(clone)); // hw
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
            clone.stamp = null;
            
#if UNITY_EDITOR
            clone.gizmosUpdate = GizmosMode.Off;
#endif
        }

        internal override bool OnEndReceiveRay(RaySensor sensor)
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
    }
}
