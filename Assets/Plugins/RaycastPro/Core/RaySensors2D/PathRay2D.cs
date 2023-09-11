namespace RaycastPro.RaySensors2D
{
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public abstract class PathRay2D : RaySensor2D
    {
        public bool pathCast = true;
        public override Vector3 Tip => PathPoints.LastOrBase(transform.position).ToDepth(z);
        public override float RayLength => PathPoints.GetPathLength();
        public override Vector3 BasePoint => PathPoints.Count > 0 ? PathPoints[0].ToDepth(z) : transform.position;
        public override Vector2 HitDirection => PathPoints.LastDirection(LocalDirection);
        public List<Vector2> PathPoints = new List<Vector2>();
        public override float ContinuesDistance
        {
            get
            {
                if (hit)
                {
                    var distance = 0f;
                    for (var i = PathPoints.Count - 1; i > 0; i--)
                    {
                        if (i == DetectIndex + 1)
                        {
                            distance += (PathPoints[i] - hit.point).magnitude;
                            break;
                        }
                        distance += (PathPoints[i] - PathPoints[i - 1]).magnitude;
                    }
                    return distance;
                }
                return 0;
            }
        }

        public int DetectIndex = -1;
        private Vector2 _dir;
        protected int PathCast(out RaycastHit2D hit, float radius = 0)
        {
            hit = new RaycastHit2D();
            var minD = MinDepth;
            var maxD = MaxDepth;
            if (radius == 0)
            {
                for (var i = 0; i < PathPoints.Count - 1; i++)
                {
                    hit = Physics2D.Linecast(PathPoints[i], PathPoints[i + 1], detectLayer.value, minD, maxD);
                    if (!hit.transform) continue;
                    return i;
                }
            }
            else
            {
                for (var i = 0; i < PathPoints.Count - 1; i++)
                {
                    _dir = PathPoints[i + 1] - PathPoints[i];
                    hit = Physics2D.CircleCast(PathPoints[i], radius, _dir, _dir.magnitude, detectLayer.value, minD, maxD);
                    if (!hit.transform) continue;
                    return i;
                }
            }
            return -1;
        }
        protected abstract void UpdatePath();
        
#if UNITY_EDITOR

        protected void FullPathDraw(float radius = 0f, bool cap = false)
        {
            if (IsManuelMode)
            {
                UpdatePath();
                DrawPath2D(PathPoints.ToDepth(z), isDetect: hit, breakPoint: hit.point, radius: radius, detectIndex: DetectIndex,z: z, drawDisc: true,
                    coneCap: true);
            }
            else
            {
                DrawPath2D(PathPoints.ToDepth(z), isDetect: hit, breakPoint:hit.point, radius: radius, detectIndex: DetectIndex, z: z, drawDisc: true,
                    coneCap: true);
            }
        }
        protected void PathRayGeneralField(SerializedObject _so)
        {
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(pathCast)));
            GeneralField(_so);
        }
#endif
    }
}