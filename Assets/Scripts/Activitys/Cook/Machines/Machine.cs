using System.Collections.Generic;
using UnityEngine;
using Fusion;

public abstract class Machine : NetworkBehaviour
{
    // 
    private LocalView _processingIngrodientsView = default;

    [Tooltip("‰ÁH‚ðs‚Á‚Ä‚¢‚éˆÊ’u")]
    public Transform _machineTransform = default;

    [SerializeField]
    private int _machineID = 1;

    private ProcessingType _processingType = ProcessingType.Bake;

    public int MachineID => _machineID;

    public ProcessingType ProcessType => _processingType;
    
    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public void ProcessEvent()
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

        _processingIngrodientsView.transform.position = _machineTransform.position;
        _processingIngrodientsView.transform.rotation = _machineTransform.rotation;
    }

    public void UnSetProcessingIngrodient()
    {
        _processingIngrodientsView = null;
    }
}