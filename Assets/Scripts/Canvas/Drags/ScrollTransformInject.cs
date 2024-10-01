using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScrollTransformInject : MonoBehaviour
{
    [SerializeField, InterfaceType(typeof(ITransformInjectable))]
    private List<UnityEngine.Object> ITransformInjectables = new List<Object>();
    private List<ITransformInjectable> transformInjectables => ITransformInjectables.OfType<ITransformInjectable>().ToList();
    private void Awake()
    {
        foreach(ITransformInjectable transformInjectable in this.GetComponentsInChildren<ITransformInjectable>(true))
        {
            transformInjectable.TransformInject(this.transform);
        }
        foreach(ITransformInjectable transformInjectable in transformInjectables)
        {
            transformInjectable.TransformInject(this.transform);
        }
    }
}
