using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnKey : FlickParent
{
    private Image image;
    private Color startColor;

    private Vector3 startSize;

    private Vector3 pushSize;
    private float push_xSize = 0.37f;

    protected override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
        startColor = image.color;

        startSize = transform.localScale;
        pushSize = startSize;
        pushSize.x = push_xSize;

    }


    protected override void PointerClick()
    {

    }

    protected override void PointerDown()
    {
        image.color = ButtonColor.PushColor;
        transform.localScale = pushSize;
    }

    protected override void PointerEnter()
    {

    }

    protected override void PointerUp()
    {
        image.color = startColor;
        transform.localScale = startSize;
        flickManager.Return();
    }
}
