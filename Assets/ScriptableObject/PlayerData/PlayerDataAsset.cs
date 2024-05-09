using UnityEngine;

/// <summary>
/// �v���C���[�̃p�����[�^�N���X�iPC)
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataAsset/Normal")]
public class PlayerDataAsset : PlayerDataAssetBase
{
    [Header("Camera")]
    [Tooltip("�v���C���[�̉�]���x")]
    [SerializeField] private float rotationSpeed = 1.0f;
    [Tooltip("�J�����̏���p�x")]
    [SerializeField] private float verticalMaxAngle = 90.0f;
    [Tooltip("�J�����̉����p�x")]
    [SerializeField] private float verticalMinAngle = -90.0f;

    /// <summary>
    /// �v���C���[�̉�]���x
    /// </summary>
    public float RotationSpeed => rotationSpeed;
    /// <summary>
    /// �J�����̏���p�x
    /// </summary>
    public float VerticalMaxAngle => verticalMaxAngle;
    /// <summary>
    /// �J�����̉����p�x
    /// </summary>
    public float VerticalMinAngle => verticalMinAngle;
}
