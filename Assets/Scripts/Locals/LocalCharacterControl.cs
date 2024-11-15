using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Layer_lab._3D_Casual_Character;
using TMPro;
using Cysharp.Threading.Tasks;
using Fusion;

public class LocalCharacterControl : MonoBehaviour
{
    public static LocalCharacterControl Instance;
    private AnimationControl animationControl;
    public LocalCharcterBase LocalCharcterBase { get; set; }
    private RemoteView remoteView;
    private ICharacterControl canvasCharacterControl;
    void Awake()
    {
        Instance = this;
        FindAsyncRemoteView().Forget();
        animationControl = FindObjectOfType<AnimationControl>();
        LocalCharcterBase = GetComponentInChildren<LocalCharcterBase>();
    }
    public void InjectCanvasCharacterControl(ICharacterControl characterControl)
    {
        this.canvasCharacterControl = characterControl;
    }

    private async UniTaskVoid FindAsyncRemoteView()
    {
        remoteView = await FindObjectOfType<LocalRemoteSeparation>().ReceiveRemoteView();
    }
    //public void PlayAnimation(AnimationControl.AnimType animType, int index)
    //{
    //    PlayAnimation(animationControl.GetAnimation(new AnimationControl.AnimData(animType, index)));
    //}
    public void PlayAnimation(AnimationClip clip)
    {
        AnimationControl.AnimData animData = animationControl.GetAnimData(clip);
        FindObjectOfType<CharacterRPCManager>().Rpc_PlayEmote(animData.AnimType, animData.Index, remoteView.GetComponent<NetworkObject>());
        canvasCharacterControl.PlayAnimation(clip);
        this.GetComponent<CharacterControl>().PlayAnimation(clip);
    }
}
public interface ICharacterControl
{
    void PlayAnimation(AnimationClip clip);
}
