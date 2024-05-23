using UnityEngine;

public class XScrollObject : MonoBehaviour, IHorizontalOnlyScrollable,ITransformInjectable
{
    private Transform canvasTransform;
    public void Scroll(Vector2 moveValue, float sensitivity)
    {
        this.transform.position -= canvasTransform.right * moveValue.x / (1500f / sensitivity);
    }

    void ITransformInjectable.TransformInject(Transform transform)
    {
        canvasTransform = transform;
    }
}
