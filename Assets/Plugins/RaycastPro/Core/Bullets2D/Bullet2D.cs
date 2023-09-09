namespace RaycastPro.Bullets2D
{
    using RaySensors2D;

#if UNITY_EDITOR
    using UnityEditor;

#endif
    
    public abstract class Bullet2D : BaseBullet
    {
        public BaseCaster caster;
        public RaySensor2D raySource;
        public RaySensor2D collisionRay;
        public float Z => transform.position.z;
        internal override void Cast<R>(BaseCaster _caster, R raySensor)
        {
            caster = _caster;
            
            raySource = raySensor as RaySensor2D;
            
            transform.forward = raySource.LocalDirection;
            transform.position = raySource.BasePoint;
            
            OnCast(); // Auto Setup 3D Bullet
            onCast?.Invoke(caster);
            if (trailRenderer) TrailSetup();
            if (collisionRay)
            {
                collisionRay.enabled = false;
            }

            onCast?.Invoke(caster);
        }

        public override void SetCollision(bool turn)
        {
            collisionRay.enabled = turn;
        }

        private float ignoreTime;
        protected override void CollisionRun(float deltaTime)
        {
            if (ignoreTime > 0)
            {
                ignoreTime -= deltaTime;
                return;
            }

            if (!collisionRay.Cast()) return;

            if (collisionRay.cloneRaySensor)
            {
                ignoreTime = baseIgnoreTime;
                transform.position = collisionRay.cloneRaySensor.BasePoint;
                transform.right = collisionRay.cloneRaySensor.Direction;
            }
            else
            {
                InvokeDamageEvent(collisionRay.hit.transform);
                if (endOnCollide) OnEnd(caster);
            }
        }

#if UNITY_EDITOR
        
        protected override void CollisionRayField(SerializedObject _so)
        {
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(collisionRay)));
        }
        internal override void OnGizmos()
        {
            DrawCap(transform.position, transform.right, 4);
        }
#endif
    }
}