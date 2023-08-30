namespace RaycastPro.RaySensors2D
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using Editor;
#endif

    [AddComponentMenu("RaycastPro/Rey Sensors/" + nameof(ReflectRay2D))]
    public sealed class ReflectRay2D : PathRay2D, IRadius
    {
        /// <summary>
        /// Get List of reflected hits.
        /// </summary>
        public readonly List<RaycastHit2D> RaycastHits = new List<RaycastHit2D>();

        public LayerMask reflectLayer;

        [SerializeField] private float radius;

        public float Radius
        {
            get => radius;
            set => radius = Mathf.Max(0, value);
        }

        private Vector2 point, _direction;
        private Vector3 _tPosition;
        private float distance;
        private RaycastHit2D h;
        
        private void ReflectCast()
        {
            PathPoints.Clear();
            RaycastHits.Clear();
            _tPosition = transform.position;
            point = _tPosition.To2D();
            _direction = Direction;
            PathPoints.Add(_tPosition);
            distance = direction.magnitude;

            DetectIndex = -1;
            hit = default;
            var queriesSetting = Physics2D.queriesStartInColliders;
            Physics2D.queriesStartInColliders = false;
            while (true)
            {
                h = new RaycastHit2D();
                if (radius > 0)
                {
                    h = Physics2D.CircleCast(point, radius,  _direction, distance, reflectLayer.value | detectLayer.value,
                        MinDepth, MaxDepth);
                }
                else
                {
                    h = Physics2D.Raycast(point, _direction, distance, reflectLayer.value | detectLayer.value,
                        MinDepth, MaxDepth); 
                }
                if (h)
                {
                    RaycastHits.Add(h);
                    PathPoints.Add(h.centroid);
                    var onHit = detectLayer.InLayer(h.transform.gameObject);
                    if (onHit)
                    {
                        DetectIndex = PathPoints.Count - 1;
                        isDetect = FilterCheck(h);
                        break;
                    }
                    distance -= (h.centroid - point).magnitude;
                    point = h.point;
                    _direction = Vector3.Reflect(_direction, h.normal);
                    continue;
                }
                PathPoints.Add(point + _direction.normalized * distance);
                break;
            }
            Physics2D.queriesStartInColliders = queriesSetting;
        }

        protected override void OnCast()
        {
            isDetect = false;
            ReflectCast();
        }


#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Send a reflective 2D ray to the <i>Reflect layer</i> and detect the point of impact in the <i>Detect layer</i>." +
#pragma warning restore CS0414
                                     HAccurate + HPathRay + HRecursive + HIRadius;

        internal override void OnGizmos()
        {
            EditorUpdate();
            DrawPath2D(PathPoints.ToDepth(z).ToList(), hit.point.ToDepth(z), isDetect, radius, drawDisc: false, coneCap: true, detectIndex: DetectIndex);
            DrawDepthLine(Position2D, Tip, isDetect ? DetectColor : DefaultColor);
            if (!hit) return;
            DrawNormal2D(hit, z);
            DrawNormalFilter();
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                DirectionField(_so);
                RadiusField(_so);
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(reflectLayer)), CReflectLayer.ToContent(TReflectLayer));
            }

            if (hasGeneral) GeneralField(_so);
            if (hasEvents) EventField(_so);
            if (hasInfo)
            {
                InformationField(() => RaycastHits.ForEach(r =>
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(r.transform.name);
                    GUILayout.Label(r.point.ToString());
                    GUILayout.EndHorizontal();
                }));
            }
        }
#endif
    }
}