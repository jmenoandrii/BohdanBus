using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField]
    private Transform _speedometerArrow;
    [SerializeField]
    private Bus _bus;

    private float _minAngle = 60f;
    private float _maxAngle = 300f;

    private int _minSpeed = 0;
    private int _maxSpeed = 80;

    private void Update()
    {
        _speedometerArrow.localRotation = Quaternion.Euler(0f, GetAngle(Mathf.RoundToInt(_bus.CurrentSpeed)), 0f);
    }

    private float GetAngle(int i)
    {
        i = Mathf.Clamp(i, _minSpeed, _maxSpeed);

        return Mathf.Lerp(_minAngle, _maxAngle, (float)(i - _minSpeed) / (_maxSpeed - _minSpeed));
    }
}
