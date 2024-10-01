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
    public MessageInformation(string message, MessageSender messageSender)
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

    private List<Vector3> messagesUpPositionLimit = new List<Vector3>();

    private Vector3 upPosition;
    private Vector3 downPosition;

    private List<MessageInformation> messageInformations = new List<MessageInformation>();
    public void Message(MessageInformation messageInformation)
    {
        messageInformations.Add(messageInformation);
        for (int i = 0; i < messages.Count; i++)
        {
            if (messages[i].MessageIndex == messageInformations.Count - 1)
            {
                messages[i].Message(messageInformations[messageInformations.Count - 1].Message);
                Vector3 position = messages[i].GetComponent<RectTransform>().localPosition;
                position.x = SenderXPosition(messageInformation.MessageSender);
                messages[i].GetComponent<RectTransform>().localPosition = position;
            }

            messages[i].ScrollCancellation();
            //InjectLimit(messages[i]);
        }
    }
    public void InjectMessage(List<MessageInformation> messageInformations)
    {
        for (int i = 0; i < messageInformations.Count; i++)
        {
            this.messageInformations.Add(messageInformations[i]);
        }

        for (int i = 0; i < messages.Count; i++)
        {
            messages[i].Message("");
        }
    }
    private void Start()
    {
        for (int i = 0; i < messages.Count; i++)
        {
            messages[i].transform.position = messages[0].transform.position;

            messages[i].MessageIndex = i;
            messages[i].Message("");
        }
        float yMove = 0f;
        for (int i = 0; i < messages.Count; i++)
        {
            Vector3 positon = messages[i].gameObject.GetComponent<RectTransform>().localPosition;
            positon.y -= yMove;
            yMove += 50f;
            messages[i].gameObject.GetComponent<RectTransform>().localPosition = positon;

            messages[i].OnMessageEnd += OnMessageEnd;

            messagesUpPositionLimit.Add(positon);
            //InjectLimit(messages[i]);
        }

        upPosition = messages[0].gameObject.GetComponent<RectTransform>().localPosition;
        upPosition.y += 50f;

        downPosition = messages[messages.Count - 1].gameObject.GetComponent<RectTransform>().localPosition;
        downPosition.y -= 50f;

        for (int i = 0; i < messages.Count; i++)
        {
            messages[i].InjectUpEndPosition(upPosition);
            messages[i].InjectDownEndPositoin(downPosition);
        }
    }

    private void OnMessageEnd(MessageEnd messageEnd, MessageView message)
    {
        switch (messageEnd)
        {
            case MessageEnd.Up:
                float upAdjustment = message.GetComponent<RectTransform>().localPosition.y - upPosition.y;

                Vector3 test = downPosition;
                test.y += 50f + upAdjustment;
                message.GetComponent<RectTransform>().localPosition = test;

                int maxIndex = 0;
                for (int i = 0; i < messages.Count; i++)
                {
                    if (maxIndex < messages[i].MessageIndex)
                    {
                        maxIndex = messages[i].MessageIndex;
                    }
                }
                if (messageInformations.Count > maxIndex + 1)
                {
                    message.MessageIndex = maxIndex + 1;
                    //InjectLimit(message);

                    message.Message(messageInformations[maxIndex + 1].Message);
                    Vector3 position = message.GetComponent<RectTransform>().localPosition;
                    position.x = SenderXPosition(messageInformations[maxIndex + 1].MessageSender);
                    message.GetComponent<RectTransform>().localPosition = position;

                    message.ScrollCancellation();
                }
                else
                {
                    message.Message("");
                }

                if (message.MessageIndex + 1 >= messageInformations.Count)
                {
                    MessageView[] messageViews = SortMessageView(messages.ToArray());
                    for (int i = 0; i < messageViews.Length; i++)
                    {
                        messageViews[i].ScrollCancellation();
                        messageViews[i].InjectUpLimitPosition(messagesUpPositionLimit[i]);
                    }
                }

                break;
            case MessageEnd.Down:
                float downAdjustment = message.GetComponent<RectTransform>().localPosition.y - downPosition.y;
                Vector3 testDown = upPosition;
                testDown.y -= 50f - downAdjustment;
                message.GetComponent<RectTransform>().localPosition = testDown;

                int minIndex = messageInformations.Count;
                for (int i = 0; i < messages.Count; i++)
                {
                    if (minIndex > messages[i].MessageIndex)
                    {
                        minIndex = messages[i].MessageIndex;
                    }
                }

                if (0 <= minIndex - 1)
                {
                    message.MessageIndex = minIndex - 1;
                    //InjectLimit(message);

                    message.Message(messageInformations[minIndex - 1].Message);
                    Vector3 positionX = message.GetComponent<RectTransform>().localPosition;
                    positionX.x = SenderXPosition(messageInformations[minIndex - 1].MessageSender);
                    message.GetComponent<RectTransform>().localPosition = positionX;

                    message.ScrollCancellation();
                }
                else
                {
                    message.Message("");
                }

                if (message.MessageIndex - 1 < 0)
                {
                    MessageView[] messageViews = SortMessageView(messages.ToArray());
                    for (int i = 0; i < messageViews.Length; i++)
                    {
                        messageViews[i].ScrollCancellation();
                        messageViews[i].InjectDownLimitPosition(messagesUpPositionLimit[i]);
                    }
                }
                break;
        }
    }

    public float SenderXPosition(MessageSender messageSender)
    {
        if (messageSender == MessageSender.Me)
        {
            return senderMe.GetComponent<RectTransform>().localPosition.x;
        }
        else
        {
            return senderOther.GetComponent<RectTransform>().localPosition.x;
        }
    }
    
    //private void InjectLimit(MessageView messageView)
    //{
    //    if (messageView.MessageIndex < messages.Count || messageView.MessageIndex > messageInformations.Count - messages.Count)
    //    {
    //        if (messageView.MessageIndex < messages.Count)
    //        {
    //            messageView.InjectDownLimitPosition(messagesUpPositionLimit[messageView.MessageIndex]);
    //        }
    //        if (messageView.MessageIndex > messageInformations.Count - messages.Count)
    //        {
    //            if (messageInformations.Count < messages.Count)
    //            {
    //                messageView.InjectUpLimitPosition(messagesUpPositionLimit[messageView.MessageIndex]);
    //            }
    //            else
    //            {
    //                messageView.InjectUpLimitPosition(messagesUpPositionLimit[messageView.MessageIndex - (messageInformations.Count - messages.Count + 1)]);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        messageView.ScrollCancellation();
    //    }
    //}


    public MessageView[] SortMessageView(MessageView[] messages)
    {
        MessageView[] messageViews = new MessageView[messages.Length];
        List<MessageView> list = new List<MessageView>();
        for (int i = 0; i < messages.Length; i++)
        {
            list.Add(messages[i]);
        }


        for (int ii = 0; ii < messages.Length; ii++)
        {
            int index = 0;
            int min = int.MaxValue;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].MessageIndex < min)
                {
                    min = list[i].MessageIndex;
                    index = i;
                }
            }

            messageViews[ii] = list[index];
            list.Remove(list[index]);
        }

        return messageViews;
    }
}

