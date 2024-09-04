using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Fusion;

public class DM : MonoBehaviour, IPointerClickHandler, ISendableMessage
{
    [SerializeField]
    private TextMeshProUGUI textMesh;
    [SerializeField]
    private TextMeshProUGUI message;
    private OwnInformation ownInformation;
    public void OnPointerClick(PointerEventData eventData)
    {
        ownInformation.RPC_Message(ownInformation.MyPlayerRef, "Hello",GateOfFusion.Instance.NetworkRunner.LocalPlayer);
    }

    public void Player(OwnInformation ownInformation)
    {
        this.ownInformation = ownInformation;
        textMesh.text = ownInformation.Name;
    }

    public bool IsTarget(PlayerRef playerRef)
    {
        return playerRef == ownInformation.MyPlayerRef;
    }

    public void Message(string message)
    {
        this.message.text = message;
    }
}
