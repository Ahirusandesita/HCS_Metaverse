using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnKey : FlickKeyParent
{
    protected override void OnPointerClick()
    {

    }

    protected override void OnPointerDown()
    {
        PointerDownAnimation();
    }

    protected override void OnPointerEnter()
    {

    }

    protected override void OnPointerUp()
    {
        PointerUpAnimation();
        flickManager.Return();
    }
}
