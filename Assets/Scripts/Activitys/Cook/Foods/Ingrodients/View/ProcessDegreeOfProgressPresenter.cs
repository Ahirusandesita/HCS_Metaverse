using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class ProcessDegreeOfProgressPresenter : MonoBehaviour, IAction<float>
{
    [SerializeField]
    private Ingrodients ingrodients;
    [SerializeField]
    ProcessDegreeOfProgressView processDegreeOfProgressView;
    private IPracticableRPCEvent practicableRPCEvent;
    public void Action(float t)
    {
        processDegreeOfProgressView.View(1f - (t));
    }
    public void Inject(IPracticableRPCEvent practicableRPCEvent)
    {
        this.practicableRPCEvent = practicableRPCEvent;
    }
    private void Awake()
    {
        ingrodients.TimeItTakesProperty.Subscribe((data) =>
        {
           practicableRPCEvent.RPC_Event<ProcessDegreeOfProgressPresenter>(this.gameObject, data.NowTimeItTakes / data.MaxTimeItTakes);
        }).AddTo(this);
    }
}