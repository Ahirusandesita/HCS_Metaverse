using System.Diagnostics;
using TMPro;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class ChatSender : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField = default;
    [SerializeField] private ChatSystem chatSystem = default;

    private PlayerInputActions.UIActions UIActions => Inputter.UI;


    [Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        inputField ??= GetComponent<TMP_InputField>();
        chatSystem ??= FindObjectOfType<ChatSystem>();
    }

    private void Awake()
    {
        // 入力完了時のコールバックを設定
        inputField.onSubmit.AddListener(_ =>
        {
                // ChatSystemに入力内容を送信
                chatSystem.SendManually(inputField.text);
            inputField.text = string.Empty;
        });

        // InputFieldのアクティブ状態を切り替える
        UIActions.Enable();
        UIActions.Chat.performed += isChatOpen =>
        {
            if (inputField.isFocused)
            {
                inputField.ActivateInputField();
            }
            else if (inputField.text.Equals(string.Empty))
            {
                inputField.DeactivateInputField();
            }
        };
    }
}