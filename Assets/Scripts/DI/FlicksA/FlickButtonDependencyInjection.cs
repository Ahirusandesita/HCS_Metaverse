using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(FlickParentNormalKey))]
public class FlickButtonDependencyInjection : MonoBehaviour
{
    [SerializeField,InterfaceType(typeof(IFlickButtonChild))]
    private List<UnityEngine.Object> IFlickButtonChild = new List<UnityEngine.Object>();
    private List<IFlickButtonChild> flickChildren => IFlickButtonChild.OfType<IFlickButtonChild>().ToList();

    private void Awake()
    {
        FlickParentNormalKey flickButtonParent = this.GetComponent<FlickParentNormalKey>();

        List<IFlickButtonChild> flickButtonChildren = new List<IFlickButtonChild>();
        foreach(FlickKeyChild item in flickChildren)
        {
            flickButtonChildren.Add(item);
        }
        flickButtonParent.FlickChildInject(flickButtonChildren);

        foreach (FlickKeyChild flickChild in flickChildren)
        {
            flickChild.FlickParentInject((IFlickButtonParent)flickButtonParent);
        }
    }
}
