using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovementController _movementController;
    [SerializeField] private PlayerInteraction _interaction;
    [SerializeField] private CinemachineCamera _camera;

    private PlayerInputHandler _inputHandler;

    public void Init(PlayerInputHandler inputHandler)
    {
        _inputHandler = inputHandler;
        _inputHandler.OnInteractEvent += HandleInteract;
        _movementController.Init(inputHandler);
    }

    public void OnCarEntered()
    {
        _inputHandler.Disable();
    }

    public void OnCarExited()
    {
        _inputHandler.Enable();
    }

    private void HandleInteract()
    {
        if (_interaction.CanInteract)
        {
            _interaction.CurrentInteractable.Interact(this);
        }
    }
}