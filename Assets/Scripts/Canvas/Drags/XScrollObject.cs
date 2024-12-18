using System;
using UnityEngine;
public struct ScrollLimitData
{
    public float limit;
    public bool isUseLimit;

    public ScrollLimitData(float limit, bool isUseLimit)
    {
        this.limit = limit;
        this.isUseLimit = isUseLimit;
    }
}
public class XScrollObject : MonoBehaviour, IHorizontalOnlyScrollable, ITransformInjectable
{
    private Transform canvasTransform;

    private ScrollLimitData leftLimit = new ScrollLimitData(0f, false);
    private ScrollLimitData rightLimit = new ScrollLimitData(0f, false);
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();  
    }
    public void Scroll(Vector2 moveValue, float sensitivity)
    {
        Vector3 nextPos = rectTransform.localPosition;
        nextPos.x -= (canvasTransform.right * moveValue / sensitivity).x;
        nextPos.y -= (canvasTransform.right * moveValue / sensitivity).y;

        if (leftLimit.isUseLimit && leftLimit.limit > nextPos.x)
        {
            Vector3 correctionPos = rectTransform.localPosition;
            correctionPos.x = leftLimit.limit;

            rectTransform.localPosition = correctionPos;
            return;
        }
        if (rightLimit.isUseLimit && rightLimit.limit < nextPos.x)
        {
            Vector3 correctionPos = rectTransform.localPosition;
            correctionPos.x = rightLimit.limit;

            rectTransform.localPosition = correctionPos;
            return;
        }

        rectTransform.localPosition -= canvasTransform.right * moveValue.x / sensitivity;
    }

    void ITransformInjectable.TransformInject(Transform transform)
    {
        canvasTransform = transform;
    }

    public void InjectLeftLimit(float limitLeftPos)
    {
        leftLimit = new ScrollLimitData(limitLeftPos, true);
    }
    public void InjectRightLimit(float limitRightPos)
    {
        rightLimit = new ScrollLimitData(limitRightPos, true);
    }

    public void UnSubscribe(Action action)
    {
        throw new NotImplementedException();
    }
}
