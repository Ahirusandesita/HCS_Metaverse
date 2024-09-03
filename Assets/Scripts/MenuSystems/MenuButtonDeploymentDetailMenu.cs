using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonDeploymentDetailMenu : MenuButton
{
    [SerializeField]
    private List<DetailMenu> detailMenus = new List<DetailMenu>();

    private void Start()
    {
        foreach (DetailMenu detailMenu in detailMenus)
        {
            detailMenu.UnDeployment();
        }
    }

    public override void StartMenu()
    {
        foreach (DetailMenu detailMenu in detailMenus)
        {
            detailMenu.Deployment();
        }
    }
    public override void EndMenu()
    {
        foreach (DetailMenu detailMenu in detailMenus)
        {
            detailMenu.UnDeployment();
        }
    }
}
