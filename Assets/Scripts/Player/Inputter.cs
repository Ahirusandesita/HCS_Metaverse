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

    private readonly PlayerInputActions pias = default;  // �����̂ł��̖����ŋ�����

    private readonly Subject<Unit> onMoveStartedSubject = new Subject<Unit>();
    private readonly Subject<Unit> onMoveFinishedSubject = new Subject<Unit>();
    private readonly ReactiveProperty<bool> isSprintInputRP = new ReactiveProperty<bool>();
    private readonly ReactiveProperty<bool> isChatOpenRP = new ReactiveProperty<bool>(false);

    // Jump�̂�PlayerController�����珑��������������
    public ReactiveProperty<bool> IsJumpInputRP { get; set; } = new ReactiveProperty<bool>();
    public IReadOnlyReactiveProperty<bool> IsSprintInputRP => isSprintInputRP;
    public Vector2 MoveDir { get; private set; }
    public Vector2 LookDir { get; private set; }
    /// <summary>
    /// �Ō��LookAction�𑀍삵���f�o�C�X
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

            // ���͂��ꂽ�f�o�C�X�𔻒肷��
            // �{��InputSystem�͓��͎҂��B�����A���͎҂ɂ���ď����̕���͑z�肵�Ă��Ȃ����A
            // �v���C���[�̏����ŕ��򂪕K�v�ɂȂ邽�߁A�������������B
            LastLookedDevice = context.control.layout switch
            {
                "Delta" => DeviceType.Mouse,
                "Stick" => DeviceType.GamepadOrXR,
#if UNITY_EDITOR
                "Key" => DeviceType.Debug,
                "Button" => DeviceType.Debug,
                _ => throw new DeviceException("[����FLook]��Mouse, Keyboard, Gamepad, XR�ȊO�̃f�o�C�X������͂���܂����B"),
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
        throw new NotSupportedException("InputSystem���L���ɂȂ��Ă��܂���B" +
            "���̃v���W�F�N�g�ł�InputSystem�̎g�p��O��Ƃ��Ă��邽�߁A�����̓V�X�e���ł͂Ȃ�InputSystem���g�p���Ă��������B");
#endif

#if UNITY_EDITOR
        // VR����̃f�o�b�O�p�ɁAEditor�ł̂݃L�[�o�C���h��ǉ�
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
