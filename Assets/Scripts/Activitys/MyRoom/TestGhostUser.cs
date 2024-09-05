using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGhostUser : MonoBehaviour
{
    GhostModel a;
    public GameObject aaa;

    private void Start()
    {
        a = new GhostModel().CreateModel(aaa);
        foreach (var item in GetComponentsInChildren<MeshRenderer>())
        {
            item.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            a.ChangeColor(true);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            a.ChangeColor(false);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            a.Spawn();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            a.Despawn();
        }
    }
}
