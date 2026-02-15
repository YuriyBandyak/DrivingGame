using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [SerializeField] private VehicleDrivingController _drivingController;
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private EnterVehicleInteraction _enterInteraction;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private Collider[] _vehicleColliders =new Collider[0];

    private VehicleInputHandler _inputHandler;
    private PlayerController _currentPlayer;

    public void Init(VehicleInputHandler inputHandler)
    {
        _inputHandler = inputHandler;
        _enterInteraction.OnEnterVehicleEvent += HandleEnterVehicle;
        _inputHandler.OnExitVehicleEvent += TryHandleExitVehicle;

        _drivingController.Init(inputHandler);

        _camera.gameObject.SetActive(false);
        DisableInternalCollision();
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

    private void DisableInternalCollision()
    {
        for (int i = 0; i < _vehicleColliders.Length; i++)
        {
            for (int j = i + 1; j < _vehicleColliders.Length; j++)
            {
                Physics.IgnoreCollision(_vehicleColliders[i], _vehicleColliders[j]);
            }
        }
    }

    #region Editor stuff
#if UNITY_EDITOR
    [ContextMenu("Gather all colliders")]
    private void GatherAllColliders()
    {
        _vehicleColliders = GetComponentsInChildren<Collider>();
        EditorUtility.SetDirty(this);
    }
#endif 
    #endregion
}
