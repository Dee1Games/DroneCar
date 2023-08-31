namespace RaycastPro.RaySensors2D
{
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public abstract class PathRay2D : RaySensor2D
    {
        [SerializeField]
        protected bool pathCast = true;
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
        
#if UNITY_EDITOR
        protected void PathRayGeneralField(SerializedObject _so)
        {
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(pathCast)));
            GeneralField(_so);
        }
#endif
    }
}