using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class CommoditySpawnManager : MonoBehaviour
{
    [SerializeField]
    private AllCommodityAsset allCommodityAsset;
    [SerializeField]
    FoodSpawnManagerRPC rpc;
    [SerializeField]
    private AllSpawn allSpawn;

    private List<NetworkInformation> networkInformations = new List<NetworkInformation>();

    public async void SpawnNetworkView(int index, Vector3 rotation, Vector3 position, int machineID)
    {
        //for(int i = 0; i < allCommodityAsset.Commodities.Count; i++)
        //{
        //    Commodity item = allCommodityAsset.Commodities[i];
        //    if(item.CommodityAsset.CommodityID == index)
        //    {
        //        index = i;
        //        break;
        //    }
        //}

        NetworkView networkView = await GateOfFusion.Instance.SpawnAsync(allCommodityAsset.NetworkViews[index], position, Quaternion.Euler(rotation));
        LocalView localView = Instantiate(allCommodityAsset.Commodities[index], position, Quaternion.Euler(rotation)).GetComponent<LocalView>();
        //localView.GetComponent<LocalIngrodients>().PutMachine(machineID);
        FindObjectOfType<MachineIDManager>().GetMachine(machineID).UnSetProcessingIngrodient();

        localView.NetworkViewInject(networkView);

        AllSpawn instance = await GateOfFusion.Instance.SpawnAsync(allSpawn);
        await instance.Async();
        GateOfFusion.Instance.Despawn(instance);
        rpc.RPC_CommodityLocalSpawn(index, rotation, position, networkView.GetComponent<NetworkObject>(), machineID);
        networkInformations.Add(new NetworkInformation(networkView, index));
    }

    public async void SpawnNetworkView(int index, Vector3 rotation, Vector3 position)
    {
        //for(int i = 0; i < allCommodityAsset.Commodities.Count; i++)
        //{
        //    Commodity item = allCommodityAsset.Commodities[i];
        //    if(item.CommodityAsset.CommodityID == index)
        //    {
        //        index = i;
        //        break;
        //    }
        //}

        NetworkView networkView = await GateOfFusion.Instance.SpawnAsync(allCommodityAsset.NetworkViews[index], position, Quaternion.Euler(rotation));
        LocalView localView = Instantiate(allCommodityAsset.Commodities[index], position, Quaternion.Euler(rotation)).GetComponent<LocalView>();
        //localView.GetComponent<LocalIngrodients>().PutMachine(machineID);
        localView.NetworkViewInject(networkView);
        AllSpawn instance = await GateOfFusion.Instance.SpawnAsync(allSpawn);
        await instance.Async();
        GateOfFusion.Instance.Despawn(instance);
        rpc.RPC_CommodityLocalSpawn(index, rotation, position, networkView.GetComponent<NetworkObject>());
        networkInformations.Add(new NetworkInformation(networkView, index));
    }

    public void SpawnLocalView(int index, Vector3 rotation, Vector3 position, NetworkView networkView, int machineID)
    {
        LocalView localView = Instantiate(allCommodityAsset.Commodities[index], position, Quaternion.Euler(rotation)).GetComponent<LocalView>();
        //localView.GetComponent<LocalIngrodients>().PutMachine(machineID);
        FindObjectOfType<MachineIDManager>().GetMachine(machineID).UnSetProcessingIngrodient();
        localView.NetworkViewInject(networkView);
        networkInformations.Add(new NetworkInformation(networkView, index));
    }

    public void SpawnLocalView(int index, Vector3 rotation, Vector3 position, NetworkView networkView)
    {
        LocalView localView = Instantiate(allCommodityAsset.Commodities[index], position, Quaternion.Euler(rotation)).GetComponent<LocalView>();
        //localView.GetComponent<LocalIngrodients>().PutMachine(machineID);
        localView.NetworkViewInject(networkView);
        networkInformations.Add(new NetworkInformation(networkView, index));
    }

    public void NewMember(PlayerRef player)
    {
        foreach (NetworkInformation networkInformation in networkInformations)
        {
            Debug.LogError(networkInformation.NetworkView.OneGrab);
            if (!networkInformation.NetworkView.OneGrab)
            {
                rpc.RPC_Joined(player, networkInformation.ID, networkInformation.NetworkView.GetComponent<NetworkObject>());
            }
            else
            {
                rpc.RPC_JoinedOneGrab(player, networkInformation.ID, networkInformation.NetworkView.GetComponent<NetworkObject>());
            }
        }
    }

    public bool ExistInformation(NetworkView networkView)
    {
        return networkInformations.Any((information) => information.NetworkView == networkView);      
    }
    public void Despawn(NetworkView networkView)
    {
        NetworkInformation information = default;
        foreach (NetworkInformation item in networkInformations)
        {
            if (item.NetworkView == networkView)
            {
                information = item;
            }
        }
        if (information == null)
        {
            Debug.LogError("NetworkView‚Ì•sˆê’v");
            return;
        }
        networkInformations.Remove(information);
        Destroy(information.NetworkView.LocalView.gameObject);
    }
}
