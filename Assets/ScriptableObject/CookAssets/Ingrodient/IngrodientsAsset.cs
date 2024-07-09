using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ProcessingType
{
    /// <summary>
    /// èƒÇ≠
    /// </summary>
    Bake,
    /// <summary>
    /// êÿÇÈ
    /// </summary>
    Cut,
    /// <summary>
    /// ógÇ∞ÇÈ
    /// </summary>
    Fry,
    /// <summary>
    /// êUÇÈ
    /// </summary>
    Shake,
    /// <summary>
    /// êÜÇ≠
    /// </summary>
    Boil,
    /// <summary>
    /// ç¨Ç∫ÇÈ
    /// </summary>
    Mix
}

public interface IIngrodientAsset
{
    void SetUp(List<IngrodientsDetailInformation> ingrodientsDetailInformation);
}
[CreateAssetMenu(fileName = "IngrodientAsset", menuName = "ScriptableObjects/Foods/IngrodientsAsset")]
public class IngrodientsAsset : ScriptableObject,IIngrodientAsset
{
    public IngrodientsAsset(IngrodientsAsset coppyAsset)
    {
        this.ingrodientsType = coppyAsset.ingrodientsType;
        this.ingrodientsDetailInformations = coppyAsset.ingrodientsDetailInformations;
    }

    /// <summary>
    /// ãÔçﬁÇÃñºëO
    /// </summary>
    [SerializeField]
    private IngrodientsType ingrodientsType;
    [SerializeField]
    private List<IngrodientsDetailInformation> ingrodientsDetailInformations = new List<IngrodientsDetailInformation>();

    public IngrodientsType IngrodientsType => ingrodientsType;
    public IReadOnlyList<IngrodientsDetailInformation> IngrodientsDetailInformations => ingrodientsDetailInformations;

    void IIngrodientAsset.SetUp(List<IngrodientsDetailInformation> ingrodientsDetailInformations)
    {
        this.ingrodientsDetailInformations = ingrodientsDetailInformations;
    }

    public bool SubToIngrodientsDetailInformationsTimeItTakes(ProcessingType processableType, float subValue)
    {
        foreach (IngrodientsDetailInformation information in ingrodientsDetailInformations)
        {
            if (information.ProcessingType == processableType)
            {
                return information.SubToTimeItTakes(subValue);
            }
        }

        Debug.Log("ProcessingTypeÇ™éwíËäO");
        return default;
    }
}

/// <summary>
/// ãÔçﬁÇÃè⁄ç◊èÓïÒ
/// </summary>
[System.Serializable]
public class IngrodientsDetailInformation
{
    /// <summary>
    /// â¡çHâ¬î\Ç»É^ÉCÉv
    /// </summary>
    [SerializeField]
    private ProcessingType processableType;
    /// <summary>
    /// â¡çHÇ…ä|Ç©ÇÈéûä‘
    /// </summary>
    [SerializeField]
    private float timeItTakes;

    /// <summary>
    /// â¡çHå„ÇÃäÆê¨ïi
    /// </summary>
    [SerializeField]
    private Commodity commodity;

    public ProcessingType ProcessingType => processableType;
    public float TimeItTakes => timeItTakes;
    public Commodity Commodity => commodity;

    public IngrodientsDetailInformation(ProcessingType processableType,float timeItTakes,Commodity commodity)
    {
        this.processableType = processableType;
        this.timeItTakes = timeItTakes;
        this.commodity = commodity;
    }

    public bool SubToTimeItTakes(float subValue)
    {
        timeItTakes -= subValue;

        if (timeItTakes <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
