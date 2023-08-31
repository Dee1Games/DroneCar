namespace RaycastPro.Bullets
{
    using Planers;
    using RaySensors;
    using UnityEngine;
    
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    
    public abstract class Bullet : BaseBullet
    {
        public BaseCaster caster;
        public RaySensor raySource;
        
        private float ignoreTime;
        private void OnDestroy() => onEnd?.Invoke(caster);
        internal override void Cast<R>(BaseCaster _caster, R raySensor)
        {
            caster = _caster;
            if (raySensor is RaySensor _r)
            {
                raySource = _r;
            }
            onCast?.Invoke(caster);
            OnCast(); // Auto Setup 3D Bullet
        }

        protected bool CollisionRun(Vector3 direction, float deltaTime)
        {
            if (!hasCollision) return false;
            RaycastHit hit;
            if (radius > 0)
            {
                Physics.Raycast(transform.position + direction.normalized * offset, direction, out hit, length,
                    detectLayer.value, triggerInteraction);
            }
            else
            {
                Physics.SphereCast(transform.position + direction.normalized * offset, radius, direction, out hit,
                    length, detectLayer.value, triggerInteraction);
            }
            if (!hit.transform) return false;
            if (planarSensitive || ignoreTime > 0)
            {
                ignoreTime -= deltaTime;
                if (hit.transform.TryGetComponent(out Planar planar))
                {
                    var data = planar.GetTransitionData(hit, direction);
                    if (data.Length == 1)
                    {
                        transform.position = data[0].position + direction * planar.offset;
                        transform.rotation = data[0].rotation;
                        ignoreTime = .1f;
                    }
                    else
                    {
                        for (var i = 0; i < data.Length; i++)
                        {
                            var _b = Instantiate(this, data[i].position + direction * planar.offset, data[i].rotation);
                            _b.planarSensitive = false;
                            ignoreTime = .1f;
                            _b.endFunction = EndType.Destroy;
                        }
                        enabled = false;
                        OnEnd();
                    }
                    return true;
                }
            }
            InvokeDamageEvent(hit.transform);
            OnEnd();
            return true;
        }
#if UNITY_EDITOR
        internal override void OnGizmos()
        {
            var _forward = transform.forward;
            DrawCap(transform.position, _forward, 5);
            
            var _pos = transform.position;

            Handles.color = HelperColor;

            var _tOffset = _forward * offset;
            
            DrawCapsuleLine(_pos + _tOffset, _pos + _tOffset + _forward*length, radius, _t: transform);
        }
#endif
    }
}