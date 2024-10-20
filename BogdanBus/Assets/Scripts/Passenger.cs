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
    [SerializeField]
    public bool hasData;

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
    public Seat driverPoint;
    [SerializeField]
    public Seat seatPoint;
    [SerializeField]
    public List<Transform> controlPointList;

    private void Update()
    {
        if (!hasData)
            return;

        if (!_isInBus && canGoToBus && door.IsOpen)
        {
            GoToBus();
        }
        else if (_isInBus && !_isFarePaid && driverPoint.tag == "FreeSeat")
        {
            GoToDriver();
        }
        else if (_isInBus && _isSitting == false)
        {
            GoToSeat();
        }
        else
        {
            Stand();
        }
    }

    private void GoToBus()
    {
        transform.position = doorPoint.position;

        _isInBus = true;
    }

    private void GoToDriver()
    {
        driverPoint.GiveUp();
        _isSitting = false; // if Passenger sitted before 'GoToDriver'

        transform.position = driverPoint.transform.position;

        _isFarePaid = true;
    }

    private void GoToSeat()
    {
        transform.position = seatPoint.transform.position;

        _isSitting = true;
    }

    private void Stand()
    {
    }
}
