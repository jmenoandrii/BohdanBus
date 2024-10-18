using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gearbox : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    private Bus _bus;

    public void Interact()
    {

    }

    public void Interact(bool up)
    {
        Bus.Gear _newGear = _bus.GearState;
        _newGear += up ? -1 : 1;

        _bus.ShiftGear(_newGear);
    }
}
