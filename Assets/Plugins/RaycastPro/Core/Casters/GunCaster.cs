namespace RaycastPro
{
    using UnityEngine;
    using System;
    using Bullets;
    using Bullets2D;
    using RaySensors;
    using RaySensors2D;
    using Casters;
    using Casters2D;
    using System.Collections;
    using UnityEngine.Events;
    
#if UNITY_EDITOR
    using UnityEditor;
    using Editor;
#endif

    [Serializable]
    public class BulletEvent : UnityEvent<Bullet> { }
    
    [Serializable]
    public class BulletEvent2D : UnityEvent<Bullet2D> { }
    public abstract class GunCaster<B, C, R> : BaseCaster where B : BaseBullet where C : UnityEngine.Object
    {
        [SerializeField]
        public B[] bullets = Array.Empty<B>();

        [SerializeField]
        public B[] cloneBullets = Array.Empty<B>();
        
        [SerializeField]
        public C[] ignoreColliders = Array.Empty<C>();

        [SerializeField] public Ammo ammo;
        
        [SerializeField]
        public Transform trackTarget;

        [SerializeField] protected int index;
        [SerializeField] protected int cloneIndex;
        [SerializeField] public bool usingAmmo = true;

        [SerializeField] private bool arrayCasting;
        
        [SerializeField] protected int arrayCapacity = 6;
        
        /// <summary>
        /// Performed in Caster only works with Array Casting
        /// </summary>
        public override bool Performed {
            get
            {
                var any = false;
                foreach (var cloneBullet in cloneBullets)
                {
                    if (cloneBullet && cloneBullet.isActiveAndEnabled) any = true;
                }
                return any;
            }
            protected set {} }
        public enum CastType
        {
            /// <summary>
            /// 0
            /// </summary>
            Together,
            /// <summary>
            /// 1
            /// </summary>
            Sequence,
            /// <summary>
            /// 2
            /// </summary>
            Random,
            /// <summary>
            /// 3
            /// </summary>
            PingPong
        }

        private B tempBullet;
        
        private void OnArrayCast(B bulletObject, Vector3 basePoint)
        {
            if (cloneBullets[cloneIndex])
            {
                // Replacing
                if (bulletObject.bulletID != cloneBullets[cloneIndex].bulletID)
                {
                    Destroy(cloneBullets[cloneIndex].gameObject);
                    tempBullet = Instantiate(bulletObject, basePoint, transform.rotation);
                    // Set Clone Index to this bullet
                    cloneBullets[cloneIndex] = tempBullet;
                    BulletSetup(tempBullet);
                }
                else // Refreshing
                {
                    tempBullet = cloneBullets[cloneIndex];
                    Refresh_ArrayCasting_Clone(tempBullet);
                }
            }
            else
            {
                tempBullet = Instantiate(bulletObject, basePoint, transform.rotation);
                // Set Clone Index to this bullet
                cloneBullets[cloneIndex] = tempBullet;
                BulletSetup(tempBullet);
            }
            
            // Go To Next Clone
            cloneIndex = ++cloneIndex % cloneBullets.Length;
            // End function must be to "Disable"
            tempBullet.endFunction = BaseBullet.EndType.Disable;
        }

        private void OnNormalCast(B bulletObject, Vector3 basePoint)
        {
            tempBullet = Instantiate(bulletObject, basePoint, transform.rotation);
            tempBullet.endFunction = BaseBullet.EndType.Destroy;
            BulletSetup(tempBullet);
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private void Refresh_ArrayCasting_Clone(B _bullet)
        {
            _bullet.gameObject.SetActive(true);
            _bullet.enabled = true;
            _bullet.position = 0;
            _bullet.Life = 0;
        }

        private Vector3 basePoint;
        private B _bulletObject;

        public override void Reload()
        {
            ammo.inRate = false;
            ammo.inReload = false;
        }
        protected bool BulletCast(int _index, R _raySensor, Action<B> onCast = null)
        {
            if (usingAmmo && !ammo.Use(this)) return false;
            _bulletObject = bullets[_index];

            if (_raySensor != null)
            {
                if (_raySensor is RaySensor r) basePoint = r.BasePoint;
                else if (_raySensor is RaySensor2D r2D) basePoint = r2D.BasePoint;
            }
            else
            {
                basePoint = transform.position;
            }

            if (arrayCasting) OnArrayCast(_bulletObject, basePoint);
            else OnNormalCast(_bulletObject, basePoint);
            tempBullet.Cast(this, _raySensor);
            onCast?.Invoke(tempBullet);
            return true;
        }
        
        protected bool BulletCast(int _index, R[] _raySensor)
        {
            if (usingAmmo && !ammo.Use(this, _raySensor.Length)) return false;
            var bulletObject = bullets[_index];
            if (arrayCasting) // IN ARRAY CASTING
            {
                foreach (var rayS in _raySensor)
                {
                    if (rayS is RaySensor r) basePoint = r.BasePoint;
                    else if (rayS is RaySensor2D r2D) basePoint = r2D.BasePoint;
                    OnArrayCast(bulletObject, basePoint);
                    tempBullet.Cast(this, rayS);
                }
            }
            else // IN NORMAL CASTING
            {
                foreach (var rayS in _raySensor)
                {
                    if (rayS is RaySensor r) basePoint = r.BasePoint;
                    else if (rayS is RaySensor2D r2D) basePoint = r2D.BasePoint;
                    OnNormalCast(bulletObject, basePoint);
                    tempBullet.Cast(this, rayS);
                }
            }
            return true;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private void BulletSetup(B _bullet)
        {
            if (ignoreColliders.Length > 0)
            {
                if (typeof(C) == typeof(Collider))
                {
                    var bulletColliders = _bullet.GetComponentsInChildren<Collider>();
                    IgnoreCollider(bulletColliders, Array.ConvertAll(ignoreColliders, i => i as Collider));
                }
                else if (typeof(C) == typeof(Collider2D))
                {
                    var bulletColliders = _bullet.GetComponentsInChildren<Collider2D>();
                    IgnoreCollider(bulletColliders, Array.ConvertAll(ignoreColliders, i => i as Collider2D));
                }
            }
                
            // Add Track Target 
            if (trackTarget)
            {
                if (_bullet is TrackerBullet _trB) _trB.target = trackTarget;
                else if (_bullet is TrackerBullet2D _trB2D)  _trB2D.target = trackTarget;
            }
            
            // Rotation Fixer
            _bullet.transform.up = transform.up;
        }
        
        protected Coroutine multiCast;
        protected IEnumerator IMultiCast(int _index, int count)
        {
            if (ammo.magazineAmount == 0) ammo.IReload();
            
            for (int i = 0; i < count; i++)
            {
                while (ammo.IsInRate)
                {
                    yield return new WaitForEndOfFrame();
                    if (ammo.IsReloading) // Stops when magazine Off
                    {
                        break;
                    }
                }
                Cast(_index);
            }
        }
        public void MultipleCast(int _index, int count)
        {
            if (multiCast != null) StopCoroutine(multiCast);
            
            multiCast = StartCoroutine(IMultiCast(_index, count));
        }

#if UNITY_EDITOR


        protected const string CTogether = "Together";
        protected const string CSequence = "Sequence"; 
        protected const string CRandom = "Random"; 
        protected const string CPingPong = "PingPong";
        
        protected const string TTogether = "Shooting the index bullet in all RaySensors Together.";
        protected const string TSequence = "Shooting in sequence of RaySensors index in wait of ammo reload Time and In Between."; 
        protected const string TRandom = "Random Shooting in any RaySensor."; 
        protected const string TPingPong = "Shooting bullets back and forth on the RaySensor index.";
        /// <summary>
        /// This Script worked but No using for now
        /// </summary>
        /// <param name="property"></param>
        protected void BulletField(SerializedProperty property)
        {
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.BeginProperty(rect, property.displayName.ToContent(), property);
            EditorGUI.BeginChangeCheck();
            var newBullet = EditorGUI.ObjectField(rect, property.objectReferenceValue, typeof(Bullet), false);
            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = newBullet;
            }
            EditorGUI.EndProperty();
        }
        protected void GeneralField(SerializedObject _so)
        {
            #region ArrayCastingField
            BeginHorizontal();

            GUI.enabled = !Application.isPlaying;
            
            var arrCasting = _so.FindProperty(nameof(arrayCasting));
            var arrCapacity = _so.FindProperty(nameof(arrayCapacity));
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(arrCasting,
                (arrCasting.boolValue ? "Array Capacity" : CArrayCasting).ToContent(TArrayCasting));
            GUI.enabled &= arrCasting.boolValue;
            PropertyMaxIntField(arrCapacity, GUIContent.none, 1);
            if (EditorGUI.EndChangeCheck())
            {
                if (arrCasting.boolValue)
                {
                    arrCapacity.intValue = Mathf.Max(1, arrCapacity.intValue);
                    _so.FindProperty(nameof(cloneBullets)).arraySize = arrCapacity.intValue;
                }
                else cloneBullets = null;
            }
            
            GUI.enabled = true;
            EndHorizontal();
            #endregion
            BeginVerticalBox();
            
            var bulletsProp = _so.FindProperty(nameof(bullets));
            RCProEditor.PropertyArrayField(bulletsProp,
                "Bullets".ToContent("Bullets"), i => $"Bullet {i+1}".ToContent($"Index {i}"));
            PropertySliderField(_so.FindProperty(nameof(index)), 0,  bullets != null ? Mathf.Max(bulletsProp.arraySize-1,0) : 0 , "Index".ToContent(),
                I => { });
            EndVertical();

            if (!(this is BasicCaster || this is BasicCaster2D))
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(trackTarget)),
                    "Track Target".ToContent("Track Target"));
            }
            BeginVerticalBox();
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(usingAmmo)), "Using Ammo".ToContent("Using Ammo"));
            if (usingAmmo)
            {
                if (ammo == null) ammo = new Ammo();

                var ammoProp = _so.FindProperty(nameof(ammo));

                ammo?.EditorPanel(ammoProp);
            }
            else ammo = null;
            EndVertical();
            
            BaseField(_so, hasInteraction: false);
            
            BeginVerticalBox();
            RCProEditor.PropertyArrayField(_so.FindProperty(nameof(ignoreColliders)), "Ignore List".ToContent(),
                (i) => $"Collider {i+1}".ToContent($"Index {i}"));
            EndVertical();
        }
        protected void InformationField()
        {
            InformationField(() =>
            {
                if (cloneBullets == null) return;

                for (var i = 0; i < cloneBullets.Length; i++)
                {
                    if (!cloneBullets[i]) continue;

                    var cloneBullet = cloneBullets[i];
                    BeginHorizontal();
                    EditorGUILayout.LabelField($"{i} {cloneBullet.GetInstanceID().ToString()}",
                        GUILayout.Width(200));
                    ProgressField(cloneBullet.life/cloneBullet.lifeTime, CLifeTime);
                    EndHorizontal();
                }
            });
        }
#endif
    }
}