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

    private Delete delete = new Delete(1);


    protected override void PointerClick()
    {

    }

    protected override void PointerDown()
    {
        flickManager.SendMessage(delete);
        PointerDownAnimation();
    }

    protected override void PointerEnter()
    {

    }

    protected override void PointerUp()
    {
        PointerUpAnimation();
    }
}
