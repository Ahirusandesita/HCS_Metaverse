
public abstract class SelectArgs
{
    private class NullSelectArgs : SelectArgs { }

    private static SelectArgs s_empty = default;
    public static SelectArgs Empty
    {
        get
        {
            s_empty ??= new NullSelectArgs();
            return s_empty;
        }
    }
}

public class ItemSelectArgs : SelectArgs
{
    public readonly int id = default;

    public ItemSelectArgs(int id)
    {
        this.id = id;
    }
}