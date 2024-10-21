using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [Header("Passenger Data")]
    [SerializeField]
    private Door.Mark _doorMark;
    public Door.Mark DoorMark { get => _doorMark; }

    [SerializeField]
    private BusStop _busStopOfDestination;
    public BusStop GetDestination { get => _busStopOfDestination; }

    [Header("Bus Points")]
    [SerializeField]
    private Transform _doorPoint;
    [SerializeField]
    private Transform _exitPoint;
    [SerializeField]
    private Seat _driverPoint;
    [SerializeField]
    private Seat _seatPoint;
    [SerializeField]
    private List<Transform> _controlPointList;

    private bool _isTransfered = false;

    [SerializeField]
    private State _state;
    public State GetState { get => _state; }
    public bool IsTransfered { get => _isTransfered; set => _isTransfered = value; }

    [SerializeField]
    private Destination _destination;

    [Header("Movement Settings")]
    [SerializeField]
    private float _speed = 2f;
    [SerializeField]
    private float _deltaBottom = 0.5f;
    [SerializeField]
    private float _deltaForward = 1f;
    private Vector3 targetPosition;

    private void Update()
    {
        Moving();
    }

    private void Moving()
    {
        if (_state == State.OnBusStop || _state == State.LeftBus)
            return;

        _destination = Destination.None;

        if (_state == State.BusAccessible)
        {
            _destination = Destination.ToBus;
            if (Vector3.Distance(transform.position, targetPosition) > _deltaForward)
                MoveTo(_doorPoint.position - Vector3.up * _deltaBottom);
            else
                MoveTo(_doorPoint.position);
        }
        else if (_state == State.InBus)
        {
            if (_destination != Destination.ToDriver)
            {
                Debug.Log($"take: {this}");
                _driverPoint.Take();
            }
            _destination = Destination.ToDriver;
            MoveTo(_driverPoint.transform.position);
        }
        else if (_state == State.FarePaid)
        {
            _destination = Destination.ToSeat;
            MoveTo(_seatPoint.transform.position);
        }
        else if (_state == State.StandUp)
        {
            _destination = Destination.ToExit;
            if (Vector3.Distance(transform.position, targetPosition) > _deltaForward)
                MoveTo(_exitPoint.position - Vector3.up * _deltaBottom);
            else
                MoveTo(_exitPoint.position);
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
                case Destination.ToBus:
                    Debug.Log($"ToBus: {this}");
                    _state = State.InBus;
                    break;
                case Destination.ToDriver:
                    Debug.Log($"ToDriver: {this}");
                    break;
                case Destination.ToSeat:
                    Debug.Log($"ToSeat: {this}");
                    _state = State.Sitting;
                    Quaternion forwardRotation = Quaternion.Euler(0, _seatPoint.transform.rotation.eulerAngles.y, 0);
                    transform.rotation = forwardRotation;
                    break;
                case Destination.ToExit:
                    Debug.Log($"ToExit: {this}");
                    _state = State.LeftBus;
                    break;
            }
        }
    }

    private IEnumerator WaitForPayment()
    {
        //while (!_isFarePaid)
        //{
        //    if (Input.GetKeyDown(KeyCode.P))
        //    {
        //        _isFarePaid = true;
        //        Debug.Log($"Pay: {this}");

        //        yield return new WaitForSeconds(0.5f);                                
        //        MoveTo(_seatPoint.transform.position);
        //    }
        //    yield return null;                       
        //}

        yield return null; 
    }

    public void StandUp()
    {
        _state = State.StandUp;
        _seatPoint.GiveUp();
    }

    public void StartBoarding()
    {
        _state = State.BusAccessible;
    }

    public void StopBoarding()
    {
        _state = State.ReadyBoard;
    }

    public void PayedFare()
    {
        _state = State.FarePaid;
        _destination = Destination.ToSeat;
        _driverPoint.GiveUp();
    }


    public void SetDataLine(Transform doorPoint, Seat driverPoint, Seat seat, List<Transform> controlPointList)
    {
        _doorPoint = doorPoint;
        _driverPoint = driverPoint;
        _seatPoint = seat;
        _controlPointList = controlPointList;

        _state = State.ReadyBoard;
    }

    public void SetExitPoint(Transform exitPoint)
    {
        _exitPoint = exitPoint;
    }

    private enum Destination
    {
        None,
        ToBus,
        ToDriver,
        ToSeat,
        ToExit
    }

    public enum State
    {
        OnBusStop,
        ReadyBoard,
        BusAccessible,
        InBus,
        FarePaid,
        Sitting,
        StandUp,
        LeftBus
    }
}
