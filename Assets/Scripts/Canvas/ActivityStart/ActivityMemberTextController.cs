using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using KumaDebug;
using Cysharp.Threading.Tasks;

public class ActivityMemberTextController : MonoBehaviour
{
	private Room _currentRoom = default;
	[SerializeField]
	private Text _text = default;
	[SerializeField]
	private float _offsetZ = 1;
	[SerializeField]
	private float _offsetY = 0.5f;
	private Transform _displayPosition = default;
	private Transform DisplayPositionTransform { get => _displayPosition ??= FindObjectOfType<OVRCameraRig>().transform; }

	private async void OnEnable()
	{
		_text ??= GetComponent<Text>();
		bool isCanceled = await UniTask.WaitUntil(() 
			=> GateOfFusion.Instance.NetworkRunner != null,cancellationToken:destroyCancellationToken)
			.SuppressCancellationThrow();
		isCanceled = await UniTask.WaitUntil(()
			=> RoomManager.Instance.GetCurrentRoom(GateOfFusion.Instance.NetworkRunner.LocalPlayer) != null,
			cancellationToken:destroyCancellationToken).SuppressCancellationThrow();
		if (isCanceled)
		{
			XKumaDebugSystem.LogWarning("ƒLƒƒƒ“ƒZƒ‹‚³‚ê‚Ü‚µ‚½");
			return;
		}
		_currentRoom = RoomManager.Instance.GetCurrentRoom(GateOfFusion.Instance.NetworkRunner.LocalPlayer);
		DisplayTextData();
	}

	public void UpdateText()
	{
		DisplayTextData();
	}

	private void DisplayTextData()
	{

		_text.text = "";
		foreach (PlayerRef playerRef in _currentRoom.JoinRoomPlayer)
		{
			_text.text += playerRef.ToString();
			_text.text += "\n";
		}
	}

	private void Update()
	{
		Vector3 position = DisplayPositionTransform.position + DisplayPositionTransform.forward * _offsetZ;
		transform.root.position = new Vector3(position.x, position.y + _offsetY, position.z);
		transform.root.rotation = _displayPosition.rotation;
	}
}
