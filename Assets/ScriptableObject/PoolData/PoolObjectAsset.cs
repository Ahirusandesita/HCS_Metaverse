using UnityEngine;

/// <summary>
/// �v�[�����O����I�u�W�F�N�g�̃f�[�^
/// </summary>
[CreateAssetMenu(fileName = "PoolObjectData", menuName = "ScriptableObjects/PoolObjectData")]
public class PoolObjectAsset : ScriptableObject
{
    [Space]
    [SerializeField] private PoolObject prefab = default;
    [SerializeField, Min(0)] private int maxCreateCount = 10;

    public PoolObject Prefab => prefab;
    public int MaxCreateCount => maxCreateCount;
}

/// <summary>
/// �v�[�����O����G�t�F�N�g�̃f�[�^
/// </summary>
[CreateAssetMenu(fileName = "PoolEffectData", menuName = "ScriptableObjects/PoolEffectData")]
public class PoolEffectAsset : ScriptableObject
{
    [Space]
    [SerializeField] private ParticleSystem particle = default;
    [SerializeField, Min(0)] private int maxCreateCount = 10;

    public ParticleSystem Partilce => particle;
    public int MaxCreateCount => maxCreateCount;
}