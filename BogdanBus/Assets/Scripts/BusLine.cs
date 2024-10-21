using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bus))]
public class BusLine : MonoBehaviour
{
    private Bus _bus;

    [Header("Doors")]
    [SerializeField]
    private GameObject _frontDoor;
    [SerializeField]
    private GameObject _backDoor;

    [Header("Points")]
    [SerializeField]
    private Transform _frontDoorPoint;
    [SerializeField]
    private Transform _backDoorPoint;
    [SerializeField]
    private Seat _driverPoint;
    [SerializeField]
    private Transform _controlPointPool;
    private List<Transform> _controlPointList;
    [SerializeField]
    private Transform _seatPointPool;

    [Header("Other")]
    [SerializeField]
    private GameObject _passengerPool;


    // Doors
    public GameObject FrontDoor { get => _frontDoor; }
    public GameObject BackDoor { get => _backDoor; }
    // Points
    public Transform FrontDoorPoint { get => _frontDoorPoint; }
    public Transform BackDoorPoint { get => _backDoorPoint; }
    public Seat DriverPoint { get => _driverPoint; }
    public List<Transform> ControlPointList { get => _controlPointList; }
    // Bus Speed
    public float BusSpeed { get => _bus.CurrentSpeed; }
    public GameObject PassengerPool { get => _passengerPool; }


    private void Awake()
    {
        _bus = GetComponent<Bus>();

        // Get control points
        _controlPointList = new List<Transform>();
        foreach (Transform point in _controlPointPool)
            _controlPointList.Add(point);
    }

    public Seat GetFreeSeat()
    {
        List<Seat> freeSeatPointList = new();

        foreach (Transform point in _seatPointPool)
        {
            Seat seat = point.GetComponent<Seat>();
            if (!seat.IsTaken)
                freeSeatPointList.Add(seat);
        }

        int index = Random.Range(0, freeSeatPointList.Count);
        freeSeatPointList[index].Take();
        return freeSeatPointList[index];
    }
}
