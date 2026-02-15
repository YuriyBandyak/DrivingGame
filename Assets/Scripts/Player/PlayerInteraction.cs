using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerInteraction : MonoBehaviour
{
    // should be list if there would be more interactions 
    private Interactable _currentInteractable;

    public Interactable CurrentInteractable => _currentInteractable;
    public bool CanInteract => _currentInteractable != null;

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            _currentInteractable = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Interactable>() == _currentInteractable)
        {
            _currentInteractable = null;
        }
    }
}
