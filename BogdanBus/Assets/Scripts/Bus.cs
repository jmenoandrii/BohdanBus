using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float _breakingForce = 300f;
    [SerializeField]
    private float _maxTurnAngle = 15f;

    private float _curAcceleration;
    private float _curBreakingForce;
    private float _curTurnAngle;

    private void FixedUpdate()
    {
        Acceleration();
        Breaking();
        Turning();
    }

    private void Acceleration()
    {
        _curAcceleration = _acceleration * Input.GetAxis("Vertical");

        if (_curAcceleration < 0)
            return;

        _frontLeftWheel.motorTorque = _curAcceleration;
        _frontRightWheel.motorTorque = _curAcceleration;
    }

    private void Breaking()
    {
        if (Input.GetKey(KeyCode.Space))
            _curBreakingForce = _breakingForce;
        else
            _curBreakingForce = _curAcceleration < 0 ? -_curAcceleration : 0f;

        _frontLeftWheel.brakeTorque = _curBreakingForce;
        _frontRightWheel.brakeTorque = _curBreakingForce;
        _backLeftWheel.brakeTorque = _curBreakingForce;
        _backRightWheel.brakeTorque = _curBreakingForce;
    }

    private void Turning()
    {
        _curTurnAngle = _maxTurnAngle * Input.GetAxis("Horizontal");

        _frontLeftWheel.steerAngle = _curTurnAngle;
        _frontRightWheel.steerAngle = _curTurnAngle;
    }
}
