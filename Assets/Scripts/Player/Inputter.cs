using UnityEngine;
using System;

public class Inputter : IDisposable
{
    public enum DeviceType
    {
        KeyboardMouse,
        GamepadOrXR,
    }

    private readonly PlayerInputActions pias = default;  // �����̂ł��̖����ŋ�����

    // Jump�̂�PlayerController�����珑��������������
    public bool IsJump { get; set; }
    public bool IsSprint { get; private set; }
    public Vector2 MoveDir { get; private set; }
    public Vector2 LookDir { get; private set; }
    /// <summary>
    /// �Ō��LookAction�𑀍삵���f�o�C�X
    /// </summary>
    public DeviceType LastLookedDevice { get; private set; }


    public Inputter()
    {
#if ENABLE_INPUT_SYSTEM
        pias = new PlayerInputActions();
        pias.Enable();

        pias.Player.Move.performed += context => MoveDir = context.ReadValue<Vector2>();
        pias.Player.Move.canceled += context => MoveDir = Vector2.zero;

        pias.Player.Look.performed += context =>
        {
            LookDir = context.ReadValue<Vector2>();

            // ���͂��ꂽ�f�o�C�X�𔻒肷��
            // �{��InputSystem�͓��͎҂��B�����A���͎҂ɂ���ď����̕���͑z�肵�Ă��Ȃ����A
            // �v���C���[�̏����ŕ��򂪕K�v�ɂȂ邽�߁A�������������B
            LastLookedDevice = context.control.layout switch
            {
                "Delta" => DeviceType.KeyboardMouse,
                "Stick" => DeviceType.GamepadOrXR,
                _ => throw new DeviceException("[����FLook]��Keyboard, Gamepad, XR�ȊO�̃f�o�C�X������͂���܂����B"),
            };
        };
        pias.Player.Look.canceled += _ => LookDir = Vector2.zero;

        pias.Player.Jump.performed += _ => IsJump = true;
        pias.Player.Jump.canceled += _ => IsJump = false;

        pias.Player.Sprint.performed += _ => IsSprint = true;
        pias.Player.Sprint.canceled += _ => IsSprint = false;
#else
        throw new NotSupportedException("InputSystem���L���ɂȂ��Ă��܂���B" +
            "���̃v���W�F�N�g�ł�InputSystem�̎g�p��O��Ƃ��Ă��邽�߁A�����̓V�X�e���ł͂Ȃ�InputSystem���g�p���Ă��������B");
#endif
    }

    public void Dispose()
    {
        pias?.Dispose();
    }
}
