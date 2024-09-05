using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Fusion;
public enum MessageSender
{
    Me,
    Other
}
public class MessageInformation
{
    public readonly string Message;
    public MessageSender MessageSender;
    public MessageInformation(string message,MessageSender messageSender)
    {
        this.Message = message;
        this.MessageSender = messageSender;
    }
}
public class DM : MonoBehaviour
{
    [SerializeField]
    private Transform senderMe;
    [SerializeField]
    private Transform senderOther;

    [SerializeField]
    private List<MessageView> messages = new List<MessageView>();

    private Vector3 upPosition;
    private Vector3 downPosition;

    private List<MessageInformation> messageInformations = new List<MessageInformation>();
    public void Message(MessageInformation messageInformation)
    {
        messageInformations.Add(messageInformation);
    }
    public void InjectMessage(List<MessageInformation> messageInformations)
    {
        this.messageInformations = messageInformations;
    }

    private void Start()
    {
        for(int i = 0; i < messages.Count; i++)
        {
            messages[i].transform.position = messages[0].transform.position;

            messages[i].MessageIndex = i;
        }
        float yMove = 0f;
        for(int i = 0; i < messages.Count; i++)
        {
            Vector3 positon = messages[i].gameObject.GetComponent<RectTransform>().localPosition;
            positon.y -= yMove;
            yMove += 50f;
            messages[i].gameObject.GetComponent<RectTransform>().localPosition = positon;

            messages[i].OnMessageEnd += OnMessageEnd;
        }

        upPosition = messages[0].gameObject.GetComponent<RectTransform>().localPosition;
        upPosition.y += 50f;

        downPosition = messages[messages.Count - 1].gameObject.GetComponent<RectTransform>().localPosition;
        downPosition.y -= 50f;

        for(int i = 0; i < messages.Count; i++)
        {
            messages[i].InjectUpEndPosition(upPosition);
            messages[i].InjectDownEndPositoin(downPosition);
        }
    }

    private void OnMessageEnd(MessageEnd messageEnd,MessageView message)
    {
        switch (messageEnd)
        {
            case MessageEnd.Up:
                message.GetComponent<RectTransform>().localPosition = downPosition;

                int maxIndex = 0;
                for(int i = 0; i < messages.Count; i++)
                {
                    if(maxIndex < messages[i].MessageIndex)
                    {
                        maxIndex = messages[i].MessageIndex;
                    }
                }
                message.MessageIndex = maxIndex + 1;


                if (messageInformations.Count > maxIndex)
                {
                    message.Message(messageInformations[maxIndex + 1].Message);
                    Vector3 position = message.GetComponent<RectTransform>().localPosition;
                    position.x = SenderXPosition(messageInformations[maxIndex + 1].MessageSender);
                    message.GetComponent<RectTransform>().localPosition = position;
                }
                break;
            case MessageEnd.Down:
                message.GetComponent<RectTransform>().localPosition = upPosition;

                int minIndex = 10000000;
                for(int i = 0; i < messages.Count;i++)
                {
                    if(minIndex > messages[i].MessageIndex)
                    {
                        minIndex = messages[i].MessageIndex;
                    }
                }

                message.MessageIndex = minIndex - 1;

                if (0 <= minIndex)
                {
                    message.Message(messageInformations[minIndex - 1].Message);

                    Vector3 positionX = message.GetComponent<RectTransform>().localPosition;
                    positionX.x = SenderXPosition(messageInformations[minIndex - 1].MessageSender);
                    message.GetComponent<RectTransform>().localPosition = positionX;
                }
                break;
        }
    }
    
    public float SenderXPosition(MessageSender messageSender)
    {
        if(messageSender == MessageSender.Me)
        {
            return senderMe.position.x;
        }
        else
        {
            return senderOther.position.x;
        }
    }
}
