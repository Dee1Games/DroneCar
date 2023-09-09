using System.Collections;
using System.Collections.Generic;
using RaycastPro.Detectors;
using RaycastPro.RaySensors;
using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class EnergyGun : MonoBehaviour
    {
        public WaveRay waveRay;
        public LineDetector lineDetector;

        public List<HoverEnemy> detectedEnemies;
        public float linerSetupTime = 2f;
        public float DPS = 24;
        void Start()
        {
            lineDetector.SyncDetection(detectedEnemies);
        }
        
        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                waveRay.linerEndPosition += Time.deltaTime / linerSetupTime;
                waveRay.Cast();
                
                foreach (var detectedEnemy in detectedEnemies)
                {
                    detectedEnemy.TakeDamage(Time.deltaTime*DPS);
                    detectedEnemy.body.AddForce(-transform.forward * 7);
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                waveRay.gameObject.SetActive(true);
            }

            if (Input.GetMouseButtonUp(0))
            {
                waveRay.gameObject.SetActive(false);
                waveRay.linerEndPosition = 0;
            }
        
            // Optimized Way

        }
    }
}
