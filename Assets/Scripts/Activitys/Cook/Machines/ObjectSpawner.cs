using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Fusion;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _spawnObject = default;

    [SerializeField]
    private Transform _spawnTransform = default;

    private async void Start()
    {
        Leader leader = GameObject.FindObjectOfType<Leader>();

        Debug.Log($"<color=blue>リーダー権限：{leader.IsLeader}</color>");

        if (leader.IsLeader)
        {
            try
            {
                if (_spawnTransform != null)
                {
                    GameObject instance = await GateOfFusion.Instance.SpawnAsync(_spawnObject, _spawnTransform.position, _spawnTransform.rotation);
                    Debug.Log($"<color=blue>スポーンさせたよ：{instance.name}, {instance.GetComponent<NetworkObject>().StateAuthority.PlayerId} </color>");
                }
                else
                {
                    GameObject instance = await GateOfFusion.Instance.SpawnAsync(_spawnObject);
                    Debug.Log($"<color=blue>スポーンさせたよ：{instance.name}, {instance.GetComponent<NetworkObject>().StateAuthority.PlayerId} </color>");
                }
            }
            catch
            {
                GameObject instance = await GateOfFusion.Instance.SpawnAsync(_spawnObject);
                Debug.Log($"<color=blue>スポーンさせたよ：{instance.name}, {instance.GetComponent<NetworkObject>().StateAuthority.PlayerId} </color>");
            }
        }
    }
}
