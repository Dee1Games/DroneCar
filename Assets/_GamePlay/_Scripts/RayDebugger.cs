using System.Collections;
using System.Collections.Generic;
using RaycastPro.RaySensors;
using UnityEngine;

public class RayDebugger : MonoBehaviour
{
    [SerializeField] private PointerRay _pointerRay;
    
    private IHitable _hitable;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_pointerRay.Cast())
            {
                _hitable = _pointerRay.Hit.transform.GetComponent<IHitable>();

                Debug.Log(_pointerRay.Hit.transform.name);

                _hitable?.OnHit(CarCore._, transform.position, _pointerRay.Influence * 100, false);
            }
        }
    }    
}
