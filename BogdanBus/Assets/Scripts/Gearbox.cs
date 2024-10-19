using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Gearbox : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    private Bus _bus;

    private Animator _animator;
    private bool _isChange;
    private int _rotationCoef;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rotationCoef = _animator.GetInteger("rotationCoef");
        SetActiveCoef();
    }

    private void SetActiveCoef()
    {
        switch (_bus.GearState)
        {
            case Bus.Gear.Drive:
                _rotationCoef = 1;
                break;
            case Bus.Gear.Park:
                _rotationCoef = 0;
                break;
            case Bus.Gear.Reverse:
                _rotationCoef = -1;
                break;
        }
    }

    public void Interact()
    {

    }

    public void Interact(bool up)
    {
        Bus.Gear _newGear = _bus.GearState;
        _newGear += up ? -1 : 1;

        _isChange = _bus.ShiftGear(_newGear);

        // Aniation
        if (_isChange)
        {
            SetActiveCoef();
            _animator.SetInteger("rotationCoef", _rotationCoef);
        }
        else
        {
            /* 
                ! play sound ! 
                can't switch gear
            */
        }
    }
}
