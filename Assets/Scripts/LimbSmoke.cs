using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbSmoke : MonoBehaviour
{
    public int level;
    public Transform poolParent;

    public void ReturnToPool()
    {
        transform.parent = poolParent;
        gameObject.SetActive(false);
    }
}
