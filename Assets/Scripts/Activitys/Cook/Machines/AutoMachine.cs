using System;
using UnityEngine;
public class AutoMachine : Machine,IAction<Ingrodients>,IAction
{
    private bool canProcessing = false;
    public bool CanProcessing
    {
        get
        {
            return canProcessing;
        }
        set
        {
            canProcessing = value;
        }
    }

    public bool isGrab = false;
    public bool IsGrab
    {
        get
        {
            return isGrab;
        }
        set
        {
            isGrab = value;
        }
    }

    private Action processingAction;
    private IPracticableRPCEvent practicableRPCEvent;
    private void Update()
    {
        processingAction?.Invoke();
    }
    public override void StartProcessed(IngrodientsDetailInformation ingrodientsDetailInformation)
    {
        //timeItTakes = ingrodientsDetailInformation.TimeItTakes;
        ingrodients.transform.parent = ingrodientTransform;
        //testRPC
        practicableRPCEvent.RPC_Event<AutoMachine, Ingrodients>(this.gameObject, ingrodients.gameObject);

        processingAction += () =>
        {
            if (!canProcessing)
            {
                return;
            }

            if (ingrodients.SubToIngrodientsDetailInformationsTimeItTakes(ProcessingType, Time.deltaTime))
            {
                Commodity commodity = ingrodients.ProcessingStart(ProcessingType, this.transform);
                commodity.transform.parent = ingrodientTransform;
                commodity.OnPointable += (eventArgs) =>
                {
                    if (eventArgs.GrabType == GrabType.Grab)
                    {
                        commodity.Grab();
                    }
                };
                processingAction = null;
                Debug.Log("â¡çHäÆóπ");
            }
        };
    }

    //testRPC
    /// <summary>
    /// â¡çHíÜíf
    /// </summary>
    public void ProcessingInterruption()
    {
        practicableRPCEvent.RPC_Event<AutoMachine>(this.gameObject);
    }

    public void Action(Ingrodients t)
    {
        ingrodients = t;
        ingrodients.transform.parent = ingrodientTransform;
    }

    public void Inject(IPracticableRPCEvent practicableRPCEvent)
    {
        this.practicableRPCEvent = practicableRPCEvent;
    }

    public void Action()
    {
        processingAction = null;
        ingrodients.transform.parent = null;
    }
}