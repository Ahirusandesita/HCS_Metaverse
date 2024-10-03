using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IMarkProcess
{
    void Process(MarkViewEventArgs markEventArgs, MarkData markData);
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
    private UnityEngine.Object IMarkProcess;
    private IMarkProcess markProcess => IMarkProcess as IMarkProcess;
    private MarkView instanceView;
    [SerializeField]
    private GameObject a;
    public void TransformInject(Transform mapCanvas)
    {
        instanceView = Instantiate(markView, mapCanvas);
        instanceView.OnMarkClick += (data) => markProcess.Process(data, new MarkData(this.transform.position));
    }
    public void MarkViewPosition(Vector2 position)
    {
        a.GetComponent<RectTransform>().localPosition = position;
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
