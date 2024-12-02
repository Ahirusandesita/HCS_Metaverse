using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DishManager : MonoBehaviour
{
    [SerializeField, Tooltip("皿のアセット")]
    private GameObject _dishAsset = default;

    [SerializeField, Tooltip("皿の初期スポーン地点")]
    private Transform _spawnTransform = default;

    [SerializeField, Tooltip("デバッグ用　皿消すかどうか")]
    private bool _doDeleteDish = true;

    public async void InstanceNewDish(NetworkObject dishObject)
    {
        if (!_doDeleteDish)
        {
            return;
        }

        GateOfFusion gateOfFusion = new GateOfFusion();

        GameObject newDishObject = await gateOfFusion.SpawnAsync(_dishAsset);

        gateOfFusion.Despawn<NetworkObject>(dishObject);

        dishObject = newDishObject.GetComponent<NetworkObject>();

        dishObject.transform.position = _spawnTransform.position;
        dishObject.transform.rotation = _spawnTransform.rotation;
    }
}
