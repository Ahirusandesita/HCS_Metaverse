using UnityEngine;
public class NotExistMaterial : MonoBehaviour, IInventoryRetractable
{
    private AppearanceInfo_Mesh appearanceInfo_Mesh;

    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private MeshRenderer meshRenderer;
    private void Awake()
    {
        appearanceInfo_Mesh = new AppearanceInfo_Mesh(
            meshFilter.mesh,
            meshRenderer.materials,
            new Vector3(2250f, 2250f, 2250f)
        ); ;


        this.gameObject.SetActive(false);
    }
    AppearanceInfo_Mesh IInventoryRetractable.Appearance()
    {
        return appearanceInfo_Mesh;
    }
}
