using RaycastPro.Casters;
using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class Shooting : MonoBehaviour
    {
        [SerializeField] private BasicCaster _caster;

        private void Start()
        {
            _caster = GetComponent<BasicCaster>();
        }

        void Update()
        {
            // Simple Coding
            if (Input.GetMouseButtonDown(0)) _caster.Cast(0);
        }
    }
}