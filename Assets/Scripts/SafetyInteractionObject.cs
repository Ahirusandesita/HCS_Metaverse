using System.Collections;
using System.Collections.Generic;
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
            Interaction.Disengage.Enable();
            Player.Disable();
        };
        Interaction.Disengage.performed += _ =>
        {
            SafetyClose();
            Interaction.Disable();
            Player.Enable();
        };
    }

    void IInteraction.Open()
    {
        UnityEngine.Debug.Log("Open!!");
        Interaction.Interact.Enable();
    }

    void IInteraction.Close()
    {
        Interaction.Interact.Disable();
    }

    protected abstract void SafetyOpen();
    protected abstract void SafetyClose();

    public abstract void Select(SelectArgs selectArgs);
    public abstract void Unselect(SelectArgs selectArgs);

    public virtual void Hover(SelectArgs selectArgs) { }
    public virtual void Unhover(SelectArgs selectArgs) { }
}
