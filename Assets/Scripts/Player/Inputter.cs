using UnityEngine;
using System;

public class Inputter : IDisposable
{
    public enum DeviceType
    {
        KeyboardMouse,
        GamepadOrXR,
    }

    private readonly PlayerInputActions pias = default;  // 長いのでこの命名で許して

    // JumpのみPlayerController側から書き換えを許可する
    public bool IsJump { get; set; }
    public bool IsSprint { get; private set; }
    public Vector2 MoveDir { get; private set; }
    public Vector2 LookDir { get; private set; }
    /// <summary>
    /// 最後にLookActionを操作したデバイス
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

            // 入力されたデバイスを判定する
            // 本来InputSystemは入力者を隠蔽し、入力者によって処理の分岐は想定していないが、
            // プレイヤーの処理で分岐が必要になるため、泣く泣く実装。
            LastLookedDevice = context.control.layout switch
            {
                "Delta" => DeviceType.KeyboardMouse,
                "Stick" => DeviceType.GamepadOrXR,
                _ => throw new DeviceException("[操作：Look]がKeyboard, Gamepad, XR以外のデバイスから入力されました。"),
            };
        };
        pias.Player.Look.canceled += _ => LookDir = Vector2.zero;

        pias.Player.Jump.performed += _ => IsJump = true;
        pias.Player.Jump.canceled += _ => IsJump = false;

        pias.Player.Sprint.performed += _ => IsSprint = true;
        pias.Player.Sprint.canceled += _ => IsSprint = false;
#else
        throw new NotSupportedException("InputSystemが有効になっていません。" +
            "このプロジェクトではInputSystemの使用を前提としているため、旧入力システムではなくInputSystemを使用してください。");
#endif
    }

    public void Dispose()
    {
        pias?.Dispose();
    }
}
