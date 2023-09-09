namespace RaycastPro.Detectors
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif
    [Serializable]
    public class ColliderEvent : UnityEvent<Collider> { }

    [Serializable]
    public class TransformEvent : UnityEvent<Transform> { }

    public abstract class Detector : BaseDetector
    {
        /// <summary>
        /// Temp Detected Point
        /// </summary>
        protected Vector3 TDP;
        private Vector3 TSP;
        
        #region BlockSystem

        public Vector3 blockSolverOffset;
        public Vector3 detectVector;

        /// <summary>
        /// Solver Point in world Space
        /// </summary>
        public override Vector3 SolverPoint => checkLineOfSight ? transform.TransformPoint(blockSolverOffset) : transform.position;
        
        public override Vector3 DetectVectorPoint => transform.TransformPoint(detectVector);

        #endregion
        protected Func<Collider, Vector3> SetupDetectFunction()
        {
            switch (solverType)
            {
                case SolverType.Ignore: return c => c.transform.position;
                case SolverType.Pivot:
                    if (boundsCenter) return c => c.bounds.center;
                    return c => c.transform.position;
                case SolverType.Nearest:
                    if (boundsSolver) return c => c.ClosestPointOnBounds(transform.position);
                    return c => c.ClosestPoint(transform.position);
                case SolverType.Furthest:
                    if (boundsSolver) return c => c.ClosestPointOnBounds(SolverPoint + (c.transform.position - SolverPoint) * int.MaxValue);
                    return c => c.ClosestPoint(SolverPoint + (c.transform.position - SolverPoint) * int.MaxValue);
                case SolverType.Focused:
                    if (boundsSolver) return c => c.ClosestPointOnBounds(transform.TransformPoint(detectVector));
                    return c => c.ClosestPoint(transform.TransformPoint(detectVector));
                case SolverType.Dodge:
                    return c =>
                    {
                        TSP = checkLineOfSight ? SolverPoint : transform.position;
                        var closetPoint = boundsSolver || c is MeshCollider
                            ? (Func<Vector3, Vector3>) c.ClosestPointOnBounds : c.ClosestPoint;
                        var _ct = c.transform;
                        var cPos = c.transform.position;
                        var crossUp = Vector3.Cross(cPos - transform.position, transform.right);
                        var cross = Vector3.Cross(cPos - transform.position, transform.up);
                        TDP = cPos;
                        if (!Physics.Linecast(TSP, TDP, out var hit, blockLayer.value, triggerInteraction) ||
                            hit.transform == _ct) return TDP;
                        TDP = closetPoint(c.bounds.center);
                        if (!Physics.Linecast(TSP, TDP, out hit, blockLayer.value, triggerInteraction) ||
                            hit.transform == _ct) return TDP;
                        TDP = closetPoint(cPos + cross * int.MaxValue);
                        if (!Physics.Linecast(TSP, TDP, out hit, blockLayer.value, triggerInteraction) ||
                            hit.transform == _ct) return TDP;
                        TDP = closetPoint(cPos - cross * int.MaxValue);
                        if (!Physics.Linecast(TSP, TDP, out hit, blockLayer.value, triggerInteraction) ||
                            hit.transform == _ct) return TDP;
                        TDP = closetPoint(cPos + crossUp * int.MaxValue);
                        if (!Physics.Linecast(TSP, TDP, out hit, blockLayer.value, triggerInteraction) ||
                            hit.transform == _ct) return TDP;
                        TDP = closetPoint(cPos - crossUp * int.MaxValue);
                        if (!Physics.Linecast(TSP, TDP, out hit, blockLayer.value, triggerInteraction) ||
                            hit.transform == _ct) return TDP;
                        return TDP;
                    };
            }
            return c => c.transform.position;
        }

        /// <summary>
        /// Check Line of Sight
        /// </summary>
        /// <param name="point"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        protected bool CheckSolverPass(Vector3 point, Collider c)
        {
            if (!checkLineOfSight)
            {
#if UNITY_EDITOR
                SetupGates(c, point, false, default);
#endif
                return true;
            }

            if (Physics.Linecast(SolverPoint, point, out var blockHit, blockLayer.value, triggerInteraction) &&
                blockHit.transform != c.transform)
            {
#if UNITY_EDITOR
                SetupGates(c, point, true, blockHit);
#endif
                return false;
            }
#if UNITY_EDITOR
            SetupGates(c, point, false, default);
#endif
            return true;
        }
#if UNITY_EDITOR
        protected void SetupGates(Collider c, Vector3 point, bool blocked, RaycastHit blockHit)
        {
            PanelGate += () => DetectorInfoField(c.transform, point, blocked);
            GizmoGate += () =>
            {
                if (blocked)
                {
                    DrawBlockLine(SolverPoint, point, c.transform, blockHit);
                    if (IsGuide)
                    {
                        Gizmos.color = BlockColor;
                        if (boundsSolver) Gizmos.DrawWireCube(c.bounds.center, c.bounds.size);
                    }
                }
                else
                {
                    DrawDetectedCollider(c, point);
                    DrawDetectorGuide(point);
                }
            };
        }
        protected void DrawDetectVector()
        {
            if (!RCProPanel.DrawGuide) return;

            if (solverType == SolverType.Focused)
            {
                var point = transform.TransformPoint(detectVector);
                
                {
                    Gizmos.DrawLine(transform.position, point);
                    Gizmos.DrawWireSphere(point, DotSize);
                }
                if (RCProPanel.ShowLabels) Handles.Label(point, solverType + " Vector");
            }

            if (solverType != SolverType.Ignore && checkLineOfSight)
            {
                Gizmos.color = HelperColor;
                Gizmos.DrawLine(transform.position, SolverPoint);
                Gizmos.DrawWireSphere(SolverPoint, DotSize);
            }
        }
        protected abstract void DrawDetectorGuide(Vector3 point);
        protected void DrawDetectedCollider(Collider c, Vector3 detectPoint)
        {
            GizmoColor = DetectColor;
            if (IsLabel) Handles.Label(c.transform.position, c.name);
            if (IsDetectLine) DrawLine(checkLineOfSight ? SolverPoint : transform.position, detectPoint);
            if (IsGuide)
            {
                Gizmos.color = boundsSolver ? DetectColor : HelperColor;
                Gizmos.DrawWireCube(c.bounds.center, c.bounds.size);
            }
        }
        protected void SolverField(SerializedObject _so)
        {
            BaseSolverField(_so, () =>
            {
                if (IsIgnoreSolver) return;
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(blockLayer)), CBlockLayer.ToContent(TBlockLayer));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(boundsSolver)), CBoundsSolver.ToContent(TBoundsSolver));
                if (IsPivotSolver)
                {
                    EditorGUILayout.PropertyField(_so.FindProperty(nameof(boundsCenter)), CBoundsCenter.ToContent(TBoundsCenter));
                }
                if (IsFocusedSolver)
                {
                    EditorGUILayout.PropertyField(_so.FindProperty(nameof(detectVector)), CFocusPoint.ToContent(TFocusPoint));
                }
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(checkLineOfSight)), CCheckLineOfSight.ToContent(TCheckLineOfSight));
                GUI.enabled = checkLineOfSight && GUI.enabled;
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(blockSolverOffset)), CBlockSolverOffset.ToContent(TBlockSolverOffset));
                GUI.enabled = true;
            });
        }

        protected void GeneralField(SerializedObject _so, bool layerField = true, bool hasTagField = true)
        {
            if (this is IPulse) PulseField(_so);
            if (layerField) DetectLayerField(_so);
            if (hasTagField) TagField(_so);
        }
#endif
    }
}