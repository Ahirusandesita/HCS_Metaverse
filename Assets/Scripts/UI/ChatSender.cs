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
        // ���͊������̃R�[���o�b�N��ݒ�
        inputField.onSubmit.AddListener(_ =>
        {
            // ChatSystem�ɓ��͓��e�𑗐M
            chatSystem.SendManually(inputField.text);
            inputField.text = string.Empty;
        });

        // InputField�̃A�N�e�B�u��Ԃ�؂�ւ���
        inputter = new Inputter().AddTo(this);
        inputter.IsChatOpenRP.Subscribe(isChatOpen =>
        {
            if (isChatOpen)
            {
                inputField.ActivateInputField();
            }
            else if (inputField.text.Equals(string.Empty))
            {
                inputField.DeactivateInputField();
            }
        });
    }
}
