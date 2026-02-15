using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPosition;
    [SerializeField] private Transform[] _carSpawnPositions = new Transform[0];

    [SerializeField] private VehicleController[] _carPrefabs = new VehicleController[0];
    [SerializeField] private PlayerController _playerPrefab;

    [SerializeField] private InputsManager _inputsManager;

    private void Start()
    {
        _inputsManager.Init();
        SpawnPlayer();
        SpawnCars();
    }
    private void SpawnPlayer()
    {
        var player = Instantiate(_playerPrefab, _playerSpawnPosition.position, Quaternion.identity);
        player.Init(_inputsManager.PlayerInputHandler);
    }

    private void SpawnCars()
    {
        for (int i = 0; i < _carPrefabs.Length; i++)
        {
            var carPrefab = _carPrefabs[i];
            var car = Instantiate(carPrefab, _carSpawnPositions[i].position, Quaternion.identity);
            car.Init(_inputsManager.VehicleInputHandler); 
        }
    }
}

