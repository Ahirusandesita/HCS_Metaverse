using UnityEngine;

/// <summary>
/// Ghost Shader��K�p�������f���𐶐�����B��Ƀn�E�W���O�Ŏg�p����
/// </summary>
public class GhostModelManager
{
    private const string MATERIAL_NAME = "Ghost";
    private const string TEXTURE_NAME = "_Texture";
    private const string COLOR_NAME = "_Ghost_Color";

    private readonly Color32 trueColor = new Color32(15, 255, 31, 255);
    private readonly Color32 falseColor = new Color32(255, 31, 15, 255);

    private GameObject instance = default;
    private BoxCollider boxCollider = default;
    private Material material = default;


    public GhostModelManager()
    {
        instance = new GameObject(nameof(GhostModelManager));
        boxCollider = instance.AddComponent<BoxCollider>();
        // �uGhost�vMaterial�����[�h
        material = Resources.Load<Material>(MATERIAL_NAME);

        instance.SetActive(false);
        boxCollider.isTrigger = true;
    }

    /// <summary>
    /// �n���ꂽGameObject��GhostModel�𐶐�����B�\���̑O�ɕK���s���K�v������B
    /// </summary>
    /// <param name="ghostOrigin"></param>
    /// <param name="defaultColor"></param>
    public GhostModelManager CreateModel(GameObject ghostOrigin, Color? defaultColor = null)
    {
        var filters = ghostOrigin.GetComponentsInChildren<MeshFilter>();
        var renderers = ghostOrigin.GetComponentsInChildren<MeshRenderer>();

        // MeshFilter���t���Ă��Ȃ��I�u�W�F�N�g�͗�O�𓊂���
        if (filters.Length == 0)
        {
            throw new System.ArgumentException($"Mesh�����݂��Ȃ��I�u�W�F�N�g���� {nameof(GhostModelManager)} �ɃA�N�Z�X���悤�Ƃ��Ă��܂��B", nameof(ghostOrigin));
        }

        // Mesh�����̂��߂̍\����
        var combineInstances = new CombineInstance[filters.Length];

        for (int i = 0; i < combineInstances.Length; i++)
        {
            // CombineInstance�\���̂Ƀf�[�^���Z�b�g -----------------------------------------
            combineInstances[i].mesh = filters[i].sharedMesh;
            var transform = filters[i].transform;
            // Transform��񂩂�Matrix4x4�^�ɕϊ�����iScale�͐��Scale��p����j
            combineInstances[i].transform = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            // ----------------------------------------------------------------------------

            // �����ڂ�\������I�u�W�F�N�g�𐶐� ----------------------------------------------
            var child = new GameObject($"GhostModel ({i})");
            child.transform.SetParent(instance.transform);
            var filter = child.AddComponent<MeshFilter>();
            var renderer = child.AddComponent<MeshRenderer>();
            // ----------------------------------------------------------------------------

            // �����ڂ�ݒ� -----------------------------------------------------------------
            filter.sharedMesh = filters[i].sharedMesh;
            renderer.material = material;
            var texture = renderers[i].material.mainTexture;
            renderer.material.SetTexture(TEXTURE_NAME, texture);
            // �����J���[
            if (defaultColor is null)
            {
                renderer.material.SetColor(COLOR_NAME, trueColor);
            }
            else
            {
                renderer.material.SetColor(COLOR_NAME, (Color)defaultColor);
            }
            // -----------------------------------------------------------------------------

            // Transform�����R�s�[�iScale�͐��Scale��p����j
            child.transform.SetPositionAndRotation(filters[i].transform.position, filters[i].transform.rotation);
            child.transform.localScale = filters[i].transform.lossyScale;
        }

        // Mesh����
        // BoxCollider�̂��߂�Bounds�ł̂ݎg�p����̂ŁA�V�������ꂽMesh���͔̂j��
        var mesh = new Mesh();
        mesh.CombineMeshes(combineInstances, true);
        var bounds = mesh.bounds;

        boxCollider.size = bounds.size;
        boxCollider.center = bounds.center;
        return this;
    }

    public void Spawn()
    {
        if (instance is null)
        {
            return;
        }
        instance.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Despawn()
    {
        if (instance is null)
        {
            return;
        }
        instance.SetActive(false);
    }

    public void DisposeModel()
    {
        Object.Destroy(instance);
        instance = null;
        boxCollider = null;
        material = null;
    }

    public void ChangeColor(bool condition)
    {
        var color = condition ? trueColor : falseColor;
        var renderers = instance.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.material.SetColor(COLOR_NAME, color);
        }
    }

    public void ChangeColor(Color color)
    {
        var renderers = instance.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.material.SetColor(COLOR_NAME, color);
        }
    }
}