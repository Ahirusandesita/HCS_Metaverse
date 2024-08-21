using UnityEngine;

namespace HCSMeta.Function
{
    /// <summary>
    /// プーリングするオブジェクトのデータ
    /// </summary>
    [CreateAssetMenu(fileName = "PoolObjectData", menuName = "ScriptableObjects/PoolObjectAsset/Prefab")]
    public class PoolObjectAsset : ScriptableObject
    {
        [Space]
        [SerializeField] private PoolObject prefab = default;
        [SerializeField, Min(0)] private int maxCreateCount = 10;

        public PoolObject Prefab => prefab;
        public int MaxCreateCount => maxCreateCount;
    }
}