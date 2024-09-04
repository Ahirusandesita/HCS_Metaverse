using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class DM : MonoBehaviour,IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI textMesh;
    private OwnInformation ownInformation;
    public void OnPointerClick(PointerEventData eventData)
    {
        ownInformation.RPC_Message(ownInformation.MyPlayerRef, "Hello");
    }

    public void Player(OwnInformation ownInformation)
    {
        this.ownInformation = ownInformation;
        textMesh.text = ownInformation.MyPlayerRef.ToString();
    }
}
