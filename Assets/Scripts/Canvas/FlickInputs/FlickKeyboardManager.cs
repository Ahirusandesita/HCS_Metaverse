using UnityEngine;
using TMPro;

/// <summary>
/// １つのフリックキーボードを管理する
/// </summary>
public class FlickKeyboardManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;
    //入力されている文字列
    private string text;

    private IFlickKeyEnabledAndDisabled[] flickButtonOpeningAndClosings;
    private IFlickKeyCaseConvertible[] flickButtonCaseConvertibles;

    private SendChat sendChat;
    private void Awake()
    {
        flickButtonOpeningAndClosings = this.GetComponentsInChildren<IFlickKeyEnabledAndDisabled>(true);
        flickButtonCaseConvertibles = this.GetComponentsInChildren<IFlickKeyCaseConvertible>(true);

        sendChat = GameObject.FindObjectOfType<SendChat>();

        foreach(FlickKeyParent flickKeyParent in this.GetComponentsInChildren<FlickKeyParent>(true))
        {
            flickKeyParent.FlickManagerInject(this);
        }
    }
    /// <summary>
    /// 引数以外のIFlickKeyEnabledAndDisabled型のキーを無効化する
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
    /// 無効化されているキーの有効化
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
    /// テキスト送信
    /// </summary>
    /// <param name="keyString"></param>
    public new void SendMessage(string keyString)
    {
        text += keyString;
        textMeshProUGUI.text = text;
    }
    /// <summary>
    /// CaseConversion送信
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
    /// Delete送信
    /// </summary>
    /// <param name="delete"></param>
    public void SendMessage(Delete delete)
    {
        text = text.Remove(text.Length - delete.DeleteValue);
        textMeshProUGUI.text = text;
    }

    /// <summary>
    /// 入力文字確定
    /// </summary>
    public void Return()
    {
        sendChat.Send_ToOthers(text);
        textMeshProUGUI.text = "";
        text = "";
    }
}
