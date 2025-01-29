using System.Collections.Generic;
using UnityEngine;

public interface IEditOnlyGhost
{
    /// <summary>
    /// 設置可能ステートに応じて見た目を連動させる
    /// </summary>
    /// <param name="canPlace">設置可能かどうか</param>
    void SetPlaceableState(bool canPlace);
    /// <summary>
    /// Ghostの色を変更する
    /// </summary>
    /// <param name="color">色</param>
    void ChangeColor(Color color);
}

/// <summary>
/// Ghost Shaderを適用したモデルを生成する。主にハウジングで使用する
/// </summary>
public class GhostModel : IEditOnlyGhost
{
    /// <summary>
    /// ゴーストモデルの原点の位置
    /// </summary>
    public enum PivotType
    {
        Under,
        Center,
    }

    /// <summary>
    /// 配置形式
    /// </summary>
    public enum PlacingStyle
    {
        /// <summary>
        /// 床
        /// </summary>
        Ground,
        /// <summary>
        /// 壁掛け
        /// </summary>
        Wall,
        /// <summary>
        /// 棚板
        /// </summary>
        Shelf,
    }

    private const string MATERIAL_NAME = "Ghost";
    private const string TEXTURE_NAME = "_Texture";
    private const string COLOR_NAME = "_Ghost_Color";

    // 主にハウジングで使用するため、「置ける/置けない」のような対となる2色を用意
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
        // 「Ghost」Materialをロード
        material = Resources.Load<Material>(MATERIAL_NAME);

        instance.SetActive(false);
        boxCollider.isTrigger = true;
        rigidbody.isKinematic = true;
    }

    /// <summary>
    /// 渡されたGameObjectのGhostModelを生成する。表示の前に必ず行う必要がある。
    /// </summary>
    /// <param name="ghostOrigin"></param>
    /// <param name="defaultColor"></param>
    public GhostModel CreateModelSimple(GameObject ghostOrigin, Color? defaultColor = null)
    {
        var filters = ghostOrigin.GetComponentsInChildren<MeshFilter>();
        var renderers = ghostOrigin.GetComponentsInChildren<MeshRenderer>();

        // MeshFilterが付いていないオブジェクトは例外を投げる
        if (filters.Length == 0)
        {
            throw new System.ArgumentException($"Meshが存在しないオブジェクトから {nameof(GhostModel)} にアクセスしようとしています。", nameof(ghostOrigin));
        }

        // Mesh結合のための構造体
        var combineInstances = new CombineInstance[filters.Length];

        for (int i = 0; i < combineInstances.Length; i++)
        {
            // CombineInstance構造体にデータをセット -----------------------------------------
            combineInstances[i].mesh = filters[i].sharedMesh;
            var transform = filters[i].transform;
            // Transform情報からMatrix4x4型に変換する（Scaleは絶対Scaleを用いる）
            combineInstances[i].transform = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            // ----------------------------------------------------------------------------

            // Meshを表示するオブジェクトを生成 ----------------------------------------------
            var child = new GameObject($"GhostModel ({i})");
            child.transform.SetParent(instance.transform);
            var filter = child.AddComponent<MeshFilter>();
            var renderer = child.AddComponent<MeshRenderer>();
            // ----------------------------------------------------------------------------

            // 見た目を設定 -----------------------------------------------------------------
            filter.sharedMesh = filters[i].sharedMesh;
            // 元々ついていた数分、Materialを作成
            var ghostMaterials = new List<Material>();
            for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
            {
                ghostMaterials.Add(material);
            }

            renderer.SetMaterials(ghostMaterials);
            defaultColor ??= correctColor;

            for (int k = 0; k < renderers[i].sharedMaterials.Length; k++)
            {
                // 作成するMaterialはすべてGhostだが、中身のTextureを変える
                renderer.materials[k].SetTexture(TEXTURE_NAME, renderers[i].sharedMaterials[k].mainTexture);
                renderer.materials[k].SetColor(COLOR_NAME, (Color)defaultColor);
            }
            // -----------------------------------------------------------------------------

            // Transform情報をコピー（Scaleは絶対Scaleを用いる）
            child.transform.SetPositionAndRotation(filters[i].transform.position, filters[i].transform.rotation);
            child.transform.localScale = filters[i].transform.lossyScale;
        }

        // Mesh結合
        // BoxColliderのためのBoundsでのみ使用するので、新しく作られたMesh自体は破棄
        // 結合したMeshを使ってもよいが、MeshRendererは元のモデルと同じ数用意したい（Textureが違う可能性を考慮）ので、
        // MeshFilterも同じく元モデルと同じように使用している
        var mesh = new Mesh();
        mesh.CombineMeshes(combineInstances, true);
        var bounds = mesh.bounds;
        // 一部モデルのboundsの生成に失敗した場合のハンドリング
        // mesh.boundsだとcenterがずれるケースがあったため、renderer.boundsで対応
        if (bounds.extents == Vector3.zero && combineInstances.Length == 1)
        {
            bounds = renderers[0].bounds;
        }

        boxCollider.size = bounds.size;
        boxCollider.center = bounds.center;
        if (boxCollider.size == Vector3.zero && boxCollider.center == Vector3.zero)
        {
            Debug.LogError("PlaceableObject のMesh情報にアクセスできません。" +
                "ModelImporterのRead/Write設定を直接ONにするか、ModelSettingChanger Window を使用して設定を変更してください。");
        }
        return this;
    }

    /// <summary>
    /// 渡されたGameObjectのGhostModelを生成し、設置機能（ハウジング機能）を追加する。表示の前に必ず行う必要がある。
    /// <br>※表示されるだけではなく、設置可能判定などが行われるようになる</br>
    /// </summary>
    /// <param name="placeableObject"></param>
    /// <param name="player">追従対象のオブジェクト（多くの場合プレイヤー）</param>
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
    /// モデルが存在する場合に表示する
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
    /// モデルが存在する場合に非表示にする
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
    /// オブジェクトを破棄する。呼び出し後、このインスタンスは使用不可となる
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
        throw new System.NotSupportedException($"{nameof(GhostModel)} インスタンスで配置機能が許可されていません。" +
            $"許可するには {nameof(CreateModelSimple)} ではなく {nameof(CreateModel)} を実行してください。");
    }
}