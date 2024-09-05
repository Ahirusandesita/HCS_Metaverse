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
        for(int i = 0; i < messages.Count; i++)
        {
            if(messages[i].MessageIndex == messageInformations.Count - 1)
            {
                messages[i].Message(messageInformations[messageInformations.Count - 1].Message);
                Vector3 position = messages[i].GetComponent<RectTransform>().localPosition;
                position.x = SenderXPosition(messageInformation.MessageSender);
                messages[i].GetComponent<RectTransform>().localPosition = position;
            }
        }
         
    }
    public void InjectMessage(List<MessageInformation> messageInformations)
    {
        for(int i = 0; i < messageInformations.Count; i++)
        {
            this.messageInformations.Add(messageInformations[i]);
        }
    }

    private void Start()
    {
        for(int i = 0; i < messages.Count; i++)
        {
            messages[i].transform.position = messages[0].transform.position;

            messages[i].MessageIndex = i;
            messages[i].Message("");
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
                float upAdjustment = message.GetComponent<RectTransform>().localPosition.y - upPosition.y;

                Vector3 test = downPosition;
                test.y += 50f + upAdjustment;
                message.GetComponent<RectTransform>().localPosition = test;

                int maxIndex = 0;
                for(int i = 0; i < messages.Count; i++)
                {
                    if(maxIndex < messages[i].MessageIndex)
                    {
                        maxIndex = messages[i].MessageIndex;
                    }
                }
                    

                if (messageInformations.Count > maxIndex + 1)
                {
                    message.MessageIndex = maxIndex + 1;
                    message.Message(messageInformations[maxIndex + 1].Message);
                    Vector3 position = message.GetComponent<RectTransform>().localPosition;
                    position.x = SenderXPosition(messageInformations[maxIndex + 1].MessageSender);
                    message.GetComponent<RectTransform>().localPosition = position;
                }
                else
                {
                    message.Message("");
                }
                break;
            case MessageEnd.Down:
                float downAdjustment = message.GetComponent<RectTransform>().localPosition.y - downPosition.y;
                Vector3 testDown = upPosition;
                testDown.y -= 50f - downAdjustment;
                message.GetComponent<RectTransform>().localPosition = testDown;

                int minIndex = 10000000;
                for(int i = 0; i < messages.Count;i++)
                {
                    if(minIndex > messages[i].MessageIndex)
                    {
                        minIndex = messages[i].MessageIndex;
                    }
                }

                if (0 <= minIndex - 1)
                {
                    message.MessageIndex = minIndex - 1;

                    message.Message(messageInformations[minIndex - 1].Message);
                    Vector3 positionX = message.GetComponent<RectTransform>().localPosition;
                    positionX.x = SenderXPosition(messageInformations[minIndex - 1].MessageSender);
                    message.GetComponent<RectTransform>().localPosition = positionX;
                }
                else
                {
                    message.Message("");
                }
                break;
        }
    }
    
    public float SenderXPosition(MessageSender messageSender)
    {
        if(messageSender == MessageSender.Me)
        {
            return senderMe.GetComponent<RectTransform>().localPosition.x;
        }
        else
        {
            return senderOther.GetComponent<RectTransform>().localPosition.x;
        }
    }
}
