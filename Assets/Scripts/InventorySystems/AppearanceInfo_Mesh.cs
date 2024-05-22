using UnityEngine;
/// <summary>
/// 表示するためのMesh情報とサイズ
/// </summary>
public class AppearanceInfo_Mesh
{
    private Mesh mesh;
    private Material material;
    private Vector3 size;

    public Mesh Mesh => mesh;
    public Material Material => material;
    public Vector3 Size => size;

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