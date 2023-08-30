using System;
using System.Collections;
using RaycastPro;
using RaycastPro.Casters;
using TMPro;
using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class GunSwap : MonoBehaviour
    {
        public static GunSwap singleton;
    
        public GameObject[] guns;

        private int index;

        [SerializeField] private TextMeshProUGUI amountText;

        public BasicCaster basicCaster;

        private void Awake()
        {
            singleton = this;
        }

        private void Start()
        {
            basicCaster.onCast.AddListener(b =>
            {
                amountText.text = $"{basicCaster.ammo.Amount} / {basicCaster.ammo.MagazineAmount}";
            });
        }

        public void Revive(HoverEnemy enemy, float delay)
        {
            StartCoroutine(Reviver(enemy, delay));
        }
        private IEnumerator Reviver(HoverEnemy enemy, float delay)
        {
            yield return new WaitForSeconds(delay);
            enemy.Revive();
        }
    
        public void SetTimeScale(float value)
        {
            Time.timeScale = value;
        }
        public void OnChange(Int32 value)
        {
            index = value;
            for (var i = 0; i < guns.Length; i++)
            {
                guns[i].gameObject.SetActive(false);
            }
            guns[value].gameObject.SetActive(true);
            guns[value].GetComponentInChildren<BaseCaster>().Reload(); // Manuel Reload
        }
    }
}
