using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private GameObject go;

    private bool isUsed;

    public void Activate()
    {
        go.SetActive(true);
        isUsed = false;
    }

    public void Deactivate()
    {
        go.SetActive(false);
        isUsed = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (isUsed)
            return;
        
        if (other.GetComponentInParent<PlayerVehicle>() != null)
        {
            isUsed = true;
            CoinParticlePool.Instance.ShowOne(other.transform, GameManager.Instance.coinReward);
            UserManager.Instance.AddCoins(GameManager.Instance.coinReward);
            Deactivate();
        }
    }
}
