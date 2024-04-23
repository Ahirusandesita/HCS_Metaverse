using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MasterServerConect : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private LocalRemoteSeparation localRemoteReparation;

    private void Start()
    {
        //マスタサーバへ接続
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Instantiate(localRemoteReparation);
    }
}
