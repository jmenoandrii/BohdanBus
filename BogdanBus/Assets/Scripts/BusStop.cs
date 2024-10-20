using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BusStop : MonoBehaviour
{
    [Header("BusStopTrigers")]
    [SerializeField]
    private bool _isBusHere;
    [SerializeField]
    private float _deltaNeighborhood = 0.2f;    // neighborhood of stoped speed (0)

    [Header("PassengerTrigers")]
    [SerializeField]
    private bool _canMove;
    [SerializeField]
    private bool _isBusStoped;
    [SerializeField]
    private bool _isDriverPlaceFree;
    [SerializeField]
    private Door[] _doorArr;  // 0 - front , 1 - back

    [Header("PassengerPoints")]
    [SerializeField]
    private Transform[] _doorPointArr;  // 0 - front , 1 - back
    [SerializeField]
    private List<Transform> _controlPointList;  // front , center , back
    [SerializeField]
    private List<Transform> _sitPointList;
    [SerializeField]
    private Transform _driverPoint;
    [SerializeField]
    private bool _isDataGathered;

    private void Start()
    {
        _doorArr = new Door[2];
        _doorPointArr = new Transform[2];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Bus bus))
        {
            _isBusHere = true;
            
            if (!_isDataGathered)
            {
                _isDataGathered = GatherPointPool(bus);
                // Get Doors
                _doorArr[0] = bus.transform.Find("__Doors/_FrontDoor").GetComponent<Door>();
                _doorArr[1] = bus.transform.Find("__Doors/_BackDoor").GetComponent<Door>();
                if (_doorArr[0] == null || _doorArr[1] == null)
                    _isDataGathered = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Bus>())
        {
            _isBusHere = false;
        }
    }

    private bool GatherPointPool(Bus bus)
    {
        // Clear lists
        _sitPointList.Clear();
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
        Debug.Log("DoorPool");

        // DriverPoint
        _driverPoint = bus.transform.Find("__BusPointPool/__DriverPoint");
        if (_driverPoint == null)
            return false;
        Debug.Log("DriverPoint");

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
        Debug.Log("ÑontrolPoint");

        // SitPool
        Transform sitPointPool = bus.transform.Find("__BusPointPool/__SitPointPool");
        if (sitPointPool != null)
        {
            foreach (Transform sitPoint in sitPointPool)
            {
                _sitPointList.Add(sitPoint);
            }
        }
        else
            return false;
        Debug.Log("SitPool");

        return true;
    }

    private bool CheckCanMove()
    {
        return true;
    }
}
