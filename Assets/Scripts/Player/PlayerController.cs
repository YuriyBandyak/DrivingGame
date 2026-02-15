using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovementController _movementController;
    [SerializeField] private PlayerInputHandler _inputHandler;
    [SerializeField] private PlayerInteraction _interaction;

    private void Start()
    {
        _inputHandler.OnInteractEvent += HandleInteract;
    }

    private void HandleInteract()
    {
        if (_interaction.CanInteract)
        {
            _interaction.CurrentInteractable.Interact(this);
        }
    }
}