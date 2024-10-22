using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] bool _isBus;

    public bool IsBus => _isBus;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bus>())
            _isBus = true;
    }
}
