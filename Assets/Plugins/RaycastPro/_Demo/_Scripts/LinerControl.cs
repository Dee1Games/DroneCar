using RaycastPro.RaySensors;
using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class LinerControl : MonoBehaviour
    {
        [SerializeField] private RaySensor raySensor;

        private float sinus;
        private void Update()
        {
            sinus = (Mathf.Sin(Time.time) + .8f) * .6f;
            raySensor.linerBasePosition = sinus - .2f;
            raySensor.linerEndPosition = sinus + .2f;
        }
    }
}
