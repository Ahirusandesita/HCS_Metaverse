using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using HCSMeta.Activity.Cook.Interface;

#if UNITY_EDITOR
namespace HCSMeta.Activity.Cook
{
    public class IngrodientsInitializeWindow : EditorWindow
    {
        private string assetName;
        private Commodity[] commodities = new Commodity[System.Enum.GetValues(typeof(ProcessingType)).Length];
        private float[] timeItTakes = new float[System.Enum.GetValues(typeof(ProcessingType)).Length];
        private Ingrodients ingrodients;
        private bool canCreatePrefab;

        private IngrodientsAsset ingrodientsAsset = null;
        private bool once = true;

        [MenuItem("Initialize/Activity/Cook/IngrodientsCreate")]
        static void Init()
        {
            IngrodientsInitializeWindow window = (IngrodientsInitializeWindow)EditorWindow.GetWindow(typeof(IngrodientsInitializeWindow));
            window.Show();

        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("素材をセット");
            assetName = EditorGUILayout.TextField($"Assetの名前", assetName);
            ingrodientsAsset = (IngrodientsAsset)EditorGUILayout.ObjectField("既存のAssetの更新", ingrodientsAsset, typeof(IngrodientsAsset), true);
            if (GUILayout.Button("更新"))
            {
                once = true;
                assetName = ingrodientsAsset.name;
            }


            ingrodients = (Ingrodients)EditorGUILayout.ObjectField("素材", ingrodients, typeof(Ingrodients), true);
            canCreatePrefab = EditorGUILayout.Toggle("Prefabを更新する", canCreatePrefab);
            //ingrodients = ingroObject.GetComponent<Ingrodients>();

            if (once)
            {
                once = false;
                if (ingrodientsAsset != null)
                    for (int i = 0; i < ingrodientsAsset.IngrodientsDetailInformations.Count; i++)
                    {
                        int enumIndex = (int)ingrodientsAsset.IngrodientsDetailInformations[i].ProcessingType;
                        commodities[enumIndex] = ingrodientsAsset.IngrodientsDetailInformations[i].Commodity;
                        timeItTakes[enumIndex] = ingrodientsAsset.IngrodientsDetailInformations[i].TimeItTakes;
                    }
            }

            if (ingrodients != null)
            {
                for (int i = 0; i < System.Enum.GetValues(typeof(ProcessingType)).Length; i++)
                {
                    commodities[i] = (Commodity)EditorGUILayout.ObjectField($"{(ProcessingType)i} の完成品", commodities[i], typeof(Commodity), true);

                    if (commodities[i] == null) { continue; }

                    timeItTakes[i] = EditorGUILayout.FloatField($"完成までにかかる時間", timeItTakes[i]);
                }
            }

            if (GUILayout.Button("SetUp"))
            {
                Create();
            }
            if (GUILayout.Button("Reset"))
            {
                Reset();
            }
        }

        private void Create()
        {
            IngrodientsAsset ingrodient = ScriptableObject.CreateInstance<IngrodientsAsset>();

            List<IngrodientsDetailInformation> ingrodientsDetailInformation = new List<IngrodientsDetailInformation>();

            for (int i = 0; i < commodities.Length; i++)
            {
                if (commodities[i] == null) { continue; }

                ingrodientsDetailInformation.Add(new IngrodientsDetailInformation((ProcessingType)i, timeItTakes[i], commodities[i]));
            }
            IIngrodientAsset iIngrodient = ingrodient;
            iIngrodient.SetUp(ingrodientsDetailInformation);
            var fileName = $"{assetName}.asset";
            var path = "Assets/ScriptableObject/CookAssets/Ingrodient";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            AssetDatabase.CreateAsset(ingrodient, Path.Combine(path, fileName));

            if (canCreatePrefab)
            {
                string outputPath = $"Assets/ScriptableObject/CookAssets/Foods/Ingrodients/{assetName}.prefab";

                ingrodients.GetComponent<IIngrodientsModerator>().IngrodientsAsset = ingrodient;

                if (ingrodients.gameObject.activeInHierarchy)
                {
                    if (PrefabUtility.IsAnyPrefabInstanceRoot(ingrodients.gameObject))
                    {
                        PrefabUtility.SaveAsPrefabAsset(ingrodients.gameObject, outputPath);
                    }
                    else
                    {
                        PrefabUtility.SaveAsPrefabAssetAndConnect(ingrodients.gameObject, outputPath, InteractionMode.AutomatedAction);
                    }
                }
                else
                {
                    PrefabUtility.SaveAsPrefabAsset(ingrodients.gameObject, outputPath);
                }
                //PrefabUtility.UnloadPrefabContents(ingrodients.gameObject);
                UnityEditor.EditorUtility.SetDirty(ingrodients.gameObject);

                if (ingrodients.gameObject.activeInHierarchy)
                {
                    Editor.DestroyImmediate(ingrodients.gameObject);
                }
                Reset();
            }
        }
        private void Reset()
        {
            assetName = default;
            commodities = new Commodity[System.Enum.GetValues(typeof(ProcessingType)).Length];
            timeItTakes = new float[System.Enum.GetValues(typeof(ProcessingType)).Length];
            ingrodients = default;
            canCreatePrefab = default;
            once = true;
            ingrodientsAsset = null;
        }

    }
}
#endif
