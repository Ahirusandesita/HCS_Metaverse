using System.Collections.Generic;
using UnityEngine;


public class NullMachine : IProcessable
{
    void IProcessable.Processing(Ingrodients ingrodients)
    {

    }
}


public abstract class Machine : MonoBehaviour, IProcessable
{
    [SerializeField]
    private ProcessingType processingType;
    public ProcessingType ProcessingType => processingType;

    [SerializeField]
    protected Transform ingrodientTransform;

    protected Ingrodients ingrodients;
    protected float timeItTakes;

    void IProcessable.Processing(Ingrodients ingrodients)
    {
        this.ingrodients = ingrodients;
        ingrodients.transform.position = ingrodientTransform.position;

        foreach (IngrodientsDetailInformation ingrodientsDetailInformation in ingrodients.IngrodientsAsset.IngrodientsDetailInformations)
        {
            if (ingrodientsDetailInformation.ProcessingType == ProcessingType)
            {
                ISwitchableGrabbableActive switchableGrabbableActive = ingrodients;
                switchableGrabbableActive.Inactive();
                IngrodientsDetailInformation information = new IngrodientsDetailInformation(ingrodientsDetailInformation.ProcessingType, ingrodientsDetailInformation.TimeItTakes, ingrodientsDetailInformation.Commodity);
                StartProcessed(information);
            }
        }
    }

    public IProcessable ProcessedCertification(Ingrodients ingrodients)
    {
        foreach (IngrodientsDetailInformation item in ingrodients.IngrodientsAsset.IngrodientsDetailInformations)
        {
            if (item.ProcessingType == processingType)
            {
                return this;
            }
        }
        Debug.Log("can not processed");
        return new NullMachine();
    }

    public abstract void StartProcessed(IngrodientsDetailInformation ingrodientsDetailInformation);
}