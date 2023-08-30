using RaycastPro.RaySensors;
using RaycastPro.RaySensors2D;
using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class SyncRotation : MonoBehaviour
    {
        public RaySensor raySensor;
        public RaySensor2D raySensor2D;

        void Update()
        {
            if (raySensor)
            {
                raySensor.Cast();
                transform.forward = raySensor.TipDirection;
            }
            else if (raySensor2D)
            {
                raySensor2D.Cast();
                transform.forward = raySensor2D.TipDirection;
            }
        }
    }
}
