
public interface IInputControllable { }

public static class Inputter
{
    public enum InputActionPreset
	{
        /// <summary>
        /// 最も基本的な入力バインド
        /// <br>移動、転回、ジャンプ、スプリント、インタラクト</br>
        /// </summary>
        Default,
        /// <summary>
        /// ハウジング中の入力バインド
        /// <br>移動、転回、ジャンプ、Signed（オブジェクトの転回や上昇下降）、設置</br>
        /// </summary>
        Placing,
	}

    private static PlayerInputActions s_inputActions = default;

    private static PlayerInputActions InputActions
    {
        get
        {
            return s_inputActions;
        }
    }

    public static PlayerInputActions.PlayerActions Player => InputActions.Player;
    public static PlayerInputActions.UIActions UI => InputActions.UI;
    public static PlayerInputActions.VRHeadActions VRHead => InputActions.VRHead;
    public static PlayerInputActions.PlacingModeActions PlacingMode => InputActions.PlacingMode;

    static Inputter()
	{
        s_inputActions = new PlayerInputActions();
	}

    public static void ChangeInputPreset(IInputControllable controllable, InputActionPreset preset)
	{
		switch (preset)
		{
			case InputActionPreset.Default:
                XDebug.Log("Preset", UnityEngine.Color.magenta);
                Player.Enable();

                UI.Disable();
                VRHead.Disable();
                PlacingMode.Disable();
				break;
			case InputActionPreset.Placing:
                PlacingMode.Enable();
                Player.Move.Enable();
                Player.Look.Enable();
                Player.Jump.Enable();

                Player.SprintOrWarp.Disable();
                Player.Interact.Disable();
                UI.Disable();
                VRHead.Disable();
				break;
			default:
				break;
		}

#if UNITY_EDITOR
        XDebug.Log($"Change Input Preset: ON -> {preset}, CHANGER -> {controllable}", "orange");
#endif
    }
}