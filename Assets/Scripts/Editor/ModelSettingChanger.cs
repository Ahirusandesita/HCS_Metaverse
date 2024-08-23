using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ModelImporter�ɂ����Mesh�̐ݒ��ύX����Window
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
                GUILayout.Label("<b><color=#f5f5f5>Outline��\��������Prefab�iOutlineManager���A�^�b�`����Ă���Prefab�j���擾���A\n" +
                    "����Prefab���g�p���Ă���Mesh��ModelSetting���ꊇ�ŕύX���܂��B\n" +
                    "����ɂ��AOutline���\������Ȃ���肪��������܂��B</color></b>", style);
                EditorGUILayout.Space(16f);

                if (GUILayout.Button("Serach"))
                {
                    // ���ׂĂ�Prefab��AssetPath���擾
                    var assetPaths = AssetDatabase.FindAssets($"t:Prefab")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .ToArray();

                    modelImporters = new List<ModelImporter>();
                    targetsPath = new List<string>();
                    meshesPath = new List<string>();
                    foreach (var path in assetPaths)
                    {
                        // AssetPath����GameObject���擾����B
                        // ����GameObject��Outline���������Ă���̂Ȃ�΁A�g�p���Ă���Mesh���擾���Ă����B
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

                // ModelSetting��ύX����Ώۂ̃I�u�W�F�N�g��Label�\������
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
                    // �ݒ��ύX���A�ăC���|�[�g
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
