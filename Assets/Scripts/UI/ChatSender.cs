using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class ChatSender : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField = default;
    [SerializeField] private ChatSystem chatSystem = default;


    private void Awake()
    {
        inputField.onSubmit.AddListener(delegate
        {
            chatSystem.SendManually(inputField.text);
            inputField.text = string.Empty;
        });
    }

    private void Reset()
    {
        inputField ??= GetComponent<TMP_InputField>();
        chatSystem ??= FindObjectOfType<ChatSystem>();
    }
}
