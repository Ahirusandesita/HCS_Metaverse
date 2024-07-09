using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ProcessingType
{
    /// <summary>
    /// �Ă�
    /// </summary>
    Bake,
    /// <summary>
    /// �؂�
    /// </summary>
    Cut,
    /// <summary>
    /// �g����
    /// </summary>
    Fry,
    /// <summary>
    /// �U��
    /// </summary>
    Shake,
    /// <summary>
    /// ����
    /// </summary>
    Boil,
    /// <summary>
    /// ������
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
    /// ��ނ̖��O
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

        Debug.Log("ProcessingType���w��O");
        return default;
    }
}

/// <summary>
/// ��ނ̏ڍ׏��
/// </summary>
[System.Serializable]
public class IngrodientsDetailInformation
{
    /// <summary>
    /// ���H�\�ȃ^�C�v
    /// </summary>
    [SerializeField]
    private ProcessingType processableType;
    /// <summary>
    /// ���H�Ɋ|���鎞��
    /// </summary>
    [SerializeField]
    private float timeItTakes;

    /// <summary>
    /// ���H��̊����i
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
