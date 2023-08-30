using UnityEditor;

namespace RaycastPro.Planers2D
{
#if UNITY_EDITOR
    using Editor;
#endif

    using RaySensors2D;
    using UnityEngine;

    [AddComponentMenu("RaycastPro/Planers/" + nameof(PortalPlanar2D))]
    public sealed class PortalPlanar2D : Planar2D
    {
        public Transform outer;

        private Vector2 point;
        private RaySensor2D clone;
        private Transform _tOuter;
        private Vector3 forward, inverse;
        public override void OnReceiveRay(RaySensor2D sensor)
        {
            if (!sensor.cloneRaySensor) return;
            point = sensor.hit.point;
            clone = sensor.cloneRaySensor;
            if (!clone) return;
            if (clone.liner) clone.liner.enabled = sensor.liner.enabled;
            _tOuter = outer ? outer : transform;
            forward = new Vector3();
            switch (baseDirection)
            {
                case DirectionOutput.NegativeHitNormal: forward = -sensor.hit.normal; break;
                case DirectionOutput.HitDirection: forward = sensor.HitDirection; break;
                case DirectionOutput.SensorLocal: forward = sensor.LocalDirection; break;
                case DirectionOutput.PlanarForward: forward = transform.right; break;
            }

            clone.transform.position = _tOuter.PortalPoint(transform, point).ToDepth(_tOuter.position.z) + _tOuter.TransformDirection(forward * offset);
            inverse = transform.InverseTransformDirection(forward);
            clone.transform.right = _tOuter.TransformDirection(inverse);
            ApplyLengthControl(sensor);
        }

        /// <summary>
        /// Null Back For now
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        internal override TransitionData[] GetTransitionData2D(RaycastHit2D hit, Vector2 direction)
        {
            return new TransitionData[] { };
        }
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Transferring the Planer Sensitive 2D Ray sequence to the outer gate.";
#pragma warning restore CS0414
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(outer)), COuter.ToContent());
            }

            if (hasGeneral) GeneralField(_so);

            if (hasEvents) EventField(_so);
        }
#endif
    }
}