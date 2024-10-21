using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bus))]
public class BoardingSystem : MonoBehaviour
{
    [Header("Bus Settings")]
    [SerializeField] private Door _frontDoor;
    [SerializeField] private Door _backDoor;
    [SerializeField] private Transform _frontDoorPoint;
    [SerializeField] private Transform _backDoorPoint;
    [SerializeField] private Transform _frontDoorExitPoint;
    [SerializeField] private Transform _backDoorExitPoint;
    [SerializeField] private Seat _driverPoint;
    [SerializeField] private Transform _controlPointPool;
    [SerializeField] private Transform _seatPointPool;
    [SerializeField] private TicketPrinter _ticketPrinter;
    [SerializeField] private GameObject _passengerPool;
    [SerializeField] private float _deltaSpeed = 0.2f;

    [Header("*** View zone ***")]
    [SerializeField] private BusStop _currentBusStop;
    [SerializeField] private List<Passenger> _passengers = new List<Passenger>();
    [SerializeField] private List<Passenger> _boardingPassengerList = new List<Passenger>();
    [SerializeField] private List<Passenger> _payingPassengerList = new List<Passenger>();
    [SerializeField] private List<Passenger> _validatedPassengerList = new List<Passenger>();
    private List<Transform> _controlPointList = new List<Transform>();
    private Bus _bus;

    // Doors
    public bool IsOpenFrontDoor => _frontDoor.IsOpen;
    public bool IsOpenBackDoor => _backDoor.IsOpen;
    public Transform FrontDoorPoint => _frontDoorPoint;
    public Transform BackDoorPoint => _backDoorPoint;
    public Seat DriverPoint => _driverPoint;
    public List<Transform> ControlPointList => _controlPointList;
    public float BusSpeed => _bus.CurrentSpeed;
    public GameObject PassengerPool => _passengerPool;

    private void Awake()
    {
        _bus = GetComponent<Bus>();
        InitializeControlPoints();
    }

    private void Update()
    {
        HandlePassengers();
    }

    private void HandlePassengers()
    {
        if (_passengers.Count == 0) return;

        for (int i = _passengers.Count - 1; i >= 0; i--)
        {
            Passenger passenger = _passengers[i];

            if (passenger.GetState <= Passenger.State.BusAccessible)
            {
                if (IsReadyForBoarding(passenger))
                    passenger.StartBoarding();
                else
                    passenger.StopBoarding();
            }
            else if (passenger.GetState == Passenger.State.InBus)
            {
                if (!passenger.IsTransfered)
                {
                    passenger.transform.SetParent(_passengerPool.transform);
                    passenger.IsTransfered = true;
                }

                if (_ticketPrinter.GetState == TicketPrinter.State.Returned)
                {
                    passenger.PayedFare();
                    _ticketPrinter.Reset();
                }
            }
            else if (passenger.GetState == Passenger.State.Sitting)
            {
                if (_currentBusStop != null && passenger.GetDestination == _currentBusStop && IsReadyForDisembark(passenger))
                {
                    passenger.StandUp();
                }
            }
            else if (passenger.GetState == Passenger.State.LeftBus)
            {
                passenger.transform.SetParent(null);
                _passengers.RemoveAt(i);
            }
        }
    }

    private bool IsReadyForBoarding(Passenger passenger)
    {
        return BusSpeed <= _deltaSpeed &&
               BusSpeed >= 0 &&
               IsCorrectDoorOpen(passenger);
    }

    private bool IsReadyForDisembark(Passenger passenger)
    {
        return IsCorrectDoorOpen(passenger) && BusSpeed <= _deltaSpeed && BusSpeed >= 0;
    }

    private bool IsCorrectDoorOpen(Passenger passenger)
    {
        return (passenger.DoorMark == Door.Mark.Front && IsOpenFrontDoor) ||
               (passenger.DoorMark == Door.Mark.Back && IsOpenBackDoor);
    }

    private void InitializeControlPoints()
    {
        _controlPointList = new List<Transform>();
        foreach (Transform point in _controlPointPool)
            _controlPointList.Add(point);
    }

    public Seat GetFreeSeat()
    {
        List<Seat> freeSeatList = new List<Seat>();

        foreach (Transform point in _seatPointPool)
        {
            Seat seat = point.GetComponent<Seat>();
            if (!seat.IsTaken)
                freeSeatList.Add(seat);
        }

        if (freeSeatList.Count == 0) return null;

        int index = Random.Range(0, freeSeatList.Count);
        freeSeatList[index].Take();
        return freeSeatList[index];
    }

    public void AddPassengerToBus(Passenger passenger)
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

    public void SetCurrentBusStop(BusStop busStop)
    {
        _currentBusStop = busStop;
    }

    public void ClearCurrentBusStop()
    {
        /*
            If bus drive away:
            - we forget about this 'BusStop'
            - we forget about passenger, who didn't board the bus
         */
        _currentBusStop = null;
        _boardingPassengerList.Clear();
    }

    //////

    public void AddPassengerToBoardingList(Passenger passenger)
    {
        _boardingPassengerList.Add(passenger);
    }
}
