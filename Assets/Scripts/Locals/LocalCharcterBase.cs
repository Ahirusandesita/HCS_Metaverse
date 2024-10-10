using Cysharp.Threading.Tasks;
using Fusion;
using Layer_lab._3D_Casual_Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCharcterBase : MonoBehaviour
{
    private RemoteView remoteView;
    private ICharacterBase canvasCharacterBase;
    private void Awake()
    {
        FindAsyncRemoteView().Forget();
    }
    public void CanvasCharacterBaseInject(ICharacterBase characterBase)
    {
        this.canvasCharacterBase = characterBase;
    }
    private async UniTaskVoid FindAsyncRemoteView()
    {
        remoteView = await FindObjectOfType<LocalRemoteSeparation>().ReceiveRemoteView();
    }
    public void SetItem(PartsType partsType, int idx)
    {
        if (FindObjectOfType<CharacterRPCManager>())
        {
            FindObjectOfType<CharacterRPCManager>().Rpc_ChangeWear(partsType, idx, remoteView.GetComponent<NetworkObject>());
            canvasCharacterBase.SetItem(partsType, idx);
        }
    }
}
public interface ICharacterBase
{
    void SetItem(PartsType partsType, int index);
}
