/// <summary>
/// �啶���������ϊ��L�[
/// </summary>
public class CaseConversionKey : FlickKeyParent
{
    /// <summary>
    /// �ϊ����
    /// </summary>
    public class CaseConversionInfo
    {
        /// <summary>
        /// �ϊ��w��^�C�v
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
        //�啶���ɕϊ�����悤�Ɏw�肵��FlickManager�ɓ`�B
        caseConversion = new CaseConversionInfo(CaseConversionInfo.ConversionType.Upper);
        flickManager.SendMessage(caseConversion);

        PointerDownAnimation();
    }

    protected override void OnPointerEnter()
    {

    }

    protected override void OnPointerUp()
    {
        //�������ɕϊ�����悤�Ɏw�肵��FlickManager�ɓ`�B
        caseConversion = new CaseConversionInfo(CaseConversionInfo.ConversionType.Lower);
        flickManager.SendMessage(caseConversion);

        PointerUpAnimation();
    }
}
