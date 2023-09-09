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
        /// return's direction of 2 last points of ray hit.
        /// </summary>
        public override Vector3 HitDirection => PathPoints.LastDirection(LocalDirection);

        /// <summary>
        /// return's total distance of the path. Alternative: PathPoints.GetPathLength().
        /// </summary>
        public override float RayLength => PathPoints.GetPathLength();
        
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

        // /// <summary>
        // /// Let ray to process path cast or only update path points.
        // /// </summary>
        // public bool processPathCast;
        //
        public override Vector3 BasePoint => PathPoints.Count > 0 ? PathPoints.First() : transform.position;
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
        public int DetectIndex = -1;

#if UNITY_EDITOR
        protected void PathRayGeneralField(SerializedObject _so)
        {
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(pathCast)));
            GeneralField(_so);
        }
#endif

    }
}