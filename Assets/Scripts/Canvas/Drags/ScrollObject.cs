using System;
using UnityEngine;

public class ScrollObject : MonoBehaviour, IScrollable,ITransformInjectable
{
    private Transform canvasTransform;
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }
    public void Scroll(Vector2 moveValue, float sensitivity)
    {
        rectTransform.localPosition -= Vector3.right * moveValue.x / (1500f / sensitivity);
        rectTransform.localPosition -= Vector3.up * moveValue.y
            / (1500f / sensitivity);
    }

    void ITransformInjectable.TransformInject(Transform transform)
    {
        canvasTransform = transform;
    }

    public void UnSubscribe(Action action)
    {
        throw new NotImplementedException();
    }
}
