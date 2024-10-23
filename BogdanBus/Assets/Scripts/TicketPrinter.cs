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

    [Header("Sounds")]
    [SerializeField] AudioSource _audioPrint;
    [SerializeField] AudioSource _audioRip;

    public void Interact()
    {
        if (_driverPoint.IsTaken)
        {
            if (_state == State.Idle)
            {
                _state = State.Printed;

                // ~~~ audio (print) ~~~
                if (_audioRip.isPlaying)
                    _audioRip.Stop();

                if (!_audioPrint.isPlaying)
                    _audioPrint.Play();
            }
            else if (_state == State.Printed)
            {
                _state = State.Returned;

                // ~~~ audio (rip) ~~~
                if (_audioPrint.isPlaying)
                    _audioPrint.Stop();

                if (!_audioRip.isPlaying)
                    _audioRip.Play();
            }
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
