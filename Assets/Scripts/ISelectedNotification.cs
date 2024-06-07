
public interface ISelectedNotification
{
    void Select(SelectArgs selectArgs);
    void Unselect(SelectArgs selectArgs);
}

public sealed class NullSelectedNotification : ISelectedNotification
{
    void ISelectedNotification.Select(SelectArgs selectArgs) { }
    void ISelectedNotification.Unselect(SelectArgs selectArgs) { }
}