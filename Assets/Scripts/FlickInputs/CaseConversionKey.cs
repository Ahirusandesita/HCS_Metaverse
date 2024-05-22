/// <summary>
/// 大文字小文字変換キー
/// </summary>
public class CaseConversionKey : FlickKeyParent
{
    /// <summary>
    /// 変換情報
    /// </summary>
    public class CaseConversionInfo
    {
        /// <summary>
        /// 変換指定タイプ
        /// </summary>
        public enum ConversionType
        {
            Upper,
            Lower
        }
        private ConversionType conversionType;
        public ConversionType GetOnlyConversionType => conversionType;
        public CaseConversionInfo(ConversionType conversionType)
        {
            this.conversionType = conversionType;
        }
    }

    private CaseConversionInfo caseConversion = new CaseConversionInfo(CaseConversionInfo.ConversionType.Lower);


    protected override void OnPointerClick()
    {

    }

    protected override void OnPointerDown()
    {
        //大文字に変換するように指定してFlickManagerに伝達
        caseConversion = new CaseConversionInfo(CaseConversionInfo.ConversionType.Upper);
        flickManager.SendMessage(caseConversion);

        PointerDownAnimation();
    }

    protected override void OnPointerEnter()
    {

    }

    protected override void OnPointerUp()
    {
        //小文字に変換するように指定してFlickManagerに伝達
        caseConversion = new CaseConversionInfo(CaseConversionInfo.ConversionType.Lower);
        flickManager.SendMessage(caseConversion);

        PointerUpAnimation();
    }
}
