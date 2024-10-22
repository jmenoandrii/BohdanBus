using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _OnTriggerEnterEvent;

    public bool isActivated { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<Bus>() && !isActivated)
        {
            _OnTriggerEnterEvent.Invoke();
            isActivated = true;
        }
    }
}
