using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Bus))]
public class BusLine : MonoBehaviour
{
    private Bus _bus;

    [Header("Doors")]
    [SerializeField]
    private Door _frontDoor;
    [SerializeField]
    private Door _backDoor;

    [Header("Points")]
    [SerializeField]
    private Transform _frontDoorPoint;
    [SerializeField]
    private Transform _backDoorPoint;
    [SerializeField]
    private Transform _frontDoorExitPoint;
    [SerializeField]
    private Transform _backDoorExitPoint;
    [SerializeField]
    private Seat _driverPoint;
    [SerializeField]
    private Transform _controlPointPool;
    private List<Transform> _controlPointList;
    [SerializeField]
    private Transform _seatPointPool;
    [SerializeField]
    private TicketPrinter _ticketPrinter;

    [Header("Other")]
    [SerializeField]
    private GameObject _passengerPool;
    [SerializeField]
    private List<Passenger> _passengers;

    private BusStop _currentBusStop;
    [SerializeField]
    private float _deltaSpeed = 0.2f;

    // Doors
    public bool IsOpenFrontDoor { get => _frontDoor.IsOpen; }
    public bool IsOpenBackDoor { get => _backDoor.IsOpen; }
    // Points
    public Transform FrontDoorPoint { get => _frontDoorPoint; }
    public Transform BackDoorPoint { get => _backDoorPoint; }
    public Seat DriverPoint { get => _driverPoint; }
    public List<Transform> ControlPointList { get => _controlPointList; }
    // Bus Speed
    public float BusSpeed { get => _bus.CurrentSpeed; }
    public GameObject PassengerPool { get => _passengerPool; }


    private void Awake()
    {
        _bus = GetComponent<Bus>();

        // Get control points
        _controlPointList = new List<Transform>();
        foreach (Transform point in _controlPointPool)
            _controlPointList.Add(point);
    }

    private void Update()
    {
        if (_passengers.Count != 0)
        {
            foreach (Passenger passenger in _passengers)
            {
                if (passenger.GetState == Passenger.State.InBus)
                {
                    if (_ticketPrinter.GetState == TicketPrinter.State.Returned)
                    {
                        passenger.PayedFare();
                        _ticketPrinter.Reset();
                    }
                }
                else if (passenger.GetState == Passenger.State.Sitting)
                {
                    if (passenger.GetDestination == _currentBusStop)
                    {
                        bool isOpenDoor = false;

                        if (passenger.DoorMark == Door.Mark.Front && IsOpenFrontDoor)
                        {
                            isOpenDoor = true;
                        }
                        else if (passenger.DoorMark == Door.Mark.Back && IsOpenBackDoor)
                        {
                            isOpenDoor = true;
                        }

                        if (BusSpeed <= _deltaSpeed && BusSpeed >= 0 && isOpenDoor)
                        {
                            passenger.StandUp();
                        }
                    }
                }
                else if (passenger.GetState == Passenger.State.LeftBus)
                {
                    passenger.transform.SetParent(null);
                    _passengers.Remove(passenger);
                }
            }
        }
    }

    public void SetCurrentBusStop(BusStop busStop)
    {
        _currentBusStop = busStop;
    }

    public Seat GetFreeSeat()
    {
        List<Seat> freeSeatPointList = new();

        foreach (Transform point in _seatPointPool)
        {
            Seat seat = point.GetComponent<Seat>();
            if (!seat.IsTaken)
                freeSeatPointList.Add(seat);
        }

        int index = Random.Range(0, freeSeatPointList.Count);
        freeSeatPointList[index].Take();
        return freeSeatPointList[index];
    }

    public void AddPassenger(Passenger passenger)
    {
        if (passenger == null) return;
        _passengers.Add(passenger);
        switch (passenger.DoorMark)
        {
            case Door.Mark.Front:
                passenger.SetExitPoint(_frontDoorExitPoint);
                break;
            case Door.Mark.Back:
                passenger.SetExitPoint(_backDoorExitPoint);
                break;
        }
    }
}
