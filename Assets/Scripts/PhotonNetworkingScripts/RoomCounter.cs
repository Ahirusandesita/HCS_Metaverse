using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RoomCounter : NetworkBehaviour
{
	private static RoomCounter _instance = default;
	private int[] _roomCounter;

	public int this[int index] { get => _roomCounter[index]; }

	public static RoomCounter Instance { get => _instance; }
	private void Awake()
	{
		if(_instance is null)
		{
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		_roomCounter = new int[System.Enum.GetValues(typeof(ActivityType)).Length];
	}
}
