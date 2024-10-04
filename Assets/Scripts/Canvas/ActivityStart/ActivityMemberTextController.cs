using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class ActivityMemberTextController : MonoBehaviour
{
    private Room _currentRoom = default;
    [SerializeField]
    private Text _text = default;
    [SerializeField]
    private float _offsetZ = 10;
    [SerializeField]
    private float _offsetY = 10;
    [SerializeField]
    private Transform _displayPosition = default;

    private void OnEnable()
    {
        _text ??= GetComponent<Text>();
        _currentRoom = RoomManager.Instance.GetCurrentRoom(GateOfFusion.Instance.NetworkRunner.LocalPlayer);
        if (_displayPosition == null)
        {
            _displayPosition = FindObjectOfType<OVRCameraRig>().transform;
        }
        Debug.LogError(_displayPosition);
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
        Vector3 position = _displayPosition.position + _displayPosition.forward * _offsetZ;
        transform.root.position = new Vector3(position.x,position.y + _offsetY,position.z);
        transform.root.rotation = _displayPosition.rotation;
    }
}
