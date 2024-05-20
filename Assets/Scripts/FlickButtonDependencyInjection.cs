using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlickParentNormalKey))]
public class FlickButtonDependencyInjection : MonoBehaviour
{
    [SerializeField]
    private List<FlickChild> flickChildren = new List<FlickChild>();

    private void Awake()
    {
        FlickParentNormalKey flickButtonParent = this.GetComponent<FlickParentNormalKey>();

        List<IFlickButtonChild> flickButtonChildren = new List<IFlickButtonChild>();
        foreach(FlickChild item in flickChildren)
        {
            flickButtonChildren.Add(item);
        }
        flickButtonParent.FlickChildInject(flickButtonChildren);

        foreach (FlickChild flickChild in flickChildren)
        {
            flickChild.FlickParentInject((IFlickButtonParent)flickButtonParent);
        }
    }
}
