using System;

public class EnterVehicleInteraction : Interactable
{
    public event Action<PlayerController> OnEnterVehicleEvent;

    public override void Interact(PlayerController playerController)
    {
        OnEnterVehicleEvent?.Invoke(playerController);
    }
}
