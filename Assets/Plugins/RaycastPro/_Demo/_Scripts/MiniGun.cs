using RaycastPro.Casters;
using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class MiniGun : MonoBehaviour
    {
        public AdvanceCaster advanceCaster;
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                advanceCaster.Cast(0);
            }
        }
    }
}
