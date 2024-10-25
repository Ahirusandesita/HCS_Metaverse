using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using Fusion;

public class CharacterAnimatorController : MonoBehaviour
{
    private const string _ANIMAITOR_MOVE_FLAG_NAME = "IsMove";
    [SerializeField]
    private Animator _myAnimator = default;
    [SerializeField]
    private CharacterRPCManager _characterRPCManager = default;
    private NetworkObject _myRemoteViewNetworkObject = default;

    private void Reset()
    {
        FindAnimator();
    }
    private async void Start()
    {
        FindAnimator();
        await FindCharacterRPCManager();
        await FindMyRemoteViewNetworkObject();
        Inputter.Player.Move.performed += MoveAnimationActive;
        Inputter.Player.Move.canceled += MoveAnimationCancel;
    }
	private void OnDisable()
	{
        Inputter.Player.Move.performed -= MoveAnimationActive;
        Inputter.Player.Move.canceled -= MoveAnimationCancel;
    }

	private async UniTask FindMyRemoteViewNetworkObject()
    {
        RemoteView remoteView = await FindObjectOfType<LocalRemoteSeparation>().ReceiveRemoteView();
        _myRemoteViewNetworkObject = remoteView.GetComponent<NetworkObject>();
    }

    private async UniTask FindCharacterRPCManager()
    {
        await UniTask.WaitUntil(() => FindObjectOfType<CharacterRPCManager>());
        _characterRPCManager ??= FindObjectOfType<CharacterRPCManager>();
    }

    private void FindAnimator()
    {
        _myAnimator ??= GetComponent<Animator>();
        if (_myAnimator == null)
        {
            XDebug.LogError("アニメーターが見つかりません", Color.red);
        }
    }


    private void MoveAnimationActive(InputAction.CallbackContext callbackContext)
    {
        //のちのち移動が成功していない時はアニメーションを動かさない
        Vector2 inputVector = callbackContext.ReadValue<Vector2>();
        if (inputVector != Vector2.zero)
        {
            _myAnimator.SetBool(_ANIMAITOR_MOVE_FLAG_NAME, true);
            _characterRPCManager.Rpc_AnimationBoolControl(_myRemoteViewNetworkObject,_ANIMAITOR_MOVE_FLAG_NAME,true);
        }
    }

    private void MoveAnimationCancel(InputAction.CallbackContext callbackContext)
    {
        _myAnimator.SetBool(_ANIMAITOR_MOVE_FLAG_NAME, false);
        _characterRPCManager.Rpc_AnimationBoolControl(_myRemoteViewNetworkObject, _ANIMAITOR_MOVE_FLAG_NAME, false);
    }
}
