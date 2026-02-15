using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleDrivingController : MonoBehaviour
{
    private const int TRACTION_MINIMAL_WHEELS_COUNT = 2;

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
    [SerializeField] private float _tiltAngleModifier = 5f;
    [SerializeField] private float _pitchAngleModifier = 3f;
    [SerializeField] private float _maxTiltAngle = 15f;
    [SerializeField] private float _maxPitchAngle = 13f;
    [SerializeField] private float _bodySmooth = 6f;

    [Header("Body Vertical Suspension")]
    [SerializeField] private float _verticalOffset = 0.3f;
    [SerializeField] private float _verticalSpring = 250f;
    [SerializeField] private float _verticalDamping = 20f;
    [SerializeField] private float _maxVerticalOffset = 0.7f;

    [Header("Other")]
    [SerializeField] private Vector3 _centerOfMass;

    private VehicleInputHandler _inputHandler;
    private LayerMask _groundLayer;

    private float _currentSteerInput;
    private float _currentMotorInput;

    private float _bodyOffsetY;
    private float _bodyVerticalVelocity;
    private Vector3 _previousVelocity;

    private bool _isInputEnabled = false;

    public void Init(VehicleInputHandler inputHander)
    {
        _rb.centerOfMass = _centerOfMass;

        _bodyOffsetY = _verticalOffset;
        Vector3 pos = _rb.position;
        pos.y += _bodyOffsetY;
        _carBody.position = pos;

        _inputHandler = inputHander;

        _groundLayer = ExtensionMethods.GetInverseLayerMask("Vehicle");
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
        var groundedWheelsCount = GetGroundedWheelsCount();
        var hasTraction = groundedWheelsCount >= TRACTION_MINIMAL_WHEELS_COUNT;

        if (_isInputEnabled && hasTraction)
        {
            ReadInput();

            HandleMotor();
            HandleSteering();
        }

        if (hasTraction)
        {
            ApplySidewaysGrip();
        }

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
        // Lean
        Vector3 velocityDelta = _rb.linearVelocity - _previousVelocity;
        float lateralAccel = Vector3.Dot(velocityDelta, transform.right) / Time.fixedDeltaTime;
        float forwardAccel = Vector3.Dot(velocityDelta, transform.forward) / Time.fixedDeltaTime;

        float roll = -lateralAccel * _tiltAngleModifier;
        float pitch = -forwardAccel * _pitchAngleModifier;

        roll = Mathf.Clamp(roll, -_maxTiltAngle, _maxTiltAngle);
        pitch = Mathf.Clamp(pitch, -_maxPitchAngle, _maxPitchAngle);

        Quaternion targetRot = Quaternion.Euler(pitch, 0f, roll);

        _carBody.localRotation = Quaternion.Slerp(
            _carBody.localRotation,
            targetRot,
            _bodySmooth * Time.fixedDeltaTime
        );

        //Debug.Log(
        //    $"Velocity Delta: {velocityDelta}, \n" +
        //    $"Lateral Accel: {lateralAccel}, \n" +
        //    $"Forward Accel: {forwardAccel}, \n" +
        //    $"Roll: {roll}, \n" +
        //    $"Pitch: {pitch}");

        _previousVelocity = _rb.linearVelocity;

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

    private int GetGroundedWheelsCount()
    {
        int _groundedWheelCount = 0;
        float groundCheckDistance = _wheelRadius + 0.1f; // to ensure proper ground detection

        for (int i = 0; i < _wheelMeshes.Length; i++)
        {
            Transform wheel = _wheelMeshes[i];

            bool grounded = Physics.Raycast(
                wheel.position,
                Vector3.down,
                groundCheckDistance,
                _groundLayer
            );

            Debug.DrawRay(wheel.position, Vector3.down * groundCheckDistance, grounded ? Color.green : Color.red);

            if (grounded)
                _groundedWheelCount++;
        }

        return _groundedWheelCount;
    }
}
