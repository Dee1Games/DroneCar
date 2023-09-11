namespace RaycastPro.Casters2D
{
    using Bullets2D;
    using RaySensors2D;
    using UnityEngine;
    using System;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif
    [AddComponentMenu("RaycastPro/Casters/" + nameof(AdvanceCaster2D))]
    public sealed class AdvanceCaster2D : GunCaster<Bullet2D, Collider2D, RaySensor2D>
    {
        public BulletEvent2D onCast;
        
        public RaySensor2D[] raySensors = Array.Empty<RaySensor2D>();
        
        public int currentIndex;

        [SerializeField]
        private bool pingPongPhase;

        public CastType castType = CastType.Together;
        
        protected override void OnCast() => Cast(index);
        public override void Cast(int _index)
        {
            // Last Note: Adding Index debugging. Bullet cast returning true when successfully shot
            switch (castType)
            {
                case CastType.Together:
                    BulletCast(_index, raySensors, bullet => onCast?.Invoke(bullet));
                    break;
                case CastType.Sequence:
                    if (BulletCast(_index, raySensors[currentIndex], bullet => onCast?.Invoke(bullet)))
                    {
                        currentIndex = ++currentIndex % raySensors.Length;
                    }
                    break;
                case CastType.Random:
                    if (BulletCast(_index, raySensors[new System.Random().Next(0, raySensors.Length)], bullet => onCast?.Invoke(bullet)))
                    {
                        currentIndex = ++currentIndex % raySensors.Length;
                    }
                    break;
                case CastType.PingPong:
                    if (BulletCast(_index, raySensors[currentIndex], bullet => onCast?.Invoke(bullet)))
                    {
                        currentIndex = pingPongPhase ? --currentIndex : ++currentIndex;

                        if (currentIndex == raySensors.Length - 1 || currentIndex == 0) pingPongPhase = !pingPongPhase;
                    }
                    break;
            }
        }
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Bullet caster, with the ability to adjust Ammo, detect all types of bullets automatically and RaySensors different shooting modes." + HAccurate + HDependent;
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            foreach (var sensor in raySensors)
            {
                var tip = sensor ? sensor.Tip : transform.position + transform.forward;
                var position = sensor ? sensor.BasePoint : transform.position;

                DrawCapLine2D(position, tip);
            }
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                BeginVerticalBox();
                RCProEditor.PropertyArrayField(_so.FindProperty(nameof(raySensors)),
                    CRaySensor.ToContent(TRaySensor), i => $"RaySensors {i+1}".ToContent($"Index {i}"));
                EndVertical();
                PropertyEnumField(_so.FindProperty(nameof(castType)), 4, CCastType.ToContent(TCastType), new GUIContent[]
                {
                    CTogether.ToContent(TTogether),
                    CSequence.ToContent(TSequence),
                    CRandom.ToContent(TRandom),
                    CPingPong.ToContent(TPingPong),
                });
                
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
        private static readonly string[] events = new[] {nameof(onCast)};
#endif
    }
}