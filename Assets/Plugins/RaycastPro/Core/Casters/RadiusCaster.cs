using RaycastPro.Detectors;

namespace RaycastPro.Casters
{
    using RaySensors;
    using UnityEngine;
    using Bullets;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif
    
    [AddComponentMenu("RaycastPro/Casters/" + nameof(RadiusCaster))]
    public sealed class RadiusCaster : GunCaster<BasicBullet, Collider, RaySensor>
    {
        [SerializeField]
        [Tooltip("Automatically shoots to all Detected Colliders.")]
        public ColliderDetector cDetector;
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void Cast(int _bulletIndex)
        {
#if UNITY_EDITOR
            alphaCharge = AlphaLifeTime;
#endif
            if (AmmoCheck(cDetector.DetectedColliders.Count))
            {
                BulletCast(_bulletIndex, cDetector);
            }
        }

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "A simple shooter with the ability to cover Basic Bullets that can help you to test and launch the gun immediately." + HAccurate + HDependent + HPreview;
#pragma warning restore CS0414

        private Vector3 _p1, _p2;
        internal override void OnGizmos()
        {
            
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(cDetector)));
                GunField(_so);
            }
            if (hasGeneral) GeneralField(_so);

            if (hasEvents) 
            {
                EventFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(EventFoldout, CEvents.ToContent(TEvents),
                    RCProEditor.HeaderFoldout);
                EditorGUILayout.EndFoldoutHeaderGroup();
                if (EventFoldout) RCProEditor.EventField(_so, events);
            }
            if (hasInfo) InformationField();
        }
        private readonly string[] events = new[] {nameof(onCast), nameof(onReload), nameof(onRate)};
#endif
    }
}