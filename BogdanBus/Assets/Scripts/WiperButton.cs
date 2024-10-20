using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiperButton : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    private Wipers _wiper;

    public void Interact()
    {
        _wiper.Interact();
    }
}
