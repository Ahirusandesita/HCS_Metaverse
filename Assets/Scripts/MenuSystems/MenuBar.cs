using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IMenuManager
{
    void ActiveMenu(MenuButton menuButton);
}
public class MenuBar : MonoBehaviour, IMenuManager
{
    [SerializeField]
    private RectTransform standardTransform;

    [SerializeField]
    private List<MenuButton> menuButtons;
    private MenuButton activeMenu;

    private void Awake()
    {
        Vector3 size = standardTransform.localScale;
        Vector3 position = standardTransform.localPosition;

        Debug.Log($"size{standardTransform.localScale.x} position{standardTransform.position}");
        foreach (MenuButton menuButton in menuButtons)
        {
            menuButton.InjectMenuManager(this);

            menuButton.GetComponent<RectTransform>().localScale = size;
            menuButton.GetComponent<RectTransform>().localPosition = position;

            int s = 0;
            if(menuButtons.Count > 5)
            {
                s = menuButtons.Count - 5;
            }
            menuButton.GetComponent<XScrollObject>().InjectLeftLimit(position.x - ((size.x + 0.25f) * 100f) * s );
            menuButton.GetComponent<XScrollObject>().InjectRightLimit(position.x);
            //test
            position.x += (size.x + 0.25f) * 100f;
        }


    }

    public void ActiveMenu(MenuButton menuButton)
    {
        if (activeMenu != null)
        {
            activeMenu.EndMenu();
        }
        activeMenu = menuButton;

    }
}
