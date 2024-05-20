using Photon.Pun;
using UnityEngine;

public class SendChat : MonoBehaviour
{
    private PhotonView _PhotonViewControl;

    private ChatSystem chatSystem;
    private void Awake()
    {

        _PhotonViewControl = GetComponent<PhotonView>();

        chatSystem = GameObject.FindObjectOfType<ChatSystem>();

    }

    public void Send_ToOthers(string message)
    {

        _PhotonViewControl.RPC("Message", RpcTarget.Others, message);
        chatSystem.SendManually(message);
    }

    [PunRPC]
    private void Message(string message)
    {
        chatSystem.SendManually(message);
    }
}
