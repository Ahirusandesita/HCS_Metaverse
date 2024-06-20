using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PokeableCanvasInformation : DependencyInformation
{
    public readonly Transform CameraTransform;
    public readonly Transform HandTransform;
    public readonly Transform Parent;
    public PokeableCanvasInformation(Transform cameraTransform,Transform handTransform)
    {
        this.CameraTransform = cameraTransform;
        this.HandTransform = handTransform;
    }
    public PokeableCanvasInformation(Transform cameraTransform, Transform handTransform,Transform parent)
    {
        this.CameraTransform = cameraTransform;
        this.HandTransform = handTransform;
        this.Parent = parent;
    }
}
public class PokeableCanvasInjector : MonoBehaviour, IDependencyInjector<PokeableCanvasInformation>
{
    [SerializeField, InterfaceType(typeof(IPutCanvasInHand))]
    private List<UnityEngine.Object> IPutCanvasInHands;
    private List<IPutCanvasInHand> putCanvasInHand => IPutCanvasInHands.OfType<IPutCanvasInHand>().ToList();

    public void Inject(PokeableCanvasInformation information)
    {
        foreach(IPutCanvasInHand @object in putCanvasInHand)
        {
            @object.Inject(information.CameraTransform, information.HandTransform);
        }
    }
}
