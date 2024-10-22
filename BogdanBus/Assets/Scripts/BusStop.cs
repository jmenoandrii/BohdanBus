using System.Collections.Generic;
using UnityEngine;

public class BusStop : MonoBehaviour
{
    [Header("Bus Stop Data")]
    [SerializeField] private List<Passenger> _passengerList;
    private BoardingSystem _busSystem;

    [Header("*** View zone ***")]
    [SerializeField] private bool _hasPassenger;
    [SerializeField] private State _state;

    public State GetState => _state;

    private void Start()
    {
        _hasPassenger = _passengerList.Count > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BoardingSystem busSystem))
        {
            _busSystem = busSystem;
            _busSystem.SetCurrentBusStop(this);

            TransferDataToPassengers();

            foreach (Passenger passenger in _passengerList)
            {
                _busSystem.AddPassengerToBoardingList(passenger);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<BoardingSystem>())
        {
            if (_passengerList.Count > 0)
            {
                _state = State.IsMissed;
                ClearPassenger();
            }
            else
                _state = State.IsDone;
            _busSystem.ClearCurrentBusStop();
            _busSystem = null;
        }
    }

    private void TransferDataToPassengers()
    {
        /*
            Perform it for the passenger if passenger has the 'OnBusStop' state
         */
        foreach (Passenger passenger in _passengerList)
        {
            if (passenger.GetState >= Passenger.State.ReadyBoard)
                continue;

            Transform doorPoint = GetDoorPoint(passenger);
            passenger.SetDataLine(doorPoint, _busSystem.DriverPoint, _busSystem.GetFreeSeat(), _busSystem.ControlPointList);
            passenger.SetExitPointLine(passenger.DoorMark == Door.Mark.Front ? _busSystem.FrontDoorExitPoint : _busSystem.BackDoorExitPoint, _busSystem.VanishingPoint);
        }
    }

    private Transform GetDoorPoint(Passenger passenger)
    {
        return passenger.DoorMark == Door.Mark.Front ? _busSystem.FrontDoorPoint : _busSystem.BackDoorPoint;
    }

    public void ForgetAboutPassenger(Passenger passenger)
    {
        /*
            If the bus sends a command that the passengers boarded
         */
        _passengerList.Remove(passenger);
    }

    public void ClearPassenger()
    {
        /* 
            bus stop is missed
         */
        foreach (Passenger passenger in _passengerList)
        {
            passenger.gameObject.SetActive(false);
        }
        _passengerList.Clear();
    }

    public enum State
    {
        None,
        IsMissed,
        IsDone
    }
}
