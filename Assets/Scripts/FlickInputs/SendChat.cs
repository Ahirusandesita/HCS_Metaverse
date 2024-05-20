using Photon.Pun;
using UnityEngine;

public class SendChat : MonoBehaviour
{
    private PhotonView _PhotonViewControl;
    
    private void Awake()
    {

        _PhotonViewControl = GetComponent<PhotonView>();

    }

    public void Send_ToOthers(string message)
    {

        _PhotonViewControl.RPC("Message", RpcTarget.Others, message);

    }

    [PunRPC]
    private void Message(string message)
    {

    }
}
