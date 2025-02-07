using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct AngleRange
{
    public readonly float startAngle;
    public readonly float endAngle;
    public AngleRange(float startAngle, float endAngle)
    {
        this.startAngle = startAngle;
        this.endAngle = endAngle;
    }
}
public class RadialMenu : MonoBehaviour
{
    public AngleRange AngleRange { get; set; }

    public event Action<int, string> OnSelect;
    private int id;
    private string prefabName;
    private Vector3 localScale;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        this.localScale = this.transform.localScale;
    }
    public void Inject(int id, string name)
    {
        this.id = id;
        this.prefabName = name;
    }
    public void InjectSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
    public void Select()
    {
        OnSelect?.Invoke(id, prefabName);
    }

    public void Hover()
    {
        this.transform.localScale = new Vector3(localScale.x + (localScale.x * 0.1f), localScale.y + (localScale.y * 0.1f), localScale.z + (localScale.z * 0.1f));
    }
    public void UnHover()
    {
        transform.localScale = localScale;
    }
}
public static class RadialMenuExtends
{
    public static RadialMenu AngleMatch(this RadialMenu[] radialMenus, float angle)
    {
        for (int i = 0; i < radialMenus.Length; i++)
        {
            if (radialMenus[i].AngleRange.startAngle <= angle && radialMenus[i].AngleRange.endAngle > angle)
            {
                return radialMenus[i];
            }
        }

        return null;
    }
}
