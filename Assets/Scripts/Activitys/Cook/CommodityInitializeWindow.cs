using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class CommodityInitializeWindow : EditorWindow
{
    private string fileName;

    private Commodity commodity;
    [SerializeField]
    private List<Commodity> commodities = new List<Commodity>();
    private List<List<Commodity>> subsets;

    private string[] assetNames = new string[100];
    private Commodity[] prefabs = new Commodity[100];
    private List<string> trueAssetNames = new List<string>();
    private List<Commodity> truePrefabs = new List<Commodity>();

    private Vector2 _scrollPosition = Vector2.zero;

    private List<Commodity> exsistCommodities = new List<Commodity>();

    [MenuItem("Initialize/Activity/Cook/CommodityCreate")]
    static void Init()
    {
        CommodityInitializeWindow window = (CommodityInitializeWindow)EditorWindow.GetWindow(typeof(CommodityInitializeWindow));
        window.Show();
    }

    void StartUp()
    {
        var guids = AssetDatabase.FindAssets("t:GameObject");
        var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
        List<GameObject> list = paths.Select(_ => AssetDatabase.LoadAssetAtPath<GameObject>(_)).ToList();

        exsistCommodities = new List<Commodity>();

        foreach (GameObject gameObject in list)
        {
            if (gameObject.TryGetComponent<Commodity>(out Commodity commodity))
            {
                exsistCommodities.Add(commodity);
            }
        }
    }
    void OnGUI()
    {
        if (GUILayout.Button("StartUp"))
        {
            StartUp();
        }
        fileName = EditorGUILayout.TextField("FileName", fileName);

        var so = new SerializedObject(this);

        so.Update();
        EditorGUILayout.PropertyField(so.FindProperty("commodities"), true);

        so.ApplyModifiedProperties();

        if (commodities.Count >= 2)
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            subsets = GetSubsets(commodities.ToArray());

            int index = -1;

            foreach (var subset in subsets)
            {
                index++;
                bool isMatch = false;
                foreach (Commodity item in exsistCommodities)
                {
                    if (item.CanInstanceCommodity(subset.ToArray()))
                    {
                        isMatch = true;
                        break;
                    }
                }

                if (isMatch)
                {
                    continue;
                }


                GUILayout.Label("パターン", EditorStyles.boldLabel);
                assetNames[index] = EditorGUILayout.TextField("アセットの名前", assetNames[index]);
                prefabs[index] = (Commodity)EditorGUILayout.ObjectField("Commodity", prefabs[index], typeof(Commodity), true);


                foreach (Commodity commodity in subset)
                {
                    EditorGUILayout.ObjectField(commodity, typeof(Commodity), false);
                }
                GUILayout.Label("", EditorStyles.boldLabel);
                GUILayout.Label("", EditorStyles.boldLabel);
            }

            EditorGUILayout.EndScrollView();
        }
        if (GUILayout.Button("SetUp"))
        {
            SetUp();
        }


    }

    private void SetUp()
    {
        StartUp();
        List<List<Commodity>> subsetWorks = new List<List<Commodity>>();
        int index = 0;
        foreach (var subset in subsets)
        {
            bool isMatch = false;
            foreach (Commodity exsistCommodity in exsistCommodities)
            {
                if (exsistCommodity.CanInstanceCommodity(subset.ToArray()))
                {
                    isMatch = true;
                    break;
                }
            }
            if (!isMatch)
            {
                subsetWorks.Add(subset);

                trueAssetNames.Add(assetNames[index]);
                truePrefabs.Add(prefabs[index]);
            }
            index++;
        }


        int assetIndex = 0;
        foreach (var subset in subsetWorks)
        {
            CommodityAsset ingrodient = ScriptableObject.CreateInstance<CommodityAsset>();
            ICommodityAssetModerator commodityAssetModerator = ingrodient;
            List<Commodity> item = new List<Commodity>();
            foreach (Commodity commodity in subset)
            {
                item.Add(commodity);
            }
            commodityAssetModerator.SetCommodities(item);

            var fileName = $"{trueAssetNames[assetIndex]}.asset";
            var path = $"Assets/ScriptableObject/CookAssets/Commoditys/{this.fileName}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            AssetDatabase.CreateAsset(ingrodient, Path.Combine(path, fileName));

            string outputPath = $"Assets/ScriptableObject/CookAssets/Foods/Commoditys/{this.fileName}/";
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            truePrefabs[assetIndex].GetComponent<ICommodityModerator>().SetCommodityAsset(ingrodient);

            if (truePrefabs[assetIndex].gameObject.activeInHierarchy)
            {
                if (PrefabUtility.IsAnyPrefabInstanceRoot(truePrefabs[assetIndex].gameObject))
                {
                    PrefabUtility.SaveAsPrefabAsset(truePrefabs[assetIndex].gameObject, outputPath);
                }
                else
                {
                    PrefabUtility.SaveAsPrefabAssetAndConnect(truePrefabs[assetIndex].gameObject, outputPath + $"{trueAssetNames[assetIndex]}.prefab", InteractionMode.AutomatedAction);
                }
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(truePrefabs[assetIndex].gameObject, outputPath);
            }
            //PrefabUtility.UnloadPrefabContents(ingrodients.gameObject);
            UnityEditor.EditorUtility.SetDirty(truePrefabs[assetIndex].gameObject);

            if (truePrefabs[assetIndex].gameObject.activeInHierarchy)
            {
                Editor.DestroyImmediate(truePrefabs[assetIndex].gameObject);
            }
            assetIndex++;
        }

        GrantCommodityID.Initialize();
    }




    static List<List<Commodity>> GetSubsets(Commodity[] array)
    {
        List<List<Commodity>> subsets = new List<List<Commodity>>();
        GenerateSubsets(array, new List<Commodity>(), 0, subsets);
        return subsets;
    }

    static void GenerateSubsets(Commodity[] array, List<Commodity> current, int index, List<List<Commodity>> subsets)
    {
        if (current.Count > 0)
        {
            subsets.Add(new List<Commodity>(current));
        }

        for (int i = index; i < array.Length; i++)
        {
            current.Add(array[i]);

            GenerateSubsets(array, current, i + 1, subsets);
            current.RemoveAt(current.Count - 1);
        }
    }

}
#endif