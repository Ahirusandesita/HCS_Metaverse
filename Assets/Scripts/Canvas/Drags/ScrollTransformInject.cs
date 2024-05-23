using UnityEngine;

public class ScrollTransformInject : MonoBehaviour
{
    private void Awake()
    {
        foreach(ITransformInjectable transformInjectable in this.GetComponentsInChildren<ITransformInjectable>(true))
        {
            transformInjectable.TransformInject(this.transform);
        }
    }
}
