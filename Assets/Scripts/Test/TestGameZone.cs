using UnityEngine;
using Fusion;

public class TestGameZone : MonoBehaviour, IInteraction, ISelectedNotification
{
    public ISelectedNotification SelectedNotification => this;
    [SerializeField]
    private GameFrame gameFrame;
    [SerializeField]
    private MasterServerConect _masterServer;
    [SerializeField,Header("�A�N�e�B�r�e�B(���[���h)")]
    private WorldType _worldType;

    [ContextMenu("Close")]
    public void Close()
    {
        Debug.Log("Nishigaki");
        gameFrame.Close();
        RPCManager.Instance.Rpc_RoomLeftOrClose(_masterServer.Runner.LocalPlayer);
    }
    [ContextMenu("Open")]
    public void Open()
    {
        gameFrame.GameStart();
        //���[���ɎQ������
        RPCManager.Instance.Rpc_RoomJoinOrCreate(_worldType, _masterServer.Runner.LocalPlayer);
    }

    public void Select(SelectArgs selectArgs)
    {
        
    }

    public void Unselect(SelectArgs selectArgs)
    {
        
    }
}
