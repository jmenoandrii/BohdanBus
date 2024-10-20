using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BusStop : MonoBehaviour
{
    [Header("Bus Stop Data")]
    [SerializeField]
    private BusComponents _bus = null;
    [SerializeField]
    private float _deltaNeighborhood = 0.2f;    // neighborhood of stoped speed (0)
    [SerializeField]
    private List<Passenger> _passengerList;
    [SerializeField]
    private bool _isDataTransferred;

    private void Update()
    {
        if (_bus != null && _passengerList.Count != 0)
        {
            byte i = 0;
            while (_passengerList.Count != i)
            {
                _passengerList[i].canGoToBus = (_bus.BusSpeed <= _deltaNeighborhood &&
                _bus.BusSpeed >= 0);

                if (_passengerList[i].IsInBus)
                {
                    _passengerList[i].transform.SetParent(_bus.PassengerPool.transform);
                    _passengerList[i].canGoToBus = false;
                    _passengerList.RemoveAt(i);
                }
                else
                    i++;
                if (i > _passengerList.Count)
                    i = 0;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BusComponents bus))
        {
            _bus = bus;

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

    private void TransferDataToPassenger()
    {
        /* Door , DoorPoints , DriverPoints , SeatPoints */
        
        foreach (Passenger passenger in _passengerList)
        {
            // Door
            switch (passenger.DoorMark)
            {
                case Door.DoorMark.Front:
                    passenger.doorPoint = _bus.FrontDoorPoint;
                    passenger.door = _bus.FrontDoor.GetComponent<Door>();
                    break;
                case Door.DoorMark.Back:
                    passenger.doorPoint = _bus.BackDoorPoint;
                    passenger.door = _bus.BackDoor.GetComponent<Door>();
                    break;
            }

            // Driver
            passenger.driverPoint = _bus.DriverPoint;

            // Seat
            List<Seat> freeSeatPointList = _bus.FreeSeatPointList;
            int index = Random.Range(0, freeSeatPointList.Count);
            passenger.seatPoint = freeSeatPointList[index];
            freeSeatPointList[index].Take();

            // Contol points
            passenger.controlPointList = _bus.ControlPointList;

            // Cheked
            passenger.hasData = true;
        }

        _isDataTransferred = true;
    }
}
