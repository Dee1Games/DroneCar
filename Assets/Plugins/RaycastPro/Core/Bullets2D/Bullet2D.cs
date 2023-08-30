
using RaycastPro.Planers2D;

namespace RaycastPro.Bullets2D
{
    using UnityEngine;
    using RaySensors2D;

#if UNITY_EDITOR
    using UnityEditor;

#endif
    
    public abstract class Bullet2D : BaseBullet
    {
        public BaseCaster caster;
        
        public RaySensor2D raySource;
        public float Z => transform.position.z;

        // TEMP DISABLE DEPTH CUZ OF NO FEATURE SUPPORTING FOR NOW
        
        // public float minDepth = .5f;
        // public float maxDepth = -.5f;
        //
        // public float MinDepth => transform.position.z + minDepth;
        // public float MaxDepth => transform.position.z + maxDepth;
        internal override void Cast<R>(BaseCaster _caster, R raySensor)
        {
            caster = _caster;
            
            if (raySensor is RaySensor2D _r) raySource = _r;
            
            onCast?.Invoke(caster);

            OnCast(); // Auto Setup 3D Bullet
        }

        void X()
        {

        }
        
        protected bool CollisionRun(Vector2 direction, float deltaTime)
        {
            if (!hasCollision) return false;
            
            RaycastHit2D hit;
            if (radius > 0)
            {
                hit = Physics2D.Raycast(transform.position, direction, length, detectLayer.value);
            }
            else
            {
                hit = Physics2D.CircleCast(transform.position, radius, direction, length, detectLayer.value);
            }
            if (!hit.transform) return false;

            
            if (planarSensitive)
            {
                if (hit && hit.transform.TryGetComponent(out Planar2D planar))
                {
                    var data = planar.GetTransitionData2D(hit, direction);
                    if (data.Length == 1)
                    {
                        transform.position = data[0].position + (Vector3) direction * planar.offset;
                        transform.rotation = data[0].rotation;
                        
                        //TempIgnore();
                    }
                    else
                    {
                        for (int i = 0; i < data.Length; i++)
                        {
                            var _b = Instantiate(this, data[i].position+ (Vector3) direction * planar.offset, data[i].rotation);
                            _b.planarSensitive = false;
                            //_b.TempIgnore();
                            _b.endFunction = EndType.Destroy;
                        }
                        enabled = false;
                        OnEnd();
                    }
                }
            }

            InvokeDamageEvent(hit.transform);
            OnEnd();
            return true;
        }

#if UNITY_EDITOR
        internal override void OnGizmos()
        {
            var _forward = transform.right;
            
            DrawCap(transform.position, _forward, 5);
            
            var _tOffset = _forward * offset;
            
            var _pos = transform.position+_tOffset;

            Handles.color = HelperColor;



            DrawCircleLine(_pos, _pos + _forward, radius);
        }
#endif
    }
}