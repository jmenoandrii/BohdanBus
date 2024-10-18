using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gearbox : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    private Bus _bus;
    [SerializeField]
    private Bus.Gear _gear = Bus.Gear.Park;

    public void Interact()
    {

    }

    public void Interact(bool up)
    {
        Bus.Gear _curGear = _gear;
        _curGear += up ? 1 : 0;

        _gear = _bus.ShiftGear(_gear) ? _curGear : _gear;
    }
}
