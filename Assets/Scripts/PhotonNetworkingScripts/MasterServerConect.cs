using UnityEngine;

public interface IMasterServerConectable
{
    void Connect();
}

public class MasterServerConect : MonoBehaviour/*MonoBehaviourPunCallbacks,*/, IMasterServerConectable
{
    [SerializeField]
    private LocalRemoteSeparation localRemoteReparation;

    //public override void OnConnectedToMaster()
    //{
    //    // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
    //    PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    //}

    //public override void OnJoinedRoom()
    //{
    //    localRemoteReparation.RemoteViewCreate();
    //}

    void IMasterServerConectable.Connect()
    {
        //PhotonNetwork.ConnectUsingSettings();
    }
}
