using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Bus))]
public class BusComponents : MonoBehaviour
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


    // Doors
    public GameObject FrontDoor { get => _frontDoor; }
    public GameObject BackDoor { get => _backDoor; }
    // Points
    public Transform FrontDoorPoint { get => _frontDoorPoint; }
    public Transform BackDoorPoint { get => _backDoorPoint; }
    public Seat DriverPoint { get => _driverPoint; }
    public List<Transform> ControlPointList { get => _controlPointList; }
    public List<Seat> FreeSeatPointList 
    { 
        get
        {
            List<Seat> freeSeatPointList = new List<Seat>();
            // Get free seat points
            foreach (Transform point in _seatPointPool)
            {
                Seat seat = point.GetComponent<Seat>();
                if (!seat.IsTaken)
                    freeSeatPointList.Add(seat);
            }
            return freeSeatPointList;
        }
    }
    // Bus Speed
    public float BusSpeed { get => _bus.CurrentSpeed; }


    private void Awake()
    {
        _bus = GetComponent<Bus>();

        // Get control points
        _controlPointList = new List<Transform>();
        foreach (Transform point in _controlPointPool)
            _controlPointList.Add(point);
    }
}
