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

    private List<NetworkInformation> networkInformations = new List<NetworkInformation>();

    public async void SpawnNetworkView(int index, Vector3 rotation, Vector3 position)
    {
        NetworkView networkView = await GateOfFusion.Instance.SpawnAsync(allCommodityAsset.NetworkViews[index], position, Quaternion.Euler(rotation));
        LocalView localView = Instantiate(allCommodityAsset.Commodities[index], position, Quaternion.Euler(rotation)).GetComponent<LocalView>();

        localView.NetworkViewInject(networkView);
        rpc.RPC_CommodityLocalSpawn(index, rotation, position, networkView.GetComponent<NetworkObject>());
        networkInformations.Add(new NetworkInformation(networkView, index));
    }

    public void SpawnLocalView(int index, Vector3 rotation, Vector3 position, NetworkView networkView)
    {
        LocalView localView = Instantiate(allCommodityAsset.Commodities[index], position, Quaternion.Euler(rotation)).GetComponent<LocalView>();
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

    public void Despawn(NetworkView networkView)
    {
        networkInformations.Remove(networkInformations.Where((information) => information.NetworkView == networkView).First());
    }
}
