using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDMInjector : MonoBehaviour
{
    [SerializeField]
    private FlickKeyboardManager keyboardManager;
    [SerializeField, InterfaceType(typeof(ISendableMessage))]
    private UnityEngine.Object ISendableMessage;
    private ISendableMessage sendableMessage => ISendableMessage as ISendableMessage;

    private void Awake()
    {
        keyboardManager.InjectSendableMessage(sendableMessage);
    }

    public void InjectTest(ISendableMessage sendableMessage)
    {
        keyboardManager.InjectSendableMessage(sendableMessage);
    }
}
