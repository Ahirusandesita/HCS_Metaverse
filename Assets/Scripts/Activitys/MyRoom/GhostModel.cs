using UnityEngine;

public interface IEditOnlyGhost
{
    void SetPlaceableState(bool canPlace);
    void ChangeColor(Color color);
}

/// <summary>
/// Ghost Shader��K�p�������f���𐶐�����B��Ƀn�E�W���O�Ŏg�p����
/// </summary>
public class GhostModel : IEditOnlyGhost
{
    private const string MATERIAL_NAME = "Ghost";
    private const string TEXTURE_NAME = "_Texture";
    private const string COLOR_NAME = "_Ghost_Color";

    // ��Ƀn�E�W���O�Ŏg�p���邽�߁A�u�u����/�u���Ȃ��v�̂悤�ȑ΂ƂȂ�2�F��p��
    private readonly Color32 correctColor = new Color32(15, 255, 31, 255);
    private readonly Color32 incorrectColor = new Color32(255, 31, 15, 255);

    private GameObject instance = default;
    private BoxCollider boxCollider = default;
    private Material material = default;

    private bool enablePlacingFunction = false;
    private bool canPlace = false;


    public bool CanPlace
    {
        get
        {
            if (!enablePlacingFunction)
            {
                ThrowException();
                return false;
            }

            return canPlace;
        }
    }


    public GhostModel()
    {
        instance = new GameObject(nameof(GhostModel));
        boxCollider = instance.AddComponent<BoxCollider>();
        var rigidbody = instance.AddComponent<Rigidbody>();
        // �uGhost�vMaterial�����[�h
        material = Resources.Load<Material>(MATERIAL_NAME);

        instance.SetActive(false);
        boxCollider.isTrigger = true;
        rigidbody.isKinematic = true;
    }

    /// <summary>
    /// �n���ꂽGameObject��GhostModel�𐶐�����B�\���̑O�ɕK���s���K�v������B
    /// </summary>
    /// <param name="ghostOrigin"></param>
    /// <param name="defaultColor"></param>
    public GhostModel CreateModel(GameObject ghostOrigin, Color? defaultColor = null)
    {
        var filters = ghostOrigin.GetComponentsInChildren<MeshFilter>();
        var renderers = ghostOrigin.GetComponentsInChildren<MeshRenderer>();

        // MeshFilter���t���Ă��Ȃ��I�u�W�F�N�g�͗�O�𓊂���
        if (filters.Length == 0)
        {
            throw new System.ArgumentException($"Mesh�����݂��Ȃ��I�u�W�F�N�g���� {nameof(GhostModel)} �ɃA�N�Z�X���悤�Ƃ��Ă��܂��B", nameof(ghostOrigin));
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

            // Mesh��\������I�u�W�F�N�g�𐶐� ----------------------------------------------
            var child = new GameObject($"GhostModel ({i})");
            child.transform.SetParent(instance.transform);
            var filter = child.AddComponent<MeshFilter>();
            var renderer = child.AddComponent<MeshRenderer>();
            // ----------------------------------------------------------------------------

            // �����ڂ�ݒ� -----------------------------------------------------------------
            filter.sharedMesh = filters[i].sharedMesh;
            renderer.material = material;
            var texture = renderers[i].sharedMaterial.mainTexture;
            renderer.material.SetTexture(TEXTURE_NAME, texture);

            defaultColor ??= correctColor;
            renderer.material.SetColor(COLOR_NAME, (Color)defaultColor);
            // -----------------------------------------------------------------------------

            // Transform�����R�s�[�iScale�͐��Scale��p����j
            child.transform.SetPositionAndRotation(filters[i].transform.position, filters[i].transform.rotation);
            child.transform.localScale = filters[i].transform.lossyScale;
        }

        // Mesh����
        // BoxCollider�̂��߂�Bounds�ł̂ݎg�p����̂ŁA�V�������ꂽMesh���͔̂j��
        // ��������Mesh���g���Ă��悢���AMeshRenderer�͌��̃��f���Ɠ������p�ӂ������iTexture���Ⴄ�\�����l���j�̂ŁA
        // MeshFilter�������������f���Ɠ����悤�Ɏg�p���Ă���
        var mesh = new Mesh();
        mesh.CombineMeshes(combineInstances, true);
        var bounds = mesh.bounds;

        boxCollider.size = bounds.size;
        boxCollider.center = bounds.center;
        return this;
    }

    /// <summary>
    /// GhostModel�ɐݒu�@�\�i�n�E�W���O�@�\�j��ǉ�����
    /// <br>���\������邾���ł͂Ȃ��A�ݒu�\����Ȃǂ��s����悤�ɂȂ�</br>
    /// </summary>
    /// <param name="parent">�Ǐ]�Ώۂ̃I�u�W�F�N�g�i�����̏ꍇ�v���C���[�j</param>
    /// <returns></returns>
    public GhostModel AddPlacingFunction(Transform player)
    {
        instance.AddComponent<PlacingTarget>().Initialize(this, player);
        enablePlacingFunction = true;
        return this;
    }

    public Vector3 GetGhostPosition()
    {
        return instance.transform.position;
    }

    public Quaternion GetGhostRotation()
    {
        return instance.transform.rotation;
    }

    /// <summary>
    /// ���f�������݂���ꍇ�ɕ\������
    /// </summary>
    public void Spawn()
    {
        if (instance is null)
        {
            return;
        }
        instance.SetActive(true);
    }

    /// <summary>
    /// ���f�������݂���ꍇ�ɔ�\���ɂ���
    /// </summary>
    public void Despawn()
    {
        if (instance is null)
        {
            return;
        }
        instance.SetActive(false);
    }

    /// <summary>
    /// �I�u�W�F�N�g��j������B�Ăяo����A���̃C���X�^���X�͎g�p�s�ƂȂ�
    /// </summary>
    public void DisposeModel()
    {
        Object.Destroy(instance);
        instance = null;
        boxCollider = null;
        material = null;
    }

    public void ChangeColor(Color color)
    {
        var renderers = instance.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.material.SetColor(COLOR_NAME, color);
        }
    }

    void IEditOnlyGhost.SetPlaceableState(bool canPlace)
    {
        if (!enablePlacingFunction)
        {
            ThrowException();
            return;
        }

        this.canPlace = canPlace;
        var color = canPlace ? correctColor : incorrectColor;
        ChangeColor(color);
    }

    private void ThrowException()
    {
        throw new System.NotSupportedException($"{nameof(GhostModel)} �C���X�^���X�Ŕz�u�@�\��������Ă��܂���B" +
            $"������ɂ� {nameof(AddPlacingFunction)} �����s���Ă��������B");
    }
}