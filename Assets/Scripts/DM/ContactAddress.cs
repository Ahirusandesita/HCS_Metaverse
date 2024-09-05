using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;
using TMPro;
public class ContactAddress : MonoBehaviour, IPointerClickHandler,ISendableMessage
{
    [SerializeField]
    private TextMeshProUGUI playerNameText;

    private OwnInformation ownInformation;
    private DM dm;

    private List<MessageInformation> messageHistories = new List<MessageInformation>();

    public void OnPointerClick(PointerEventData eventData)
    {
        dm.InjectMessage(messageHistories);
    }

    public void InjectOwinInformation(OwnInformation ownInformation)
    {
        this.ownInformation = ownInformation;
        playerNameText.text = ownInformation.MyPlayerRef.ToString();
    }
    public void InjectDM(DM dm)
    {
        this.dm = dm;
    }
    public bool IsTarget(PlayerRef playerRef)
    {
        return playerRef == ownInformation.MyPlayerRef;
    }

    public void Message(string message)
    {
        //dm.Message(message);
        dm.Message(new MessageInformation(message, MessageSender.Other));
        messageHistories.Add(new MessageInformation(message, MessageSender.Other));
    }
    void ISendableMessage.SendMessage(string message)
    {
        ownInformation.RPC_Message(ownInformation.MyPlayerRef, message, GateOfFusion.Instance.NetworkRunner.LocalPlayer);

        dm.Message(new MessageInformation(message, MessageSender.Me));
        messageHistories.Add(new MessageInformation(message, MessageSender.Me));
    }
}
