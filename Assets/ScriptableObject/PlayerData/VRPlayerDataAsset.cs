using UnityEngine;

/// <summary>
/// vC[Ìp[^NXiVRj
/// </summary>
[CreateAssetMenu(fileName = "VRPlayerData", menuName = "ScriptableObjects/PlayerDataAsset/VR")]
public class VRPlayerDataAsset : PlayerDataAssetBase
{
    [Tooltip("ñ]p[]")]
    [SerializeField] private float rotateAngle = 30f;

    /// <summary>
    /// x²Ìñ]p[]
    /// </summary>
    public float RotateAngle => rotateAngle;
}
