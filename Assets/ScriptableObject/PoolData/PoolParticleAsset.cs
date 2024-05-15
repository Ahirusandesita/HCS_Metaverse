using UnityEngine;

/// <summary>
/// プーリングするエフェクトのデータ
/// </summary>
[CreateAssetMenu(fileName = "PoolParticleData", menuName = "ScriptableObjects/PoolObjectAsset/Particle")]
public class PoolParticleAsset : ScriptableObject
{
    [Space]
    [SerializeField] private ParticleSystem particle = default;
    [SerializeField, Min(0)] private int maxCreateCount = 10;

    public ParticleSystem Partilce => particle;
    public int MaxCreateCount => maxCreateCount;
}