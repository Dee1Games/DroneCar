using RaycastPro.RaySensors;
using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class EnergyGun : MonoBehaviour
    {
        public WaveRay waveRay;

        private HoverEnemy _hoverEnemy;

        public float DPS = 24;
        void Start()
        {
            waveRay.onBeginDetect.AddListener(hit =>
            {
                hit.transform.TryGetComponent(out _hoverEnemy);
            });
            waveRay.onEndDetect.AddListener(hit =>
            {
                _hoverEnemy = null;
            });
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                waveRay.gameObject.SetActive(true);
            }

            if (Input.GetMouseButtonUp(0))
            {
                waveRay.gameObject.SetActive(false);
            }
        
            if (_hoverEnemy)
            {
                _hoverEnemy.TakeDamage(Time.deltaTime*DPS);
            }
        }
    }
}
