using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnTriggerEnterEvent;

    private bool isActivated;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<Bus>() && !isActivated)
        {
            OnTriggerEnterEvent.Invoke();
            isActivated = true;
        }
    }
}
