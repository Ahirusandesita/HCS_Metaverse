using System;
using UnityEngine;
using UniRx;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class Inputter : IDisposable
{
    public enum DeviceType
    {
        Mouse,
        GamepadOrXR,
#if UNITY_EDITOR
        /// <summary>
        /// Editor only
        /// </summary>
        Debug,
#endif
    }

    private readonly PlayerInputActions pias = default;  // 長いのでこの命名で許して

    private readonly Subject<Unit> onMoveStartedSubject = new Subject<Unit>();
    private readonly Subject<Unit> onMoveFinishedSubject = new Subject<Unit>();
    private readonly ReactiveProperty<bool> isSprintInputRP = new ReactiveProperty<bool>();
    private readonly ReactiveProperty<bool> isChatOpenRP = new ReactiveProperty<bool>(false);

    // JumpのみPlayerController側から書き換えを許可する
    public ReactiveProperty<bool> IsJumpInputRP { get; set; } = new ReactiveProperty<bool>();
    public IReadOnlyReactiveProperty<bool> IsSprintInputRP => isSprintInputRP;
    public Vector2 MoveDir { get; private set; }
    public Vector2 LookDir { get; private set; }
    /// <summary>
    /// 最後にLookActionを操作したデバイス
    /// </summary>
    public DeviceType LastLookedDevice { get; private set; }

    public IObservable<Unit> OnMoveStartedSubject => onMoveStartedSubject;
    public IObservable<Unit> OnMoveFinishedSubject => onMoveFinishedSubject;
    public IReadOnlyReactiveProperty<bool> IsChatOpenRP => isChatOpenRP;


    public Inputter()
    {
#if ENABLE_INPUT_SYSTEM
        pias = new PlayerInputActions();
        pias.Enable();

        pias.Player.Move.started += context => onMoveStartedSubject.OnNext(Unit.Default);
        pias.Player.Move.performed += context => MoveDir = context.ReadValue<Vector2>();
        pias.Player.Move.canceled += context =>
        {
            MoveDir = Vector2.zero;
            onMoveFinishedSubject.OnNext(Unit.Default);
        };

        pias.Player.Look.performed += context =>
        {
            LookDir = context.ReadValue<Vector2>();

            // 入力されたデバイスを判定する
            // 本来InputSystemは入力者を隠蔽し、入力者によって処理の分岐は想定していないが、
            // プレイヤーの処理で分岐が必要になるため、泣く泣く実装。
            LastLookedDevice = context.control.layout switch
            {
                "Delta" => DeviceType.Mouse,
                "Stick" => DeviceType.GamepadOrXR,
#if UNITY_EDITOR
                "Key" => DeviceType.Debug,
                "Button" => DeviceType.Debug,
                _ => throw new DeviceException("[操作：Look]がMouse, Keyboard, Gamepad, XR以外のデバイスから入力されました。"),
#else
                _ => DeviceType.GamepadOrXR,
#endif
            };
        };
        pias.Player.Look.canceled += _ => LookDir = Vector2.zero;

        pias.Player.Jump.performed += _ => IsJumpInputRP.Value = true;
        pias.Player.Jump.canceled += _ => IsJumpInputRP.Value = false;

        pias.Player.Sprint.performed += _ => isSprintInputRP.Value = true;
        pias.Player.Sprint.canceled += _ => isSprintInputRP.Value = false;

        pias.Player.Chat.performed += _ => isChatOpenRP.Value = !isChatOpenRP.Value;
#else
        throw new NotSupportedException("InputSystemが有効になっていません。" +
            "このプロジェクトではInputSystemの使用を前提としているため、旧入力システムではなくInputSystemを使用してください。");
#endif

#if UNITY_EDITOR
        // VR操作のデバッグ用に、Editorでのみキーバインドを追加
        pias.Player.Look.AddCompositeBinding("2DVector")
            .With("Left", "<Keyboard>/k")
            .With("Right", "<Keyboard>/semicolon");
        pias.Player.Look.AddCompositeBinding("2DVector")
            .With("Left", "<Mouse>/leftButton")
            .With("Right", "<Mouse>/rightButton");
#endif
    }

    public void Dispose()
    {
        pias?.Dispose();
        onMoveStartedSubject?.Dispose();
        onMoveFinishedSubject?.Dispose();
        isChatOpenRP?.Dispose();
    }
}
