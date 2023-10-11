namespace RaycastPro.Detectors
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using RaySensors;

#if UNITY_EDITOR
    using UnityEditor;
    using Editor;
#endif

    using UnityEngine;
    
    [AddComponentMenu("RaycastPro/Detectors/" + nameof(PathDetector))]
    public class PathDetector : Detector, IPulse
    {
        public PathRay pathRay;
        
        public RaycastEvent onHit;
        public RaycastEvent onNewHit;
        public RaycastEvent onLostHit;
        public ColliderEvent onDetectCollider;
        public ColliderEvent onNewCollider;
        public ColliderEvent onLostCollider;
        public override bool Performed
        {
            get => DetectedHits.Count > 0;
            protected set { }
        }
        
        protected void Start() // Refreshing
        {
            PreviousColliders = new HashSet<Collider>();
            DetectedColliders = new HashSet<Collider>();
        }
        protected override void OnCast()
        {
            if (!pathRay.enabled) pathRay.Cast();

            PreviousHits = DetectedHits.ToArray();
            PreviousColliders = new HashSet<Collider> (DetectedColliders);

#if UNITY_EDITOR
            CleanGate();
#endif
            if (pathRay)
            {
                if (usingTagFilter)
                {
                    if (pathRay is IRadius radius)
                    {
                        PathCastAll(pathRay.PathPoints, ref DetectedHits, radius.Radius);
                        foreach (var r in DetectedHits) if (!r.collider.CompareTag(tagFilter)) DetectedHits.Remove(r);
                    }
                    else
                    {
                        PathCastAll(pathRay.PathPoints, ref DetectedHits);
                        foreach (var r in DetectedHits)
                        {
                            if (!r.collider.CompareTag(tagFilter)) DetectedHits.Remove(r);
#if UNITY_EDITOR
                            else
                            {
                                GizmoGate += () => { DrawCross(r.point, r.normal); };   
                            }
#endif
                        }
                    }
                }
                else
                {
                    if (pathRay is IRadius radius) PathCastAll(pathRay.PathPoints, ref DetectedHits, radius.Radius);
                    else PathCastAll(pathRay.PathPoints, ref DetectedHits);
                }
            }
            
            DetectedColliders.Clear();
            foreach (var _dHit in DetectedHits) DetectedColliders.Add(_dHit.collider);
            
#if UNITY_EDITOR
            foreach (var c in DetectedColliders) PassColliderGate(c);
#endif
            if (onHit != null) foreach (var _member in DetectedHits) onHit.Invoke(_member);
            if (onNewHit != null) foreach (var _member in DetectedHits.Except(PreviousHits)) onNewHit.Invoke(_member);
            if (onLostHit != null) foreach (var _member in PreviousHits.Except(DetectedHits)) onLostHit.Invoke(_member);
            if (onDetectCollider != null) foreach (var _member in DetectedColliders) onDetectCollider.Invoke(_member);
            if (onNewCollider != null) foreach (var _member in DetectedColliders.Except(PreviousColliders)) onNewCollider.Invoke(_member);
            if (onLostCollider != null) foreach (var _member in PreviousColliders.Except(DetectedColliders)) onLostCollider.Invoke(_member);
        }

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Receive all passing hits from the entered path ray." + HAccurate + HIPulse + HPathRay + HRDetector + HDependent;
#pragma warning restore CS0414

        protected readonly string[] CEventNames = {"onHit", "onNewHit", "onLostHit", "onDetectCollider", "onNewCollider", "onLostCollider"};
        internal override void OnGizmos()
        {
            EditorUpdate();
            DrawPath(pathRay.PathPoints, drawSphere:true, radius: (pathRay is IRadius _iRad ? _iRad.Radius+DotSize : 0f), dotted: true);
            Handles.color = DetectColor;
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                if (pathRay)
                {
                    BeginVerticalBox();
                    RCProEditor.TypeField("Path Ray", ref pathRay);
                    var _tSo = new SerializedObject(pathRay);
                    _tSo.Update();
                    pathRay?.EditorPanel(_tSo, hasMain: true, hasGeneral: false, hasEvents: false, hasInfo: false);
                    _tSo.ApplyModifiedProperties();
                    
                    EndVertical();
                }
                else RCProEditor.TypeField("Path Ray", ref pathRay);
            }

            if (hasGeneral)
            {
                GeneralField(_so);
                BaseField(_so);
            }
            
            if (hasEvents)
            {
                EventField(_so);
                if (EventFoldout) RCProEditor.EventField(_so, CEventNames);
            }
            if (hasInfo) InformationField(PanelGate);
        }

        protected override void DrawDetectorGuide(Vector3 point) { }
#endif
        public List<RaycastHit> DetectedHits = new List<RaycastHit>();
        public RaycastHit[] PreviousHits = Array.Empty<RaycastHit>();
        public HashSet<Collider> DetectedColliders = new HashSet<Collider>();
        public HashSet<Collider> PreviousColliders  = new HashSet<Collider>();
    }
}