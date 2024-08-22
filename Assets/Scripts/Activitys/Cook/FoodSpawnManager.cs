using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawnManager : MonoBehaviour, ISelectedNotification, IActivityNotification
{
    [System.Serializable]
    private class FoodInfo
    {
        [SerializeField] private FoodIDView foodID = default;
        [SerializeField] private Transform foodBox = default;

        public FoodIDView FoodID => foodID;
        public Transform FoodBox => foodBox;
    }

    [SerializeField] private ItemBundleAsset foodItemAsset = default;
    [SerializeField] private List<FoodInfo> foodLineup = default;
    private List<GameObject> displayFoods = default;

    public ItemBundleAsset FoodItemAsset => foodItemAsset;
    private void Awake()
    {
        OnStart();
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
#if UNITY_EDITOR
        // Conditionalはメソッド内はコンパイルされてしまうので、仕方なく二重
        foodItemAsset = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(ItemBundleAsset)}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<ItemBundleAsset>)
                .Where(asset => asset.GenresHandled == ItemGenre.Food)
                .First();
#endif
    }

    public async void Select(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        var asset = foodItemAsset.GetItemAssetByID(itemSelectArgs.id);
        var position = itemSelectArgs.position;
        var foodItem = await IDisplayItem.InstantiateSync(asset, position, Quaternion.identity, this);
        displayFoods.Add(foodItem.gameObject);
        displayFoods.Remove(itemSelectArgs.gameObject);
    }

    public void Unselect(SelectArgs selectArgs)
    {

    }

    public async void OnStart()
    {
        displayFoods = new List<GameObject>();

        foreach (var food in foodLineup)
        {
            var asset = foodItemAsset.GetItemAssetByID(food.FoodID);
            var position = food.FoodBox.position + Vector3.up;

            var foodItem = await IDisplayItem.InstantiateSync(asset, position, Quaternion.identity, this);
            displayFoods.Add(foodItem.gameObject);
        }
    }

    void IActivityNotification.OnFinish()
    {
        foreach (var foodObj in displayFoods)
        {
            Destroy(foodObj);
        }
    }
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
    [CustomEditor(typeof(FoodSpawnManager))]
    public class FoodSpawnManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space(12f);

            if (GUILayout.Button("Update Display Options"))
            {
                try
                {
                    FoodIDViewDrawer.UpdateDisplayOptions();
                }
                // 要素ない状態でボタン押すと例外出る→うざいので握りつぶす
                catch (System.NullReferenceException) { }
            }
        }
    }
}
#endif