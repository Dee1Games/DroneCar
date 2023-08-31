using System;
using RaycastPro.RaySensors;

namespace RaycastPro.Casters2D
{
    using UnityEngine;
    using RaySensors2D;
    using Bullets2D;

#if UNITY_EDITOR
    using UnityEditor;
    using Editor;
#endif

    [AddComponentMenu("RaycastPro/Casters/" + nameof(BasicCaster2D))]
    public sealed class BasicCaster2D : GunCaster<Bullet2D, Collider2D, RaySensor2D>
    {
        public BulletEvent2D onCast;
        protected override void OnCast() => Cast(index);
        public override void Cast(int _index) => BulletCast(_index, null, b => onCast?.Invoke(b)); // basic caster inject start, end positions to bullets

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info =
            "A simple bullet that travels directly from the origin to the tip of the raySensor with avoiding the path." +
            HAccurate + HDependent;
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            var _t = transform;
            DrawCapLine2D(_t.position, _t.position + _t.right);
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