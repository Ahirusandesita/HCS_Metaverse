using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DishManager : MonoBehaviour
{
    [SerializeField, Tooltip("�M�̃A�Z�b�g")]
    private GameObject _dishAsset = default;

    [SerializeField, Tooltip("�M�̏����X�|�[���n�_")]
    private Transform _spawnTransform = default;

    [SerializeField, Tooltip("�f�o�b�O�p�@�M�������ǂ���")]
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
