

namespace RaycastPro.RaySensors2D
{
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
        using UnityEditor;
#endif


    [AddComponentMenu("")]
    public sealed class CloneRay2D : PathRay2D
    {
        private Transform getter;
        private Transform outer;

        private RaySensor2D sensor;
        
        // Non Allocation 
        private List<Vector2> _tPath = new List<Vector2>();
        
        private float radius;

        internal void CopyFrom(RaySensor2D raySensor, Transform _getter, Transform _outer)
        {
            getter = _getter;
            outer = _outer;

            sensor = raySensor;
            minDepth = raySensor.minDepth;
            maxDepth = raySensor.maxDepth;
            minAngle = raySensor.minAngle;
            maxAngle = raySensor.maxAngle;
            detectLayer = raySensor.detectLayer;

            planarSensitive = raySensor.planarSensitive;
            
            anyPlanar = raySensor.anyPlanar;
            
            if (!anyPlanar) planers = raySensor.planers;

            planers = raySensor.planers;
            stamp = raySensor.stamp;
            stampAutoHide = raySensor.stampAutoHide;
            stampOffset = raySensor.stampOffset;
            stampOnHit = raySensor.stampOnHit;
            syncStamp = raySensor.syncStamp;

            cutOnHit = raySensor.cutOnHit;

#if UNITY_EDITOR
            gizmosUpdate = raySensor.gizmosUpdate;
#endif

            if (raySensor.liner)
            {
                liner = CopyComponent(raySensor.liner, gameObject);

                UpdateLiner();
            }
        }

        protected override void OnCast()
        {
            PathPoints.Clear();
            DetectIndex = -1;
            if (!sensor) return;

            if (sensor is PathRay2D pathRay)
            {
                _tPath.Clear();
                
                _tPath.Add(Vector2.zero);
                
                for (var i = pathRay.DetectIndex + 1; i < pathRay.PathPoints.Count; i++)
                    _tPath.Add(pathRay.PathPoints[i]-sensor.hit.point);

                foreach (var p in _tPath) PathPoints.Add( transform.TransformDirection(getter.InverseTransformDirection(p))+transform.position);
            }
            else
            {
                var path = new List<Vector3>
                {
                    getter.transform.InverseTransformPoint(sensor.hit.point),
                    getter.transform.InverseTransformPoint(sensor.Tip)
                };

                foreach (var p in path) PathPoints.Add(outer.TransformPoint(p));
            }
            if (sensor is IRadius iRadius) radius = iRadius.Radius;
            
            PathCast(PathPoints, out hit, out var detectIndex, MinDepth, MaxDepth, radius);

            DetectIndex = detectIndex;
        }

#if UNITY_EDITOR

#pragma warning disable CS0414
        private static string Info = "This ray executes a copy of the input ray to the planar and is simply not adjustable." + HVirtual;
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            EditorUpdate();
            DrawPath2D(PathPoints.ToDepth(z), hit.point, detectIndex: DetectIndex, radius: radius);
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            BeginVerticalBox();
            
            if (IsPlaying)
            {
                if (sensor) EditorGUILayout.LabelField("Clone From: " + sensor.gameObject.name);
                if (getter) EditorGUILayout.LabelField("Getter: " + getter.gameObject.name);
                if (outer) EditorGUILayout.LabelField("Outer: " + outer.gameObject.name);
            }

            EditorGUILayout.LabelField("Clone Rays can't be modified.");
            
            EndVertical();

            BaseField(_so, hasInfluence: false, hasInteraction: false, hasUpdateMode: false);

            InformationField();
        }
#endif
    }
}