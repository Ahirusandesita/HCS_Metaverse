using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
namespace UnityEditor
{
    using UnityEditor.SceneManagement;

    /// <summary>
    /// プラットフォームに応じてシーンのPrefabを切り替えるエディタ拡張
    /// </summary>
    public class PlatformChanger : EditorWindow
    {
        /// <summary>
        /// Jsonに保存するデータ
        /// </summary>
        [Serializable]
        private class PCData
        {
            public List<BuildObjectData> androidBuildObjectData = default;
            public List<BuildObjectData> windowsBuildObjectData = default;
            public BuildType currentBuildType = default;
        }

        /// <summary>
        /// 生成するPrefabデータ
        /// </summary>
        [Serializable]
        private class BuildObjectData
        {
            [PrefabField] public GameObject prefab = default;
            [CustomField("Non Serialize")] public Transform parent = default;
            public SceneAsset sceneAsset = default;
        }

        public enum BuildType
        {
            Android,
            Windows,
        }

        [SerializeField] private List<BuildObjectData> androidBuildObjectData = default;
        [SerializeField] private List<BuildObjectData> windowsBuildObjectData = default;
        [SerializeField] private BuildType currentBuildType = default;

        private PCData data = default;
        private SerializedObject target = default;
        private Vector2 scrollPosition = Vector2.zero;

        private readonly GUIContent androidLabel = new GUIContent("Android Build Prefabs");
        private readonly GUIContent windowsLabel = new GUIContent("Windows Build Prefabs");
        private readonly GUIStyle style = new GUIStyle();
        private const string FILE_NAME = "PlatformChangerData";
        private const string ANDROID_TAG = "Android";
        private const string WINDOWS_TAG = "Windows";


        [MenuItem("Window/Platform Changer")]
        public static void OpenWindow()
        {
            var window = GetWindow<PlatformChanger>();
            window.titleContent = new GUIContent("Platform Changer");
            window.Show();
        }

        private void OnEnable()
        {
            LoadData();

            target = new SerializedObject(this);
            style.richText = true;
            style.fontSize = 14;
        }

        private void OnDisable()
        {
            SaveData();
        }

        private void OnGUI()
        {
            target.Update();
            EditorGUI.BeginChangeCheck();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.Space(16);

            // 変数をSerializedPropertyに変換し、表示する
            var androidProperty = target.FindProperty(nameof(androidBuildObjectData));
            EditorGUILayout.PropertyField(androidProperty, androidLabel);

            EditorGUILayout.Space(16);

            var windowsProperty = target.FindProperty(nameof(windowsBuildObjectData));
            EditorGUILayout.PropertyField(windowsProperty, windowsLabel);

            EditorGUILayout.Space(16);
            EditorGUILayout.LabelField($"<color=white>Current Platform: <b>{currentBuildType}</b></color>", style);
            EditorGUILayout.Space(2);

            // ボタンを押すとAndroidモードに変更する
            if (GUILayout.Button("Change Platform to Android"))
            {
                currentBuildType = BuildType.Android;
                string currentSceneName = SceneManager.GetActiveScene().name;

                // シーン上のオブジェクトのうち、指定のタグが付いているGameObjectを配列で取得
                var deleteObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                    .Where(obj => AssetDatabase.GetAssetOrScenePath(obj).Contains(".unity"))
                    .Where(obj => obj.CompareTag(ANDROID_TAG) || obj.CompareTag(WINDOWS_TAG))
                    .ToArray();

                // 上で取得したGameObjectを完全に削除する
                foreach (var deleteObj in deleteObjects)
                {
                    DestroyImmediate(deleteObj);
                }

                // 設定されたAndroidモードのPrefabをシーン上に生成する
                foreach (var buildObject in androidBuildObjectData)
                {
                    try
                    {
                        // 設定されたPrefabが現在のシーンのものでなければ、次のインデックスへ
                        if (buildObject.sceneAsset.name != currentSceneName)
                        {
                            Debug.Log($"現在のSceneが、{nameof(PlatformChanger)}のプロパティに設定されたSceneと違うため、生成がスキップされました。");
                            continue;
                        }
                    }
                    // 例外：SceneAssetがアタッチされてなかったとき
                    catch (NullReferenceException)
                    {
                        Debug.LogWarning($"{nameof(PlatformChanger)}のプロパティにSceneAssetが設定されていません。");
                        continue;
                    }

                    // シーン上にPrefabをPrefabのまま生成し、タグを変更
                    if (buildObject.parent is null)
                    {
                        (PrefabUtility.InstantiatePrefab(buildObject.prefab) as GameObject).tag = ANDROID_TAG;
                    }
                    else
                    {
                        (PrefabUtility.InstantiatePrefab(buildObject.prefab, buildObject.parent) as GameObject).tag = ANDROID_TAG;
                    }
                }
            }

            // ボタンを押すとWindowsモードに変更する
            if (GUILayout.Button("Change Platform to Windows"))
            {
                currentBuildType = BuildType.Windows;
                string currentSceneName = SceneManager.GetActiveScene().name;

                // シーン上のオブジェクトのうち、指定のタグが付いているGameObjectを配列で取得
                var deleteObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                    .Where(obj => AssetDatabase.GetAssetOrScenePath(obj).Contains(".unity"))
                    .Where(obj => obj.CompareTag(ANDROID_TAG) || obj.CompareTag(WINDOWS_TAG))
                    .ToArray();

                // 上で取得したGameObjectを完全に削除する
                foreach (var deleteObj in deleteObjects)
                {
                    DestroyImmediate(deleteObj);
                }

                // 設定されたWindowsモードのPrefabをシーン上に生成する
                foreach (var buildObject in windowsBuildObjectData)
                {
                    try
                    {
                        // 設定されたPrefabが現在のシーンのものでなければ、次のインデックスへ
                        if (buildObject.sceneAsset.name != currentSceneName)
                        {
                            Debug.Log($"現在のSceneが、{nameof(PlatformChanger)}のプロパティに設定されたSceneと違うため、生成がスキップされました。");
                            continue;
                        }
                    }
                    // 例外：SceneAssetがアタッチされてなかったとき
                    catch (NullReferenceException)
                    {
                        Debug.LogWarning($"{nameof(PlatformChanger)}のプロパティにSceneAssetが設定されていません。");
                        continue;
                    }

                    // シーン上にPrefabをPrefabのまま生成し、タグを変更
                    if (buildObject.parent is null)
                    {
                        (PrefabUtility.InstantiatePrefab(buildObject.prefab) as GameObject).tag = WINDOWS_TAG;
                    }
                    else
                    {
                        (PrefabUtility.InstantiatePrefab(buildObject.prefab, buildObject.parent) as GameObject).tag = WINDOWS_TAG;
                    }
                }
            }

            // 表示している内容に変更があったとき
            if (EditorGUI.EndChangeCheck())
            {
                // シーンに変更があったことを伝える
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                SaveData();
            }

            EditorGUILayout.HelpBox("必ず、オブジェクト変更を行うシーン上で Change Platform を行ってください。", MessageType.Info);
            EditorGUILayout.EndScrollView();
            target.ApplyModifiedProperties();
        }

        /// <summary>
        /// データをJsonに保存
        /// </summary>
        private void SaveData()
        {
            data.androidBuildObjectData = androidBuildObjectData;
            data.windowsBuildObjectData = windowsBuildObjectData;
            data.currentBuildType = currentBuildType;
            EditorSaveSystem.Save(FILE_NAME, data);
        }

        /// <summary>
        /// データをJsonから読み込む
        /// </summary>
        private void LoadData()
        {
            data ??= new PCData();
            EditorSaveSystem.Load(FILE_NAME, data);
            androidBuildObjectData = data.androidBuildObjectData;
            windowsBuildObjectData = data.windowsBuildObjectData;
            currentBuildType = data.currentBuildType;
        }
    }
}
#endif