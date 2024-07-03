using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CommodityCreate : MonoBehaviour
{
    [MenuItem("Assets/Create/New Prefab With Components")]
    public static void CreatePrefabWithComponents()
    {
        string name = "target";
        string outputPath = "Assets/WithComponents.prefab";

        GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags(
            name,
            HideFlags.HideInHierarchy,
            typeof(Rigidbody), typeof(BoxCollider)
        );

        PrefabUtility.SaveAsPrefabAsset(gameObject,outputPath);

        Editor.DestroyImmediate(gameObject);
    }

    public class WindowTest : EditorWindow
    {
        private string assetName;
        private Commodity[] commodities = new Commodity[System.Enum.GetValues(typeof(ProcessingType)).Length];
        private int[] timeItTakes = new int[System.Enum.GetValues(typeof(ProcessingType)).Length];
        private Ingrodients ingrodients;
        private bool canCreatePrefab;

        [MenuItem("Initialize/Activity/Cool/Commodity")]
        static void Init()
        {
            WindowTest window = (WindowTest)EditorWindow.GetWindow(typeof(WindowTest));
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("素材をセット");
            assetName = EditorGUILayout.TextField($"Assetの名前", assetName);
            ingrodients = (Ingrodients)EditorGUILayout.ObjectField("素材", ingrodients, typeof(Ingrodients), true);
            canCreatePrefab = EditorGUILayout.Toggle("Prefabを生成しますか", canCreatePrefab);
            //ingrodients = ingroObject.GetComponent<Ingrodients>();

            if (ingrodients != null)
            {
                for (int i = 0; i < System.Enum.GetValues(typeof(ProcessingType)).Length; i++)
                {
                    commodities[i] = (Commodity)EditorGUILayout.ObjectField($"{(ProcessingType)i} の完成品", commodities[i], typeof(Commodity), true);

                    if (commodities[i] == null) { continue; }

                    timeItTakes[i] = EditorGUILayout.IntField($"完成までにかかる時間", timeItTakes[i]);
                }
            }

            if (GUILayout.Button("SetUp"))
            {
                Create();
            }
        }


        public void Create()
        {
            IngrodientsAsset ingrodient = ScriptableObject.CreateInstance<IngrodientsAsset>();

            List<IngrodientsDetailInformation> ingrodientsDetailInformation = new List<IngrodientsDetailInformation>();

            for(int i = 0; i < commodities.Length; i++)
            {
                if(commodities[i] == null) { continue; }

                ingrodientsDetailInformation.Add(new IngrodientsDetailInformation((ProcessingType)i, timeItTakes[i], commodities[i]));
            }
            IIngrodientAsset iIngrodient = ingrodient;
            iIngrodient.SetUp(ingrodientsDetailInformation);
            var fileName = $"{assetName}.asset";
            var path = "Assets/ScriptableObject/CookAssets/Foods/Ingrodients";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            AssetDatabase.CreateAsset(ingrodient, Path.Combine(path, fileName));
        }
    }
}

