﻿namespace RaycastPro.RaySensors
{
    using UnityEngine.Events;
    using System;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    using UnityEngine;
    
    [Serializable]
    public class VectorEvent : UnityEvent<Vector3> {}

    [AddComponentMenu("RaycastPro/Rey Sensors/" + nameof(NoiseRay))]
    public sealed class NoiseRay : RaySensor, IPulse
    {
        private Vector3 currentDirection;
        private Vector2 random;
        public Vector2 randomRadius;

        public float pulse = .4f;
        private float currentTime;
        
        public VectorEvent onPulse;

        private void Reset()
        {
            OnPulse();
        }

        private void OnEnable()
        {
            OnPulse();
        }

        private void OnPulse()
        {
            random = UnityEngine.Random.insideUnitCircle;
            random.x *= randomRadius.x;
            random.y *= randomRadius.y;
            
            if (local)
            {
                currentDirection = LocalDirection + transform.TransformDirection(random);
            }
            else
            {
                currentDirection = new Vector3(direction.x + random.x, direction.y + random.y, direction.z);
            }
            onPulse?.Invoke(currentDirection);
        }
        protected override void OnCast()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= pulse)
            {
                currentTime = 0;
                OnPulse();
            }
            
            Physics.Raycast(transform.position, currentDirection, out hit,
                DirectionLength, detectLayer.value, triggerInteraction);
        }

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Emits a ray on pulse time along direction with random point inside a circle." + HAccurate + HDirectional + HPreview;
#pragma warning restore CS0414

        private Vector3 _p;
        internal override void OnGizmos()
        {
            EditorUpdate();
            _p = transform.position;
            Gizmos.color = Performed ? DetectColor : DefaultColor;
            if (IsManuelMode)
            {
                Gizmos.DrawRay(transform.position, currentDirection);
            }
            else
            {
                DrawBlockLine(_p, _p + currentDirection, hit);
            }

            DrawNormal(hit);
        }

        private static readonly string[] eventsName = new string[]
            {"onDetect", "onPulse", "onBeginDetect", "onEndDetect", "onChange", "onCast"};
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                DirectionField(_so);
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(pulse)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(randomRadius)));
            }
            if (hasGeneral) GeneralField(_so);
            if (hasEvents) EventField(_so, eventsName);
            if (hasInfo) InformationField();
        }
#endif
        public override Vector3 Tip => transform.position + Direction;
        public override float RayLength => direction.magnitude;
        public override Vector3 Base => transform.position;
    }
}