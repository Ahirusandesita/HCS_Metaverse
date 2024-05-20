using System.Diagnostics;
using TMPro;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class ChatSender : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField = default;
    [SerializeField] private ChatSystem chatSystem = default;

    private Inputter inputter = default;


    [Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        inputField ??= GetComponent<TMP_InputField>();
        chatSystem ??= FindObjectOfType<ChatSystem>();
    }

    private void Awake()
    {
        inputField.onSubmit.AddListener(delegate
        {
            chatSystem.SendManually(inputField.text);
            inputField.text = string.Empty;
        });

        inputter = new Inputter();
        inputter.IsChatOpenRP.Subscribe(isChatOpen =>
        {
            if (isChatOpen)
            {
                inputField.Select();
            }
            else if (inputField.text.Equals(string.Empty))
            {
                inputField.ReleaseSelection();
            }
        });
    }
}
