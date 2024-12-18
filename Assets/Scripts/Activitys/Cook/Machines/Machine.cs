using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class Machine : NetworkBehaviour, IManualProcess
{
    // 
    private LocalView _processingIngrodientsView = default;

    [SerializeField]
    private int _machineID = 1;

    [SerializeField]
    private float _processingValue = 1f;

    [SerializeField]
    private Transform _processerTransform = default;

    [SerializeField]
    private ProcessingType _processingType = ProcessingType.Bake;

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
        if (_processingIngrodientsView == null)
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
        _processingIngrodientsView = null;
    }
}