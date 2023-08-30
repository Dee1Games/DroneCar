namespace RaycastPro.Planers
{
    using RaySensors;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Planers/" + nameof(PortalPlanar))]
    public sealed class PortalPlanar : Planar
    {
        [SerializeField] public Transform outer;

#if UNITY_EDITOR
        
#pragma warning disable CS0414
        private static string Info = "Transferring the Planer Sensitive Ray sequence to the outer gate.";
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            DrawPlanar();

            if (!outer) return;

            var _dSize = DotSize * transform.lossyScale.magnitude;
            
            var p1 = transform.position + transform.forward * _dSize;

            var p2 = outer.transform.position - outer.transform.forward * _dSize;

            Handles.DrawDottedLine(transform.position, p1, StepSizeLine);
            Handles.DrawDottedLine(p1, p2, StepSizeLine);
            Handles.DrawDottedLine(outer.transform.position, p2, StepSizeLine);
            Handles.ConeHandleCap(0, outer.transform.position,
                Quaternion.LookRotation(outer.transform.position - p2, transform.up), DotSize,
                EventType.Repaint);
        }
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(outer)),
                    COuter.ToContent(TOuter));
            }

            if (hasGeneral) GeneralField(_so);

            if (hasEvents) EventField(_so);
        }
#endif

        internal override TransitionData[] GetTransitionData(RaycastHit hit, Vector3 direction)
        {
            tOuter = outer ? outer.transform : transform;
            pos = tOuter.transform.forward * offset + tOuter.PortalPoint(transform, hit.point);
            inverse = transform.InverseTransformDirection(direction);
            var rot = Quaternion.LookRotation(tOuter.TransformDirection(inverse), tOuter.up);

            var data = new TransitionData
            {
                position = pos,
                rotation = rot
            };
            
            return new TransitionData[] {data};
        }

        private Vector3 _hitPoint, pos, forward, inverse;
        private RaySensor clone;
        private Transform tOuter;
        internal override void OnReceiveRay(RaySensor sensor)
        {
            if (!sensor.cloneRaySensor) return;
            
            pos = sensor.hit.point;
            clone = sensor.cloneRaySensor;
            if (!clone) return;
            
            if (clone.liner) clone.liner.enabled = sensor.liner.enabled;
            tOuter = outer ? outer : transform;
            forward = new Vector3();
            switch (baseDirection)
            {
                case DirectionOutput.NegativeHitNormal: forward = -sensor.hit.normal; break;
                case DirectionOutput.HitDirection: forward = sensor.HitDirection; break;
                case DirectionOutput.SensorLocal: forward = sensor.LocalDirection.normalized; break;
                case DirectionOutput.PlanarForward: forward = transform.forward; break;
            }
            clone.transform.position = tOuter.PortalPoint(transform, pos);
            inverse = transform.InverseTransformDirection(baseDirection == DirectionOutput.PlanarForward ? transform.forward : forward);
            clone.transform.rotation = Quaternion.LookRotation(tOuter.TransformDirection(inverse), tOuter.up);
            clone.transform.position += clone.transform.forward * offset;
            ApplyLengthControl(sensor);
        }
    }
}