namespace RaycastPro.Detectors2D
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif
    using UnityEngine;

    public abstract class ColliderDetector2D : Detector2D
    {

        protected RaycastHit2D _blockHit;
        
        public Collider2DEvent onDetectCollider;
        public Collider2DEvent onNewCollider;
        public Collider2DEvent onLostCollider;
        public List<Collider2D> DetectedColliders { get; protected set; } = new List<Collider2D>();
        public Collider2D[] PreviousColliders { get; protected set; } = Array.Empty<Collider2D>();

        public Collider2D[] ignoreList = Array.Empty<Collider2D>();
        
        /// <summary>
        /// This will find Smart Solver Point, Setup in: (OnEnable)
        /// </summary>
        protected Func<Collider2D, Vector2> DetectFunction;
        
        public override bool Performed
        {
            get => DetectedColliders.Count > 0; protected set {} }
        
        #region Methods
        public Collider2D FirstMember => DetectedColliders.FirstOrDefault();

        public Collider2D LastMember => DetectedColliders.LastOrDefault();
        
        /// <summary>
        /// It will calculate nearest collider based on current detected colliders on detector.
        /// </summary>
        /// <param name="nearest">Define a collider2D in your script and ref to it for get the nearest.</param>
        public void GetNearestCollider(ref Collider2D nearest)
        {
            var _cDistance = Mathf.Infinity;
            nearest = null;
            foreach (var _col in DetectedColliders)
            {
                _tDis = (_col.transform.position - transform.position).sqrMagnitude;
                if (_tDis < _cDistance)
                {
                    _cDistance = _tDis;
                    nearest = _col;
                }
            }
        }
        /// <summary>
        /// It will calculate the furthest collider based on current detected colliders on detector.
        /// </summary>
        /// <param name="furthest">Define a collider2D in your script and ref to it for get the furthest.</param>
        public void GetFurthestCollider(ref Collider2D furthest)
        {
            var _cDistance = 0f;
            furthest = null;
            foreach (var _col in DetectedColliders)
            {
                _tDis = (_col.transform.position - transform.position).sqrMagnitude;
                if (_tDis > _cDistance)
                {
                    _cDistance = _tDis;
                    furthest = _col;
                }
            }
        }
        #endregion
        
        protected void Start() // Refreshing
        {
            PreviousColliders = Array.Empty<Collider2D>();
            DetectedColliders = new List<Collider2D>();
        }
        
        protected float _tDis;

        protected void EventsCallback()
        {
            if (onDetectCollider != null)
            {
                foreach (var c in DetectedColliders) onDetectCollider.Invoke(c);
            }

            if (onNewCollider != null)
            {
                foreach (var c in DetectedColliders.Except(PreviousColliders)) onNewCollider.Invoke(c);
            }
            
            if (onLostCollider != null)
            {
                foreach (var c in PreviousColliders.Except(DetectedColliders)) onLostCollider.Invoke(c);
            }
        }

        /// <summary>
        /// Check Tag Filter and Ignore List
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected bool CheckGeneralPass(Collider2D c) => c && !ignoreList.Contains(c) && (!usingTagFilter || c.CompareTag(tagFilter));

        /// <summary>
        /// Sync Component Type List with detected colliders.
        /// </summary>
        /// <param name="detections"></param>
        /// <typeparam name="T"></typeparam>
        public void SyncDetection<T>(List<T> detections, Action<T> onNew = null, Action<T> onLost = null)
        {
            // Save States in list when new collider Detected
            onNewCollider.AddListener(C =>
            {
                if (C.TryGetComponent(out T instance))
                {
                    detections.Add(instance);
                    onNew?.Invoke(instance);
                }
            });
            // Save States in list when new collider Detected
            onLostCollider.AddListener(C =>
            {
                if (C.TryGetComponent(out T instance))
                {
                    detections.Remove(instance);
                    onLost?.Invoke(instance);
                }
            });
        }
        /// <summary>
        /// UnSync Component Type List with detected colliders.
        /// </summary>
        /// <param name="detections"></param>
        /// <typeparam name="T"></typeparam>
        public void UnSyncDetection<T>(List<T> detections, Action<T> onNew = null, Action<T> onLost = null)
        {
            onNewCollider?.RemoveListener(C =>
            {
                if (C.TryGetComponent(out T instance))
                {
                    detections.Add(instance);
                    onNew?.Invoke(instance);
                }

            });
            onLostCollider?.RemoveListener(C =>
            {
                if (C.TryGetComponent(out T instance))
                {
                    detections.Remove(instance);
                    onLost?.Invoke(instance);
                }
            });
        }
        private void OnEnable() => DetectFunction = SetupDetectFunction();
#if UNITY_EDITOR
        protected readonly string[] CEventNames = {"onDetectCollider", "onNewCollider", "onLostCollider"};

        private void OnValidate()
        {
            DetectFunction = SetupDetectFunction();
        }

        protected Color GetPolygonColor(bool blockCondition = false) =>
            (blockCondition ? BlockColor : DetectedColliders.Any() ? DetectColor : DefaultColor).ToAlpha(RCProPanel.alphaAmount);

        protected void IgnoreListField(SerializedObject _so)
        {
            BeginVerticalBox();
            RCProEditor.PropertyArrayField(_so.FindProperty(nameof(ignoreList)), "Ignore List".ToContent(),
                (i) => $"Collider {i+1}".ToContent($"Index {i}"));
            EndVertical();
        }
#endif
    }
}