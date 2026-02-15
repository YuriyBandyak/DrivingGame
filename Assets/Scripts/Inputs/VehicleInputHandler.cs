using System;
using UnityEngine;

public class VehicleInputHandler : MonoBehaviour
{
    public event Action OnExitVehicleEvent;

    private InputSystem_Actions.VehicleActions _inputActions;

    public float Throttle => _inputActions.Throttle.ReadValue<float>();
    public float Steering => _inputActions.Steering.ReadValue<float>();
    public float Look => _inputActions.Look.ReadValue<float>();
    public bool ExitVehicle => _inputActions.ExitVehicle.triggered;

    public void Init(InputSystem_Actions.VehicleActions vehicleActions)
    {
        _inputActions = vehicleActions;
        _inputActions.ExitVehicle.performed += ctx => OnExitVehicleEvent?.Invoke();
    }

    public void Enable(bool state = true)
    {
        if (state)
        {
            _inputActions.Enable();
        }
        else
        {
            _inputActions.Disable();
        }
    }

    public void Disable() => Enable(false);
}
