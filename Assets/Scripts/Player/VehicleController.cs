using Unity.Cinemachine;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [SerializeField] private VehicleDrivingController _drivingController;
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private EnterVehicleInteraction _enterInteraction;
    [SerializeField] private Transform _exitPoint;

    private VehicleInputHandler _inputHandler;
    private PlayerController _currentPlayer;

    public void Init(VehicleInputHandler inputHandler)
    {
        _inputHandler = inputHandler;
        _enterInteraction.OnEnterVehicleEvent += HandleEnterVehicle;
        _inputHandler.OnExitVehicleEvent += TryHandleExitVehicle;

        _drivingController.Init(inputHandler);

        _camera.gameObject.SetActive(false);
    }

    private void HandleEnterVehicle(PlayerController controller)
    {
        if (_currentPlayer != null)
        {
            Debug.LogError("Vehicle occupied");
            return;
        }

        _currentPlayer = controller;
        _currentPlayer.gameObject.SetActive(false);
        _inputHandler.Enable();
        _drivingController.OnInputTurnedOn();
        _camera.gameObject.SetActive(true);
        _currentPlayer.OnCarEntered();
    }

    private void TryHandleExitVehicle()
    {
        if (_currentPlayer == null)
        {
            Debug.LogError($"Vehicle is empty, can't exit");
            return;
        }

        _currentPlayer.transform.position = _exitPoint.position;
        _currentPlayer.gameObject.SetActive(true);
        _inputHandler.Disable();
        _drivingController.OnInputTurnedOff();
        _camera.gameObject.SetActive(false);
        _currentPlayer.OnCarExited();
        
        _currentPlayer = null;
    }
}
