using System;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private const string SPEED_PARAM = "Speed";
    private const string IS_GROUNDED_PARAM = "IsGrounded";  

    [Header("Movement")]
    [SerializeField] private float _walkSpeed = 1f;
    [SerializeField] private float _runSpeed = 4f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _dampTime = 0.1f;

    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _controller;

    private PlayerInputHandler _inputHandler;
    
    private Vector3 _velocity;
    private float _currentSpeed;

    public void Init(PlayerInputHandler inputHandler)
    {
        _inputHandler = inputHandler;
    }

    private void Update() 
    {
        HandleMovement();
        ApplyGravity();
        UpdateAnimator();
    }

    private void HandleMovement()
    {
        float inputX = _inputHandler.Move.x;
        float inputZ = _inputHandler.Move.y;
        bool isRunning = _inputHandler.Run;

        Vector3 inputDir = new Vector3(inputX, 0f, inputZ);
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);

        if (inputDir.magnitude > 0.01f)
        {
            // Camera relative movement
            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight = _cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;

            // Rotate toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );

            _currentSpeed = isRunning ? _runSpeed : _walkSpeed;
            _controller.Move(moveDir * _currentSpeed * Time.deltaTime);
        }
        else
        {
            _currentSpeed = 0f;
        }
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _velocity.y < 0f)
        {
            _velocity.y = -2f;
        }

        _velocity.y += Physics.gravity.y * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        float normalizedSpeed = _currentSpeed / _runSpeed;
        _animator.SetFloat(SPEED_PARAM, normalizedSpeed, _dampTime, Time.deltaTime);
        _animator.SetBool(IS_GROUNDED_PARAM, _controller.isGrounded);
    }
}
