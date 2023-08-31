namespace RaycastPro
{
    using Bullets;
    using Bullets2D;
    using UnityEngine;
    using System;
    using UnityEngine.Events;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    public enum MoveType
    {
        Speed,
        Duration,
        Curve,
    }
    
    [Serializable]
    public class CasterEvent : UnityEvent<BaseCaster> { }

    public abstract class BaseBullet : RaycastCore
    {
        public override bool Performed { get => false; protected set {} }
        
        public TimeMode timeMode = TimeMode.DeltaTime;
        
        public string bulletID;
        
        /// <summary>
        /// Invoke on Cast
        /// </summary>
        public CasterEvent onCast;
        /// <summary>
        /// Ending Before Delay
        /// </summary>
        public CasterEvent onEndCast;
        /// <summary>
        /// Ending On Delay
        /// </summary>
        public CasterEvent onEnd;
        
        //protected Coroutine RunCoroutine;
        public MoveType moveType = MoveType.Speed;

        internal float position;

        /// <summary>
        /// Set Bullet Position as "Clamp01" on the path.
        /// </summary>
        public float Position
        {
            get => position;
            set => position = Mathf.Clamp01(position);
        }
        
        [SerializeField]
        public bool hasCollision = true;
        [SerializeField]
        public bool planarSensitive = true;
        
        [SerializeField] internal float radius = .2f;
        
        [SerializeField]
        public float offset = 0f;
        
        [SerializeField]
        public float length = .5f;
        
        public float Radius
        {
            get => radius;
            set => radius = Mathf.Max(0,value);
        }

        [SerializeField] private string callMethod = "OnBullet";
        [SerializeField] private bool messageUpward;
        
        [SerializeField] public float damage = 10;
        public float speed = 6;
        /// <summary>
        /// Life time from instantiate. (use negative to infinite Life)
        /// </summary>
        public float lifeTime = 10;
        internal float life;
        public float Life
        {
            get => life;
            internal set => life = value;
        }

        public float endDelay;
        public enum EndType { Disable, Destroy }

        [SerializeField]
        public EndType endFunction = EndType.Destroy;

        #region Puplic Methods

        public void InstantiateOnPosition(GameObject prefab) =>
            Instantiate(prefab, transform.position, transform.rotation);

        #endregion
        
        #region Updates

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
        public abstract void RuntimeUpdate();
        #endregion
        internal abstract void Cast<R>(BaseCaster _caster, R raySensor);
        protected void OnEnd()
        {
            position = 1;
            
            if (this is Bullet _b)
            {
                onEndCast?.Invoke(_b.caster);
                onEnd?.Invoke(_b.caster);
            }
            else if (this is Bullet2D _b2d)
            {
                onEndCast?.Invoke(_b2d.caster);
            }
            
            if (endFunction == EndType.Destroy)
            {
                Destroy(gameObject, endDelay);
            }
            else
            {
                StartCoroutine(DelayRun(endDelay, () => gameObject.SetActive(false)));
            }
        }
        
        /// <summary>
        /// This Script need to be divided for more optimizing.
        /// </summary>
        /// <param name="target"></param>
        protected void InvokeDamageEvent(Transform target)
        { if (callMethod == "") return;

            if (messageUpward)
            {
                if (this is Bullet _blt)    
                {
                    target.SendMessageUpwards(callMethod, _blt, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    target.SendMessageUpwards(callMethod, this as Bullet2D, SendMessageOptions.DontRequireReceiver);
                }
            }
            
            else
            {
                if (this is Bullet _blt)
                {
                    target.SendMessage(callMethod, _blt, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    target.SendMessage(callMethod, this as Bullet2D, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        
        /// <summary>
        /// Optimized if delta is calculated before run this method
        /// </summary>
        /// <param name="delta"></param>
        protected void UpdateLifeProcess(float delta)
        {
            if (life >= lifeTime && lifeTime >= 0) OnEnd();
            else life += delta;
        }

#if UNITY_EDITOR
        protected void CastTypeField(SerializedProperty _moveType, SerializedProperty _speed, SerializedProperty _duration,
            SerializedProperty _curve)
        {
            BeginVerticalBox();
            
            PropertyEnumField(_moveType, 3, CMoveType.ToContent(TMoveType), new GUIContent[]
            {
                CSpeed.ToContent("By Speed"),
                CDuration.ToContent("By Duration"),
                CCurve.ToContent("By Animation Curve"),
            });

            switch (moveType)
            {
                case MoveType.Speed:
                    EditorGUILayout.PropertyField(_speed);
                    break;
                case MoveType.Duration:
                    EditorGUILayout.PropertyField(_duration);
                    break;
                case MoveType.Curve:
                    BeginHorizontal();
                    

                    InLabelWidth(() =>
                    {
                        EditorGUILayout.CurveField(_curve, RCProEditor.Aqua, new Rect(0, 0, 1, 1), CCurve.ToContent(CCurve));

                    }, 80);
                    
                    EditorGUILayout.PropertyField(_duration, GUIContent.none, GUILayout.Width(30));

                    EndHorizontal();
                    break;
            }

            EndVertical();
        }
        protected void EventField(SerializedObject _so)
        {
            EventFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(EventFoldout, CEvents.ToContent(TEvents),
                RCProEditor.HeaderFoldout());
            if (EventFoldout) RCProEditor.EventField(_so, events); EditorGUILayout.EndFoldoutHeaderGroup();
        }

        protected static readonly string[] events = new[] {nameof(onCast), nameof(onEndCast), nameof(onEnd)};
        protected void GeneralField(SerializedObject _so)
        {
            BeginVerticalBox();
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(lifeTime)), CLifeTime.ToContent(TLifeTime));
            PropertyMaxField(_so.FindProperty(nameof(endDelay)), "End Delay".ToContent("Disable bullet before end"));
            PropertyTimeModeField(_so.FindProperty(nameof(timeMode)));
            var hasCollisionProp = _so.FindProperty(nameof(hasCollision));
            EditorGUILayout.PropertyField(hasCollisionProp);
            GUI.enabled = hasCollisionProp.boolValue;
            DetectLayerField(_so);
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(length)),
                CLength.ToContent(CLength));
            RadiusField(_so);
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(offset)), COffset.ToContent(COffset));
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(planarSensitive)), CPlanarSensitive.ToContent(TPlanarSensitive));
            GUI.enabled = true;
            EndVertical();
            BeginVerticalBox();
            BeginHorizontal();
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(callMethod)), "Call Method".ToContent("Calls this method on every MonoBehaviour in bullet's hit target." +
                "\nex: ----------------------------------"+
                "\npublic void OnBullet(Bullet _bullet)" +
                "\n{" +
                "\n    Add_Character_Hp(-_bullet.damage);"+
                "\n}" +
                "\n -------------------------------------"+
                "\n(Just leave it empty to cancel messaging.)"));

            LocalField(_so.FindProperty(nameof(messageUpward)), "U".ToContent("Calls the method on every ancestor of the behaviour in addition to every MonoBehaviour."));
            EndHorizontal();
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(damage)));
            EndVertical();
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(bulletID)),
                "BulletID".ToContent("Set it for renew bullet type in array casting mode."));
            BaseField(_so);
        }
#endif
    }
}