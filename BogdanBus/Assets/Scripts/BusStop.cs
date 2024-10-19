using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusStop : MonoBehaviour
{
    [SerializeField]
    private bool _isBusHere;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bus>())
        {
            _isBusHere = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Bus>())
        {
            _isBusHere = false;
        }
    }
}
