using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IMarkProcess
{
    void Process(MarkViewEventArgs markEventArgs, MarkData markData);
    void CanvasTransformInject(Transform canvasTransform);
}
public class MarkData
{
    public readonly Vector3 MarkPosition;
    public MarkData(Vector3 markPosition)
    {
        this.MarkPosition = markPosition;
    }
}
public class Mark : MonoBehaviour
{
    [SerializeField]
    private MarkView markView;
    [SerializeField, InterfaceType(typeof(IMarkProcess))]
    private List<UnityEngine.Object> IMarkClickProcess;
    private List<IMarkProcess> markClickProcess => IMarkClickProcess.OfType<IMarkProcess>().ToList();
    [SerializeField, InterfaceType(typeof(IMarkProcess))]
    private List<UnityEngine.Object> IMarkHoverProcess;
    private List<IMarkProcess> markHoverProcess => IMarkHoverProcess.OfType<IMarkProcess>().ToList();
    [SerializeField, InterfaceType(typeof(IMarkProcess))]
    private List<UnityEngine.Object> IMarkUnHoverProcess;
    private List<IMarkProcess> markUnHoverProcess => IMarkUnHoverProcess.OfType<IMarkProcess>().ToList();



    private MarkView instanceView;
    public void TransformInject(Transform mapCanvas)
    {
        instanceView = Instantiate(markView, mapCanvas);


        instanceView.OnMarkClick += (data) =>
        {
            switch (data.MarkProcessType)
            {
                case MarkProcessType.Select:
                    foreach (IMarkProcess markProcess in markClickProcess)
                    {
                        markProcess.Process(data, new MarkData(this.transform.position));
                    }
                    break;
                case MarkProcessType.Hover:
                    foreach (IMarkProcess markProcess in markHoverProcess)
                    {
                        markProcess.Process(data, new MarkData(this.transform.position));
                    }
                    break;
                case MarkProcessType.UnHover:
                    foreach (IMarkProcess markProcess in markUnHoverProcess)
                    {
                        markProcess.Process(data, new MarkData(this.transform.position));
                    }
                    break;
            }
        };

        foreach(IMarkProcess markProcess in markClickProcess)
        {
            markProcess.CanvasTransformInject(mapCanvas);
        }
        foreach (IMarkProcess markProcess in markHoverProcess)
        {
            markProcess.CanvasTransformInject(mapCanvas);
        }
        foreach (IMarkProcess markProcess in markUnHoverProcess)
        {
            markProcess.CanvasTransformInject(mapCanvas);
        }
    }
    public void MarkViewPosition(Vector2 position)
    {
        instanceView.GetComponent<RectTransform>().localPosition = position;
    }
    public void MarkOutCamera()
    {
        instanceView.gameObject.SetActive(false);
    }
    public void MarkInCamera()
    {
        instanceView.gameObject.SetActive(true);
    }
}
