using System.Collections.Generic;

namespace RaycastPro.RaySensors
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Rey Sensors/" + nameof(RadialRay))]
    public sealed class RadialRay : RaySensor
    {
        public float arcAngle = 60f;
        public Vector3 ArcStartPoint => Quaternion.AngleAxis(-arcAngle / 2, transform.up) * Direction;
        public Vector3 ArcEndPoint => Quaternion.AngleAxis(arcAngle / 2, transform.up) * Direction;
        
        [SerializeField]
        private float value;
        public float Value => value;
        [SerializeField] private int subdivide = 3;
        public int Subdivide
        {
            get => subdivide;
            set
            {
                subdivide = (byte) Mathf.Max(1,value);
            }
        }
        public Stack<RaycastHit> raycastHits = new Stack<RaycastHit>();
        public override Vector3 Tip => transform.position + Direction;
        public override float RayLength => TipLength;
        public override Vector3 BasePoint => transform.position;

        private RaycastHit raycastHit;
        private Vector3 _pos, _angledPos;
        private float pow, step;
        private bool condition;
        protected override void OnCast()
        {
#if UNITY_EDITOR
            CleanGate();
#endif
            _pos = transform.position;
            pow = Mathf.Pow(2, subdivide);
            step = arcAngle / pow;
            hit = default;

            raycastHits.Clear();
            for (var i = 0; i <= pow; i++)
            {
                _angledPos = _pos + Quaternion.AngleAxis(step * i, transform.up) * ArcStartPoint;
                condition = Physics.Linecast(_pos, _angledPos, out raycastHit, detectLayer.value, triggerInteraction);

                if (!hit.transform) hit = raycastHit;

                raycastHits.Push(raycastHit);
#if UNITY_EDITOR
                var clone = _angledPos;
                var _p = raycastHit.point;
                var _n = raycastHit.normal;
                GizmoGate += () =>
                {
                    DrawCross(_p, _n);
                    Handles.color = condition ? DetectColor : DefaultColor;
                    Handles.DrawLine(_pos, clone);
                };
#endif
                if (condition && raycastHit.distance <= hit.distance) hit = raycastHit;
            }
        }

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Send multiple rays at an angle and detect the hit Info when each of them hits." + HAccurate + HDirectional;
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            EditorUpdate();
            var color = (Performed ? DetectColor : DefaultColor);
            DrawZTest(() => Handles.DrawSolidArc(transform.position, transform.up, ArcStartPoint, arcAngle, DirectionLength),
                color.ToAlpha(RCProPanel.alphaAmount/2), color.ToAlpha(RCProPanel.alphaAmount));

            DrawNormal(hit);
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                DirectionField(_so);
                PropertySliderField(_so.FindProperty(nameof(arcAngle)), 0f, 360f, CArcAngle.ToContent(CArcAngle));
                PropertySliderField(_so.FindProperty(nameof(subdivide)), 0, RCProPanel.maxSubdivideTime, CSubdivide.ToContent(TSubdivide), _ => {});
            }
            if (hasGeneral) GeneralField(_so);
            if (hasEvents) EventField(_so);
            if (hasInfo)
            {
                InformationField(() =>
                {
                    if (!hit.transform) return;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(hit.transform.name);
                    GUILayout.Label(hit.distance.ToString());
                    GUILayout.EndHorizontal();
                    ProgressField(value, "Value");
                });
            }
        }
#endif
        public RaycastHit[] DetectedHits { get; set; } = Array.Empty<RaycastHit>();
    }
}