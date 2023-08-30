using UnityEditor;

namespace RaycastPro.Planers
{
    using RaySensors;
    using UnityEngine;

    [AddComponentMenu("RaycastPro/Planers/" + nameof(ReflectPlanar))]
    public sealed class ReflectPlanar : Planar
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "The reflection of the Planar Sensitive Ray from the Hit Point.";
#pragma warning restore CS0414
        internal override void OnGizmos() => DrawPlanar();
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasGeneral) GeneralField(_so);

            if (hasEvents) EventField(_so);
        }
#endif

        internal override TransitionData[] GetTransitionData(RaycastHit hit, Vector3 direction)
        {
            var data = new TransitionData
            {
                position = hit.point,
                rotation = Quaternion.LookRotation(Vector3.Reflect(direction, hit.normal), transform.up)
            };

            return new TransitionData[] {data};
        }
        private Vector3 forward, look;
        private RaySensor clone;
        internal override void OnReceiveRay(RaySensor sensor)
        {
            var clone = sensor.cloneRaySensor;
            if (!clone) return;
            if (clone.liner) clone.liner.enabled = sensor.liner.enabled;
            
            forward = -GetForward(sensor, transform.forward);
            clone.transform.forward = Vector3.Reflect(sensor.TipDirection, forward).normalized;
            clone.transform.position = sensor.hit.point - GetForward(sensor, transform.forward) * offset;
            ApplyLengthControl(sensor);
        }
    }
}