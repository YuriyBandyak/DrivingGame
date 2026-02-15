using UnityEngine;

public class InputsManager : MonoBehaviour
{
    [SerializeField] private VehicleInputHandler _vehicleInputHandler;
    [SerializeField] private PlayerInputHandler _playerInputHandler;

    private InputSystem_Actions inputActions;

    public VehicleInputHandler VehicleInputHandler => _vehicleInputHandler;
    public PlayerInputHandler PlayerInputHandler => _playerInputHandler;

    public void Init()
    {
        inputActions = new();
        _vehicleInputHandler.Init(inputActions.Vehicle);
        _playerInputHandler.Init(inputActions.Character);

        _vehicleInputHandler.Disable();
        _playerInputHandler.Enable();
    }
}

