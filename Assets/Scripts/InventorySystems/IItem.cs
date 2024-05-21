using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AppearanceInfo_Mesh
{
    private Mesh mesh;
    private Material material;
    private Vector3 size;

    public Mesh Mesh => mesh;
    public Material Material => material;

    public AppearanceInfo_Mesh(Mesh mesh, Material material)
    {
        this.mesh = mesh;
        this.material = material;
        size = new Vector3(0.6f, 0.6f, 0.6f);
    }
    public AppearanceInfo_Mesh(Mesh mesh, Material material, Vector3 size)
    {
        this.mesh = mesh;
        this.material = material;
        this.size = size;
    }
}
public interface IItem
{
    /// <summary>
    /// アイテム使用
    /// </summary>
    void Use();
    /// <summary>
    /// アイテムをしまう
    /// </summary>
    void CleanUp();

    void TakeOut(Vector3 position);
}
public interface IInventory_Mesh
{
    AppearanceInfo_Mesh Appearance();
}
public interface IInventory
{
    bool HasItem { get; }
    void PutAway(IItem item);
    IItem TakeOut();
}