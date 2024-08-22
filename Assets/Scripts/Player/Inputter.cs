
public static class Inputter
{
    private static PlayerInputActions s_inputActions = default;
    private static PlayerInputActions InputActions
    {
        get
        {
            s_inputActions ??= new PlayerInputActions();
            return s_inputActions;
        }
    }

    public static PlayerInputActions.PlayerActions Player => InputActions.Player;
    public static PlayerInputActions.UIActions UI => InputActions.UI;
    public static PlayerInputActions.VRHeadActions VRHead => InputActions.VRHead;
    public static PlayerInputActions.InteractionActions Interaction => InputActions.Interaction;
}