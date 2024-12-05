using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class FoodSpawnManager : MonoBehaviour, ISelectedNotification
{
    [System.Serializable]
    private class FoodInfo
    {
        [SerializeField] private ItemIDView foodID = default;
        [SerializeField] private Transform foodBox = default;

        public ItemIDView FoodID => foodID;
        public Transform FoodBox => foodBox;
    }

    [SerializeField] private ItemBundleAsset foodItemAsset = default;
    [SerializeField] private List<FoodInfo> foodLineup = default;
    private List<GameObject> displayFoods = default;

    public ItemBundleAsset FoodItemAsset => foodItemAsset;

    [SerializeField]
    private FoodSpawnManagerRPC selectedNotification;

    private void Start()
    {
        FindObjectOfType<ActivityProgressManagement>().OnStart += () =>
        {
            Debug.LogError("料理だすよ");
            OnStart();
        };
        FindObjectOfType<ActivityProgressManagement>().OnFinish += () =>
        {
            OnFinish();
        };
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
#if UNITY_EDITOR
        // Conditionalはメソッド内はコンパイルされてしまうので、仕方なく二重
        foodItemAsset = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(ItemBundleAsset)}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<ItemBundleAsset>)
                .First();
#endif
    }

    public async void Select(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        var asset = foodItemAsset.GetItemAssetByID(itemSelectArgs.id);
        var position = itemSelectArgs.position;
        var foodItem = await IDisplayItem.InstantiateSync(asset, position, Quaternion.identity, this);
        selectedNotification.RPC_Unti(foodItem.gameObject.GetComponent<NetworkObject>(), itemSelectArgs.id, itemSelectArgs.position);
        displayFoods.Add(foodItem.gameObject);
        displayFoods.Remove(itemSelectArgs.gameObject);
    }
    public void UntiHuzakennaSelect(NetworkObject networkObject,int id,Vector3 position)
    {
        IDisplayItem displayItem = networkObject.GetComponent<IDisplayItem>();
        var asset = foodItemAsset.GetItemAssetByID(id);
        var itemSelectArgs = new ItemSelectArgs(asset.ID, asset.Name, position, displayItem.gameObject);
        displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, this);
    }

    public void Unselect(SelectArgs selectArgs)
    {

    }

    public async void OnStart()
    {
            if (true)
            {
                displayFoods = new List<GameObject>();

                for (int i = 0; i < foodLineup.Count; i++)
                {
                    var asset = foodItemAsset.GetItemAssetByID(foodLineup[i].FoodID);
                    var position = foodLineup[i].FoodBox.position + Vector3.up;

                    var foodItem = await IDisplayItem.InstantiateSync(asset, position, Quaternion.identity, this);
                    selectedNotification.RPC_FoodSpawn(foodItem.gameObject.GetComponent<NetworkObject>(), i);
                    displayFoods.Add(foodItem.gameObject);
                }
            }
    }
    public void UntiHuzakenna(NetworkObject networkObject,int index)
    {
        IDisplayItem displayItem = networkObject.GetComponent<IDisplayItem>();
        var asset = foodItemAsset.GetItemAssetByID(foodLineup[index].FoodID);
        var position = foodLineup[index].FoodBox.position + Vector3.up;

        var itemSelectArgs = new ItemSelectArgs(asset.ID, asset.Name, position, displayItem.gameObject);
        displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs,this);
    }

    void OnFinish()
    {
        if (GateOfFusion.Instance.IsLeader)
        {
            foreach (var foodObj in displayFoods)
            {
                //GateOfFusion.Instance.Despawn<NetworkObject>(foodObj.GetComponent<NetworkObject>());
            }
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
                    ItemIDViewDrawer.UpdateDisplayOptions();
                }
                // 要素ない状態でボタン押すと例外出る→うざいので握りつぶす
                catch (System.NullReferenceException) { }
            }
        }
    }
}
#endif