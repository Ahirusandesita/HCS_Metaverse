using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseConversionConKey : FlickParent
{

    private Image image;
    private Color startColor;

    private Vector3 startSize;

    private Vector3 pushSize;
    private float push_xSize = 0.37f;

    protected override void Awake()
    {
        base.Awake();
        image.GetComponent<Image>();
        startColor = image.color;

        startSize = transform.localScale;
        pushSize = startSize;
        pushSize.x = push_xSize;

    }

    public class CaseConversion
    {
        public enum ConversionType
        {
            Upper,
            Lower
        }
        private ConversionType conversionType;
        public ConversionType GetOnlyConversionType => conversionType;
        public CaseConversion(ConversionType conversionType)
        {
            this.conversionType = conversionType;
        }
    }

    private CaseConversion caseConversion = new CaseConversion(CaseConversion.ConversionType.Lower);


    protected override void PointerClick()
    {

    }

    protected override void PointerDown()
    {
        caseConversion = new CaseConversion(CaseConversion.ConversionType.Upper);
        flickManager.SendMessage(caseConversion);
        image.color = ButtonColor.PushColor;
        transform.localScale = pushSize;
    }

    protected override void PointerEnter()
    {

    }

    protected override void PointerUp()
    {
        caseConversion = new CaseConversion(CaseConversion.ConversionType.Lower);
        flickManager.SendMessage(caseConversion);
        image.color = startColor;
        transform.localScale = startSize;
    }
}
