using UnityEngine;
using TMPro;

/// <summary>
/// �P�̃t���b�N�L�[�{�[�h���Ǘ�����
/// </summary>
public class FlickKeyboardManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;
    //���͂���Ă��镶����
    private string text;

    private IFlickKeyEnabledAndDisabled[] flickButtonOpeningAndClosings;
    private IFlickKeyCaseConvertible[] flickButtonCaseConvertibles;

    private SendChat sendChat;
    private void Awake()
    {
        flickButtonOpeningAndClosings = this.GetComponentsInChildren<IFlickKeyEnabledAndDisabled>(true);
        flickButtonCaseConvertibles = this.GetComponentsInChildren<IFlickKeyCaseConvertible>(true);

        sendChat = GameObject.FindObjectOfType<SendChat>();
    }
    /// <summary>
    /// �����ȊO��IFlickKeyEnabledAndDisabled�^�̃L�[�𖳌�������
    /// </summary>
    /// <param name="flickButtonOpeningAndClosing"></param>
    public void OtherFlickKeyDisabled(IFlickKeyEnabledAndDisabled flickButtonOpeningAndClosing)
    {
        foreach (IFlickKeyEnabledAndDisabled item in flickButtonOpeningAndClosings)
        {
            if (item == flickButtonOpeningAndClosing)
            {
                continue;
            }

            item.Disabled();
        }
    }
    /// <summary>
    /// ����������Ă���L�[�̗L����
    /// </summary>
    /// <param name="flickButtonOpeningAndClosing"></param>
    public void OtherFlickKeyEnabled(IFlickKeyEnabledAndDisabled flickButtonOpeningAndClosing)
    {
        foreach (IFlickKeyEnabledAndDisabled item in flickButtonOpeningAndClosings)
        {
            item.Enabled();
        }
    }
    /// <summary>
    /// �e�L�X�g���M
    /// </summary>
    /// <param name="keyString"></param>
    public new void SendMessage(string keyString)
    {
        text += keyString;
        textMeshProUGUI.text = text;
    }
    /// <summary>
    /// CaseConversion���M
    /// </summary>
    /// <param name="caseConversion"></param>
    public void SendMessage(CaseConversionKey.CaseConversionInfo caseConversion)
    {
        foreach(IFlickKeyCaseConvertible item in flickButtonCaseConvertibles)
        {
            item.Conversion(caseConversion);
        }
    }
    /// <summary>
    /// Delete���M
    /// </summary>
    /// <param name="delete"></param>
    public void SendMessage(Delete delete)
    {
        text = text.Remove(text.Length - 1);
        textMeshProUGUI.text = text;
    }

    /// <summary>
    /// ���͕����m��
    /// </summary>
    public void Return()
    {
        sendChat.Send_ToOthers(text);
        textMeshProUGUI.text = "";
        text = "";
    }
}
