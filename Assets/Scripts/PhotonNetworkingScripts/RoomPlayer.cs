using Fusion;
public class RoomPlayer
{
	private string _sessionName;
	private PlayerRef _playerData;
	public RoomPlayer(PlayerRef playerData, string sessionName = "")
	{
		_sessionName = sessionName;
		_playerData = playerData;
	}
	public string SessionName { get => _sessionName; set => _sessionName = value; }
	public PlayerRef PlayerData { get => _playerData; }
	public static bool operator ==(RoomPlayer roomPlayer, RoomPlayer roomPlayer1)
	{
		return roomPlayer._playerData == roomPlayer1._playerData;
	}
	public static bool operator !=(RoomPlayer roomPlayer, RoomPlayer roomPlayer1)
		=> roomPlayer._playerData != roomPlayer1._playerData;
	public static bool operator ==(RoomPlayer roomPlayer, PlayerRef playerData)
		=> roomPlayer._playerData == playerData;
	public static bool operator !=(RoomPlayer roomPlayer, PlayerRef playerData)
		=> roomPlayer._playerData == playerData;
	public override bool Equals(object obj)
	{
		if (obj is PlayerRef playerRef)
		{
			return _playerData == playerRef;
		}
		else if (obj is RoomPlayer roomPlayer)
		{
			return this == roomPlayer;
		}
		return false;
	}
	public override int GetHashCode() => base.GetHashCode();
}
