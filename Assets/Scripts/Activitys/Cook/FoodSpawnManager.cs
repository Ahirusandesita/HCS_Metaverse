using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class NetworkInformation
{
    public readonly NetworkView NetworkView;
    public readonly int ID;

    public NetworkInformation(NetworkView networkView, int id)
    {
        this.NetworkView = networkView;
        this.ID = id;
    }
}
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
    private FoodSpawnManagerRPC foodSpawnRPC;
    [SerializeField]
    private AllSpawn allSpawn;

    private Vector3 upVector = new Vector3(0, 0.4f, 0);

    private List<NetworkInformation> networkInformations = new List<NetworkInformation>();

    private void Start()
    {
        FindObjectOfType<ActivityProgressManagement>().OnStart += () =>
        {
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

    public async void StartSpawnNetworkView(int id, Vector3 position, int index)
    {
        ItemAsset itemAsset = foodItemAsset.GetItemAssetByID(id);
        NetworkView networkView = await GateOfFusion.Instance.SpawnAsync(itemAsset.NetworkView, position, Quaternion.identity);
        networkInformations.Add(new NetworkInformation(networkView, id));

        AllSpawn allSpawnInstance = await GateOfFusion.Instance.SpawnAsync(allSpawn);
        await allSpawnInstance.Async();
        GateOfFusion.Instance.Despawn(allSpawnInstance);
        foodSpawnRPC.RPC_StartSpawnLocalView(id, networkView.GetComponent<NetworkObject>(), index);
    }
    public async void SpawnNetworkView(int id, Vector3 position)
    {
        ItemAsset itemAsset = foodItemAsset.GetItemAssetByID(id);
        NetworkView networkView = await GateOfFusion.Instance.SpawnAsync(itemAsset.NetworkView, position, Quaternion.identity);
        networkInformations.Add(new NetworkInformation(networkView, id));
        AllSpawn allSpawnInstance = await GateOfFusion.Instance.SpawnAsync(allSpawn);
        await allSpawnInstance.Async();
        GateOfFusion.Instance.Despawn(allSpawnInstance);
        foodSpawnRPC.RPC_SpawnLocalView(id, position, networkView.GetComponent<NetworkObject>());
    }
    public void NewMember(PlayerRef player)
    {
        foreach (NetworkInformation networkInformation in networkInformations)
        {
            Debug.LogError(networkInformation.NetworkView.OneGrab);
            if (!networkInformation.NetworkView.OneGrab)
            {
                foodSpawnRPC.RPC_Joined(player, networkInformation.ID, networkInformation.NetworkView.GetComponent<NetworkObject>());
            }
            else
            {
                foodSpawnRPC.RPC_JoinedOneGrab(player, networkInformation.ID, networkInformation.NetworkView.GetComponent<NetworkObject>());
            }
        }
    }
    public void StartSpawnLocalView(int id, NetworkView networkView, int index)
    {
        GameObject itemObject;
        var position = foodLineup[index].FoodBox.position + upVector;
        var asset = foodItemAsset.GetItemAssetByID(foodLineup[index].FoodID);
        itemObject = Object.Instantiate(asset.DisplayItem.gameObject, position, Quaternion.identity);
        var displayItem = itemObject.GetComponent<IDisplayItem>();
        var itemSelectArgs = new ItemSelectArgs(asset.ID, asset.Name, position, displayItem.gameObject);
        displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, this);
        itemObject.GetComponent<LocalView>().NetworkViewInject(networkView);

        if (!GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            networkInformations.Add(new NetworkInformation(networkView, foodLineup[index].FoodID));
        }
    }
    public void SpawnLocalView(int id, Vector3 position, NetworkView networkView)
    {
        GameObject itemObject;
        var asset = foodItemAsset.GetItemAssetByID(id);
        itemObject = Object.Instantiate(asset.DisplayItem.gameObject, position, Quaternion.identity);
        var displayItem = itemObject.GetComponent<IDisplayItem>();
        var itemSelectArgs = new ItemSelectArgs(asset.ID, asset.Name, position, displayItem.gameObject);
        displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, this);

        itemObject.GetComponent<LocalView>().NetworkViewInject(networkView);

        if (!GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            networkInformations.Add(new NetworkInformation(networkView, id));
        }
    }
    public void LateJoinSpawnLocalView(int id, Vector3 position, NetworkView networkView)
    {
        GameObject itemObject;
        var asset = foodItemAsset.GetItemAssetByID(id);
        itemObject = Object.Instantiate(asset.DisplayItem.gameObject, position, Quaternion.identity);
        itemObject.GetComponent<Rigidbody>().isKinematic = false;
        itemObject.GetComponent<LocalView>().NetworkViewInject(networkView);
        networkInformations.Add(new NetworkInformation(networkView, id));
    }



    public void Select(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        foodSpawnRPC.RPC_SpawnNetworkView(itemSelectArgs.id, itemSelectArgs.position);
        //selectedNotification.RPC_MasterSelect(itemSelectArgs.id, itemSelectArgs.position);
    }
    public async void MasterSelect(int id, Vector3 position)
    {
        Debug.LogError("Masterとして生成");
        var asset = foodItemAsset.GetItemAssetByID(id);
        var foodItem = await IDisplayItem.InstantiateSync(asset, position, Quaternion.identity, this);
        foodSpawnRPC.RPC_NotificationInjection(foodItem.gameObject.GetComponent<NetworkObject>(), id, position);
        //displayFoods.Add(foodItem.gameObject);
        //displayFoods.Remove(itemSelectArgs.gameObject);
    }

    public void SelectedNotificationInjection(NetworkObject networkObject, int id, Vector3 position)
    {
        Debug.LogError("MasterからInjectionされた");
        IDisplayItem displayItem = networkObject.GetComponent<IDisplayItem>();
        var asset = foodItemAsset.GetItemAssetByID(id);
        var itemSelectArgs = new ItemSelectArgs(asset.ID, asset.Name, position, displayItem.gameObject);
        displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, this);
    }

    public void Unselect(SelectArgs selectArgs)
    {

    }

    public void OnStart()
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            displayFoods = new List<GameObject>();

            for (int i = 0; i < foodLineup.Count; i++)
            {
                var asset = foodItemAsset.GetItemAssetByID(foodLineup[i].FoodID);
                var position = foodLineup[i].FoodBox.position + upVector;

                foodSpawnRPC.RPC_StartSpawnNetworkView(asset.ID, position, i);
                //var foodItem = await IDisplayItem.InstantiateSync(asset, position, Quaternion.identity, this);
                //foodSpawnRPC.RPC_FoodSpawn(foodItem.gameObject.GetComponent<NetworkObject>(), i);
                //displayFoods.Add(foodItem.gameObject);
            }
        }
    }
    public void SelectedNotificationInjection(NetworkObject networkObject, int index)
    {
        IDisplayItem displayItem = networkObject.GetComponent<IDisplayItem>();
        var asset = foodItemAsset.GetItemAssetByID(foodLineup[index].FoodID);
        var position = foodLineup[index].FoodBox.position + upVector;

        var itemSelectArgs = new ItemSelectArgs(asset.ID, asset.Name, position, displayItem.gameObject);
        displayItem.Inject_ItemSelectArgsAndSelectedNotification(itemSelectArgs, this);
    }

    async void OnFinish()
    {
        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            //foreach (var foodObj in displayFoods)
            //{
            //    //GateOfFusion.Instance.Despawn<NetworkObject>(foodObj.GetComponent<NetworkObject>());
            //}
        }
    }

    public void Despawn(NetworkView networkView)
    {
        NetworkInformation information = default;
        foreach (NetworkInformation item in networkInformations)
        {
            if(item.NetworkView == networkView)
            {
                Debug.LogError("NeetworkView一致");
                information = item;
            }
        }
        networkInformations.Remove(information);
        GateOfFusion.Instance.Despawn(information.NetworkView);
        Destroy(information.NetworkView.LocalView.gameObject);
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