using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using TMPro;

public class FlickKeyChild : MonoBehaviour, IFlickButtonChild, IFlickKeyCaseConvertible
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    [SerializeField]
    private string keyString;

    [SerializeField]
    private EventTrigger eventTrigger;
    private IFlickButtonParent flickButtonParent;
    private TextMeshProUGUI outPutText;

    private Color startColor;

    [Inject]
    public void FlickParentInject(IFlickButtonParent flickButtonParent)
    {
        this.flickButtonParent = flickButtonParent;
    }

    private void Awake()
    {
        EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry();
        entryPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryPointerEnter.callback.AddListener((x) => PointerEnter());

        EventTrigger.Entry entryPointerExit = new EventTrigger.Entry();
        entryPointerExit.eventID = EventTriggerType.PointerExit;
        entryPointerExit.callback.AddListener((x) => PointerExit());

        eventTrigger.triggers.Add(entryPointerExit);
        eventTrigger.triggers.Add(entryPointerEnter);


        outPutText = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        startColor = image.color;

        textMeshProUGUI.enabled = false;
        image.enabled = false;
    }

    private void PointerEnter()
    {
        image.color = ButtonColor.PushColor;
        flickButtonParent.SendMessage(new Key(keyString, true));
    }
    private void PointerExit()
    {
        image.color = startColor;
        flickButtonParent.SendMessage(new Key(keyString, false));
    }

    void IFlickButtonChild.ButtonClose()
    {
        textMeshProUGUI.enabled = false;
        image.enabled = false;
    }

    void IFlickButtonChild.ButtonDeployment()
    {
        textMeshProUGUI.enabled = true;
        image.enabled = true;
    }
    public void Conversion(CaseConversionKey.CaseConversionInfo caseConversion)
    {
        switch (caseConversion.GetOnlyConversionType)
        {
            case CaseConversionKey.CaseConversionInfo.ConversionType.Upper:
                outPutText.text = outPutText.text.ToUpper();
                keyString = keyString.ToUpper();
                break;
            case CaseConversionKey.CaseConversionInfo.ConversionType.Lower:
                outPutText.text = outPutText.text.ToLower();
                keyString = keyString.ToLower();
                break;
        }
    }
}
