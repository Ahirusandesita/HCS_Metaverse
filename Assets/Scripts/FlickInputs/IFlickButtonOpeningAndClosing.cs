using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlickButtonOpeningAndClosing
{
    void Open();
    void Close();
}
public interface IFlickButtonCaseConvertible
{
    void Conversion(CaseConversionConKey.CaseConversion caseConversion);
}