using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class KnifeSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject _knife = default;

    private void Start()
    {
        GateOfFusion.Instance.SpawnAsync(_knife).Forget();
    }
}
