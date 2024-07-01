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

        public ItemIDViewer Food => foodID;
        public Transform FoodBox => foodBox;
    }

    [SerializeField] private AllItemAsset allItemAsset = default;
    [SerializeField] private List<FoodInfo> foodLineup = default;
    private List<GameObject> displayFoods = default;


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
#if UNITY_EDITOR
        // Conditionalはメソッド内はコンパイルされてしまうので、仕方なく二重
        allItemAsset = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(AllItemAsset)}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<AllItemAsset>)
                .First();
#endif
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
        foreach (var food in foodLineup)
        {
            var asset = allItemAsset.GetItemAssetByID(food.Food);
            var position = food.FoodBox.position;
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
