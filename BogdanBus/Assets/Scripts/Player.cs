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

    [SerializeField]
    private Vector3 _turnEuler = new(0, 90, 0);
    [SerializeField]
    private Vector3 _turnOffset = new(0.6f, 0, 0);

    public float _turnSpeed = 5f;
    private Quaternion _originalRotation;
    private Vector3 _originalPosition;
    private bool _isTurningBack = false;
    private bool _isReturning = false;

    [SerializeField]
    private float _distanceRaycast = Mathf.Infinity;
    [SerializeField]
    private LayerMask _layerMaskRaycast;

    private void Update()
    {
        _mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * _sensetivity;
        MouseLook();

        TurnBack();
        CheckRaycast();
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
            _originalRotation = transform.localRotation;
            _originalPosition = transform.localPosition;
            _isTurningBack = true;
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            _isTurningBack = false;
            _isReturning = true;
        }

        if (_isTurningBack || _isReturning)
        {
            Quaternion targetRotation = _isTurningBack ? Quaternion.Euler(_turnEuler) : _originalRotation;
            Vector3 targetPosition = _isTurningBack ? _originalPosition + _turnOffset : _originalPosition;

            transform.SetLocalPositionAndRotation(Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * _turnSpeed), 
                Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * _turnSpeed));

            if (_isReturning &&
                Vector3.Distance(transform.localPosition, _originalPosition) < 0.1f &&
                Quaternion.Angle(transform.localRotation, _originalRotation) < 0.1f)
            {
                _isReturning = false;
            }
        }
    }

    private void CheckRaycast()
    {
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _distanceRaycast, _layerMaskRaycast, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent(out IInteractableObject interactableObject))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactableObject.Interact();
                }
            }
        }
    }
}
