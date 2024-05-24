using UnityEngine;

public class YScrollObject : MonoBehaviour, IVerticalOnlyScrollable,ITransformInjectable
{
    private Transform canvasTransform;

    public void Scroll(Vector2 move, float sensitivity)
    {
        this.transform.position -= canvasTransform.up * move.y / (1500f / sensitivity);
    }

    void ITransformInjectable.TransformInject(Transform transform)
    {
        this.canvasTransform = transform;
    }
}
