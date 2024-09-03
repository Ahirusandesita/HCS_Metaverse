using UnityEngine;

public class YScrollObject : MonoBehaviour, IVerticalOnlyScrollable,ITransformInjectable
{
    private Transform canvasTransform;
    private ScrollLimitData upLimit = new ScrollLimitData(0f, false);
    private ScrollLimitData downLimit = new ScrollLimitData(0f, false);
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }
    public void Scroll(Vector2 move, float sensitivity)
    {
        Vector3 nextPos = rectTransform.localPosition;
        nextPos.x -= (canvasTransform.right * move / sensitivity).x;
        nextPos.y -= (canvasTransform.right * move / sensitivity).y;

        if (upLimit.isUseLimit && upLimit.limit < nextPos.y)
        {
            Vector3 correctionPos = rectTransform.localPosition;
            correctionPos.y = upLimit.limit;

            rectTransform.localPosition = correctionPos;
            return;
        }
        if (downLimit.isUseLimit && downLimit.limit > nextPos.y)
        {
            Vector3 correctionPos = rectTransform.localPosition;
            correctionPos.y = downLimit.limit;
            rectTransform.localPosition = correctionPos;
            return;
        }

        rectTransform.localPosition -= canvasTransform.up * move.y / sensitivity;
    }

    void ITransformInjectable.TransformInject(Transform transform)
    {
        this.canvasTransform = transform;
    }

    public void InjectUpLimit(float limitLeftPos)
    {
        upLimit = new ScrollLimitData(limitLeftPos, true);
    }
    public void InjectDownLimit(float limitRightPos)
    {
        downLimit = new ScrollLimitData(limitRightPos, true);
    }
}
