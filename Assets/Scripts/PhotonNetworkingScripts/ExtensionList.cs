using System.Collections.Generic;
using Fusion;

public static partial class ExtensionList
{
	public static int IndexOf(this List<RoomPlayer> list, PlayerRef playerRef)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].PlayerData != playerRef) { continue; }
			return i;
		}
		return -1;
	}
}
