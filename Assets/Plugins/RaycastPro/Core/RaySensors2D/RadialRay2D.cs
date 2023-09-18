namespace RaycastPro.RaySensors2D
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Rey Sensors/" + nameof(RadialRay2D))]
    public sealed class RadialRay2D : RaySensor2D
    {
        public float arcAngle = 60;

        [SerializeField]
        private float value;
        public float Value => value;

        [SerializeField]
        private int cuts = 5;

        [SerializeField] private bool collectHits;
        
        private void ResetArrays(int cutCount)
        {
            cutCount = Mathf.FloorToInt(cutCount / 2) * 2 + 1;
            
            if (collectHits)
            {
                if (raycastHits == null)
                {
                    raycastHits = new RaycastHit2D[cutCount];
                }
                else
                {
                    Array.Resize(ref raycastHits, cutCount);
                }
            }
            else
            {
                raycastHits = null;
            }
        }
        
        public RaycastHit2D[] raycastHits;
        private Vector3 _pos, tip;
        private float step, length;
        private Vector3 _l;
        private int trueHit;
        private RaycastHit2D _hit;
        protected override void OnCast() 
        {
#if UNITY_EDITOR
            GizmoGate = null;
#endif
            _pos = transform.position;
            step = arcAngle / cuts;
            _l = Direction;
            
            RaycastHit2D LineCast(int stepIndex)
            {

                tip = Quaternion.AngleAxis(step * stepIndex, Vector3.forward) * _l;
                _hit = Physics2D.Linecast(_pos, _pos + tip, detectLayer.value, MinDepth, MaxDepth);
#if UNITY_EDITOR
                var _p = _hit.point.ToDepth(z)-transform.position;
                bool _b = _hit.transform;
                var _tDir = tip;
                GizmoGate += () =>
                {
                    DrawBlockLine(transform.position, transform.position + _tDir, _b, _p+transform.position);
                };
#endif
                return _hit;
            }
            hit = default;

            length = Length;
            trueHit = 0;
            for (var i = 1; i <= cuts/2f; i += 1)
            {
                RaycastHit2D _THit;
                if (hit == default)
                {
                    _THit = LineCast(0);
                    if (_THit) trueHit++;
                   if (collectHits && raycastHits != null) raycastHits[0] = _THit;
                   if (_THit)
                    {
                        hit = _THit;
                        length = hit.distance;
                    }
                }
                _THit = LineCast(i);
                if (_THit) trueHit++;
                if (collectHits && raycastHits != null)
                {
                    raycastHits[i] = _THit;
                }
                if (_THit && _THit.distance < length)
                {
                    hit = _THit;
                    length = hit.distance;
                }
                _THit = LineCast(-i);
                if (_THit) trueHit++;

               if (collectHits && raycastHits != null)
               {
                   raycastHits[i + cuts] = _THit;
               }

                if (_THit && _THit.distance < length)
                {
                    hit = _THit;
                    
                    length = hit.distance;
                }
            }

            value =  (float) trueHit / ((cuts/2)*2 + 1);
            isDetect = FilterCheck(hit, hit.point - Position2D);
        }

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Radial shape emitter, which detects the nearest point, can collect hit information." + HAccurate + HDirectional;
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            EditorUpdate();
            
            var dir = Direction.ToDepth();
            DrawDepthLine(transform.position, Tip);
            DrawNormal2D(hit, z);
            var col = hit ? (isDetect ? DetectColor : BlockColor) : DefaultColor;
            Handles.color = col.ToAlpha(RCProPanel.alphaAmount);
            Handles.DrawSolidArc(transform.position, Vector3.forward, dir, arcAngle / 2, dir.magnitude);
            Handles.DrawSolidArc(transform.position, Vector3.forward, dir, -arcAngle / 2, dir.magnitude);
            DrawNormalFilter();
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                DirectionField(_so);
                PropertySliderField(_so.FindProperty(nameof(arcAngle)), 0f, 360f, CArcAngle.ToContent());
                var propCuts = _so.FindProperty(nameof(cuts));
                PropertySliderField(propCuts, 0, 90, CCuts.ToContent(), i => {});
                EditorGUI.BeginChangeCheck();
                var propCollect = _so.FindProperty(nameof(collectHits));
                EditorGUILayout.PropertyField(propCollect);
                if (EditorGUI.EndChangeCheck()) ResetArrays(propCuts.intValue);
            }

            if (hasGeneral) GeneralField(_so);

            if (hasEvents) EventField(_so);

            if (hasInfo)
            {
                InformationField(() =>
                {
                    if (!hit) return;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(hit.transform.name);
                    GUILayout.Label(hit.distance.ToString());
                    GUILayout.EndHorizontal();
                    ProgressField(value, "Value");
                });
            }
        }

#endif

        public override Vector3 Tip => transform.position + Direction3D;

        public override float RayLength => direction.magnitude;

        public override Vector3 Base => transform.position;
    }
}