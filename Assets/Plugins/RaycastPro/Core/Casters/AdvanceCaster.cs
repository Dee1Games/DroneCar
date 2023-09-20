using System.Collections;
using UnityEditor;

namespace RaycastPro.Casters
{
    using System;
    using RaySensors;
    using UnityEngine;
    using Bullets;
    
#if UNITY_EDITOR
    using Editor;
#endif

    using Random = System.Random;

    [AddComponentMenu("RaycastPro/Casters/" + nameof(AdvanceCaster))]
    public sealed class AdvanceCaster : GunCaster<Bullet, Collider, RaySensor>
    {
        [SerializeField]
        public RaySensor[] raySensors = Array.Empty<RaySensor>();
        
        public int rayIndex;
        
        public bool pingPongPhase;

        public CastType castType = CastType.Together;

        public BulletEvent onCast;
        protected override void OnCast() => Cast(index);
        
        public void Cast(int _rayIndex, int _bulletIndex)
        {
            rayIndex = _rayIndex;
            Cast(_bulletIndex);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void Cast(int _bulletIndex)
        {
#if UNITY_EDITOR
            alphaCharge = AlphaLifeTime;
#endif
            // Last Note: Adding Index debugging. Bullet cast returning true when successfully shot
            switch (castType)
            {
                case CastType.Together:
                    BulletCast(_bulletIndex, raySensors, bullet => onCast?.Invoke(bullet));
                    break;
                case CastType.Sequence:
                    if (BulletCast(_bulletIndex, raySensors[rayIndex], bullet => onCast?.Invoke(bullet)))
                    {
                        rayIndex = ++rayIndex % raySensors.Length;
                    }
                    break;
                case CastType.Random:
                    if (BulletCast(_bulletIndex, raySensors[new Random().Next(0, raySensors.Length)], bullet => onCast?.Invoke(bullet)))
                    {
                        rayIndex = ++rayIndex % raySensors.Length;
                    }
                    break;
                case CastType.PingPong:
                    if (BulletCast(_bulletIndex, raySensors[rayIndex], bullet => onCast?.Invoke(bullet)))
                    {
                        rayIndex = pingPongPhase ? --rayIndex : ++rayIndex;
                        if (rayIndex == raySensors.Length - 1 || rayIndex == 0) pingPongPhase = !pingPongPhase;
                    }
                    break;
            }
        }


#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Bullet caster, with the ability to adjust Ammo, detect all types of bullets automatically and RaySensors different shooting modes." + HAccurate + HDependent;
#pragma warning restore CS0414

        private Vector3 tip, position;
        internal override void OnGizmos()
        {
            foreach (var sensor in raySensors)
            {
                if (!sensor) continue;
                
                position = sensor ? sensor.Base : transform.position;
                DrawCapLine(position, position + sensor.Direction);
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
                    CRaySensor.ToContent(TRaySensor), i => $"GunBarrel {i+1}".ToContent($"Index {i}"));
                EndVertical();
                BeginVerticalBox();
                PropertyEnumField(_so.FindProperty(nameof(castType)), 4, CCastType.ToContent(TCastType), new GUIContent[]
                {
                    CTogether.ToContent(TTogether),
                    CSequence.ToContent(TSequence),
                    CRandom.ToContent(TRandom),
                    CPingPong.ToContent(TPingPong),
                });
                EndVertical();
                
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
#endif
    }
}