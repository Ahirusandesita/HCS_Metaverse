using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ModelImporterによってMeshの設定を変更するWindow
/// </summary>
public class ModelSettingChanger : EditorWindow
{
    private enum WindowMode
    {
        Serach,
        Execute,
    }

    private List<ModelImporter> modelImporters = default;
    private List<string> targetsPath = default;
    private List<string> meshesPath = default;
    private WindowMode mode = WindowMode.Serach;
    private Vector2 scrollPosition = Vector2.zero;
    private bool showDetails = false;
    private GUIStyle style = default;


    [MenuItem("Window/Model Setting Changer")]
    [MenuItem("Initialize/Model Setting Changer")]
    public static void OpenWindow()
    {
        var window = GetWindow<ModelSettingChanger>();
        window.titleContent = new GUIContent("Model Setting Changer");
        window.Show();
    }

    private void OnEnable()
    {
        style = new GUIStyle { richText = true, fontSize = 16 };
    }

    private void OnGUI()
    {
        switch (mode)
        {
            case WindowMode.Serach:
                EditorGUILayout.Space(16f);
                GUILayout.Label("<b><color=#f5f5f5>Outlineを表示させるPrefab（OutlineManagerがアタッチされているPrefab）を取得し、\n" +
                    "そのPrefabが使用しているMeshのModelSettingを一括で変更します。\n" +
                    "これにより、Outlineが表示されない問題が解消されます。</color></b>", style);
                EditorGUILayout.Space(16f);

                if (GUILayout.Button("Serach"))
                {
                    // すべてのPrefabのAssetPathを取得
                    var assetPaths = AssetDatabase.FindAssets($"t:Prefab")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .ToArray();

                    modelImporters = new List<ModelImporter>();
                    targetsPath = new List<string>();
                    meshesPath = new List<string>();
                    foreach (var path in assetPaths)
                    {
                        // AssetPathからGameObjectを取得する。
                        // そのGameObjectがOutlineを実装しているのならば、使用しているMeshを取得しておく。
                        var loadAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        if (loadAsset.TryGetComponent(out OutlineManager _) || loadAsset.TryGetComponent(out Outline _))
                        {
                            var mesh = loadAsset.GetComponentsInChildren<MeshFilter>().First().sharedMesh;
                            var meshPath = AssetDatabase.GetAssetPath(mesh);
                            var assetImporter = AssetImporter.GetAtPath(meshPath);
                            if (assetImporter is ModelImporter modelImporter)
                            {
                                modelImporters.Add(modelImporter);
                                targetsPath.Add(path);
                                meshesPath.Add(meshPath);
                            }
                        }
                    }

                    mode = WindowMode.Execute;
                }
                break;

            case WindowMode.Execute:
                EditorGUILayout.Space(4f);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                // ModelSettingを変更する対象のオブジェクトをLabel表示する
                if (showDetails)
                {
                    foreach (var path in meshesPath)
                    {
                        GUILayout.Label(path);
                    }
                }
                else
                {
                    foreach (var path in targetsPath)
                    {
                        GUILayout.Label(path);
                    }
                }

                EditorGUILayout.Space(4f);
                showDetails = EditorGUILayout.Toggle("Show Mesh Path", showDetails);
                EditorGUILayout.Space(8f);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Yes"))
                {
                    // 設定を変更し、再インポート
                    foreach (var model in modelImporters)
                    {
                        model.isReadable = true;
                        model.SaveAndReimport();
                    }
                    mode = WindowMode.Serach;
                    XDebug.Log("Completed: Model Setting Changer!");
                }
                if (GUILayout.Button("No"))
                {
                    mode = WindowMode.Serach;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
                break;
        }
    }
}
