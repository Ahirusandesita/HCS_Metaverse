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
    //    // "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
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
