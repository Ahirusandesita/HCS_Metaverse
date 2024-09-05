using UnityEngine;

/// <summary>
/// Ghost Shaderを適用したモデルを生成する。主にハウジングで使用する
/// </summary>
public class GhostModel
{
    private const string MATERIAL_NAME = "Ghost";
    private const string TEXTURE_NAME = "_Texture";
    private const string COLOR_NAME = "_Ghost_Color";

    // 主にハウジングで使用するため、「置ける/置けない」のような対となる2色を用意
    private readonly Color32 correctColor = new Color32(15, 255, 31, 255);
    private readonly Color32 incorrectColor = new Color32(255, 31, 15, 255);

    private GameObject instance = default;
    private BoxCollider boxCollider = default;
    private Material material = default;


    public GhostModel()
    {
        instance = new GameObject(nameof(GhostModel));
        boxCollider = instance.AddComponent<BoxCollider>();
        // 「Ghost」Materialをロード
        material = Resources.Load<Material>(MATERIAL_NAME);

        instance.SetActive(false);
        boxCollider.isTrigger = true;
    }

    /// <summary>
    /// 渡されたGameObjectのGhostModelを生成する。表示の前に必ず行う必要がある。
    /// </summary>
    /// <param name="ghostOrigin"></param>
    /// <param name="defaultColor"></param>
    public GhostModel CreateModel(GameObject ghostOrigin, Color? defaultColor = null)
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
            renderer.material = material;
            var texture = renderers[i].sharedMaterial.mainTexture;
            renderer.material.SetTexture(TEXTURE_NAME, texture);
            // 初期カラー
            if (defaultColor is null)
            {
                renderer.material.SetColor(COLOR_NAME, correctColor);
            }
            else
            {
                renderer.material.SetColor(COLOR_NAME, (Color)defaultColor);
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

        boxCollider.size = bounds.size;
        boxCollider.center = bounds.center;
        return this;
    }

    public GhostModel SetParent(Transform parent)
    {
        instance.transform.SetParent(parent);
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

    public void ChangeColor(bool condition)
    {
        var color = condition ? correctColor : incorrectColor;
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