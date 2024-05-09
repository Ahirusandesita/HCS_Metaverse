using UnityEngine;

/// <summary>
/// �v���C���[�̃p�����[�^�N���X�iVR�j
/// </summary>
[CreateAssetMenu(fileName = "VRPlayerData", menuName = "ScriptableObjects/PlayerDataAsset/VR")]
public class VRPlayerDataAsset : PlayerDataAssetBase
{
    [Header("Camera")]
    [Tooltip("��]�p[��]")]
    [SerializeField] private float rotateAngle = 30f;

    /// <summary>
    /// x���̉�]�p[��]
    /// </summary>
    public float RotateAngle => rotateAngle;
}
