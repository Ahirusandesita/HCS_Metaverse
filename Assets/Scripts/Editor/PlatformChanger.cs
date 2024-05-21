using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class PlatformChanger : EditorWindow
{
    private enum BuildType
    {
        Android,
        Windows,
    }

    [Serializable]
    private class BuildObjectData
    {
        [PrefabField]
        public GameObject prefab = default;
        public Transform parent = default;
        public SceneAsset scene = default;
    }

    private static readonly GUIContent androidLabel = new GUIContent("Android Build Prefabs");
    private static readonly GUIContent windowsLabel = new GUIContent("Windows Build Prefabs");

    [SerializeField]
    private List<BuildObjectData> androidBuildObjectData = default;
    [SerializeField]
    private List<BuildObjectData> windowsBuildObjectData = default;
    private SerializedObject target = default;
    private BuildType currentBuildType = default;

    private readonly List<Object> instantiatedObjects = new List<Object>();
    private readonly GUIStyle style = new GUIStyle();


    [MenuItem("Window/Platform Changer")]
    public static void OpenWindow()
    {
        var window = GetWindow<PlatformChanger>();
        window.titleContent = androidLabel;
        window.Show();
    }

    private void OnEnable()
    {
        target = new SerializedObject(this);
        style.richText = true;
        style.fontSize = 14;
    }

    private void OnGUI()
    {
        target.Update();
        EditorGUILayout.Space(16);

        var androidProperty = target.FindProperty(nameof(androidBuildObjectData));
        EditorGUILayout.PropertyField(androidProperty, androidLabel);

        EditorGUILayout.Space(16);

        var windowsProperty = target.FindProperty(nameof(windowsBuildObjectData));
        EditorGUILayout.PropertyField(windowsProperty, windowsLabel);

        EditorGUILayout.Space(16);
        EditorGUILayout.LabelField($"<color=white>Current Platform: <b>{currentBuildType}</b></color>", style);
        EditorGUILayout.Space(2);

        if (currentBuildType == BuildType.Android)
        {
            if (GUILayout.Button("Change Platform to Windows"))
            {
                currentBuildType = BuildType.Windows;

                foreach (var obj in instantiatedObjects)
                {
                    DestroyImmediate(obj);
                }

                instantiatedObjects.Clear();

                foreach (var buildObject in windowsBuildObjectData)
                {
                    if (buildObject.scene.name != SceneManager.GetActiveScene().name)
                    {
                        Debug.Log($"現在のSceneが、{nameof(PlatformChanger)}のプロパティに設定されたSceneと違うため、生成がスキップされました。");
                        return;
                    }

                    if (buildObject.parent is null)
                    {
                        instantiatedObjects.Add(PrefabUtility.InstantiatePrefab(buildObject.prefab));
                    }
                    else
                    {
                        instantiatedObjects.Add(PrefabUtility.InstantiatePrefab(buildObject.prefab, buildObject.parent));
                    }
                }
            }
        }
        else if (currentBuildType == BuildType.Windows)
        {
            if (GUILayout.Button("Change Platform to Android"))
            {
                currentBuildType = BuildType.Android;

                foreach (var obj in instantiatedObjects)
                {
                    DestroyImmediate(obj);
                }

                instantiatedObjects.Clear();

                foreach (var buildObject in androidBuildObjectData)
                {
                    if (buildObject.scene.name != SceneManager.GetActiveScene().name)
                    {
                        Debug.Log($"現在のSceneが、{nameof(PlatformChanger)}のプロパティに設定されたSceneと違うため、生成がスキップされました。");
                        return;
                    }

                    if (buildObject.parent is null)
                    {
                        instantiatedObjects.Add(PrefabUtility.InstantiatePrefab(buildObject.prefab));
                    }
                    else
                    {
                        instantiatedObjects.Add(PrefabUtility.InstantiatePrefab(buildObject.prefab, buildObject.parent));
                    }
                }
            }
        }

        EditorGUILayout.HelpBox("必ず、オブジェクト変更を行うシーン上で Change Platform を行ってください。", MessageType.Warning);
        target.ApplyModifiedProperties();
    }
}
