namespace RaycastPro.RaySensors
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif
    public abstract class PathRay : RaySensor
    {
        [Tooltip("This option enables the calculation of the series of rays in Path Ray. (If you only use path points in Path Detector or..., you can disable it.)")]
        public bool pathCast = true;
        /// <summary>
        /// return's tip of the ray path. (transform position on no path definition).
        /// </summary>
        public override Vector3 Tip => PathPoints.LastOrBase(transform.position);
        
        /// <summary>
        /// return's direction of 2 last points of ray hit. (not Normalized)
        /// </summary>
        public override Vector3 HitDirection => PathPoints.LastDirection(LocalDirection);
        public override float HitLength
        {
            get
            {
                var len = 0f;
                if (hit.transform)
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

        public override void GetPath(ref List<Vector3> path, bool OnHit)
        {
            if (OnHit)
            {
                if (DetectIndex > -1)
                {
                    path = new List<Vector3>();
                    for (int i = 0; i < DetectIndex; i++)
                    {
                        path.Add(PathPoints[i]);
                    }
                    path.Add(hit.point);
                }
                else
                {
                    path = PathPoints;
                }
            }
            else
            {
                path = PathPoints;
            }
        }

        /// <summary>
        /// return's total distance of the path. Alternative: PathPoints.GetPathLength().
        /// </summary>
        public override float RayLength => PathPoints.GetPathLength();

        protected abstract void UpdatePath();
        
        protected int PathCast(float radius = 0f)
        {
            if (radius > 0)
            {
                for (var i = 0; i < PathPoints.Count - 1; i++)
                {
                    var dir = PathPoints[i + 1] - PathPoints[i];
                    if (!Physics.SphereCast(PathPoints[i], radius, dir.normalized, out hit, dir.magnitude,
                            detectLayer.value, triggerInteraction)) continue;
                    return i;
                }
            }
            else
            {
                for (var i = 0; i < PathPoints.Count - 1; i++)
                {
                    if (!Physics.Linecast(PathPoints[i], PathPoints[i + 1], out hit, detectLayer.value, triggerInteraction)) continue;
                    //if (!Physics.Linecast(path[i], path[i + 1], out hit, detectLayer.value, triggerInteraction)) continue;
                    return i;
                }
            }
            return -1;
        }
        public override Vector3 Base => PathPoints.Count > 0 ? PathPoints.First() : transform.position;
        public override float ContinuesDistance
        {
            get
            {
                if (hit.transform)
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
        
        /// <summary>
        /// List of path points in world position. 
        /// </summary>
        public List<Vector3> PathPoints = new List<Vector3>();
        /// <summary>
        /// Index of Detection PathPoint. Default when no detection: -1;
        /// </summary>
        public int DetectIndex { get; internal set; } = -1;

#if UNITY_EDITOR

        protected void FullPathDraw(float radius = 0f, bool cap = false, bool dotted = false)
        {
            if (IsManuelMode) UpdatePath();
            DrawPath(PathPoints, hit, radius, detectIndex: DetectIndex, drawSphere: true, dotted: dotted, coneCap: cap);
        }

        protected void PathRayGeneralField(SerializedObject _so)
        {
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(pathCast)));
            GeneralField(_so);
        }
#endif

    }
}