namespace RaycastPro.RaySensors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Rey Sensors/" + nameof(ChainRay))]
    public sealed class ChainRay : PathRay, IRadius
    {
        public ChainReference chainReference = ChainReference.Point;
        [SerializeField] private float radius = 0f;
        public float Radius
        {
            get => radius;
            set => radius = Mathf.Max(0,value);
        }
        public Vector3[] chainPoints = {Vector3.forward, Vector3.right, Vector3.up};
        
        public Transform[] targets = Array.Empty<Transform>();
        public enum ChainReference
        {
            /// <summary>
            /// As setup reference to transform, You could animate chain points on playmode.
            /// </summary>
            Transform,
            Point,
        }

        public bool relative;

        protected override void OnCast()
        {
            UpdatePath(PathPoints);
            if (pathCast) DetectIndex = PathCast(PathPoints, radius);
            else DetectIndex = -1;
        }

        private Transform target;
        private void UpdatePath(List<Vector3> path)
        {
            path.Clear();
            path.Add(transform.position);
            switch (chainReference)
            {
                case ChainReference.Point:
                    path.AddRange((relative ? chainPoints.ToRelative() : chainPoints).ToLocal(transform));
                    break;
                case ChainReference.Transform:
                {
                    for (var index = 0; index < targets.Length; index++) // For is fastest
                    {
                        if (targets[index]) path.Add(targets[index].position);
                    }
                    break;
                }
            }
        }
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Send a point oriented ray and return Hit information." + HAccurate + HPathRay +HIRadius + HScalable;
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            EditorUpdate();
            if (PathPoints.Count == 0) return;
            if (hit.transform) DrawNormal(hit.point, hit.normal, hit.transform.name);
            if (IsManuelMode)
            {
                UpdatePath(PathPoints);
                DrawPath(PathPoints, hit, radius, detectIndex: DetectIndex, drawSphere: true);
            }
            else
            {
                DrawPath(PathPoints, hit, radius,  detectIndex: DetectIndex, drawSphere: true);
            }

            Handles.color = Gizmos.color = HelperColor;
            Handles.DrawDottedLine(BasePoint, Tip, StepSizeLine);
            Handles.Label((BasePoint + Tip) / 2,
                "<color=#2BC6D2>Distance:</color> <color=#FFFFFF>" + TipLength.ToString("F2") + "</color>", new GUIStyle {richText = true});
            DrawCap(PathPoints.Last(), PathPoints.LastDirection(TipDirection));
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true, bool hasInfo = true)
        {
            if (hasMain)
            {
                PropertyEnumField(_so.FindProperty(nameof(chainReference)), 2, CReferenceType.ToContent(TReferenceType), new GUIContent[]
                    {"Transform".ToContent("Can adjust game object's position as chain reference."), "Point".ToContent("Adjust Points as regular vector3 positions with a relative mode option.")}
                );
                BeginVerticalBox();
                if (chainReference == ChainReference.Point)
                {
                    RCProEditor.PropertyArrayField(_so.FindProperty(nameof(chainPoints)), "Points".ToContent(),
                        (i) => $"Points {i+1}".ToContent($"Index {i}"));
                    
                    EditorGUILayout.PropertyField(_so.FindProperty(nameof(relative)),
                        CRelative.ToContent(TRelative), relative);
                }
                else RCProEditor.PropertyArrayField(_so.FindProperty(nameof(targets)), "Targets".ToContent(),
                    (i) => $"Target {i+1}".ToContent($"Index {i}"));
                
                EndVertical();
                RadiusField(_so);
            }
            
            if (hasGeneral) PathRayGeneralField(_so);

            if (hasEvents) EventField(_so);

            if (hasInfo) InformationField();
        }
#endif
    }
}