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
    private float forceBrakeMultiplier = 6f;

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
        // Set a very high braking force for immediate stop
        float adjustedBrakeForce = _breakeForce * forceBrakeMultiplier;

        // Set the current braking force to the adjusted braking force
        _curBreakingForce = adjustedBrakeForce;

        // Adjust front and rear brake force proportions if necessary
        float frontBrakeForce = _curBreakingForce * 0.8f; // More force to front wheels
        float rearBrakeForce = _curBreakingForce * 0.2f; // Less force to rear wheels

        // Apply the brake torque to the wheels
        _frontLeftWheel.brakeTorque = frontBrakeForce;
        _frontRightWheel.brakeTorque = frontBrakeForce;
        _backLeftWheel.brakeTorque = rearBrakeForce;
        _backRightWheel.brakeTorque = rearBrakeForce;

        // Optionally, reset the bus velocity if using Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 5f); // Smoothly reduce velocity
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime * 5f); // Smoothly reduce angular velocity
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
