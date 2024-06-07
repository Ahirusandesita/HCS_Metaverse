
public interface IInteraction
{
    void Open();
    void Close();
    ISelectedNotification SelectedNotification { get; }
}
