using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnKey : FlickParent
{
    protected override void PointerClick()
    {

    }

    protected override void PointerDown()
    {
        PointerDownAnimation();
    }

    protected override void PointerEnter()
    {

    }

    protected override void PointerUp()
    {
        PointerUpAnimation();
        flickManager.Return();
    }
}
