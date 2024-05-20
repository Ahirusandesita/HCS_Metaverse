using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Delete
{
    private int deleteValue;
    public int DeleteValue => deleteValue;

    public Delete(int deleteValue)
    {
        this.deleteValue = deleteValue;
    }
}
public class DeleteKey : FlickParent
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

    private Delete delete = new Delete(1);


    protected override void PointerClick()
    {

    }

    protected override void PointerDown()
    {
        flickManager.SendMessage(delete);
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
    }
}
