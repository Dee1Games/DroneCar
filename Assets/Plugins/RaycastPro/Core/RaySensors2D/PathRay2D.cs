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
        public override Vector3 Base => PathPoints.Count > 0 ? PathPoints[0].ToDepth(z) : transform.position;
        public override Vector2 HitDirection => PathPoints.LastDirection(LocalDirection);
        
        /// <summary>
        /// The length traveled from Ray to reach the target point
        /// </summary>
        public override float HitLength
        {
            get
            {
                var len = 0f;
                if (DetectIndex > -1)
                {
                    len = Vector3.Distance(hit.point,PathPoints[DetectIndex]);
                    for (var i = 1; i <= DetectIndex; i++)
                    {
                        len += PathPoints.GetEdgeLength(i);
                    }
                }
                else
                {
                    len = PathPoints.GetPathLength();
                }
                return len;
            }
        }
        public override void GetPath2D(ref List<Vector2> path) => path = PathPoints;
        public override void GetPath(ref List<Vector3> path) => path = PathPoints.ToDepth(z);
        
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

        public int DetectIndex { get; internal set; } = -1;
        private Vector2 _dir;
        protected int PathCast(out RaycastHit2D hit, float radius = 0)
        {
            hit = new RaycastHit2D();
            if (radius == 0)
            {
                for (var i = 0; i < PathPoints.Count - 1; i++)
                {
                    hit = Physics2D.Linecast(PathPoints[i], PathPoints[i + 1], detectLayer.value, MinDepth, MaxDepth);
                    if (hit) return i;
                    
                }
            }
            else
            {
                for (var i = 0; i < PathPoints.Count - 1; i++)
                {
                    _dir = PathPoints[i + 1] - PathPoints[i];
                    hit = Physics2D.CircleCast(PathPoints[i], radius, _dir, _dir.magnitude, detectLayer.value, MinDepth, MaxDepth);
                    if (hit) return i;
                }
            }
            return -1;
        }
        protected abstract void UpdatePath();
        
#if UNITY_EDITOR
        protected void FullPathDraw(float radius = 0f, bool cap = false)
        {
            if (IsManuelMode) UpdatePath();
            DrawPath2D(PathPoints.ToDepth(z), isDetect: isDetect, breakPoint: HitPointZ, radius: radius,
                detectIndex: DetectIndex, z: z, drawDisc: true,
                coneCap: cap);
        }
        protected void PathRayGeneralField(SerializedObject _so)
        {
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(pathCast)));
            GeneralField(_so);
        }
#endif
    }
}