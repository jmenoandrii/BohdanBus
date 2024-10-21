using System.Collections.Generic;
using UnityEngine;

public class BusStop : MonoBehaviour
{
    [Header("Bus Stop Data")]
    [SerializeField] private List<Passenger> _passengers;
    [SerializeField] private Transform _disappearedPointPool;
    private BoardingSystem _busSystem;

    [Header("*** View zone ***")]
    [SerializeField] private List<Transform> _disappearedPointList;
    public List<Transform> DisappearedPointList => _disappearedPointList;


    private void Awake()
    {
        foreach (Transform point in _disappearedPointPool)
            _disappearedPointList.Add(point);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BoardingSystem busSystem))
        {
            _busSystem = busSystem;
            _busSystem.SetCurrentBusStop(this);

            TransferDataToPassengers();

            foreach (Passenger passenger in _passengers)
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
        foreach (Passenger passenger in _passengers)
        {
            if (passenger.GetState >= Passenger.State.ReadyBoard)
                continue;

            Transform doorPoint = GetDoorPoint(passenger);
            passenger.SetDataLine(doorPoint, _busSystem.DriverPoint, _busSystem.GetFreeSeat(), _busSystem.ControlPointList);
        }
    }

    private Transform GetDoorPoint(Passenger passenger)
    {
        return passenger.DoorMark == Door.Mark.Front ? _busSystem.FrontDoorPoint : _busSystem.BackDoorPoint;
    }

    public void ForgetAboutPassenger()
    {
        /*
            If the bus sends a command that all passengers boarded
         */
        _passengers.Clear();
    }
}
