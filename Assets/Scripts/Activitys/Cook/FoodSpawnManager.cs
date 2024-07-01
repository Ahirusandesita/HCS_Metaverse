using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawnManager : MonoBehaviour, ISelectedNotification, IActivityNotification
{
    [System.Serializable]
    private class FoodInfo
    {
        [SerializeField] private ItemIDViewer foodID = default;
        [SerializeField] private Transform foodBox = default;

        public ItemIDViewer FoodID => foodID;
        public Transform FoodBox => foodBox;
    }

    [SerializeField] private ItemBundleAsset allItemAsset = default;
    [SerializeField] private List<FoodInfo> foodLineup = default;
    private List<GameObject> displayFoods = default;

    public ItemBundleAsset AllItemAsset => allItemAsset;


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
#if UNITY_EDITOR
        // Conditionalはメソッド内はコンパイルされてしまうので、仕方なく二重
        allItemAsset = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(ItemBundleAsset)}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<ItemBundleAsset>)
                .First();
#endif
    }

    private void Start()
    {
        var a = this as IActivityNotification;
        a.OnStart();
    }

    public void Select(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        var asset = allItemAsset.GetItemAssetByID(itemSelectArgs.id);
        var position = itemSelectArgs.position;
        var foodItem = IDisplayItem.Instantiate(asset, position, Quaternion.identity, this);
        displayFoods.Add(foodItem.gameObject);
        displayFoods.Remove(itemSelectArgs.gameObject);
    }

    public void Unselect(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        Destroy(itemSelectArgs.gameObject);
    }

    void IActivityNotification.OnStart()
    {
        displayFoods = new List<GameObject>();

        foreach (var food in foodLineup)
        {
            var asset = allItemAsset.GetItemAssetByID(food.FoodID);
            var position = food.FoodBox.position + Vector3.up;
            var foodItem = IDisplayItem.Instantiate(asset, position, Quaternion.identity, this);
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
namespace UnityEditor
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
                    ItemIDViewerDrawer.UpdateDisplayOptions();
                }
                // 要素ない状態でボタン押すと例外出る→うざいので握りつぶす
                catch (System.NullReferenceException) { }
            }
        }
    }
}
#endif