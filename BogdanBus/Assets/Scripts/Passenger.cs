using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [Header("Passenger Data")]
    [SerializeField]
    private Door.Mark _doorMark;
    public Door.Mark DoorMark { get => _doorMark; }
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
    private Transform _doorPoint;
    [SerializeField]
    private Seat _driverPoint;
    [SerializeField]
    private Seat _seatPoint;
    [SerializeField]
    private List<Transform> _controlPointList;

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
        Moving();
    }

    private void Moving()
    {
        if (!hasData) return;

        _goTo = GoTo.None;

        if (!_isInBus && canGoToBus)
        {
            _goTo = GoTo.Bus;
            if (Vector3.Distance(transform.position, targetPosition) > _deltaForward)
                MoveTo(_doorPoint.position - Vector3.up * _deltaBottom);
            else
                MoveTo(_doorPoint.position);
        }
        else if (_isInBus && !_isFarePaid && !_driverPoint.IsTaken)
        {
            if (_goTo != GoTo.Driver)
            {
                Debug.Log($"Take: {this}");
                _driverPoint.Take();
            }
            _goTo = GoTo.Driver;
            MoveTo(_driverPoint.transform.position);
        }
        else if (_isInBus && !_isSitting)
        {
            if (_isFarePaid && _goTo != GoTo.Seat)
            {
                Debug.Log($"GiveUp: {this}");
                _driverPoint.GiveUp();
            }
            _goTo = GoTo.Seat;
            MoveTo(_seatPoint.transform.position);
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

            // ��������� ����� ���������� � ���������� X � Z, ��� � ����� Y
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
                Quaternion forwardRotation = Quaternion.Euler(0, _seatPoint.transform.rotation.eulerAngles.y, 0);
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
                MoveTo(_seatPoint.transform.position);
            }
            yield return null; // ������ ���������� �����
        }
    }

    private void Stand()
    {
        // �������, ��� ����� ���������� �������� �������
    }

    public void SetDataLine(Transform doorPoint, Seat driverPoint, Seat seat, List<Transform> controlPointList)
    {
        _doorPoint = doorPoint;
        _driverPoint = driverPoint;
        _seatPoint = seat;
        _controlPointList = controlPointList;
    }

    private enum GoTo
    {
        None,
        Bus,
        Driver,
        Seat
    }
}
