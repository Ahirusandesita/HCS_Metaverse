public class AuthrityEventArgs : System.EventArgs
{
	public readonly bool Authrity;
	public AuthrityEventArgs(bool authrity)
	{
		this.Authrity = authrity;
	}
}