using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _vehicleCam;
    [SerializeField] private CinemachineCamera _playerCam;

    public void SwitchToVehicle()
    {
        _playerCam.gameObject.SetActive(false);
        _vehicleCam.gameObject.SetActive(true);
    }

    public void SwitchToPlayer()
    {
        _vehicleCam.gameObject.SetActive(false);
        _playerCam.gameObject.SetActive(true);
    }

    // TODO: temp ?
    private void Start()
    {
        // Start with player cam active
        //SwitchToVehicle();
    }
}
