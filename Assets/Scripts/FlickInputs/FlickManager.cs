using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlickManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;
    private string text;

    private IFlickButtonOpeningAndClosing[] flickButtonOpeningAndClosings;
    private IFlickButtonCaseConvertible[] flickButtonCaseConvertibles;

    private SendChat sendChat;
    private void Awake()
    {
        flickButtonOpeningAndClosings = this.GetComponentsInChildren<IFlickButtonOpeningAndClosing>(true);
        flickButtonCaseConvertibles = this.GetComponentsInChildren<IFlickButtonCaseConvertible>(true);

        sendChat = GameObject.FindObjectOfType<SendChat>();
    }

    public void StartFlick(IFlickButtonOpeningAndClosing flickButtonOpeningAndClosing)
    {
        foreach (IFlickButtonOpeningAndClosing item in flickButtonOpeningAndClosings)
        {
            if (item == flickButtonOpeningAndClosing)
            {
                continue;
            }

            item.Close();
        }
    }
    public void EndFlick(IFlickButtonOpeningAndClosing flickButtonOpeningAndClosing)
    {
        foreach (IFlickButtonOpeningAndClosing item in flickButtonOpeningAndClosings)
        {
            item.Open();
        }
    }

    public new void SendMessage(string keyString)
    {
        text += keyString;
        textMeshProUGUI.text = text;
    }
    public void SendMessage(CaseConversionConKey.CaseConversion caseConversion)
    {
        foreach(IFlickButtonCaseConvertible item in flickButtonCaseConvertibles)
        {
            item.Conversion(caseConversion);
        }
    }
    public void SendMessage(Delete delete)
    {
        text = text.Remove(text.Length - 1);
        textMeshProUGUI.text = text;
    }

    public void Return()
    {
        sendChat.Send_ToOthers(text);
        textMeshProUGUI.text = "";
        text = "";
    }
    //public void SendMessage(SpecialKeyType specialKeyType)
    //{
    //    switch (specialKeyType)
    //    {
    //        case SpecialKeyType.None: break;
    //        case SpecialKeyType.Delete:
    //            
    //            break;
    //        case SpecialKeyType.Space:
    //            text += " ";
    //            break;
    //        case SpecialKeyType.CaseConversion:
    //            char conversion = text[text.Length - 1];
    //            break;
    //        case SpecialKeyType.Decision:
    //            break;
    //    }
    //}
}
