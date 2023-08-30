namespace RaycastPro.Bullets
{
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    [AddComponentMenu("RaycastPro/Bullets/" + nameof(BasicBullet))]

    public sealed class BasicBullet : Bullet
    {
        protected override void OnCast()
        {
            if (raySource)
            {
                transform.position = raySource.BasePoint;
                transform.forward = raySource.TargetDirection.normalized;
            }
            else
            {
                transform.position = caster.transform.position;
                transform.forward = caster.transform.forward;
            }
        }
        private float delta;
        private Vector3 _forward;
        public override void RuntimeUpdate()
        {
            delta = GetModeDeltaTime(timeMode);
            _forward = transform.forward;
            transform.position += _forward * (speed * delta);
            UpdateLifeProcess(delta);
            CollisionRun(_forward, delta);
        }
        
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info =
            "A simple bullet that travels directly from the origin to the tip of the raySensor with avoiding the path." + HAccurate + HDependent + HIRadius;
#pragma warning restore CS0414
        
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(speed)),
                    CSpeed.ToContent(CSpeed));
            }

            if (hasGeneral) GeneralField(_so);

            if (hasEvents) EventField(_so);

            if (hasInfo) InformationField();
        }
#endif
    }
}