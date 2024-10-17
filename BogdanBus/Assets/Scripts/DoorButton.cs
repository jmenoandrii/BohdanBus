using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    private Door[] doors;

    public void Interact()
    {
        foreach (var door in doors)
        {
            door.Interact(); 
        }
    }
}
