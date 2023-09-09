using System.Threading.Tasks;

namespace RaycastPro
{
    using System;
    using System.Collections;
    using UnityEngine;


#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    public abstract class BaseCaster : RaycastCore
    {
        /// <summary>
        /// Allow to manuel Casting.
        /// </summary>
        /// <param name="_index">Bullet Array Index</param>
        public abstract void Cast(int _index);

        [Tooltip("Automatic creation of Pool object at start.")]
        [SerializeField] protected bool createPoolAtStart = true;
        
        [Tooltip("Automatically instantiate into this object.")]
        [SerializeField] public Transform poolManager;
        public abstract void Reload();
        #region Update
        protected void Update()
        {
            if (autoUpdate != UpdateMode.Normal) return;
            OnCast();
        }
        protected void LateUpdate()
        {
            if (autoUpdate != UpdateMode.Late) return;
            OnCast();
        }
        protected void FixedUpdate()
        {
            if (autoUpdate != UpdateMode.Fixed) return;
            OnCast();
        }

        #endregion

        protected static void CopyBullet<T>(T to, T from)
        {
            var fields = to.GetType().GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(to, field.GetValue(from));
            }

            var props = to.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(to, prop.GetValue(from, null), null);
            }
        }

        private void Awake()
        {
            if (createPoolAtStart)
            {
                poolManager = new GameObject($"==={name} Pool===").transform;
            }
        }

        [Serializable]
        public class Ammo
        {
            /// <summary>
            /// If it is active, the cost of shooting will be zero.
            /// </summary>
            [SerializeField] public bool infiniteAmmo;
            /// <summary>
            /// The total number of bullets out of the magazine
            /// </summary>
            [SerializeField] public int amount = 12;

            /// <summary>
            /// The Capacity of each magazine that will enter the reload time when it runs out.
            /// </summary>
            [SerializeField] public int magazineCapacity = 6;
            /// <summary>
            /// The number of bullets in the current magazine.
            /// </summary>
            [SerializeField] public int magazineAmount = 6;

            /// <summary>
            /// Interruption time until the magazine is filled
            /// </summary>
            [SerializeField] public float reloadTime = 2f;
            
            /// <summary>
            /// The firing pause time between each shot
            /// </summary>
            [SerializeField] public float inBetweenTime = .1f;

            /// <summary>
            /// The number of all available bullets, which is calculated from the "amount + magazineAmount".
            /// </summary>
            public int Total => amount + magazineAmount;

            [SerializeField] internal bool inRate;
            [SerializeField] internal bool inReload;
            public bool IsReloading => inReload;
            public bool IsInRate => inRate;

            public int MagazineAmount
            {
                get => magazineAmount;
                set => magazineAmount = Mathf.Clamp(value, 0, magazineCapacity);
            }
            public int MagazineCapacity
            {
                get => magazineCapacity;
                set => magazineCapacity = Mathf.Max(0, value);
            }
            public int Amount
            {
                get => amount;
                set => amount = Mathf.Max(0, value);
            }
            public bool Use(BaseCaster _caster, int _amount = 1)
            {
                if (!inRate && !inReload && (magazineAmount >= _amount || infiniteAmmo))
                {
                    inRate = true;
                    _inRateC = _caster.StartCoroutine(IRate());
                    
                    magazineAmount -= _amount;
                    if (magazineAmount < _amount)
                    {
                        inReload = true;
                        _caster.StartCoroutine(IReload());
                    }
                    return true;
                }
                return false;
            }

            private Coroutine _inRateC;
            /// <summary>
            ///  Change to Ienumerator
            /// </summary>
            internal IEnumerator IRate()
            {
                yield return new WaitForSeconds(inBetweenTime);
                inRate = false;
            }

            public float currentReloadTime { get; private set; } = 0f;
            private float dt;
            internal IEnumerator IReload()
            {
                while (currentReloadTime < reloadTime)
                {
                    currentReloadTime += Time.deltaTime;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                currentReloadTime = 0f;
                
                if (infiniteAmmo) // Bill
                {
                    magazineAmount = magazineCapacity;
                }
                else
                {
                    var needed = magazineCapacity - magazineAmount;
                    var possible = Mathf.Min(amount, needed);
                    magazineAmount += possible;
                    amount -= possible;
                }
                inReload = false;
            }

#if UNITY_EDITOR
            
            internal void EditorPanel(SerializedProperty serializedProperty)
            {
                BeginHorizontal();
                
                EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative(nameof(amount)),
                    "Amount".ToContent("Amount"));
                
                LocalField(serializedProperty.FindPropertyRelative(nameof(infiniteAmmo)), "I".ToContent("Infinite Ammo"));
                
                EndHorizontal();

                var mCapProp = serializedProperty.FindPropertyRelative(nameof(magazineCapacity));
                PropertyMaxIntField(mCapProp, "Magazine Capacity".ToContent("Magazine Capacity"));
                
                
                PropertySliderField(serializedProperty.FindPropertyRelative(nameof(magazineAmount)), 0, mCapProp.intValue, "Magazine Amount".ToContent(), i => {});

                PropertyMaxField(serializedProperty.FindPropertyRelative(nameof(reloadTime)), "Reload Time".ToContent("Reload Time"));
                
                PropertyMaxField(serializedProperty.FindPropertyRelative(nameof(inBetweenTime)), "In Between Time".ToContent("In Between Time"));
            }
#endif
        }
    }
}