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
public class DeleteKey : FlickKeyParent
{

    private Delete delete = new Delete(1);


    protected override void OnPointerClick()
    {

    }

    protected override void OnPointerDown()
    {
        flickManager.SendMessage(delete);
        PointerDownAnimation();
    }

    protected override void OnPointerEnter()
    {

    }

    protected override void OnPointerUp()
    {
        PointerUpAnimation();
    }
}
