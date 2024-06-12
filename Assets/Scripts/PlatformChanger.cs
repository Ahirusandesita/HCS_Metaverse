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
    /// �v���b�g�t�H�[���ɉ����ăV�[����Prefab��؂�ւ���G�f�B�^�g��
    /// </summary>
    public class PlatformChanger : EditorWindow
    {
        /// <summary>
        /// Json�ɕۑ�����f�[�^
        /// </summary>
        [Serializable]
        private class PCData
        {
            public List<BuildObjectData> androidBuildObjectData = default;
            public List<BuildObjectData> windowsBuildObjectData = default;
            public BuildType currentBuildType = default;
        }

        /// <summary>
        /// ��������Prefab�f�[�^
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

            // �ϐ���SerializedProperty�ɕϊ����A�\������
            var androidProperty = target.FindProperty(nameof(androidBuildObjectData));
            EditorGUILayout.PropertyField(androidProperty, androidLabel);

            EditorGUILayout.Space(16);

            var windowsProperty = target.FindProperty(nameof(windowsBuildObjectData));
            EditorGUILayout.PropertyField(windowsProperty, windowsLabel);

            EditorGUILayout.Space(16);
            EditorGUILayout.LabelField($"<color=white>Current Platform: <b>{currentBuildType}</b></color>", style);
            EditorGUILayout.Space(2);

            // �{�^����������Android���[�h�ɕύX����
            if (GUILayout.Button("Change Platform to Android"))
            {
                currentBuildType = BuildType.Android;
                string currentSceneName = SceneManager.GetActiveScene().name;

                // �V�[����̃I�u�W�F�N�g�̂����A�w��̃^�O���t���Ă���GameObject��z��Ŏ擾
                var deleteObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                    .Where(obj => AssetDatabase.GetAssetOrScenePath(obj).Contains(".unity"))
                    .Where(obj => obj.CompareTag(ANDROID_TAG) || obj.CompareTag(WINDOWS_TAG))
                    .ToArray();

                // ��Ŏ擾����GameObject�����S�ɍ폜����
                foreach (var deleteObj in deleteObjects)
                {
                    DestroyImmediate(deleteObj);
                }

                // �ݒ肳�ꂽAndroid���[�h��Prefab���V�[����ɐ�������
                foreach (var buildObject in androidBuildObjectData)
                {
                    try
                    {
                        // �ݒ肳�ꂽPrefab�����݂̃V�[���̂��̂łȂ���΁A���̃C���f�b�N�X��
                        if (buildObject.sceneAsset.name != currentSceneName)
                        {
                            Debug.Log($"���݂�Scene���A{nameof(PlatformChanger)}�̃v���p�e�B�ɐݒ肳�ꂽScene�ƈႤ���߁A�������X�L�b�v����܂����B");
                            continue;
                        }
                    }
                    // ��O�FSceneAsset���A�^�b�`����ĂȂ������Ƃ�
                    catch (NullReferenceException)
                    {
                        Debug.LogWarning($"{nameof(PlatformChanger)}�̃v���p�e�B��SceneAsset���ݒ肳��Ă��܂���B");
                        continue;
                    }

                    // �V�[�����Prefab��Prefab�̂܂ܐ������A�^�O��ύX
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

            // �{�^����������Windows���[�h�ɕύX����
            if (GUILayout.Button("Change Platform to Windows"))
            {
                currentBuildType = BuildType.Windows;
                string currentSceneName = SceneManager.GetActiveScene().name;

                // �V�[����̃I�u�W�F�N�g�̂����A�w��̃^�O���t���Ă���GameObject��z��Ŏ擾
                var deleteObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                    .Where(obj => AssetDatabase.GetAssetOrScenePath(obj).Contains(".unity"))
                    .Where(obj => obj.CompareTag(ANDROID_TAG) || obj.CompareTag(WINDOWS_TAG))
                    .ToArray();

                // ��Ŏ擾����GameObject�����S�ɍ폜����
                foreach (var deleteObj in deleteObjects)
                {
                    DestroyImmediate(deleteObj);
                }

                // �ݒ肳�ꂽWindows���[�h��Prefab���V�[����ɐ�������
                foreach (var buildObject in windowsBuildObjectData)
                {
                    try
                    {
                        // �ݒ肳�ꂽPrefab�����݂̃V�[���̂��̂łȂ���΁A���̃C���f�b�N�X��
                        if (buildObject.sceneAsset.name != currentSceneName)
                        {
                            Debug.Log($"���݂�Scene���A{nameof(PlatformChanger)}�̃v���p�e�B�ɐݒ肳�ꂽScene�ƈႤ���߁A�������X�L�b�v����܂����B");
                            continue;
                        }
                    }
                    // ��O�FSceneAsset���A�^�b�`����ĂȂ������Ƃ�
                    catch (NullReferenceException)
                    {
                        Debug.LogWarning($"{nameof(PlatformChanger)}�̃v���p�e�B��SceneAsset���ݒ肳��Ă��܂���B");
                        continue;
                    }

                    // �V�[�����Prefab��Prefab�̂܂ܐ������A�^�O��ύX
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

            // �\�����Ă�����e�ɕύX���������Ƃ�
            if (EditorGUI.EndChangeCheck())
            {
                // �V�[���ɕύX�����������Ƃ�`����
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                SaveData();
            }

            EditorGUILayout.HelpBox("�K���A�I�u�W�F�N�g�ύX���s���V�[����� Change Platform ���s���Ă��������B", MessageType.Info);
            EditorGUILayout.EndScrollView();
            target.ApplyModifiedProperties();
        }

        /// <summary>
        /// �f�[�^��Json�ɕۑ�
        /// </summary>
        private void SaveData()
        {
            data.androidBuildObjectData = androidBuildObjectData;
            data.windowsBuildObjectData = windowsBuildObjectData;
            data.currentBuildType = currentBuildType;
            EditorSaveSystem.Save(FILE_NAME, data);
        }

        /// <summary>
        /// �f�[�^��Json����ǂݍ���
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