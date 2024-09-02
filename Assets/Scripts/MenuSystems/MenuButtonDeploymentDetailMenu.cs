using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonDeploymentDetailMenu : MenuButton
{
    [SerializeField]
    private DetailMenu detailMenu;

    private void Awake()
    {
        detailMenu.gameObject.SetActive(false);
    }

    public override void StartMenu()
    {
        detailMenu.gameObject.SetActive(true);
    }
    public override void EndMenu()
    {
        detailMenu.gameObject.SetActive(false);
    }
}
