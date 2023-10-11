namespace RaycastPro
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    public enum ChainReference
    {
        /// <summary>
        /// As setup reference to transform, You could animate chain points on playmode.
        /// </summary>
        Transform,
        Point,
    }
    
    public enum ArcType
    {
        /// <summary>
        /// Trajectory arc type follows fixed acceleration force.
        /// </summary>
        Trajectory,

        /// <summary>
        /// Follows a dynamic force to the target position.
        /// </summary>
        Target
    }
    
    [Serializable]
    public class RaycastEvent : UnityEvent<RaycastHit> { }
    [Serializable]
    public class RaycastEvent2D : UnityEvent<RaycastHit2D> { }


    
    /// <summary>
    /// This class is placed above the RaySensor2D and RaySensor classes and includes their common features.
    /// </summary>
    /// <typeparam name="R">RaycastHit Type (2D, 3D)</typeparam>
    /// <typeparam name="E">Event Type (2D, 3D)</typeparam>

    public abstract class BaseRaySensor<R, E, P> : RaycastCore where P : BasePlanar
    {
        internal  R hit;
        /// <summary>
        /// Returns the current Hit.
        /// </summary>
        public R Hit => hit;
        /// <summary>
        /// Returns Hit on previous Cast.
        /// </summary>
        public R PreviousHit { protected set; get; }
        
        [Tooltip("In short of line renderer, that full follow the ray path")]
        public LineRenderer liner;

        /// <summary>
        /// When true, you can setup liner end position manually.
        /// </summary>
        public bool useLinerClampedPosition;
        
        public bool cutOnHit;
        /// <summary>
        /// End point of liner in percent value between(0, 1).
        /// </summary>
        public float linerEndPosition = 1f;

        public float linerBasePosition;
        
        [Tooltip("A Transform handler on ray body with controlling option")]
        public Transform stamp;
        /// <summary>
        /// The local space affect rotation
        /// </summary>
        public bool local = true;

        /// <summary>
        /// This ray will affect of Lossy Scale.
        /// </summary>
        public bool scalable = false;

        public bool stampOnHit;
        public bool stampAutoHide;
        public float stampOffset;
        
        [SerializeField]
        internal AxisRun syncStamp = new AxisRun();

        /// <summary>
        /// When true, ray will affect and clone by planers, just make sure your planar detect layer includes the ray layer.
        /// </summary>
        public bool planarSensitive;
        
        /// <summary>
        /// Current Planar Detected in "AnyPlanar" mode.
        /// </summary>
        [SerializeField] protected P _planar;
        
        /// <summary>
        /// Planers supported for reaction.
        /// </summary>
        [SerializeField] public P[] planers = Array.Empty<P>();

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] internal bool anyPlanar = true;
        
        #region Events
        /// <summary>
        /// Invoke when ray is activate and does casting.
        /// </summary>
        public UnityEvent onCast;

        /// <summary>
        /// Called every frame on cast when hit is detected. (Output: Hit)
        /// </summary>
        public E onDetect;
        /// <summary>
        /// Called per time when Hit changes. (Output: Hit)
        /// </summary>
        public E onChange;
        /// <summary>
        /// Called first frame when Hit begin detect. (Output: Hit)
        /// </summary>
        public E onBeginDetect;
        /// <summary>
        /// Called first frame when Hit detection lost. (Output: PreviousHit)
        /// </summary>
        public E onEndDetect;
        
        #endregion

        
        /// <summary>
        /// return's tip of ray in world space.
        /// </summary>
        public abstract Vector3 Tip { get; }
        public abstract Vector3 TipTarget { get; }
        /// <summary>
        /// return's total ray length.
        /// </summary>
        public abstract float RayLength { get; }
        /// <summary>
        /// return's vector of base ray point to tip. (Form: Tip - BasePoint)
        /// </summary>
        public Vector3 TipDirection => Tip - Base;
        /// <summary>
        /// return's "-Hit.Normal" when detect and "HitDirection" in default.
        /// </summary>
        public abstract Vector3 TargetDirection { get; }
        /// <summary>
        /// return's vector of base ray point to tip. (Form: TipDirection.magnitude)
        /// </summary>
        public float TipLength => TipDirection.magnitude;
        public abstract Vector3 Base { get; }
        public abstract bool ClonePerformed { get; }

        #region Public Methods

        public void SetLinerPosition(float position) => linerEndPosition = position;

        public void InstantiateOnTip(GameObject obj) =>
            Instantiate(obj, Tip, Quaternion.LookRotation(TipDirection));

        public void InstantiateOnTargetTip(GameObject obj) =>
            Instantiate(obj, TipTarget, Quaternion.LookRotation(TipDirection));

        public void SetPlanarSensitive(bool toggle) => planarSensitive = toggle;

        public void SetLiner(LineRenderer lineRenderer)
        {
            liner = lineRenderer;
            UpdateLiner();
        }

        public void SetStamp(Transform newStamp)
        {
            stamp = newStamp;
            UpdateStamp();
        }
        
        public Vector3 GetPositionOnPath(float pos, ref List<Vector3> path, out Vector3 forward)
        {
            forward = path.LastDirection(Vector3.up);
            for (var i = 1; i < path.Count; i++)
            {
                var _lDistance = path.GetEdgeLength(i);
                if (pos < _lDistance)
                {
                    forward = path[i] - path[i - 1];
                    return Vector3.Lerp(path[i - 1], path[i], pos / _lDistance);
                }
                pos -= _lDistance;
            }

            return path.LastOrBase(Vector3.zero);
        }
        #endregion

        #region Updates

        internal abstract void RuntimeUpdate();

        // ReSharper disable Unity.PerformanceAnalysis
        public bool Cast()
        {
#if UNITY_EDITOR
            alphaCharge = AlphaLifeTime;
#endif
            RuntimeUpdate();
            return Performed;
        }
        
        protected void Update()
        {
            if (autoUpdate != UpdateMode.Normal) return;
            RuntimeUpdate();
        }
        protected void LateUpdate()
        {
            if (autoUpdate != UpdateMode.Late) return;
            RuntimeUpdate();
        }
        protected void FixedUpdate()
        {
            if (autoUpdate != UpdateMode.Fixed) return;
            RuntimeUpdate();
        }
        #endregion
        /// <summary>
        /// To use this method, set the update mode to manual. This code updates "IsDetect, Hit, Liner, Stamp and Events".
        /// </summary>

        /// <summary>
        /// Update the Line Renderer points on the ray path.
        /// </summary>
        public abstract void UpdateLiner();
        public abstract void UpdateStamp();
        internal abstract void OnDetect();
        internal abstract void OnBeginDetect();
        internal abstract void OnEndDetect();
        protected void OnDestroy()
        {
            SafeRemove();
        }
        internal abstract void SafeRemove();

#if UNITY_EDITOR
        protected abstract void EditorUpdate();
        protected void DirectionField(SerializedObject _so, bool _local = true)
        {
            if (_local) BeginHorizontal();
            
            EditorGUILayout.PropertyField(_so.FindProperty("direction"), CDirection.ToContent(TDirection));

            if (_local)
            {
                LocalField(_so.FindProperty("local"));
                EndHorizontal();
            }
        }
        protected void PlanarField(SerializedObject _so)
        {
            BeginHorizontal();
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(planarSensitive)), CPlanarSensitive.ToContent(TPlanarSensitive),
                true);
            GUI.enabled = planarSensitive;
            EditorGUILayout.LabelField( "Any".ToContent("Any Planar"), GUILayout.Width(40f));
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(anyPlanar)), GUIContent.none,
                true,GUILayout.ExpandWidth(false), GUILayout.Width(15));
            GUI.enabled = true;
            EndHorizontal();
            if (planarSensitive && !anyPlanar)
            {
                BeginVerticalBox();
                RCProEditor.PropertyArrayField(_so.FindProperty(nameof(planers)), "Planers".ToContent(),
                    (i) => $"Planar {i+1}".ToContent($"Index {i}"));
                EndVertical();
            }
        }
        protected void StampField(SerializedObject _so)
        {
            if (stamp) BeginVerticalBox();
            EditorGUILayout.ObjectField(_so.FindProperty(nameof(stamp)), CStamp.ToContent(TStamp));
            if (!stamp) return;
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(stampOnHit)),
                CStampOnHit.ToContent(TStampOnHit));
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(stampAutoHide)),
                CStampAutoHide.ToContent(TStampAutoHide));
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(stampOffset)),
                CStampOffset.ToContent(TStampOffset));
            syncStamp.EditorPanel(_so.FindProperty(nameof(syncStamp)));
            EndVertical();
        }
        // ReSharper disable Unity.PerformanceAnalysis
        protected void LinerField(SerializedObject _so)
        {
            if (liner) BeginVerticalBox();
            BeginHorizontal();
            var prop = _so.FindProperty(nameof(liner));
            EditorGUILayout.PropertyField(prop, CLiner.ToContent(TLiner));

            if (!liner && GUILayout.Button(CAdd, GUILayout.Width(50f)))
            {
                // Fixed Liner Problem
                if (TryGetComponent(out LineRenderer lineRenderer)) liner = lineRenderer;
                else
                {
                    prop.objectReferenceValue = gameObject.AddComponent<LineRenderer>();
                    _so.ApplyModifiedProperties();
                    liner.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
                }

                liner.endWidth = Mathf.Min(RCProPanel.linerMaxWidth, .1f);
                liner.startWidth = Mathf.Min(RCProPanel.linerMaxWidth, .1f);
                liner.numCornerVertices = Mathf.Min(0, 6);
                liner.numCapVertices = Mathf.Min(0, 6);
                
                UpdateLiner();
            }

            EndHorizontal();

            #region Liner Setting

            if (!liner) return;

            EditorGUILayout.PropertyField(_so.FindProperty(nameof(useLinerClampedPosition)), "Clamped Position".ToContent("Clamped Position"));
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(cutOnHit)), "Cut on Hit".ToContent("Cut line renderer when hit on something."));

            GUI.enabled = useLinerClampedPosition;
            PropertyMinMaxField(_so.FindProperty(nameof(linerBasePosition)), _so.FindProperty(nameof(linerEndPosition)), ref linerBasePosition, ref linerEndPosition, 0, 1);
            GUI.enabled = true;

            liner.startWidth = EditorGUILayout.Slider(CStartWidth, liner.startWidth, 0f, RCProPanel.linerMaxWidth);
            liner.endWidth = EditorGUILayout.Slider(CEndWidth, liner.endWidth, 0f, RCProPanel.linerMaxWidth);
            liner.numCapVertices = EditorGUILayout.IntField(CCap, liner.numCapVertices);
            liner.numCornerVertices = EditorGUILayout.IntField(CCorner, liner.numCornerVertices);
            
            GUI.backgroundColor = Color.white;
            liner.colorGradient = EditorGUILayout.GradientField(CGradient, liner.colorGradient);
            GUI.backgroundColor = RCProEditor.Violet;

            EndVertical();

            #endregion
        }
        
        
/// <summary>
/// Main Event names
/// </summary>
        protected readonly string[] CEventNames = {"onDetect", "onBeginDetect", "onEndDetect", "onChange","onCast"};
        protected void EventField(SerializedObject _so)
        {
            EventFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(EventFoldout, CEvents.ToContent(TEvents),
                RCProEditor.HeaderFoldout);
            if (EventFoldout)  RCProEditor.EventField(_so, CEventNames);

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        protected static void ArcTypeField(ref ArcType arcType, Action value, ref Transform target, ref float velocityPower)
        {
            BeginVerticalBox();
            arcType = RCProEditor.EnumLabelField(arcType, "Arc Type".ToContent(), new[] {"A", "B"});
            
            switch (arcType)
            {
                case ArcType.Trajectory:
                {
                    value?.Invoke();
                    
                    // velocitySpace = RaycastProEditor.EnumLabelField(velocitySpace,
                    //     "Velocity Space".ToContent("Velocity Space"), new[] {"Local", "World"});

                    break;
                }
                case ArcType.Target:
                    RCProEditor.TypeField(CTarget, ref target);

                    velocityPower = EditorGUILayout.FloatField("Velocity Power", velocityPower);
                    break;
            }
            EndVertical();
        }
#endif
    }
}