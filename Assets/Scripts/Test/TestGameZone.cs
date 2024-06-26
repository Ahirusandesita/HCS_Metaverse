using UnityEngine;

public class TestGameZone : MonoBehaviour, IInteraction, ISelectedNotification
{
    public ISelectedNotification SelectedNotification => this;
    [SerializeField]
    private GameFrame gameFrame;
    public void Close()
    {
        Debug.Log("Nishigaki");
        gameFrame.Close();
    }

    public void Open()
    {
        gameFrame.GameStart();
    }

    public void Select(SelectArgs selectArgs)
    {
        
    }

    public void Unselect(SelectArgs selectArgs)
    {
        
    }
}
