namespace RaycastPro.RaySensors
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Rey Sensors/" + nameof(ReflectRay))]
    public sealed class ReflectRay : PathRay, IRadius
    {
        private List<RaycastHit> reflectHits = new List<RaycastHit>();

        public LayerMask reflectLayer;
        
        [SerializeField] private float radius;
        public float Radius
        {
            get => radius;
            set => radius = Mathf.Max(0,value);
        }
        
        public Axis planeAxis;
        public bool hasFreezeAxis;

        private RaycastHit ReflectCast(out List<RaycastHit> RaycastHits, out List<Vector3> _pathPoints)
        {
            Vector3 ApplyFreeze(Vector3 dir)
            {
                if (!hasFreezeAxis) return dir;
                switch (planeAxis)
                {
                    case Axis.X: dir.x = 0; break;
                    case Axis.Y: dir.y = 0; break;
                    case Axis.Z: dir.z = 0; break;
                }
                return dir;
            }
            _pathPoints = new List<Vector3>();
            RaycastHits = new List<RaycastHit>();
            var point = transform.position;
            var _direction = Direction;
            _direction = ApplyFreeze(_direction);
            _pathPoints.Add(transform.position);
            var distance = direction.magnitude;
            DetectIndex = -1;
            var raycastHit = new RaycastHit();
            var physicsSetting = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = false;
            while (true)
            {
                RaycastHit _hit;
                if (radius > 0)
                {
                    Physics.SphereCast(point, radius, _direction, out _hit, distance, reflectLayer.value | detectLayer.value,
                        triggerInteraction);
                }
                else
                    Physics.Raycast(point, _direction, out _hit, distance, reflectLayer.value | detectLayer.value,
                        triggerInteraction);
                    
                if(_hit.transform)
                {
                    RaycastHits.Add(_hit);
                    _pathPoints.Add(_hit.point);
                    var onHit = detectLayer.InLayer(_hit.transform.gameObject);
                    if (onHit)
                    {
                        DetectIndex = _pathPoints.Count - 1;
                        raycastHit = _hit;
                        break;
                    }
                    distance -= (_hit.point - point).magnitude;
                    point = _hit.point;
                    _direction = Vector3.Reflect(_direction, _hit.normal);
                    _direction = ApplyFreeze(_direction);
                    continue;
                }
                _pathPoints.Add(point + _direction.normalized * distance);
                break;
            }
            Physics.queriesHitBackfaces = physicsSetting;
            return raycastHit;
        }

        protected override void OnCast() => hit = ReflectCast(out reflectHits, out PathPoints);

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Send a reflective ray to the <i>Reflect layer</i> and detect the point of impact in the <i>Detect layer</i>." +
                                                 HAccurate + HPathRay + HRecursive+HIRadius;
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            EditorUpdate();

            if (IsManuelMode)
            {
                DrawPath(PathPoints, hit, radius, coneCap: true, detectIndex: DetectIndex);
                Handles.color = HelperColor;
                Handles.DrawDottedLine(transform.position, Tip, StepSizeLine);
            }
            else
            {
                ReflectCast(out _, out var _pathPoints);
                DrawPath(_pathPoints, hit, radius: radius, coneCap: true, detectIndex: DetectIndex);
                Handles.color = HelperColor;
                Handles.DrawDottedLine(transform.position, _pathPoints.Last(), StepSizeLine);
            }

            reflectHits.ForEach(p => DrawNormal(p.point, p.normal, p.transform.name));
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true, bool hasInfo = true)
        {
            if (hasMain)
            {
                DirectionField(_so);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Plane Axis");
                hasFreezeAxis = EditorGUILayout.Toggle(hasFreezeAxis, GUILayout.Width(20));
                GUI.enabled = hasFreezeAxis;
                planeAxis = (Axis) GUILayout.SelectionGrid((int) planeAxis, Enum.GetNames(typeof(Axis)), 3);
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                RadiusField(_so);
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(reflectLayer)),
                    CReflectLayer.ToContent(TReflectLayer));
            }
            if (hasGeneral) GeneralField(_so);
            if (hasEvents) EventField(_so);
            if (hasInfo) InformationField(() => reflectHits.ForEach(r =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{reflectHits.IndexOf(r)}: {r.transform.name}");
                GUILayout.Label(r.point.ToString());
                GUILayout.EndHorizontal();
            }));
        }
#endif
    }
}