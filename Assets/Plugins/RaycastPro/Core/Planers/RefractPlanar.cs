using UnityEditor;

namespace RaycastPro.Planers
{
    using RaySensors;
    using UnityEngine;

#if UNITY_EDITOR
    using EditorGUILayout = UnityEditor.EditorGUILayout;
#endif

    [AddComponentMenu("RaycastPro/Planers/" + nameof(RefractPlanar))]
    public sealed class RefractPlanar : Planar
    {
        public float refractAngle;
        public float sideAngle;

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Planar Sensitive Ray Refraction Based on refract Direction.";
#pragma warning restore CS0414
        internal override void OnGizmos() => DrawPlanar();
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                PropertySliderField(_so.FindProperty(nameof(refractAngle)), 0f, 180f, "Refract Angle".ToContent());
                PropertySliderField(_so.FindProperty(nameof(sideAngle)), 0f, 360f, "Side Angle".ToContent());
            }

            if (hasGeneral)
            {
                GeneralField(_so);
            }

            if (hasEvents)
            {
                EventField(_so);
            }
        }
#endif
        
        internal override TransitionData[] GetTransitionData(RaycastHit hit, Vector3 direction)
        {
            var data = new TransitionData
            {
                position = hit.point + direction,
                rotation = Quaternion.AngleAxis(sideAngle, Vector3.forward) *
                           Quaternion.AngleAxis(refractAngle, Vector3.right),
            };

            return new TransitionData[] {data};
        }

        private Vector3 forward, look;
        private RaySensor clone;
        internal override void OnReceiveRay(RaySensor sensor)
        {
            clone = sensor.cloneRaySensor;
            if (!clone) return;
            if (clone.liner) clone.liner.enabled = sensor.liner.enabled;
            forward = -GetForward(sensor, transform.forward);
            ApplyLengthControl(sensor);
            clone.transform.position = sensor.hit.point;
            look = Quaternion.AngleAxis(sideAngle, Vector3.forward) * Quaternion.AngleAxis(refractAngle, Vector3.right) * forward;
            clone.transform.forward = -look;
            clone.transform.position += clone.transform.forward * offset; // Apply Offset
        }
    }
}