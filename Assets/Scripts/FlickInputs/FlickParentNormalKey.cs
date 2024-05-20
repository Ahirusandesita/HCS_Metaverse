using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using TMPro;
using UnityEngine.UI;

public static class ButtonColor
{
    public static Color PushColor
    {
        get
        {
            return new Color(0.4823f, 0.4823f, 0.4823f, 1f);
        }
    }
}

public struct Key
{
    public string keyString;
    public bool canUseKey;
    public Key(string keyString,bool canUseKey)
    {
        this.keyString = keyString;
        this.canUseKey = canUseKey;
    }
}


public class FlickParentNormalKey : FlickParent, IFlickButtonParent, IFlickButtonOpeningAndClosing,IFlickButtonCaseConvertible
{
    [SerializeField]
    private string familyString;

    [SerializeField]
    private string keyString;

    private bool canButtonDown = true;

    bool canUseChildKey = false;
    private Key childKey;

    private IFlickButtonChild[] flickButtonChildren;
    private TextMeshProUGUI textMeshProUGUI;

    private Image image;
    private Color startColor;

    private Vector3 startSize;

    private Vector3 pushSize;
    private float push_xSize = 0.37f;

    [Inject]
    public void FlickChildInject(List<IFlickButtonChild> flickButtonChildren)
    {
        this.flickButtonChildren = flickButtonChildren.ToArray();
    }

    protected override void Awake()
    {
        base.Awake();
        textMeshProUGUI = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = familyString;

        image = this.GetComponent<Image>();
        startColor = image.color;

        startSize = transform.localScale;
        pushSize = startSize;
        pushSize.x = push_xSize;
    }

    protected override void PointerEnter()
    {
        canUseChildKey = true;
    }
    protected override void PointerDown()
    {
        if (!canButtonDown)
        {
            return;
        }

        transform.localScale = pushSize;
        textMeshProUGUI.text = keyString;

        image.color = ButtonColor.PushColor;

        flickManager.StartFlick(this);
        foreach (IFlickButtonChild item in flickButtonChildren)
        {
            item.ButtonDeployment();
        }
    }
    protected override void PointerUp()
    {
        if (!canButtonDown)
        {
            return;
        }

        transform.localScale = startSize;
        textMeshProUGUI.text = familyString;

        image.color = startColor;

        flickManager.EndFlick(this);

        if (!canUseChildKey)
        {
            flickManager.SendMessage(childKey.keyString);
        }
        foreach (IFlickButtonChild item in flickButtonChildren)
        {
            item.ButtonClose();
        }
    }
    protected override void PointerClick()
    {
        if (!canButtonDown)
        {
            return;
        }
        flickManager.SendMessage(keyString);
    }

    void IFlickButtonOpeningAndClosing.Open()
    {
        canButtonDown = true;
    }
    void IFlickButtonOpeningAndClosing.Close()
    {
        canButtonDown = false;
    }
    public void Conversion(CaseConversionConKey.CaseConversion caseConversion)
    {
        switch (caseConversion.GetOnlyConversionType)
        {
            case CaseConversionConKey.CaseConversion.ConversionType.Upper:
                textMeshProUGUI.text = textMeshProUGUI.text.ToUpper();
                keyString = keyString.ToUpper();
                break;
            case CaseConversionConKey.CaseConversion.ConversionType.Lower:
                textMeshProUGUI.text = textMeshProUGUI.text.ToLower();
                keyString = keyString.ToLower();
                break;
        }
    }
    public void SendMessage(Key key)
    {
        canUseChildKey = false;
        this.childKey = key;
    }
}
