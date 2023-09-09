using System;
using System.Linq;

namespace RaycastPro.Planers
{
    using System.Collections.Generic;
    using RaySensors;

    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif
    [AddComponentMenu("RaycastPro/Planers/"+nameof(DividePlanar))]
    public class DividePlanar : Planar
    {
        public float radius= 1f;
        public float forward = 1f;
        public int count = 5;
        public readonly Dictionary<RaySensor, List<RaySensor>> CloneProfile = new Dictionary<RaySensor, List<RaySensor>>();

        public override void GetForward(RaySensor raySensor, out Vector3 forward)
        {
            switch (baseDirection)
            {
                case DirectionOutput.NegativeHitNormal: forward = -raySensor.hit.normal; return;
                case DirectionOutput.HitDirection: forward = raySensor.HitDirection; return;
                case DirectionOutput.SensorLocal: forward = raySensor.LocalDirection.normalized; return;
            }
            forward = transform.forward;
        }
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Division of Planar Sensitive Ray in angles and count entered."+HDependent+HExperimental;
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
            if (hasEvents) EventField(_so);
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
            
            GetForward(sensor, out _forward);

            point = sensor.hit.point;
            _cloneT = clone.transform;
            _cloneT.rotation = Quaternion.LookRotation(_forward, transform.up);
            _cloneT.position = point + _forward * offset; // Offset most Apply after Rotation Calculating
            _cloneT.localScale = Vector3.one;

            if (!CloneProfile.ContainsKey(clone)) return;
            cloneCount = CloneProfile[clone].Count;
            
            // Switch it to non Allocator Later
            points = CircularPoints(_forward*forward, radius, _forward, _cloneT.up, cloneCount);

            
            if (_cloneT)
            {
                var _dir = clone.direction;
                switch (lengthControl)
                {
                    case LengthControl.Constant:
                        _dir = sensor.direction.normalized * length;
                        break;
                    case LengthControl.Sync:
                        _dir = sensor.direction * length;
                        break;
                    case LengthControl.Continues:
                        _dir = sensor.direction.normalized * (sensor.ContinuesDistance * length);
                        break;
                }
                
                foreach (var miniClone in CloneProfile[clone])
                {
                    if (!miniClone) continue;

                    miniClone.transform.position = _cloneT.position;
                    miniClone.transform.localScale = sensor.transform.localScale;
                    miniClone.direction = _dir;
                    
                    if (miniClone.liner) miniClone.liner.enabled = sensor.liner.enabled;
                    if (miniClone.stamp) miniClone.stamp.gameObject.SetActive(sensor.stamp.gameObject.activeInHierarchy);
                }
            }

            for (i = 0; i < cloneCount; i++)
            {
                if (!clone) return;
                if (!CloneProfile[clone][i]) continue;
                CloneProfile[clone][i].transform.rotation = Quaternion.LookRotation(points[i]);
            }
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
                c._baseRaySensor = sensor;
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
            //clone.stamp = null;
            
#if UNITY_EDITOR
            clone.gizmosUpdate = GizmosMode.Off;
#endif
        }

        internal override bool OnEndReceiveRay(RaySensor sensor)
        {
            sensor.cloneRaySensor.SafeRemove();

            return true;
        }
    }
}
