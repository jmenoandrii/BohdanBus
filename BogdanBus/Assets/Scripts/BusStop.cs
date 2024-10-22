using System.Collections.Generic;
using UnityEngine;

public class BusStop : MonoBehaviour
{
    [Header("Bus Stop Data")]
    [SerializeField] private List<Passenger> _passengerList;
    private BoardingSystem _busSystem;

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
            If the bus sends a command that all passengers boarded
         */
        _passengerList.Remove(passenger);
    }
}
