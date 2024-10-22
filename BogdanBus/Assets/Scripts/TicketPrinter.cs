using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketPrinter : MonoBehaviour, IInteractableObject
{
    [SerializeField]
    private Seat _driverPoint;
    [SerializeField]
    private State _state = State.Idle;
    public State GetState { get => _state; }

    public void Interact()
    {
        if (_driverPoint.IsTaken)
        {
            if (_state == State.Idle)
                _state = State.Printed;
            else if (_state == State.Printed)
                _state = State.Returned;
        }
    }

    public void Reset()
    {
        if (_state == State.Returned)
            _state = State.Idle;
    }

    public enum State
    {
        Idle,
        Printed,
        Returned
    }
}
