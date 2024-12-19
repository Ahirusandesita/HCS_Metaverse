using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class Machine : NetworkBehaviour, IManualProcess
{
    // 
    protected LocalView _processingIngrodientsView = default;

    [SerializeField]
    protected int _machineID = 1;

    [SerializeField]
    protected float _processingValue = 1f;

    [SerializeField]
    protected Transform _processerTransform = default;

    [SerializeField]
    protected ProcessingType _processingType = ProcessingType.Bake;

    public int MachineID => _machineID;

    public float ProcessingValue => _processingValue;

    public Transform ProcesserTransform => _processerTransform;

    public ProcessingType ProcessType => _processingType;
    
    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public void ManualProcessEvent()
    {
        if (_processingIngrodientsView == default)
        {
            return;
        }

        _processingIngrodientsView.NetworkView.GetComponent<NetworkIngrodients>().RPC_ManualProcess();
    }

    public void SetProcessingIngrodient(LocalView setIngrodientsView)
    {
        _processingIngrodientsView = setIngrodientsView;

        _processingIngrodientsView.transform.position = ProcesserTransform.position;
        _processingIngrodientsView.transform.rotation = ProcesserTransform.rotation;
    }

    public void UnSetProcessingIngrodient()
    {
        _processingIngrodientsView = default;
    }
}