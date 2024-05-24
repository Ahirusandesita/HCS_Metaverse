using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public Key(string keyString, bool canUseKey)
    {
        this.keyString = keyString;
        this.canUseKey = canUseKey;
    }
}


public class FlickParentNormalKey : FlickKeyParent, IFlickButtonParent, IFlickKeyEnabledAndDisabled, IFlickKeyCaseConvertible
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

    public void FlickChildInject(List<IFlickButtonChild> flickButtonChildren)
    {
        this.flickButtonChildren = flickButtonChildren.ToArray();
    }

    protected override void Awake()
    {
        base.Awake();
        textMeshProUGUI = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = familyString;
    }

    protected override void OnPointerEnter()
    {
        canUseChildKey = true;
    }
    protected override void OnPointerDown()
    {
        if (!canButtonDown)
        {
            return;
        }

        textMeshProUGUI.text = keyString;

        PointerDownAnimation();

        flickManager.OtherFlickKeyDisabled(this);
        foreach (IFlickButtonChild item in flickButtonChildren)
        {
            item.ButtonDeployment();
        }
    }
    protected override void OnPointerUp()
    {
        if (!canButtonDown)
        {
            return;
        }
        textMeshProUGUI.text = familyString;

        PointerUpAnimation();

        flickManager.OtherFlickKeyEnabled(this);

        if (!canUseChildKey)
        {
            flickManager.SendMessage(childKey.keyString);
        }
        foreach (IFlickButtonChild item in flickButtonChildren)
        {
            item.ButtonClose();
        }
    }
    protected override void OnPointerClick()
    {
        if (!canButtonDown)
        {
            return;
        }
        flickManager.SendMessage(keyString);
    }

    void IFlickKeyEnabledAndDisabled.Enabled()
    {
        canButtonDown = true;
    }
    void IFlickKeyEnabledAndDisabled.Disabled()
    {
        canButtonDown = false;
    }
    public void Conversion(CaseConversionKey.CaseConversionInfo caseConversion)
    {
        switch (caseConversion.GetOnlyConversionType)
        {
            case CaseConversionKey.CaseConversionInfo.ConversionType.Upper:
                textMeshProUGUI.text = textMeshProUGUI.text.ToUpper();
                keyString = keyString.ToUpper();
                break;
            case CaseConversionKey.CaseConversionInfo.ConversionType.Lower:
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
