using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRInput;
using UnityEditor.AssetImporters;
public class RadialInput : MonoBehaviour, IInputControllable
{
    private Vector2 stickInput;
    private RadialMenu[] radialMenus;
    private RadialMenu radialMenu;
    [SerializeField]
    private GameObject emit;

    private void Start()
    {
        Inputter.Player.Animation.performed += _ =>
        {
            Inputter.ChangeInputPreset(this, Inputter.InputActionPreset.Animation);
        };
        Inputter.Player.Animation.canceled += _ =>
        {
            Inputter.ChangeInputPreset(this, Inputter.InputActionPreset.Default);
        };

        Inputter.Animation.RadialCursor.performed += anpanman =>
        {
            stickInput = anpanman.ReadValue<Vector2>();
        };
        Inputter.Animation.RadialCursor.canceled += anpanman =>
        {
            stickInput = Vector2.zero;
        };
    }

    public void InjectRadialMenu(RadialMenu[] radialMenus)
    {
        this.radialMenus = radialMenus;
    }



    private void Update()
    {
        if (stickInput.x == 0f && stickInput.y == 0f)
        {
            if (radialMenu == null)
            {
                return;
            }

            radialMenu.Select();
            radialMenu.UnHover();
            radialMenu = null;
            emit.SetActive(false);
            return;
        }
        emit.SetActive(true);
        float angle = Mathf.Atan2(stickInput.y, stickInput.x) * Mathf.Rad2Deg;
        angle -= 65;

        if (angle < 0)
        {
            angle = 360f - Mathf.Abs(angle);
        }

        Debug.LogError(angle);
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, angle));
        RadialMenu workRadialMenu = radialMenus.AngleMatch(angle);
        if (radialMenu == workRadialMenu)
        {
            workRadialMenu.Hover();
        }
        else
        {
            if (radialMenu != null)
            {
                radialMenu.UnHover();
            }
        }
        radialMenu = workRadialMenu;
    }
}
