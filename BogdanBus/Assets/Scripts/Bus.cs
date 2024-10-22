using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bus;

[RequireComponent(typeof(Rigidbody))]
public class Bus : MonoBehaviour
{
    [Header("Wheels")]
    [SerializeField]
    private WheelCollider _frontLeftWheel;
    [SerializeField]
    private WheelCollider _frontRightWheel;
    [SerializeField]
    private WheelCollider _backLeftWheel;
    [SerializeField]
    private WheelCollider _backRightWheel;

    [Header("Movement Settings")]
    [SerializeField]
    private bool _canMove;
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
    private float _maxReverseSpeed = 60f;
    [SerializeField]
    private float _currentSpeed;
    public float CurrentSpeed { get => _currentSpeed; }

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
        _canMove = true;
    }

    private void FixedUpdate()
    {
        if (_canMove)
        {
            _controlAxis = Input.GetAxis("Vertical");
            _currentSpeed = _rigidbody.velocity.magnitude * 3.6f;

            PedalChecking();

            Acceleration();
            Braking();
            Turning();
        }
        else
        {
            ForcedBraking();
        }
    }

    private void PedalChecking()
    {
        _isAcceleratingPedal = _controlAxis > 0.5f;
        _isBrakingPedal = _controlAxis < -0.5f;
    }

    private void Acceleration()
    {
        if (_gear == Gear.Park)
        {
            _frontLeftWheel.motorTorque = 0;
            _frontRightWheel.motorTorque = 0;
            return;
        }

        if (_gear == Gear.Drive)
        {
            if (_currentSpeed >= _maxSpeed)
            {
                _curAcceleration = 0f;
            }
            else
            {
                _curAcceleration = Mathf.Lerp(_curAcceleration, _acceleration * _controlAxis, Time.deltaTime * 2f);
            }
        }
        else if (_gear == Gear.Reverse)
        {
            if (_currentSpeed >= _maxReverseSpeed)
            {
                _curAcceleration = 0f;
            }
            else
            {
                _curAcceleration = Mathf.Lerp(_curAcceleration, -_acceleration * _controlAxis, Time.deltaTime * 2f);
            }
        }

        _frontLeftWheel.motorTorque = Mathf.Lerp(_frontLeftWheel.motorTorque, _curAcceleration, Time.deltaTime * 3f);
        _frontRightWheel.motorTorque = Mathf.Lerp(_frontRightWheel.motorTorque, _curAcceleration, Time.deltaTime * 3f);
    }

    private void Braking()
    {
        if (_gear == Gear.Park)
        {
            _curBreakingForce = 0;
            return;
        }

        float speedFactor = Mathf.Clamp01(Mathf.Abs(_currentSpeed) / _maxSpeed);
        float adjustedBrakeForce = _breakeForce * (1 + (1 - speedFactor));

        if (_gear == Gear.Reverse)
        {
            if (_controlAxis < 0)
            {
                _curBreakingForce = Mathf.Lerp(_curBreakingForce, adjustedBrakeForce, Time.deltaTime * _brakeSmoothing);
            }
            else
            {
                _curBreakingForce = 0;
            }
        }
        else
        {
            if (_curAcceleration < 0)
            {
                _curBreakingForce = Mathf.Lerp(_curBreakingForce, adjustedBrakeForce, Time.deltaTime * _brakeSmoothing);
            }
            else
            {
                _curBreakingForce = 0f;
            }
        }

        float frontBrakeForce = _curBreakingForce * 0.7f;
        float rearBrakeForce = _curBreakingForce * 0.3f;

        _frontLeftWheel.brakeTorque = frontBrakeForce;
        _frontRightWheel.brakeTorque = frontBrakeForce;
        _backLeftWheel.brakeTorque = rearBrakeForce;
        _backRightWheel.brakeTorque = rearBrakeForce;
    }

    public void ForceStop() { _canMove = false; }


    private void ForcedBraking()
    {
        // Forcefully set the maximum braking force
        _curBreakingForce = _breakeForce * 10f; // Increase braking force for an instant stop

        // Apply the braking force to all wheels
        _frontLeftWheel.brakeTorque = _curBreakingForce;
        _frontRightWheel.brakeTorque = _curBreakingForce;
        _backLeftWheel.brakeTorque = _curBreakingForce;
        _backRightWheel.brakeTorque = _curBreakingForce;

        // Reset the bus velocity if using Rigidbody
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    private void Turning()
    {
        _curTurnAngle = _maxTurnAngle * Input.GetAxis("Horizontal");

        _frontLeftWheel.steerAngle = _curTurnAngle;
        _frontRightWheel.steerAngle = _curTurnAngle;
    }

    public bool ShiftGear(Gear newGear)
    {
        if (_currentSpeed > 1 || !_isBrakingPedal)
            return false;

        if (_gear == Gear.Drive && newGear == Gear.Park)
        {
            _gear = newGear;

            return true;
        }

        if (_gear == Gear.Park)
        {
            _gear = newGear;

            return true;
        }

        if (_gear == Gear.Reverse && newGear == Gear.Park)
        {
            _gear = newGear;

            return true;
        }

        return false;
    }

    public enum Gear
    {
        Drive,
        Park,
        Reverse
    }
}
