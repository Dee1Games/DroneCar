namespace RaycastPro.Detectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    using UnityEngine;
    public abstract class ColliderDetector : Detector
    {
        public ColliderEvent onDetectCollider;
        public ColliderEvent onNewCollider;
        public ColliderEvent onLostCollider;
        public override bool Performed
        {
            get => DetectedColliders.Count > 0;
            protected set { }
        }
        
        [SerializeField] public Collider[] ignoreList = Array.Empty<Collider>();
        
        [SerializeField]
        protected Collider[] colliders = Array.Empty<Collider>();

        /// <summary>
        /// Main List of Detected colliders. Use this for Get Colliders.
        /// </summary>
        public List<Collider> DetectedColliders { get; protected set; } = new List<Collider>();
        
        /// <summary>
        /// Array of Last Frame Detected colliders.
        /// </summary>
        public Collider[] PreviousColliders { get; protected set; } = new Collider[]{};
        
        #region Methods
        public Collider FirstMember => DetectedColliders.FirstOrDefault();

        public Collider LastMember => DetectedColliders.LastOrDefault();
        #endregion

        #region Lambdas

        public Vector3 GetAveragePosition
        {
            get
            {
                var average = Vector3.zero;
                
                foreach (var c in DetectedColliders) average += c.transform.position;
                
                return average / DetectedColliders.Count;
            }
        }

        #endregion
        #region Public Methods

        public void ActiveObject(Collider collider) => collider.gameObject.SetActive(true);
        
        public void DeactiveObject(Collider collider) => collider.gameObject.SetActive(false);
        
        public void InstantiateOnDetections(GameObject obj)
        {
            foreach (var c in DetectedColliders) Instantiate(obj, c.transform.position, c.transform.rotation);
        }
        
        public void ApplyExplosionForce(float force, float radius)
        {
            foreach (var c in DetectedColliders)
            {
                if (c.TryGetComponent(out Rigidbody _r))
                {
                    _r.AddExplosionForce(force, transform.position, radius);
                }
            }
        }
        public void AddForceToDetections(float force)
        {
            foreach (var c in DetectedColliders)
            {
                if (c.TryGetComponent(out Rigidbody _r))
                {
                    _r.AddForce((c.transform.position-transform.position).normalized * force, ForceMode.Force);
                }
            }
        }
        public void AddDynamicForceToDetections(float force)
        {
            foreach (var c in DetectedColliders)
            {
                if (c.TryGetComponent(out Rigidbody _r))
                {
                    _r.AddForce((c.transform.position-transform.position) * force, ForceMode.Force);
                }
            }
        }
        public void AddGravityForceToDetections(float radius, float force)
        {
            foreach (var c in DetectedColliders)
            {
                if (c.TryGetComponent(out Rigidbody _r))
                {
                    var direction = c.transform.position - transform.position;
                    _r.AddForce((radius*radius - direction.sqrMagnitude) * direction * force, ForceMode.Force);
                }
            }
        }
        public void DestroyDetections(float delay)
        {
            foreach (var c in DetectedColliders) Destroy(c.gameObject, delay);
        }
        #endregion

        protected void Start() // Refreshing
        {
            PreviousColliders = Array.Empty<Collider>();
            DetectedColliders = new List<Collider>();
        }

        /// <summary>
        /// Call: onDetectCollider, OnDetectNew, OnLostDetect in Optimized foreach loop
        /// </summary>
        protected void ColliderDetectorEvents()
        {
            
            if (onDetectCollider != null) foreach (var c in DetectedColliders) onDetectCollider.Invoke(c);
            if (PreviousColliders.Length == DetectedColliders.Count) return;
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

        protected Func<Collider, Vector3> DetectFunction;
        private void OnEnable() => DetectFunction = SetupDetectFunction();
        protected bool CheckGeneralPass(Collider c) => c && !ignoreList.Contains(c) && (!usingTagFilter || c.CompareTag(tagFilter));
        protected bool CheckGeneralPass(GameObject g) => g && (!usingTagFilter || g.CompareTag(tagFilter));
#if UNITY_EDITOR
        
        protected readonly string[] CEventNames = {"onDetectCollider", "onNewCollider", "onLostCollider"};
        protected bool GuideCondition =>
            RCProPanel.DrawGuide && DetectedColliders.Count <= RCProPanel.DrawGuideLimitCount;
        protected override void AfterValidate() => DetectFunction = SetupDetectFunction();
        protected void ColliderDetectorGeneralField(SerializedObject _so)
        {
            GeneralField(_so);
            NonAllocatorField(_so, _so.FindProperty(nameof(colliders)));
            BaseField(_so);
            SolverField(_so);
            IgnoreListField(_so);
        }
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