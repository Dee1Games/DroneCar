using RaycastPro.RaySensors;

namespace RaycastPro.Bullets
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Bullets/" + nameof(InstantBullet))]
    public class InstantBullet : Bullet
    {
        [Tooltip("The offset of the bullet when hitting is calculated as the inverse of the HitDirection.")]
        public float hitOffset = .1f;

        [Tooltip("When the bullet misses, it executes the end function to prevent the bullet remaining in the tips of the rays.")]
        public bool endOnMiss = true;
        
        [Tooltip("If the desired object is not fixed, you can activate this option so that the parenting action is performed and the bullet remains move along with that object.")]
        public bool forceToParentHit;

        internal override void RuntimeUpdate() => UpdateLifeProcess(GetModeDeltaTime(timeMode));

        private RaySensor lastClone;
        private Vector3 hitDirection;
        protected override void OnCast()
        {
            lastClone = (raySource.planarSensitive) ? raySource.LastClone : raySource;
            if (lastClone.hit.transform)
            {
                hitDirection = lastClone.HitDirection.normalized;
                transform.position = lastClone.TipTarget - hitDirection * hitOffset;
                transform.forward = hitDirection;
                
                if (forceToParentHit) transform.SetParent(lastClone.hit.transform, true);
                    InvokeDamageEvent(raySource.hit.transform);
            }
            else if (endOnMiss)
            {
                 OnEndCast(caster);
            }
        }
        protected override void CollisionBehaviour() { }
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Instant shots to Target Tip of sensor ray." + HDependent;
#pragma warning restore CS0414

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(hitOffset)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(endOnMiss)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(forceToParentHit)));
            }
            if (hasGeneral) GeneralField(_so);

            if (hasEvents) EventField(_so);

            if (hasInfo) InformationField();
        }
#endif

    }
}