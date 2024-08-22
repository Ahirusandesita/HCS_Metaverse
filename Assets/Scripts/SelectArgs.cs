using UnityEngine;

public abstract class SelectArgs
{
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
public class NullSelectArgs : SelectArgs { }