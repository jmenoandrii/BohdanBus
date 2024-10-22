using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [Header("Passenger Data")]
    [SerializeField] private Door.Mark _doorMark;
    public Door.Mark DoorMark { get => _doorMark; }

    [SerializeField] private BusStop _busStopOfDestination;
    public BusStop GetDestination { get => _busStopOfDestination; }

    [Header("Movement Settings")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _deltaBottom = 0.5f;
    [SerializeField] private float _deltaForward = 1f;
    [SerializeField] private float _disappearDistance = 2f;
    private Vector3 targetPosition;

    [Header("*** View zone ***")]
    // Bus points
    [SerializeField] private Vector3 _startPosition;
    [SerializeField] private Transform _doorPoint;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private Transform _vanishingPoint;
    [SerializeField] private Seat _driverPoint;
    [SerializeField] private Seat _seatPoint;
    [SerializeField] private List<Transform> _controlPointList;
    private bool _isTransfered = false;
    public bool IsTransfered { get => _isTransfered; set => _isTransfered = value; }
    // State
    [SerializeField] private State _state;
    public State GetState { get => _state; }
    [SerializeField] private Destination _destination = Destination.None;

    private void Awake()
    {
        _startPosition = this.transform.position;
    }

    private void Update()
    {
        Moving();
    }
    
    private void Moving()
    {
        if (_destination == Destination.None)
            return;

        if (_destination == Destination.ToBack)         // GoBack
        {
            MoveTo(_startPosition);
        }
        else if (_state == State.BusAccessible)         // GoToBus
        {
            if (Vector3.Distance(transform.position, _doorPoint.position) > _deltaForward)
                MoveTo(_doorPoint.position - Vector3.up * _deltaBottom);
            else
                MoveTo(_doorPoint.position);
        }
        else if (_destination == Destination.ToDriver)  // GoToDriver
        {
            MoveTo(_driverPoint.transform.position);
        }
        else if (_destination == Destination.ToSeat)    // GoToSeat
        {
            MoveTo(_seatPoint.transform.position);
        }
        else if (_destination == Destination.ToExit)    // GoToExitDoor
        {
            MoveTo(_state != State.LeftBus ? _doorPoint.position : _exitPoint.position);
        }
        else if (_destination != Destination.ToVanishing)
        {
            MoveTo(_vanishingPoint.position);
        }
    }

    private void MoveTo(Vector3 target)
    {
        targetPosition = target;

        Vector3 direction = (targetPosition - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
                   
        if (direction != Vector3.zero)
        {              
            Quaternion targetRotation = Quaternion.LookRotation(direction);
                                                       
            float targetYRotation = targetRotation.eulerAngles.y;

            Quaternion smoothRotation = Quaternion.Euler(0, targetYRotation, 0);

                            
            transform.rotation = Quaternion.Slerp(transform.rotation, smoothRotation, Time.deltaTime * _speed * 2);                                           
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            switch (_destination)
            {
                case Destination.ToBack:
                    _destination = Destination.None;
                    break;
                case Destination.ToBus:
                    _state = State.InBus;
                    break;
                case Destination.ToDriver:
                    _driverPoint.Take();
                    _state = State.Paying;
                    break;
                case Destination.ToSeat:
                    _state = State.Sitting;
                    transform.rotation = Quaternion.Euler(0, _seatPoint.transform.rotation.eulerAngles.y, 0); // forwardRotation
                    break;
                case Destination.ToExit:
                    if (_state != State.LeftBus)
                        _state = State.LeftBus;
                    else
                        _destination = Destination.ToVanishing;
                    break;
                case Destination.ToVanishing:
                    this.gameObject.SetActive(false);
                    break;
            }
        }
    }

    public void GoBack()
    {
        _seatPoint.GiveUp();
        _state = State.OnBusStop;
        _destination = Destination.ToBack;
    }

    public void StartBoarding()
    {
        _destination = Destination.ToBus;
        _state = State.BusAccessible;
    }

    public void StopBoarding()
    {
        _destination = Destination.None;
        _state = State.ReadyBoard;
    }

    public void GoToDriver()
    {
        _destination = Destination.ToDriver;
    }

    public void PayedFare()
    {
        _driverPoint.GiveUp();
        _state = State.FarePaid;
        GoToSeat();
    }

    public void GoToSeat()
    {
        _destination = Destination.ToSeat;
    }

    public void Left()
    {
        _state = State.StandUp;
        _seatPoint.GiveUp();
        _destination = Destination.ToExit;
    }

    public void SetDataLine(Transform doorPoint, Seat driverPoint, Seat seat, List<Transform> controlPointList)
    {
        _doorPoint = doorPoint;
        _driverPoint = driverPoint;
        _seatPoint = seat;
        _controlPointList = controlPointList;

        _state = State.ReadyBoard;
    }

    public void SetExitPointLine(Transform exitPoint, Transform vanishingPoint)
    {
        _exitPoint = exitPoint;
        _exitPoint = vanishingPoint;
    }

    private enum Destination
    {
        None,
        ToBack,
        ToBus,
        ToDriver,
        ToSeat,
        ToExit,
        ToVanishing
    }

    public enum State
    {
        OnBusStop,
        ReadyBoard,
        BusAccessible,
        InBus,
        Paying,
        FarePaid,
        Sitting,
        StandUp,
        LeftBus
    }
}
