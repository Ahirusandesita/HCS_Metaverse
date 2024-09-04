using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;
using TMPro;
public class ContactAddress : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI playerNameText;

    private OwnInformation ownInformation;
    private DM dm;


    public void OnPointerClick(PointerEventData eventData)
    {
        dm.InjectOwnInformation(ownInformation);
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
}
