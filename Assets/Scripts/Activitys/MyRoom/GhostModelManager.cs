using UnityEngine;

/// <summary>
/// Ghost Shaderを適用したモデルを生成する。主にハウジングで使用する
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
    public GhostModelManager CreateModel(GameObject ghostOrigin, Color? defaultColor = null)
    {
        var filters = ghostOrigin.GetComponentsInChildren<MeshFilter>();
        var renderers = ghostOrigin.GetComponentsInChildren<MeshRenderer>();

        // MeshFilterが付いていないオブジェクトは例外を投げる
        if (filters.Length == 0)
        {
            throw new System.ArgumentException($"Meshが存在しないオブジェクトから {nameof(GhostModelManager)} にアクセスしようとしています。", nameof(ghostOrigin));
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

            // 見た目を表示するオブジェクトを生成 ----------------------------------------------
            var child = new GameObject($"GhostModel ({i})");
            child.transform.SetParent(instance.transform);
            var filter = child.AddComponent<MeshFilter>();
            var renderer = child.AddComponent<MeshRenderer>();
            // ----------------------------------------------------------------------------

            // 見た目を設定 -----------------------------------------------------------------
            filter.sharedMesh = filters[i].sharedMesh;
            renderer.material = material;
            var texture = renderers[i].material.mainTexture;
            renderer.material.SetTexture(TEXTURE_NAME, texture);
            // 初期カラー
            if (defaultColor is null)
            {
                renderer.material.SetColor(COLOR_NAME, trueColor);
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