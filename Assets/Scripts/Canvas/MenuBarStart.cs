using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBarStart : MonoBehaviour, IInputControllable
{
    [SerializeField]
    private GameObject MenuBar;

    private bool isDeplo;
    private void Start()
    {
        isDeplo = false;
        Inputter.Player.Options.performed += _ =>
        {
            isDeplo = !isDeplo;
            Dep();
        };
        Dep();
    }

    private void Dep()
    {
        if (isDeplo)
        {
            MenuBar.SetActive(true);
        }
        else
        {
            GetComponentInChildren<MenuBar>().Close();
            MenuBar.SetActive(false);
        }
    }

    public void Hide()
	{
        isDeplo = false;
        Dep();
    }
}
