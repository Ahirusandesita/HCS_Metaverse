using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CombineMeshesEditor : MonoBehaviour
{
    // エディタメニューに「Tools/Combine Selected Meshes」を追加
    [MenuItem("Tools/Combine Selected Meshes")]
    static void CombineSelectedMeshes()
    {
        // シーン内で選択されているオブジェクトを取得
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("オブジェクトが選択されていません。");
            return;
        }

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        Material sharedMaterial = null;

        // 選択された各オブジェクトからメッシュを取得
        foreach (GameObject go in selectedObjects)
        {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mf == null || mr == null)
            {
                Debug.LogWarning($"オブジェクト {go.name} に MeshFilter または MeshRenderer がありません。スキップします。");
                continue;
            }

            // 1つ目のオブジェクトのマテリアルを使用（全て同じマテリアルであることを前提）
            if (sharedMaterial == null)
            {
                sharedMaterial = mr.sharedMaterial;
            }

            // CombineInstance を作成
            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            // ローカルからワールドへの変換行列を指定
            ci.transform = mf.transform.localToWorldMatrix;
            combineInstances.Add(ci);
        }

        if (combineInstances.Count == 0)
        {
            Debug.LogWarning("結合可能なメッシュが見つかりませんでした。");
            return;
        }

        // CombineInstances 配列から新しいメッシュを作成
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "CombinedMesh";
        combinedMesh.CombineMeshes(combineInstances.ToArray());

        // 新しい GameObject を作成し、MeshFilter と MeshRenderer を追加
        GameObject combinedObj = new GameObject("CombinedMesh");
        MeshFilter combinedMF = combinedObj.AddComponent<MeshFilter>();
        combinedMF.mesh = combinedMesh;
        MeshRenderer combinedMR = combinedObj.AddComponent<MeshRenderer>();
        combinedMR.sharedMaterial = sharedMaterial;

        // オプション: 元のオブジェクトを無効化する
        foreach (GameObject go in selectedObjects)
        {
            go.SetActive(false);
        }

        Debug.Log($"結合が完了しました。{combineInstances.Count} 個のメッシュを１つに統合しました。");
    }
}