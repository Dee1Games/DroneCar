using System.Collections.Generic;
using RaycastPro.Detectors;
using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class CoinCollecting : MonoBehaviour
    {
        public RangeDetector rangeDetector;
        public List<Coin> coins;

        private void Start()
        {
            rangeDetector.SyncDetection(coins, c=> c.OnCollect());
        }
    }
}