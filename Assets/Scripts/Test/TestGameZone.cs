using UnityEngine;
using Fusion;

public class TestGameZone : MonoBehaviour, IInteraction, ISelectedNotification
{
    public ISelectedNotification SelectedNotification => this;
    [SerializeField]
    private GameFrame gameFrame;
    [SerializeField]
    private MasterServerConect masterServer;
    public void Close()
    {
        Debug.Log("Nishigaki");
        gameFrame.Close();
    }

    public void Open()
    {
        gameFrame.GameStart();
        
        masterServer.CookActivityJoin();

    }

    public void Select(SelectArgs selectArgs)
    {
        
    }

    public void Unselect(SelectArgs selectArgs)
    {
        
    }
}
