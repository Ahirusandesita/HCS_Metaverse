using UnityEngine;

public class TestGameZone : MonoBehaviour, IInteraction, ISelectedNotification
{
    public ISelectedNotification SelectedNotification => this;
    [SerializeField]
    private GameFrame gameFrame;
    public void Close()
    {
        gameFrame.Close();
    }

    public void Open()
    {
        gameFrame.GameStart();
    }

    public void Select(SelectArgs selectArgs)
    {
        Debug.Log("GameŠJŽn");
    }

    public void Unselect(SelectArgs selectArgs)
    {
        
    }
}
