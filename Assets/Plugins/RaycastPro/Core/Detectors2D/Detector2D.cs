namespace RaycastPro.Detectors2D
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif
    [Serializable]
    public class Collider2DEvent : UnityEvent<Collider2D> { }
    public abstract class Detector2D : BaseDetector
    {
        /// <summary>
        /// TempDetectedPoint
        /// </summary>
        protected Vector3 TDP;
        private Vector3 TSP;

        #region BlockSystem
        public Vector2 blockSolverOffset;
        public Vector2 detectVector;

        public override Vector3 SolverPoint => checkLineOfSight ? transform.TransformPoint(blockSolverOffset).ToDepth(z) : transform.position;
        public override Vector3 DetectVectorPoint => transform.TransformPoint(detectVector);
        public Vector2 Position2D => transform.position;
        #endregion

        /// <summary>
        /// Local Min Depth
        /// </summary>
        [SerializeField] protected float minDepth = .5f;
        /// <summary>
        /// Local Max Depth
        /// </summary>
        [SerializeField] protected float maxDepth = -.5f;

        /// <summary>
        /// Return Min Depth as World space. Form : transform.position.z + minDepth
        /// </summary>
        public float MinDepth => transform.position.z + minDepth;
        
        /// <summary>
        /// Return Max Depth as World space. Form : transform.position.z + maxDepth
        /// </summary>
        public float MaxDepth => transform.position.z + maxDepth;

        public float z => transform.position.z;

        protected Func<Collider2D, Vector2> SetupDetectFunction()
        {
            switch (solverType)
            {
                case SolverType.Ignore: return c => c.transform.position;
                case SolverType.Pivot: return c => c.transform.position;
                case SolverType.Nearest: return c => c.ClosestPoint(transform.position);
                case SolverType.Furthest:
                {
                    return c => c.ClosestPoint(SolverPoint + (c.transform.position - SolverPoint) * int.MaxValue);
                }
                case SolverType.Focused: return c => c.ClosestPoint(DetectVectorPoint);
                case SolverType.Dodge:
                    return c =>
                    {
                        TDP = c.transform.position;
                        TSP = SolverPoint;
                        var maxUp = transform.up * int.MaxValue;
                        var maxRight = transform.right * int.MaxValue;
                        var _ct = c.transform;
                        var hit2D = Physics2D.Linecast(TSP, TDP, blockLayer.value, MinDepth, MaxDepth);
                        if (!hit2D || hit2D.transform == _ct) return TDP;

                        TDP = c.ClosestPoint(TDP + maxUp);
                        hit2D = Physics2D.Linecast(TSP, TDP, blockLayer.value, MinDepth, MaxDepth);
                        if (!hit2D || hit2D.transform == _ct) return TDP;

                        TDP = c.ClosestPoint(TDP - maxUp);
                        hit2D = Physics2D.Linecast(TSP, TDP, blockLayer.value, MinDepth, MaxDepth);
                        if (!hit2D || hit2D.transform == _ct) return TDP;

                        TDP = c.ClosestPoint(TDP + maxRight);
                        hit2D = Physics2D.Linecast(TSP, TDP, blockLayer.value, MinDepth, MaxDepth);
                        if (!hit2D || hit2D.transform == _ct) return TDP;

                        TDP = c.ClosestPoint(TDP - maxRight);
                        hit2D = Physics2D.Linecast(TSP, TDP, blockLayer.value, MinDepth, MaxDepth);
                        if (!hit2D || hit2D.transform == _ct) return TDP;
                        return TDP;
                    };
            }
            return c => c.transform.position;
        }

#if UNITY_EDITOR
        
        protected void BlockLineGizmo(Collider2D c, Vector3 point, RaycastHit2D blockHit = default)
        {
            GizmoGate += () =>
            {
                if(blockHit && blockHit.collider != c)
                {
                    Gizmos.color = BlockColor;
                    if (IsGuide) Gizmos.DrawWireCube(c.bounds.center, c.bounds.extents * 2);
                    DrawBlockLine2D(SolverPoint, point, z, c.transform, blockHit);
                }
                else if (IsDetectLine)
                {
                    GizmoColor = DetectColor;
                    if (IsGuide) Gizmos.DrawWireCube(c.bounds.center, c.bounds.extents * 2);
                    Handles.DrawLine(SolverPoint.ToDepth(z), point.ToDepth(z));
                }
            };
            PanelGate += () => DetectorInfoField(c.transform, point, blockHit);
        }
        protected void DrawDetectVector()
        {
            Handles.color = HelperColor;
            if (RCProPanel.DrawGuide)
            {
                Handles.DrawLine(transform.position, SolverPoint);
                Handles.DrawWireDisc(SolverPoint, Vector3.forward, DotSize);
            }
            if (RCProPanel.ShowLabels)  Handles.Label(SolverPoint, "Block Solver Point");

            if (!IsFocusedSolver) return;

            var point = transform.TransformPoint(detectVector);
            if (RCProPanel.DrawGuide)
            {
                Handles.DrawLine(transform.position, point);
                Handles.DrawWireDisc(point, Vector3.forward, DotSize);
            }

            if (RCProPanel.ShowLabels) Handles.Label(point, solverType + " Vector");
        }

        protected void DrawDepthLine(Vector3 position, bool dotted = true) =>  DrawLine(position.ToDepth(MinDepth), position.ToDepth(MaxDepth), dotted);

        protected void GeneralField(SerializedObject _so)
        {
            DetectLayerField(_so);
            TagField(_so);
            DepthField(_so);
        }

        protected void DepthField(SerializedObject _so)
        {
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(minDepth)), "Min Depth".ToContent());
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(maxDepth)), "Max Depth".ToContent());
        }
        protected void Draw4AxesDepth(float offset)
        {
            DrawDepthLine(transform.position + Vector3.right * offset);
            DrawDepthLine(transform.position - Vector3.right * offset);
            DrawDepthLine(transform.position + Vector3.up * offset);
            DrawDepthLine(transform.position - Vector3.up * offset);
        }
        protected void DrawDepthCircle(float radius)
        {
            Draw4AxesDepth(radius);

            Handles.color = DefaultColor;

            Handles.DrawWireDisc(transform.position.ToDepth(MinDepth), Vector3.forward, radius);

            Handles.DrawWireDisc(transform.position.ToDepth(MaxDepth), Vector3.forward, radius);
        }
        protected void SolverField(SerializedObject _so)
        {
            BaseSolverField(_so, () =>
            {
                if (IsIgnoreSolver) return;

                EditorGUILayout.PropertyField(_so.FindProperty(nameof(blockLayer)),
                    CBlockLayer.ToContent(TBlockLayer));

                EditorGUILayout.PropertyField(_so.FindProperty(nameof(boundsSolver)),
                    CBoundsSolver.ToContent(TBoundsSolver));
                
                if (IsPivotSolver)
                {
                    EditorGUILayout.PropertyField(_so.FindProperty(nameof(boundsCenter)),
                        CBoundsCenter.ToContent(TBoundsCenter));
                }

                if (IsFocusedSolver)
                {
                    EditorGUILayout.PropertyField(_so.FindProperty(nameof(detectVector)),
                        CFocusPoint.ToContent(TFocusPoint));
                }
                
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(checkLineOfSight)),
                    CCheckLineOfSight.ToContent(TCheckLineOfSight));

                GUI.enabled = checkLineOfSight;

                EditorGUILayout.PropertyField(_so.FindProperty(nameof(blockSolverOffset)),
                    CBlockSolverOffset.ToContent(TBlockSolverOffset));

                GUI.enabled = true; 
            });
        }
#endif
    }
}