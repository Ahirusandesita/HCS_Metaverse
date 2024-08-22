public class GrabEventArgs : System.EventArgs
{
    public readonly GrabType GrabType;
    public GrabEventArgs(GrabType grabType)
    {
        this.GrabType = grabType;
    }
}