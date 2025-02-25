/// <summary>
/// キーを大文字小文字変換することが可能
/// </summary>
public interface IFlickKeyCaseConvertible
{
    /// <summary>
    /// 大文字小文字変換
    /// </summary>
    /// <param name="caseConversion"></param>
    void Conversion(CaseConversionKey.CaseConversionInfo caseConversion);
}