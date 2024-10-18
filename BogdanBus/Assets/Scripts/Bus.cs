using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bus : MonoBehaviour
{
    [SerializeField]
    private WheelCollider _frontLeftWheel;
    [SerializeField]
    private WheelCollider _frontRightWheel;
    [SerializeField]
    private WheelCollider _backLeftWheel;
    [SerializeField]
    private WheelCollider _backRightWheel;

    [SerializeField]
    private float _acceleration = 500f;
    [SerializeField]
    private float _breakeForce = 300f;
    [SerializeField]
    private float _maxTurnAngle = 15f;

    [SerializeField]
    private float _brakeSmoothing = 3f;

    [SerializeField] 
    private float _maxSpeed = 120f;
    [SerializeField]
    private float _currentSpeed;

    [SerializeField]
    private Gear _gear = Gear.Park;

    private float _curAcceleration;
    private float _curBreakingForce;
    private float _curTurnAngle;

    [SerializeField]
    private bool _isBrakingPedal;
    [SerializeField]
    private bool _isAcceleratingPedal;

    private float _controlAxis;

    private Rigidbody _rigidbody;

    public Gear GearState { get => _gear; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _controlAxis = Input.GetAxis("Vertical");
        _currentSpeed = _rigidbody.velocity.magnitude * 3.6f;

        PedalChecking();

        Acceleration();
        Breaking();
        Turning();
    }

    private void Acceleration()
    {
        if (_gear != Gear.Drive && _gear != Gear.Reverse)
            return;

        _curAcceleration = Mathf.Lerp(_curAcceleration, _acceleration * _controlAxis, Time.deltaTime * 2f);

        if (_gear == Gear.Reverse)
            _curAcceleration = -_curAcceleration;
        else if (_curAcceleration < 0 || _curBreakingForce > 0)
        {
            return;
        }

        float speedFactor = Mathf.Clamp01(1f - (_currentSpeed / _maxSpeed));
        _curAcceleration *= speedFactor;

        _frontLeftWheel.motorTorque = Mathf.Lerp(_frontLeftWheel.motorTorque, _curAcceleration, Time.deltaTime * 3f);
        _frontRightWheel.motorTorque = Mathf.Lerp(_frontRightWheel.motorTorque, _curAcceleration, Time.deltaTime * 3f);

    }

    private void Breaking()
    {
        float speedFactor = Mathf.Clamp01(_currentSpeed / _maxSpeed);
        float adjustedBrakeForce = _breakeForce * (1 + (1 - speedFactor));

        if (_gear != Gear.Reverse && _curAcceleration < 0)
        {
            _curBreakingForce = Mathf.Lerp(_curBreakingForce, _curAcceleration < 0 ? adjustedBrakeForce : 0f, Time.deltaTime * _brakeSmoothing);
        }
        else
        {
            _curBreakingForce = 0f;
        }

        float frontBrakeForce = _curBreakingForce * 0.7f;
        float rearBrakeForce = _curBreakingForce * 0.3f;

        _frontLeftWheel.brakeTorque = frontBrakeForce;
        _frontRightWheel.brakeTorque = frontBrakeForce;
        _backLeftWheel.brakeTorque = rearBrakeForce;
        _backRightWheel.brakeTorque = rearBrakeForce;
    }

    private void Turning()
    {
        _curTurnAngle = _maxTurnAngle * Input.GetAxis("Horizontal");

        _frontLeftWheel.steerAngle = _curTurnAngle;
        _frontRightWheel.steerAngle = _curTurnAngle;
    }

    private void PedalChecking()
    {
        _isAcceleratingPedal = _controlAxis > 0.5f;
        _isBrakingPedal = _controlAxis < -0.5f;
    }

    public bool ShiftGear(Gear newGear)
    {
        if (_gear == Gear.Park)
        {
            if (_isBrakingPedal &&
                (newGear == Gear.Reverse || newGear == Gear.Neutral))
            {
                _gear = newGear;

                return true;
            }
        }

        if (_gear == Gear.Reverse)
        {
            if (_isBrakingPedal &&
                newGear == Gear.Park)
            {
                _gear = newGear;

                return true;
            }
        }

        if (_gear == Gear.Neutral)
        {
            if (newGear == Gear.Drive || 
            (newGear == Gear.Park && _isBrakingPedal))
            {
                _gear = newGear;

                return true;
            }
        }

        if (_gear == Gear.Drive)
        {
            if (newGear == Gear.Neutral)
            {
                _gear = newGear;

                return true;
            }
        }

        return false;
    }

    public enum Gear
    {
        Drive,
        Neutral,
        Park,
        Reverse
    }
}
