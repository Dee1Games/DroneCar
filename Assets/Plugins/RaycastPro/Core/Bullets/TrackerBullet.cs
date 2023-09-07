using System;
using RaycastPro.RaySensors;

namespace RaycastPro.Bullets
{
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
    using Editor;
#endif

    [AddComponentMenu("RaycastPro/Bullets/" + nameof(TrackerBullet))]
    public sealed class TrackerBullet : Bullet
    {
        public Transform target;
        public Vector3 targetPoint;

        public float force = 10f;
        public float drag = 10;
        public Vector3 trackOffset;
        
        public float distanceThreshold = .2f;

        public float turnSharpness = 15;
        public enum TrackType
        {
            PositionLerp,
            RotationLerp,
        }
        public TrackType trackType = TrackType.PositionLerp;
        
        [SerializeField]
        private AxisRun axisRun = new AxisRun();
        
        protected override void OnCast()
        {
            if (raySource)
            {
                transform.position = raySource.BasePoint;
                // Tip Direction
                transform.rotation = Quaternion.LookRotation(raySource.TipDirection, transform.up);
            }
            
            targetPoint = target.position;
            currentForce = force;
            _t = transform;
        }

        private Transform _t;
        private float _dis, _dt, currentForce;
        private Vector3 _dir;
        private Quaternion look;
        public override void RuntimeUpdate()
        {              
            _dt = GetModeDeltaTime(timeMode);
            UpdateLifeProcess(_dt);
            
            targetPoint = target ? target.position + trackOffset : _t.position;
            _dis = Vector3.Distance(_t.position, targetPoint);
            if (currentForce <= .1f)
            {
                OnEnd();
                return;
            }
            if (target && _dis <= distanceThreshold)
            {
                OnEnd();
                return;
            }
            _dir = targetPoint - _t.position;
            switch (trackType)
            {
                case TrackType.PositionLerp:
                    var lerp = Vector3.Lerp(_t.position, targetPoint, _dt * speed);
                    _t.position = lerp;
                    break;
                case TrackType.RotationLerp:
                    _t.rotation = Quaternion.Lerp(_t.rotation, Quaternion.LookRotation(_dir.normalized, transform.up), 1 - Mathf.Exp(-turnSharpness * _dt));
                    currentForce = Mathf.Lerp(currentForce, 0, 1 - Mathf.Exp(-drag * _dt));

                    var nextPoint = transform.forward * (currentForce * _dt);
                    _t.position += nextPoint;
                    //_t.rotation = Quaternion.Lerp(_t.rotation, look, 1 - Mathf.Exp(-turnSharpness * _dt));
                    break;
            }

            //if (axisRun.syncAxis) axisRun.SyncAxis(transform, _dir);
            CollisionRun(_dir, _dt);
        }

        public void UnParent(Transform transform)
        {
            transform.parent = null;
        }
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "After being fired by the caster, it follows the target." + HAccurate + HDependent;
#pragma warning restore CS0414

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                BeginVerticalBox();
                PropertyEnumField(_so.FindProperty(nameof(trackType)), 2, "Track Type".ToContent(), new GUIContent[]
                {
                    "Position Lerp".ToContent("Position Lerp"),
                    "Rotation Lerp".ToContent("Position Lerp"),
                });
                
                if (trackType == TrackType.RotationLerp)
                {
                    EditorGUILayout.PropertyField(_so.FindProperty(nameof(force)));
                    EditorGUILayout.PropertyField(_so.FindProperty(nameof(turnSharpness)));
                    EditorGUILayout.PropertyField(_so.FindProperty(nameof(drag)));
                }
                else
                {
                    EditorGUILayout.PropertyField(_so.FindProperty(nameof(speed)));
                }
                EndVertical();
                
                                
                axisRun.EditorPanel(_so.FindProperty(nameof(axisRun)));
                
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(distanceThreshold)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(trackOffset)));

            }
            
            if (hasGeneral) GeneralField(_so);

            if (hasEvents) EventField(_so);
            
            if (hasInfo) InformationField();
        }
#endif

    }
}