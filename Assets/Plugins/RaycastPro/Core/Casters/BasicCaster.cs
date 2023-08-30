namespace RaycastPro.Casters
{
    using RaySensors;
    using UnityEngine;
    using Bullets;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif
    
    [AddComponentMenu("RaycastPro/Casters/" + nameof(BasicCaster))]
    public sealed class BasicCaster : GunCaster<BasicBullet, Collider, RaySensor>
    {
        public BulletEvent onCast;
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void Cast(int _index) => BulletCast(_index, null, b => onCast?.Invoke(b));
        protected override void OnCast() => Cast(index);

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "A bullet launcher with a source from a raySensor." + HAccurate + HDependent;
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            var transformPosition = transform.position;
            
            DrawCapLine(transformPosition,transformPosition + transform.forward);
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasGeneral) GeneralField(_so);

            if (hasEvents) 
            {
                EventFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(EventFoldout, CEvents.ToContent(TEvents),
                    RCProEditor.HeaderFoldout());
                EditorGUILayout.EndFoldoutHeaderGroup();
                if (EventFoldout) RCProEditor.EventField(_so, events);
            }
            if (hasInfo) InformationField();
        }
        private static readonly string[] events = new[] {nameof(onCast)};
#endif
    }
}