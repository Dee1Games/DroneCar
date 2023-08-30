using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class Coin : MonoBehaviour
    {
        public void OnCollect()
        {
            Debug.Log($"Collecting {name}!");
        }
    }
}
