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
        
        public int currentIndex;
        
        public bool pingPongPhase;

        public CastType castType = CastType.Together;

        public BulletEvent onCast;
        protected override void OnCast() => Cast(index);
        // ReSharper disable Unity.PerformanceAnalysis
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
                    if (BulletCast(_index, raySensors[new Random().Next(0, raySensors.Length)], bullet => onCast?.Invoke(bullet)))
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

        private Vector3 tip, position;
        internal override void OnGizmos()
        {
            foreach (var sensor in raySensors)
            {
                if (sensor is PathRay _pathSensor)
                {
                    DrawPath(_pathSensor.PathPoints, _pathSensor.hit, .1f, true, true, drawSphere: false, -1, HelperColor);
                }
                else
                {
                    tip = sensor ? sensor.Tip : transform.position + transform.forward;
                    position = sensor ? sensor.BasePoint : transform.position;
                    DrawCapLine(position, tip);
                }

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
                BeginVerticalBox();
                PropertyEnumField(_so.FindProperty(nameof(castType)), 4, CCastType.ToContent(TCastType), new GUIContent[]
                {
                    CTogether.ToContent(TTogether),
                    CSequence.ToContent(TSequence),
                    CRandom.ToContent(TRandom),
                    CPingPong.ToContent(TPingPong),
                });
                EndVertical();
            }

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