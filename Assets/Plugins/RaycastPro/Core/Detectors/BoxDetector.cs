namespace RaycastPro.Detectors
{
    using UnityEngine;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Detectors/" + nameof(BoxDetector))]
    public sealed class BoxDetector : ColliderDetector, IPulse
    {
        [SerializeField]
        public Vector3 extents = Vector3.one;

        [SerializeField] private bool limited;
        [SerializeField] private int limitCount = 3;
        
        public bool Limited
        {
            get => limited;
            set
            {
                limited = value;
                if (value)
                {
                    colliders = new Collider[limitCount];
                }
            }
        }
        public int LimitCount
        {
            get => limitCount;
            set
            {
                limitCount = Mathf.Max(0, value);
                colliders = new Collider[limitCount];
            }
        }
        protected override void OnCast()
        {
#if UNITY_EDITOR
            CleanGate();
#endif
            PreviousColliders = DetectedColliders.ToArray();
            TDP = transform.position;
            if (limited)
            {
                for (var i = 0; i < colliders.Length; i++) colliders[i] = null;

                Physics.OverlapBoxNonAlloc(TDP, extents / 2, colliders, transform.rotation,
                    detectLayer.value,
                    triggerInteraction);
            }
            else
            {
                colliders = Physics.OverlapBox(TDP, extents / 2, transform.rotation, detectLayer.value,
                    triggerInteraction);
            }
            DetectedColliders.Clear();
            if (IsIgnoreSolver)
            {
                foreach (var c in colliders)
                {
                    if (CheckGeneralPass(c))
                    {
#if UNITY_EDITOR
                        PassColliderGate(c);
#endif
                        DetectedColliders.Add(c);
                    }
                }
            }
            else
            {
                foreach (var c in colliders)
                {
                    if (!CheckGeneralPass(c)) continue;
                    TDP = DetectFunction(c); 
                    boundPoint = transform.InverseTransformPoint(TDP);
                    if (new Bounds(Vector3.zero, extents).Contains(boundPoint) && CheckSolverPass(TDP, c)) DetectedColliders.Add(c);
                }
            }
            
            ColliderDetectorEvents();
        }

        private Vector3 boundPoint;

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Receiving colliders within the specified bounds with a detect point solver." +
                                     HAccurate + HIPulse + HCDetector + HLOS_Solver + HRotatable + HINonAllocator;
#pragma warning restore CS0414
        protected override void DrawDetectorGuide(Vector3 point)
        {
            if (!RCProPanel.DrawGuide) return;

            var p = transform.position;

            var pDirection = point - p;

            var reflectF = Vector3.Reflect(pDirection, transform.forward);

            var color = DetectColor;

            color.a = RCProPanel.alphaAmount;

            Gizmos.color = color;

            Gizmos.DrawLine(p + reflectF, p + pDirection);

            Gizmos.DrawLine(p + reflectF, p - pDirection);

            Gizmos.DrawLine(p - reflectF, p - pDirection);

            Gizmos.DrawLine(p - reflectF, p + pDirection);
        }
        internal override void OnGizmos()
        {
            EditorCast();
            DrawDetectVector();
            GizmoColor = DefaultColor;
            DrawRectLines(transform, new Vector3(extents.x, extents.y), -extents.z / 2, extents.z);
            DrawBox(transform, new Vector3(extents.x, extents.y), -extents.z / 2);
            DrawBox(transform, new Vector3(extents.x, extents.y), +extents.z / 2);
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)  ExtentsField(_so);
            if (hasGeneral) ColliderDetectorGeneralField(_so);
            if (hasEvents)
            {
                EventField(_so);
                if (EventFoldout) RCProEditor.EventField(_so, CEventNames);
            }
            if (hasInfo) InformationField(PanelGate);
        }
#endif
    }
}