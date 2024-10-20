using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [Header("Passenger Data")]
    [SerializeField]
    private Door.DoorMark _doorMark;
    public Door.DoorMark DoorMark { get => _doorMark; }
    [SerializeField]
    public Door door;
    [SerializeField]
    public bool hasData;

    [Header("Bus Triggers")]
    [SerializeField]
    private bool _isInBus;
    public bool IsInBus { get => _isInBus; }
    [SerializeField]
    public bool canGoToBus;
    [SerializeField]
    private bool _isFarePaid;
    [SerializeField]
    private bool _isSitting;

    [Header("Bus Points")]
    [SerializeField]
    public Transform doorPoint;
    [SerializeField]
    public Seat driverPoint;
    [SerializeField]
    public Seat seatPoint;
    [SerializeField]
    public List<Transform> controlPointList;

    [Header("Movement Settings")]
    [SerializeField]
    private float _speed = 2f; // �������� ����������
    [SerializeField]
    private float _deltaBottom = 0.5f; // ������ ��� ������ �� ������
    [SerializeField]
    private float _deltaForward = 1f; // ������ ��� ������ �� ������
    private Vector3 targetPosition;
    [SerializeField]
    private GoTo _goTo;

    private void Update()
    {
        if (!hasData) return;

        _goTo = GoTo.None;

        if (!_isInBus && canGoToBus && door.IsOpen)
        {
            _goTo = GoTo.Bus;
            if (Vector3.Distance(transform.position, targetPosition) > _deltaForward)
                MoveTo(doorPoint.position - Vector3.up * _deltaBottom);
            else
                MoveTo(doorPoint.position);
        }
        else if (_isInBus && !_isFarePaid && !driverPoint.IsTaken)
        {
            if (_goTo != GoTo.Driver)
            {
                Debug.Log($"Take: {this}");
                driverPoint.Take();
            }
            _goTo = GoTo.Driver;
            MoveTo(driverPoint.transform.position);
        }
        else if (_isInBus && !_isSitting)
        {
            if (_isFarePaid && _goTo != GoTo.Seat)
            {
                Debug.Log($"GiveUp: {this}");
                driverPoint.GiveUp();
            }
            _goTo = GoTo.Seat;
            MoveTo(seatPoint.transform.position);
        }
        else
        {
            Stand();
        }
    }

    private void MoveTo(Vector3 target)
    {
        targetPosition = target;

        // ���������� �������� �� ������� �����
        Vector3 direction = (targetPosition - transform.position).normalized;

        // ������ ��������� �������� �� ������� �������
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);

        // ������� �� ������� �����
        if (direction != Vector3.zero)
        {
            // ���������� �������� ����
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // �������� ����� Y-�������� ��������� ���������
            float targetYRotation = targetRotation.eulerAngles.y;

            // ��������� ����� ��������� � ��������� X � Z, ��� � ����� Y
            Quaternion smoothRotation = Quaternion.Euler(0, targetYRotation, 0);

            // �������� ��������
            transform.rotation = Quaternion.Slerp(transform.rotation, smoothRotation, Time.deltaTime * _speed * 2); // ������� �������� �������� ��� ����� �������
        }

        // ���� ������� ������� �������, ���������� ��������� ����
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (_goTo == GoTo.Bus)
            {
                Debug.Log($"ToBus: {this}");
                _isInBus = true;
            }
            else if (_goTo == GoTo.Driver)
            {
                Debug.Log($"ToDriver: {this}");
                StartCoroutine(WaitForPayment());
            }
            else if (_goTo == GoTo.Seat)
            {
                Debug.Log($"ToSeat: {this}");
                _isSitting = true;

                // ������� �������� ������ ���� ����, �� �� ��
                Quaternion forwardRotation = Quaternion.Euler(0, seatPoint.transform.rotation.eulerAngles.y, 0);
                transform.rotation = forwardRotation;
            }
        }
    }


    private IEnumerator WaitForPayment()
    {
        while (!_isFarePaid)
        {
            // ������, ���� ������� ������� ������ 'P'
            if (Input.GetKeyDown(KeyCode.P))
            {
                _isFarePaid = true;
                Debug.Log($"Pay: {this}");

                // ����� ����� �� ����
                yield return new WaitForSeconds(0.5f); // �������� ����� ��������� �� ����
                MoveTo(seatPoint.transform.position);
            }
            yield return null; // ������ ���������� �����
        }
    }

    private void Stand()
    {
        // �������, ��� ����� ���������� ������� �������
    }

    private enum GoTo
    {
        None,
        Bus,
        Driver,
        Seat
    }
}
