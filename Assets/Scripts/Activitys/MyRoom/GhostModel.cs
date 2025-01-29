using System.Collections.Generic;
using UnityEngine;

public interface IEditOnlyGhost
{
    /// <summary>
    /// �ݒu�\�X�e�[�g�ɉ����Č����ڂ�A��������
    /// </summary>
    /// <param name="canPlace">�ݒu�\���ǂ���</param>
    void SetPlaceableState(bool canPlace);
    /// <summary>
    /// Ghost�̐F��ύX����
    /// </summary>
    /// <param name="color">�F</param>
    void ChangeColor(Color color);
}

/// <summary>
/// Ghost Shader��K�p�������f���𐶐�����B��Ƀn�E�W���O�Ŏg�p����
/// </summary>
public class GhostModel : IEditOnlyGhost
{
    /// <summary>
    /// �S�[�X�g���f���̌��_�̈ʒu
    /// </summary>
    public enum PivotType
    {
        Under,
        Center,
    }

    /// <summary>
    /// �z�u�`��
    /// </summary>
    public enum PlacingStyle
    {
        /// <summary>
        /// ��
        /// </summary>
        Ground,
        /// <summary>
        /// �Ǌ|��
        /// </summary>
        Wall,
        /// <summary>
        /// �I��
        /// </summary>
        Shelf,
    }

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
    public GhostModel CreateModelSimple(GameObject ghostOrigin, Color? defaultColor = null)
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
            // ���X���Ă��������AMaterial���쐬
            var ghostMaterials = new List<Material>();
            for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
            {
                ghostMaterials.Add(material);
            }

            renderer.SetMaterials(ghostMaterials);
            defaultColor ??= correctColor;

            for (int k = 0; k < renderers[i].sharedMaterials.Length; k++)
            {
                // �쐬����Material�͂��ׂ�Ghost�����A���g��Texture��ς���
                renderer.materials[k].SetTexture(TEXTURE_NAME, renderers[i].sharedMaterials[k].mainTexture);
                renderer.materials[k].SetColor(COLOR_NAME, (Color)defaultColor);
            }
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
        // �ꕔ���f����bounds�̐����Ɏ��s�����ꍇ�̃n���h�����O
        // mesh.bounds����center�������P�[�X�����������߁Arenderer.bounds�őΉ�
        if (bounds.extents == Vector3.zero && combineInstances.Length == 1)
        {
            bounds = renderers[0].bounds;
        }

        boxCollider.size = bounds.size;
        boxCollider.center = bounds.center;
        if (boxCollider.size == Vector3.zero && boxCollider.center == Vector3.zero)
        {
            Debug.LogError("PlaceableObject ��Mesh���ɃA�N�Z�X�ł��܂���B" +
                "ModelImporter��Read/Write�ݒ�𒼐�ON�ɂ��邩�AModelSettingChanger Window ���g�p���Đݒ��ύX���Ă��������B");
        }
        return this;
    }

    /// <summary>
    /// �n���ꂽGameObject��GhostModel�𐶐����A�ݒu�@�\�i�n�E�W���O�@�\�j��ǉ�����B�\���̑O�ɕK���s���K�v������B
    /// <br>���\������邾���ł͂Ȃ��A�ݒu�\����Ȃǂ��s����悤�ɂȂ�</br>
    /// </summary>
    /// <param name="placeableObject"></param>
    /// <param name="player">�Ǐ]�Ώۂ̃I�u�W�F�N�g�i�����̏ꍇ�v���C���[�j</param>
    /// <param name="defaultColor"></param>
    public GhostModel CreateModel(PlaceableObject placeableObject, Transform player, Color? defaultColor = null)
    {
        CreateModelSimple(placeableObject.GhostOrigin, defaultColor);
        switch (placeableObject.PlacingStyle)
        {
            case PlacingStyle.Ground:
                instance.AddComponent<PlacingTarget>().Initialize(this, placeableObject, player);
                break;

            case PlacingStyle.Wall:
                instance.AddComponent<PlacingTarget_Wall>().Initialize(this, placeableObject, player);
                break;

            case PlacingStyle.Shelf:
                instance.AddComponent<PlacingTarget_Shelf>().Initialize(this, placeableObject, player);
                break;
        }

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
            foreach (var material in renderer.materials)
            {
                material.SetColor(COLOR_NAME, color);
            }
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
            $"������ɂ� {nameof(CreateModelSimple)} �ł͂Ȃ� {nameof(CreateModel)} �����s���Ă��������B");
    }
}