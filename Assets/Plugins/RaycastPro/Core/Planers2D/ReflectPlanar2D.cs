using UnityEditor;

namespace RaycastPro.Planers2D
{
    using RaySensors2D;
    using UnityEngine;

    [AddComponentMenu("RaycastPro/Planers/" + nameof(ReflectPlanar2D))]
    public sealed class ReflectPlanar2D : Planar2D
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "The reflection of the Planar Sensitive 2D Ray from the Hit Point.";
#pragma warning restore CS0414
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasGeneral)
            {
                GeneralField(_so);
            }

            if (hasEvents)
            {
                EventField(_so);
            }

            if (hasInfo)
            {
                InformationField();
            }
        }
#endif

        internal override TransitionData[] GetTransitionData2D(RaycastHit2D hit, Vector2 direction)
        {
            var data = new TransitionData
            {
                position = hit.point,
                rotation = Quaternion.LookRotation(Vector3.Reflect(direction, hit.normal), transform.up)
            };

            return new TransitionData[] {data};
        }
        public override void OnReceiveRay(RaySensor2D sensor)
        {
            var clone = sensor.cloneRaySensor;

            if (!clone) return;

            clone.transform.right = Vector2.Reflect(sensor.TipDirection, sensor.hit.normal);

            clone.transform.position = sensor.hit.point - GetForward(sensor, transform.right) * offset;

            ApplyLengthControl(sensor);
        }
    }
}