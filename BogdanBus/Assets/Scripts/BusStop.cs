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
                _passengerList[i].canGoToBus = false;

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
        if (other.TryGetComponent(out BusLine bus))
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
        foreach (Passenger passenger in _passengerList)
        {
            if (passenger.hasData)
                return;

            Transform doorPoint;

            // Door
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

            // Cheked
            passenger.hasData = true;
        }

        _isDataTransferred = true;
    }
}
