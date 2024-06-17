using UnityEngine;

public abstract class SafetyInteractionObject : MonoBehaviour, IInteraction, ISelectedNotification
{
    ISelectedNotification IInteraction.SelectedNotification => this;
    private PlayerInputActions.InteractionActions Interaction => Inputter.Interaction;
    private PlayerInputActions.PlayerActions Player => Inputter.Player;

    protected virtual void Awake()
    {
        Interaction.Interact.performed += _ =>
        {
            SafetyOpen();
            Interaction.Interact.Disable();
            Interaction.Disengage.Enable();
            Player.Disable();
        };
        Interaction.Disengage.performed += _ =>
        {
            SafetyClose();
            Interaction.Interact.Enable();
            Interaction.Disengage.Disable();
            Player.Enable();
        };
    }

    void IInteraction.Open()
    {
        NotificationUIManager.Instance.DisplayInteraction();
        Interaction.Interact.Enable();
    }

    void IInteraction.Close()
    {
        NotificationUIManager.Instance.HideInteraction();
        Interaction.Disable();
    }

    protected abstract void SafetyOpen();
    protected abstract void SafetyClose();

    public abstract void Select(SelectArgs selectArgs);
    public abstract void Unselect(SelectArgs selectArgs);

    public virtual void Hover(SelectArgs selectArgs) { }
    public virtual void Unhover(SelectArgs selectArgs) { }
}
