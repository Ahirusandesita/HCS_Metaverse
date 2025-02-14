
public interface IInputControllable { }

public static class Inputter
{
    public enum InputActionPreset
	{
        /// <summary>
        /// �ł���{�I�ȓ��̓o�C���h
        /// <br>�ړ��A�]��A�W�����v�A�X�v�����g�A�C���^���N�g</br>
        /// </summary>
        Default,
        /// <summary>
        /// �n�E�W���O���̓��̓o�C���h
        /// <br>�ړ��A�]��A�W�����v�A�X�v�����g�ASigned�i�I�u�W�F�N�g�̓]���㏸���~�j�A�ݒu</br>
        /// </summary>
        Placing,
        /// <summary>
        /// OverrayCanvas�p�̓��̓o�C���h
        /// <br>�ړ��A�]��A�W�����v�A�X�v�����g</br>
        /// </summary>
        UIControll,
        /// <summary>
        /// �G���[�g�p�̃��W�A�����j���[�p�̓��̓o�C���h
        /// <br>���W�A���J�[�\���A�]��</br>
        /// </summary>
        Animation,
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

    public static PlayerInputActions.AnimationActions Animation => InputActions.Animation;

    static Inputter()
	{
        s_inputActions = new PlayerInputActions();
	}

    public static void ChangeInputPreset(IInputControllable controllable, InputActionPreset preset)
	{
		switch (preset)
		{
			case InputActionPreset.Default:
                Player.Enable();

                UI.Disable();
                VRHead.Disable();
                PlacingMode.Disable();
                Animation.Disable();
                break;

			case InputActionPreset.Placing:
                PlacingMode.Enable();
                Player.Disable();
                Player.Move.Enable();
                Player.Look.Enable();
                Player.SprintOrWarp.Enable();

                UI.Disable();
                VRHead.Disable();
                Animation.Disable();
                break;

            case InputActionPreset.UIControll:
                Player.Enable();
                Player.Interact.Disable();

                UI.Disable();
                VRHead.Disable();
                PlacingMode.Disable();
                Animation.Disable();
                break;

            case InputActionPreset.Animation:
                Player.Disable();
                Player.Look.Enable();
                Player.Animation.Enable();

                Animation.Enable();
                break;

			default:
				break;
		}

#if UNITY_EDITOR
        XDebug.Log($"Change Input Preset: ON -> {preset}, CHANGER -> {controllable}", "orange");
#endif
    }
}