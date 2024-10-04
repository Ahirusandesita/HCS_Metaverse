using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinNameProcess : MonoBehaviour,IMarkProcess
{
    [SerializeField]
    private PinInformationView informationView;
    private PinInformationView instance;
    [SerializeField]
    private string pinName;
    public void CanvasTransformInject(Transform canvasTransform)
    {
        instance = Instantiate(informationView, canvasTransform);
        instance.Initialize();
        instance.gameObject.SetActive(false);
    }

    public void Process(MarkViewEventArgs markEventArgs, MarkData markData)
    {
        switch (markEventArgs.MarkProcessType)
        {
            case MarkProcessType.Hover:
                instance.gameObject.SetActive(true);
                instance.GetComponent<RectTransform>().localPosition = markEventArgs.ReadonlyRectPositionAdapter.Position;
                instance.Display(pinName);
                break;
            case MarkProcessType.UnHover:
                instance.Initialize();
                instance.gameObject.SetActive(false);
                break;
        }
    }
}
