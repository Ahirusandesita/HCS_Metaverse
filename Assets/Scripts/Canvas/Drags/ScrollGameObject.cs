using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollGameObject : MonoBehaviour, IScrollable, ITransformInjectable
{
    private Transform canvasTransform;
    public void Scroll(Vector2 moveValue, float sensitivity)
    {
        sensitivity *= 100f;
        this.transform.position -= canvasTransform.right * moveValue.x / (1500f / sensitivity);
        this.transform.position -= canvasTransform.forward * moveValue.y
            / (1500f / sensitivity);
    }

    public void UnSubscribe(Action action)
    {
        throw new NotImplementedException();
    }

    void ITransformInjectable.TransformInject(Transform transform)
    {
        canvasTransform = transform;
    }
}
