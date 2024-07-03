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

[CreateAssetMenu(fileName = "IngrodientAsset", menuName = "ScriptableObjects/Foods/IngrodientsAsset")]
public class IngrodientsAsset : ScriptableObject
{
    /// <summary>
    /// ��ނ̖��O
    /// </summary>
    [SerializeField]
    private IngrodientsType ingrodientsType;
    [SerializeField]
    private List<IngrodientsDetailInformation> ingrodientsDetailInformations = new List<IngrodientsDetailInformation>();

    public IngrodientsType IngrodientsType => ingrodientsType;
    public IReadOnlyList<IngrodientsDetailInformation> IngrodientsDetailInformations => ingrodientsDetailInformations;
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
}
