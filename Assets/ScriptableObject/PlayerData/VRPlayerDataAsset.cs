using UnityEngine;

/// <summary>
/// �v���C���[�̃p�����[�^�N���X�iVR�j
/// </summary>
[CreateAssetMenu(fileName = "VRPlayerData", menuName = "ScriptableObjects/PlayerDataAsset/VR")]
public class VRPlayerDataAsset : PlayerDataAssetBase
{
    [Header("Camera")]
    [Tooltip("x���̉�]�p[��]")]
    [SerializeField] private float xRotateAngle = 15f;

    /// <summary>
    /// x���̉�]�p[��]
    /// </summary>
    public float XRotateAngle => xRotateAngle;
}
