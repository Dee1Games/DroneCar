using RaycastPro.Detectors2D;
using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    [AddComponentMenu("")]
    public class SteeringController2D : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rigidbody2D;
        [SerializeField] private SteeringDetector2D steeringDetector;

        void Update()
        {
            rigidbody2D.velocity = steeringDetector.SteeringDirection;
        }
    }
}
