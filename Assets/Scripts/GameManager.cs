using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPosition;
    [SerializeField] private Transform _carSpawnPosition;

    [SerializeField] private VehicleController[] _carPrefabs = new VehicleController[0];
    [SerializeField] private PlayerController _playerPrefab;

    [SerializeField] private InputsManager _inputsManager;

    private void Start()
    {
        _inputsManager.Init();
        SpawnPlayer();
        SpawnCar();
    }

    private void SpawnPlayer()
    {
        var player = Instantiate(_playerPrefab, _playerSpawnPosition.position, Quaternion.identity);
        player.Init(_inputsManager.PlayerInputHandler);

    }

    private void SpawnCar()
    {
        var carPrefab = _carPrefabs[Random.Range(0, _carPrefabs.Length)];
        var car = Instantiate(carPrefab, _carSpawnPosition.position, Quaternion.identity);
        car.Init(_inputsManager.VehicleInputHandler);
    }
}

