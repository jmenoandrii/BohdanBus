using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BusStop : MonoBehaviour
{
    [Header("Bus Stop Data")]
    [SerializeField]
    private BusLine _bus = null;
    [SerializeField]
    private float _deltaSpeed = 0.2f;
    [SerializeField]
    private List<Passenger> _passengers;
    [SerializeField]
    private bool _isDataTransferred;

    private void Update()
    {
        HandlePassengers();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BusLine bus))
        {
            _bus = bus;
            _bus.SetCurrentBusStop(this);

            if (!_isDataTransferred)
            {
                TransferDataToPassenger();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Bus>())
        {
            _bus = null;
        }
    }

    private void HandlePassengers()
    {
        if (_bus == null || _passengers.Count == 0) return;

        for (int i = _passengers.Count - 1; i >= 0; i--)
        {
            Passenger passenger = _passengers[i];
            if (passenger.GetState == Passenger.State.ReadyBoard || passenger.GetState == Passenger.State.BusAccessible)
            {
                if (IsReadyForBoarding(passenger))
                    passenger.StartBoarding();
                else
                    passenger.StopBoarding();
            }

            if (passenger.GetState == Passenger.State.InBus)
            {
                _bus.AddPassenger(passenger);
                passenger.transform.SetParent(_bus.PassengerPool.transform);
                _passengers.RemoveAt(i);
            }
        }
    }

    private bool IsReadyForBoarding(Passenger passenger)
    {
        return _bus.BusSpeed <= _deltaSpeed &&
               _bus.BusSpeed >= 0 &&
               IsCorrectDoorOpen(passenger);
    }

    private bool IsCorrectDoorOpen(Passenger passenger)
    {
        return (passenger.DoorMark == Door.Mark.Front && _bus.IsOpenFrontDoor) ||
               (passenger.DoorMark == Door.Mark.Back && _bus.IsOpenBackDoor);
    }

    private void TransferDataToPassenger()
    {
        foreach (Passenger passenger in _passengers)
        {
            if (passenger.GetState >= Passenger.State.ReadyBoard)
                return;

            Transform doorPoint = null;

            switch (passenger.DoorMark)
            {
                case Door.Mark.Front:
                    doorPoint = _bus.FrontDoorPoint;
                    break;
                case Door.Mark.Back:
                    doorPoint = _bus.BackDoorPoint;
                    break;
            }
            
            passenger.SetDataLine(doorPoint, _bus.DriverPoint, _bus.GetFreeSeat(), _bus.ControlPointList);
        }

        _isDataTransferred = true;
    }
}
