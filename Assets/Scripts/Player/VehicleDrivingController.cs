using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleDrivingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform[] _wheelMeshes;
    [SerializeField] private Transform[] _frontWheelMeshes;
    [SerializeField] private Transform _carBody;

    [Header("Engine")]
    [SerializeField] private float _motorForce = 5000f;
    [SerializeField] private float _maxSpeed = 25f;

    [Header("Steering")]
    [SerializeField] private float _maxSteerAngle = 30f;
    [SerializeField] private float _bottomSteerClamp = 2f;
    [SerializeField] private float _topSteerClamp = 8f;

    [Header("Grip")]
    [Range(0f, 10f)]
    [SerializeField] private float _sidewaysGrip = 3f;

    [Header("Wheel Visuals")]
    [SerializeField] private float _wheelRadius = 0.35f;

    [Header("Body Lean (Input Based)")]
    [SerializeField] private float _tiltAngle = 5f;
    [SerializeField] private float _pitchAngle = 3f;
    [SerializeField] private float _bodySmooth = 6f;

    [Header("Body Vertical Suspension")]
    [SerializeField] private float _verticalOffset = 0.3f;
    [SerializeField] private float _verticalSpring = 250f;
    [SerializeField] private float _verticalDamping = 20f;
    [SerializeField] private float _maxVerticalOffset = 0.7f;

    private VehicleInputHandler _inputHandler;

    private float _currentSteerInput;
    private float _currentMotorInput;

    private float _bodyOffsetY;
    private float _bodyVerticalVelocity;

    private bool _isInputEnabled = false;

    public void Init(VehicleInputHandler inputHander)
    {
        _rb.centerOfMass = new Vector3(0f, -0.5f, 0f);

        _bodyOffsetY = _verticalOffset;
        Vector3 pos = _rb.position;
        pos.y += _bodyOffsetY;
        _carBody.position = pos;

        _inputHandler = inputHander;
    }

    public void OnInputTurnedOn()
    {
        _isInputEnabled = true;
    }

    public void OnInputTurnedOff()
    {
        _isInputEnabled = false;
    }

    private void FixedUpdate()
    {
        if (_isInputEnabled)
        {
            ReadInput();

            HandleMotor();
            HandleSteering(); 
        }

        ApplySidewaysGrip();

        if (_isInputEnabled)
        {
            RotateWheels(); 
        }

        UpdateCarBody();
    }


    private void ReadInput()
    {
        _currentMotorInput = _inputHandler.Throttle;
        _currentSteerInput = _inputHandler.Steering;
    }

    private void HandleMotor()
    {
        if (_rb.linearVelocity.magnitude < _maxSpeed)
        {
            _rb.AddForce(transform.forward * _currentMotorInput * _motorForce);
        }
    }

    private void HandleSteering()
    {
        float speed = _rb.linearVelocity.magnitude;
        if (speed < 0.05f) return;

        float localForwardVel = Vector3.Dot(_rb.linearVelocity, transform.forward);

        bool reversing = localForwardVel < -0.1f && _currentMotorInput <= 0f;
        float steerDirection = reversing ? -1f : 1f;

        float steerAmount = _currentSteerInput * _maxSteerAngle * steerDirection;
        float steerSpeed = Mathf.Lerp(_bottomSteerClamp, _topSteerClamp, speed / _maxSpeed);

        Quaternion rotation = Quaternion.Euler(0f, steerAmount * steerSpeed * Time.fixedDeltaTime, 0f);
        _rb.MoveRotation(_rb.rotation * rotation);
    }

    private void ApplySidewaysGrip()
    {
        Vector3 localVel = transform.InverseTransformDirection(_rb.linearVelocity);
        float sideForce = -localVel.x * _sidewaysGrip;
        _rb.AddForce(transform.right * sideForce, ForceMode.Acceleration);
    }

    private void RotateWheels()
    {
        float rotationSpeed = _rb.linearVelocity.magnitude / _wheelRadius;

        foreach (var wheel in _wheelMeshes)
        {
            wheel.Rotate(Vector3.right, rotationSpeed * Mathf.Rad2Deg * Time.fixedDeltaTime);
        }

        float steerAngle = _currentSteerInput * _maxSteerAngle;

        foreach (var frontWheel in _frontWheelMeshes)
        {
            Vector3 euler = frontWheel.localEulerAngles;
            frontWheel.localEulerAngles = new Vector3(euler.x, steerAngle, euler.z);
        }
    }

    private void UpdateCarBody()
    {
        if (_carBody == null) return;

        // Lean
        float roll = -_currentSteerInput * _tiltAngle;
        float pitch = -_currentMotorInput * _pitchAngle;

        Quaternion targetRot = Quaternion.Euler(pitch, 0f, roll);
        _carBody.localRotation = Quaternion.Slerp(
            _carBody.localRotation,
            targetRot,
            _bodySmooth * Time.fixedDeltaTime
        );

        // Vertical suspention
        float springForce = (_verticalOffset - _bodyOffsetY) * _verticalSpring;
        float velocityInfluence = -_rb.linearVelocity.y;

        _bodyVerticalVelocity += (springForce + velocityInfluence) * Time.fixedDeltaTime;
        _bodyVerticalVelocity *= Mathf.Clamp01(1f - _verticalDamping * Time.fixedDeltaTime);

        _bodyOffsetY += _bodyVerticalVelocity * Time.fixedDeltaTime;
        _bodyOffsetY = Mathf.Clamp(
            _bodyOffsetY,
            _verticalOffset - _maxVerticalOffset,
            _verticalOffset + _maxVerticalOffset
        );

        Vector3 bodyPos = _rb.position;
        bodyPos.y += _bodyOffsetY;
        _carBody.position = bodyPos;
    }
}
