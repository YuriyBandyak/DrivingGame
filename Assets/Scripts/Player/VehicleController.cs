using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [SerializeField] private VehicleDrivingController _drivingController;
    [SerializeField] private VehicleInputHandler _inputHandler;
    [SerializeField] private EnterVehicleInteraction _enterInteraction;
    [SerializeField] private Transform _exitPoint;

    private PlayerController _currentPlayer;

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        _enterInteraction.OnEnterVehicleEvent += HandleEnterVehicle;

        _inputHandler.OnExitVehicleEvent += TryHandleExitVehicle;
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
    }

    private void TryHandleExitVehicle()
    {
        if (_currentPlayer == null)
        {
            Debug.LogError($"Vehicle is empty, can't exit");
            return;
        }

        _currentPlayer.transform.position= _exitPoint.position;
        _currentPlayer.gameObject.SetActive(true);
    }
}