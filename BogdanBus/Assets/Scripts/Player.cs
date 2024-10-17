using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Vector2 _sensetivity = new Vector2(8f, 0.5f);
    [SerializeField]
    private float _xClamp;
    [SerializeField]
    private float _yClamp;
    [SerializeField]
    private Transform _cameraTransform;

    private float _xRotation = 0f;
    private float _yRotation = 0f;
    private Vector2 _mouseInput;

    public float _turnSpeed = 5f;
    private Quaternion _originalRotation;
    private Vector3 _originalPosition;
    private bool _isTurningBack = false;
    private bool _isReturning = false;
    private bool _hasMovedBack = false;

    private void Start()
    {
        _originalRotation = transform.rotation;
        _originalPosition = transform.position;
    }

    private void Update()
    {
        _mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * _sensetivity;
        MouseLook();

        TurnBack();
    }

    private void MouseLook()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!_isTurningBack && !_isReturning)
        {
            _yRotation += _mouseInput.x * Time.deltaTime;
            _yRotation = Mathf.Clamp(_yRotation, -_yClamp, _yClamp);

            transform.localRotation = Quaternion.Euler(0, _yRotation, 0);
        }

        _xRotation -= _mouseInput.y;
        _xRotation = Mathf.Clamp(_xRotation, -_xClamp, _xClamp);
        Vector3 targetRotation = transform.localEulerAngles;

        targetRotation.x = _xRotation;
        _cameraTransform.localEulerAngles = targetRotation;
    }

    private void TurnBack()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            _isTurningBack = true;
            _isReturning = false;
            _hasMovedBack = false;
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            _isTurningBack = false;
            _isReturning = true;
        }


        if (_isTurningBack)
        {
            Quaternion targetRotation = _originalRotation * Quaternion.Euler(0, 90, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * _turnSpeed);

            if (!_hasMovedBack)
            {
                Vector3 offset = new Vector3(0, -0.5f, 0);
                transform.position += transform.rotation * offset * Time.deltaTime;
                _hasMovedBack = true;
            }
        }

        if (_isReturning)
        {
            transform.position = Vector3.Lerp(transform.position, _originalPosition, Time.deltaTime * _turnSpeed);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, _originalRotation, Time.deltaTime * _turnSpeed);

            if (Vector3.Distance(transform.position, _originalPosition) < 0.1f &&
                Quaternion.Angle(transform.localRotation, _originalRotation) < 0.1f)
            {
                _isReturning = false;
                transform.position = _originalPosition;
            }
        }
    }
}
