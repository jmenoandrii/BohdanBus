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
    [SerializeField] private List<Passenger> _passengerList = new List<Passenger>();
    [SerializeField] private List<Passenger> _boardingPassengerList = new List<Passenger>();
    [SerializeField] private List<Passenger> _payingPassengerList = new List<Passenger>();
    private List<Transform> _controlPointList = new List<Transform>();
    private Bus _bus;
    private sbyte _iterator;
    private Passenger _passenger;

    // Getters
    public bool IsOpenFrontDoor => _frontDoor.IsOpen;
    public bool IsOpenBackDoor => _backDoor.IsOpen;
    public Transform FrontDoorPoint => _frontDoorPoint;
    public Transform BackDoorPoint => _backDoorPoint;
    public Transform FrontDoorExitPoint => _frontDoorExitPoint;
    public Transform BackDoorExitPoint => _backDoorExitPoint;
    public Vector3 DisappearPosition => (_frontDoorExitPoint.position + _backDoorExitPoint.position) / 2;
    public Seat DriverPoint => _driverPoint;
    public List<Transform> ControlPointList => _controlPointList;
    public float BusSpeed => _bus.CurrentSpeed;

    private void Awake()
    {
        _bus = GetComponent<Bus>();
        InitializeControlPoints();
    }

    private void Update()
    {
        HandleBoardingPassengers();
        HandlePassengers();
    }

    private void HandleBoardingPassengers()
    {
        /*
                    How do it work?
            - if '_boardingPassengerList' is empty, handler do nothing
            - if passenger has 'state' <= 'State.BusAccessible', he go to the bus or whait when can
            - if passenger has 'state' > 'State.BusAccessible' and still is in '_boardingPassengerList',
              so he arrived to the bus door and can be transfered to '_passengerList'/'_payingPassengerList'.
              Also passenger is removed from 'BusStop._passengerList'
         */
        if (_boardingPassengerList.Count == 0) return;

        _iterator = 0;
        while (_iterator < _boardingPassengerList.Count)
        {
            _passenger = _boardingPassengerList[_iterator];

            if (_passenger.GetState <= Passenger.State.BusAccessible)
            {
                if (CanPassengerEnterOrExit(_passenger))
                    _passenger.StartBoarding();
                else
                    _passenger.StopBoarding();
                _iterator++;
            }
            else
            {
                _boardingPassengerList.Remove(_passenger);
                _passenger.transform.SetParent(_passengerPool.transform);
                _passengerList.Add(_passenger);
                _payingPassengerList.Add(_passenger);
                _currentBusStop.ForgetAboutPassenger(_passenger);
            }
        }
    }

    private void HandlePassengers()
    {
        /*
                    How do it work?
            - if '_passengerList' is empty, handler do nothing
            - if '_currentBusStop' is the 'passenger.GetDestination' and he can 
              performe exiting, he left even if he didn't pay the fare.
            - if the passenger is first element in '_payingPassengerList',
              so he go to his driver point
                - if passenger stand near driver and the '_ticketPrinter' give
                  the ticket, passenger is removed from '_payingPassengerList'
                  and go to the seat, because of that next one (who is new first 
                  passenger in '_payingPassengerList') can go to the driver
            - if passenger payed the fare or simply isn't first element in 
              '_payingPassengerList', he go to the seat.
         */
        if (_passengerList.Count == 0) return;

        _iterator = 0;
        while (_iterator < _passengerList.Count)
        {
            _passenger = _passengerList[_iterator];

            if (_currentBusStop != null && _passenger.GetDestination == _currentBusStop && CanPassengerEnterOrExit(_passenger))
            {
                if (_passenger.GetState == Passenger.State.LeftBus)
                {
                    _passengerList.Remove(_passenger);
                    _passenger.transform.SetParent(null);
                    continue;
                }

                _passenger.Left();
            }

            if (_payingPassengerList.Count != 0 && _passenger == _payingPassengerList[0])
            {
                if (_passenger.GetState == Passenger.State.Paying && _ticketPrinter.GetState == TicketPrinter.State.Returned)
                {
                    _payingPassengerList.Remove(_passenger);
                    _passenger.PayedFare();
                    _ticketPrinter.Reset();
                    _iterator++;
                    continue;
                }

                _passenger.GoToDriver();
            }
            else
            {
                _passenger.GoToSeat();
            }

            _iterator++;
        }
    }

    private bool CanPassengerEnterOrExit(Passenger passenger)
    {
        return BusSpeed <= _deltaSpeed &&
               BusSpeed >= 0 &&
               IsCorrectDoorOpen(passenger);
    }

    private bool IsCorrectDoorOpen(Passenger passenger)
    {
        return passenger.DoorMark == Door.Mark.Front ? IsOpenFrontDoor : IsOpenBackDoor;
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

        _passengerList.Add(passenger);
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
        foreach (Passenger passenger in _boardingPassengerList)
        {
            passenger.GoBack();
        }
        _boardingPassengerList.Clear();
    }

    public void AddPassengerToBoardingList(Passenger passenger)
    {
        _boardingPassengerList.Add(passenger);
    }
}
