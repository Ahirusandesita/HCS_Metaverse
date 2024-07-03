using System.Collections.Generic;
using UnityEngine;
public interface IProcessable
{
    void Processing(Ingrodients ingrodients);
}
public class NullMachine : IProcessable
{
    void IProcessable.Processing(Ingrodients ingrodients)
    {

    }
}

public abstract class Machine : MonoBehaviour,IProcessable
{
    [SerializeField]
    private ProcessingType processingType;
    public ProcessingType ProcessingType => processingType;

    [SerializeField]
    private Transform ingrodientTransform;

    protected Ingrodients ingrodients;
    protected float timeItTakes;

    void IProcessable.Processing(Ingrodients ingrodients)
    {
        this.ingrodients = ingrodients;
        ingrodients.transform.position = ingrodientTransform.position;
        StartProcessed();
    }

    public IProcessable ProcessedCertification(Ingrodients ingrodients)
    {
        foreach(IngrodientsDetailInformation item in ingrodients.IngrodientsAsset.IngrodientsDetailInformations)
        {
            if(item.ProcessingType == processingType)
            {
                return this;
            }
        }
        Debug.Log("can not processed");
        return new NullMachine();
    }

    public abstract void StartProcessed();
}
