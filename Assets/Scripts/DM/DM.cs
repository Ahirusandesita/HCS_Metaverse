using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Fusion;

public class DM : MonoBehaviour, ISendableMessage
{
    [SerializeField]
    private TextMeshProUGUI message;
    private OwnInformation ownInformation;

    public void InjectOwnInformation(OwnInformation ownInformation)
    {
        this.ownInformation = ownInformation;
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
