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

    private YScrollObject yScrollObject;

    public event Action<MessageEnd, MessageView> OnMessageEnd;
    public int MessageIndex { get; set; }
    public void InjectUpEndPosition(Vector3 upEndPosition)
    {
        this.upEndPosition = upEndPosition;
    }
    public void InjectUpLimitPosition(Vector3 upEndPosition)
    {
        yScrollObject.InjectUpLimit(upEndPosition.y);
    }
    public void InjectDownEndPositoin(Vector3 downEndPosition)
    {
        this.downEndPositon = downEndPosition;
    }
    public void InjectDownLimitPosition(Vector3 downEndPosition)
    {
        yScrollObject.InjectDownLimit(downEndPosition.y);
    }
    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        yScrollObject = this.GetComponent<YScrollObject>();
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

    public void UpLimit(Vector3 position)
    {
        yScrollObject.InjectUpLimit(position.y);
    }
    public void DownLimit(Vector3 position)
    {
        yScrollObject.InjectDownLimit(position.y);
    }

    public void ScrollCancellation()
    {
        yScrollObject.UpLimitCancellation();
        yScrollObject.DownLimitCancellation();
    }
}
