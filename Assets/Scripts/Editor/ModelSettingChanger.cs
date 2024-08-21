using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.Linq;
using HCSMeta.Activity;

public class ModelSettingChanger : EditorWindow
{
    private enum WindowMode
    {
        Serach,
        Execute,
    }

    private SerializedObject target = default;
    private List<ModelImporter> modelImporters = default;
    private List<string> targetsPath = default;
    private WindowMode mode = WindowMode.Serach;
    private Vector2 scrollPosition = default;


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
        target ??= new SerializedObject(this);
    }

    private void OnGUI()
    {
        target.Update();


        if (mode == WindowMode.Serach)
        {
            if (GUILayout.Button("Serach"))
            {
                var assetPaths = AssetDatabase.FindAssets($"t:Prefab")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .ToArray();

                modelImporters = new List<ModelImporter>();
                targetsPath = new List<string>();
                foreach (var path in assetPaths)
                {
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
                        }
                    }
                }

                mode = WindowMode.Execute;
            }
        }
        else
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var path in targetsPath)
            {
                GUILayout.Label(path);
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Yes"))
            {
                mode = WindowMode.Serach;
            }
        }

        target.ApplyModifiedProperties();
    }

    //private void SetModelReadWrite()
    //{
    //    var modelImporter = assetImporter as ModelImporter;
    //    modelImporter.isReadable = true;
    //    modelImporter.SaveAndReimport();
    //}
}
