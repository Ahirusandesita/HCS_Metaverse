using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaseConversionConKey : FlickParent
{

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
        PointerDownAnimation();
    }

    protected override void PointerEnter()
    {

    }

    protected override void PointerUp()
    {
        caseConversion = new CaseConversion(CaseConversion.ConversionType.Lower);
        flickManager.SendMessage(caseConversion);
        PointerUpAnimation();
    }
}
