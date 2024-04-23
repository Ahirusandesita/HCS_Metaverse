using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MasterServerConect : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private LocalRemoteSeparation localRemoteReparation;

    private void Start()
    {
        //�}�X�^�T�[�o�֐ڑ�
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // "Room"�Ƃ������O�̃��[���ɎQ������i���[�������݂��Ȃ���΍쐬���ĎQ������j
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Instantiate(localRemoteReparation);
    }
}
