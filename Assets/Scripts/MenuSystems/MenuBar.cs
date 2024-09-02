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
            menuButton.GetComponent<XScrollObject>().InjectLeftLimit(position.x);
            //test
            position.x += (size.x + 0.05f) * 100f;
        }


    }

    public void ActiveMenu(MenuButton menuButton)
    {
        if (activeMenu == null)
        {
            return;
        }
        activeMenu.EndMenu();

        activeMenu = menuButton;
    }
}
