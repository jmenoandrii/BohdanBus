using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [Header("Passenger Data")]
    [SerializeField]
    private Door.DoorMark _doorMark;
    public Door.DoorMark DoorMark { get => _doorMark; }
    [SerializeField]
    public Door door;

    [Header("Bus Trigers")]
    [SerializeField]
    private bool _isInBus;
    public bool IsInBus { get => _isInBus; }
    [SerializeField]
    public bool canGoToBus;
    [SerializeField]
    private bool _isFarePaid;
    [SerializeField]
    private bool _isSitting;

    [Header("Bus Points")]
    [SerializeField]
    public Transform doorPoint;
    [SerializeField]
    public Transform driverPoint;
    [SerializeField]
    public Transform seatPoint;
    public bool HasPoint 
    { 
        get => doorPoint != null && driverPoint != null && seatPoint != null; 
    }

    private void Update()
    {
        if (!_isInBus && canGoToBus && door.IsOpen)
        {
            GoToBus();
        }
        else if (!_isFarePaid && driverPoint.tag == "FreeSeat")
        {
            _isSitting = false;
            GoToDriver();
        }
        else if (_isSitting == false)
        {
            GoToSeat();
        }
        else
            Stand();
    }

    private void GoToBus()
    {
        
    }

    private void GoToDriver()
    {
        driverPoint.tag = "TakenSeat";

        _isFarePaid = true;
    }

    private void GoToSeat()
    {
        _isSitting = true;
    }

    private void Stand()
    {

    }
}
