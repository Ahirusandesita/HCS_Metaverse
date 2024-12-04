using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _spawnObject = default;

    [SerializeField]
    private Transform _spawnTransform = default;

    private void Start()
    {
        Leader leader = GameObject.FindObjectOfType<Leader>();

        if (leader.IsLeader)
        {
            try
            {
                if (_spawnTransform != null)
                {
                    GateOfFusion.Instance.SpawnAsync(_spawnObject, _spawnTransform.position, _spawnTransform.rotation).Forget();
                }
                else
                {
                    GateOfFusion.Instance.SpawnAsync(_spawnObject).Forget();
                }
            }
            catch
            {
                GateOfFusion.Instance.SpawnAsync(_spawnObject).Forget();
            }
        }
    }
}
