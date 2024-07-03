using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ProcessingType
{
    /// <summary>
    /// Ä‚­
    /// </summary>
    Bake,
    /// <summary>
    /// Ø‚é
    /// </summary>
    Cut,
    /// <summary>
    /// —g‚°‚é
    /// </summary>
    Fry,
    /// <summary>
    /// U‚é
    /// </summary>
    Shake,
    /// <summary>
    /// †‚­
    /// </summary>
    Boil,
    /// <summary>
    /// ¬‚º‚é
    /// </summary>
    Mix
}

[CreateAssetMenu(fileName = "IngrodientAsset", menuName = "ScriptableObjects/Foods/IngrodientsAsset")]
public class IngrodientsAsset : ScriptableObject
{
    /// <summary>
    /// ‹ïŞ‚Ì–¼‘O
    /// </summary>
    [SerializeField]
    private IngrodientsType ingrodientsType;
    [SerializeField]
    private List<IngrodientsDetailInformation> ingrodientsDetailInformations = new List<IngrodientsDetailInformation>();

    public IngrodientsType IngrodientsType => ingrodientsType;
    public IReadOnlyList<IngrodientsDetailInformation> IngrodientsDetailInformations => ingrodientsDetailInformations;
}

/// <summary>
/// ‹ïŞ‚ÌÚ×î•ñ
/// </summary>
[System.Serializable]
public class IngrodientsDetailInformation
{
    /// <summary>
    /// ‰ÁH‰Â”\‚Èƒ^ƒCƒv
    /// </summary>
    [SerializeField]
    private ProcessingType processableType;
    /// <summary>
    /// ‰ÁH‚ÉŠ|‚©‚éŠÔ
    /// </summary>
    [SerializeField]
    private float timeItTakes;

    /// <summary>
    /// ‰ÁHŒã‚ÌŠ®¬•i
    /// </summary>
    [SerializeField]
    private Commodity commodity;

    public ProcessingType ProcessingType => processableType;
    public float TimeItTakes => timeItTakes;
    public Commodity Commodity => commodity;
}
