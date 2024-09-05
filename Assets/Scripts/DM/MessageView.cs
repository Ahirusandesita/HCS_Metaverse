using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum MessageEnd
{
    Up,
    Down
}
public class MessageView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    private RectTransform rectTransform;
    private Vector3 upEndPosition;
    private Vector3 downEndPositon;
    public event Action<MessageEnd, MessageView> OnMessageEnd;
    public int MessageIndex { get; set; }
    public void InjectUpEndPosition(Vector3 upEndPosition)
    {
        this.upEndPosition = upEndPosition;
    }
    public void InjectDownEndPositoin(Vector3 downEndPosition)
    {
        this.downEndPositon = downEndPosition;
    }
    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (rectTransform.localPosition.y > upEndPosition.y)
        {
            OnMessageEnd?.Invoke(MessageEnd.Up, this);
        }
        if (rectTransform.localPosition.y < downEndPositon.y)
        {
            OnMessageEnd?.Invoke(MessageEnd.Down, this);
        }
    }

    public void Message(string message)
    {
        this.textMeshProUGUI.text = message;
    }
}
