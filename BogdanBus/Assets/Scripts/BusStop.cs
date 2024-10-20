using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BusStop : MonoBehaviour
{
    [Header("Bus Stop Data")]
    [SerializeField]
    private Bus _bus = null;
    [SerializeField]
    private Door[] _doorArr;  // 0 - front , 1 - back
    [SerializeField]
    private float _deltaNeighborhood = 0.2f;    // neighborhood of stoped speed (0)
    [SerializeField]
    private List<Passenger> _passengerList;

    [Header("Passenger Points")]
    [SerializeField]
    private Transform[] _doorPointArr;  // 0 - front , 1 - back
    [SerializeField]
    private Transform _driverPoint;
    [SerializeField]
    private List<Transform> _controlPointList;  // front , center , back
    [SerializeField]
    private List<Transform> _seatFreePointList;
    [SerializeField]
    private bool _isDataGathered;

    private void Start()
    {
        _doorArr = new Door[2];
        _doorPointArr = new Transform[2];
    }

    private void Update()
    {
        foreach (Passenger passenger in _passengerList)
        {
            // if bus is stopped
            passenger.canGoToBus = (_bus.CurrentSpeed <= _deltaNeighborhood &&
            _bus.CurrentSpeed >= 0);

            // if passenger is in bus -> forget about him
            if (passenger.IsInBus)
                _passengerList.Remove(passenger);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Bus bus))
        {
            _bus = bus;

            // Get: FreeSeatPool
            GatherFreeSeatPoint(ref bus);

            if (!_isDataGathered)
            {
                // Get: DoorPool , DriverPoint , ÑontrolPoint
                _isDataGathered = GatherGeneralPoint(ref bus);
                
                // Get: Doors
                _doorArr[0] = bus.transform.Find("__Doors/_FrontDoor").GetComponent<Door>();
                _doorArr[1] = bus.transform.Find("__Doors/_BackDoor").GetComponent<Door>();
                if (_doorArr[0] == null || _doorArr[1] == null)
                    _isDataGathered = false;

                // Transfer: Door , DoorPoints , DriverPoints , SeatPoints
                if (_isDataGathered)
                {
                    TransferDataToPassenger();
                }
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

    private bool GatherFreeSeatPoint(ref Bus bus)
    {
        // SitPool
        Transform sitPointPool = bus.transform.Find("__BusPointPool/__SitPointPool");
        if (sitPointPool != null)
        {
            foreach (Transform sitPoint in sitPointPool)
            {
                if (sitPoint.tag == "FreeSeat")
                    _seatFreePointList.Add(sitPoint);
            }
        }
        else
            return false;

        return true;
    }

    private bool GatherGeneralPoint(ref Bus bus)
    {
        /* DoorPool , DriverPoint , ÑontrolPoint */

        // Clear lists
        _controlPointList.Clear();

        // DoorPool
        Transform doorPointPool = bus.transform.Find("__BusPointPool/__DoorPointPool");
        if (doorPointPool != null)
        {
            _doorPointArr[0] = doorPointPool.Find("__FrontDoorPoint");
            _doorPointArr[1] = doorPointPool.Find("__BackDoorPoint");
        }
        else
            return false;
        if (_doorPointArr[0] == null || _doorPointArr[1] == null)
            return false;

        // DriverPoint
        _driverPoint = bus.transform.Find("__BusPointPool/__DriverPoint");
        if (_driverPoint == null)
            return false;

        // ÑontrolPoint
        Transform controlPointPool = bus.transform.Find("__BusPointPool/__ControlPointPool");
        if (controlPointPool != null)
        {
            foreach (Transform contolPoint in controlPointPool)
            {
                _controlPointList.Add(contolPoint);
            }
        }
        else
            return false;

        return true;
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
                    passenger.doorPoint = _doorPointArr[0];
                    passenger.door = _doorArr[0];
                    break;
                case Door.DoorMark.Back:
                    passenger.doorPoint = _doorPointArr[1];
                    passenger.door = _doorArr[1];
                    break;
            }

            // Driver
            passenger.driverPoint = _driverPoint;

            // Seat
            int randomSeatIndex = Random.Range(0, _seatFreePointList.Count);
            passenger.seatPoint = _seatFreePointList[randomSeatIndex];
            _seatFreePointList[randomSeatIndex].tag = "TakenSeat";
            _seatFreePointList.RemoveAt(randomSeatIndex);
        }
    }
}
