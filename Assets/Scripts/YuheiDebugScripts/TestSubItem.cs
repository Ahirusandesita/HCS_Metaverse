using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSubItem : MonoBehaviour
{
    bool a = false;
    public void Select()
    {
        Debug.Log("Select");
        a = true;
    }

    public void UnSelect()
    {
        a = false;
    }
    private void Update()
    {
        if (a)
            this.transform.position = new Vector3(0f, 2f, 0f);
    }
}
