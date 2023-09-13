using UnityEngine;
using System.Collections;

public class ECdestroyMe : MonoBehaviour{
	
    [SerializeField] private float deathTimer = 10;
    void Start ()
	{
		Destroy(gameObject,deathTimer);
	}
}
