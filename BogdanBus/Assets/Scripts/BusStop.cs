using System.Collections.Generic;
using UnityEngine;

public class BusStop : MonoBehaviour
{
    [Header("Bus Stop Data")]
    [SerializeField] private List<Passenger> _passengers;
    private BoardingSystem _busSystem;
    private bool _isDataTransferred;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BoardingSystem busSystem))
        {
            _busSystem = busSystem;
            _busSystem.SetCurrentBusStop(this);

            TransferDataToPassengers();

            foreach (Passenger passenger in _passengers)
            {
                _busSystem.AddPassengerToBus(passenger);
            }

            _passengers.Clear();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<BoardingSystem>())
        {
            _busSystem.ClearCurrentBusStop();
            _busSystem = null;
        }
    }

    private void TransferDataToPassengers()
    {
        foreach (Passenger passenger in _passengers)
        {
            if (passenger.GetState >= Passenger.State.ReadyBoard)
                continue;

            Transform doorPoint = GetDoorPoint(passenger);
            passenger.SetDataLine(doorPoint, _busSystem.DriverPoint, _busSystem.GetFreeSeat(), _busSystem.ControlPointList);
        }

        _isDataTransferred = true;
    }

    private Transform GetDoorPoint(Passenger passenger)
    {
        return passenger.DoorMark == Door.Mark.Front ? _busSystem.FrontDoorPoint : _busSystem.BackDoorPoint;
    }
}
