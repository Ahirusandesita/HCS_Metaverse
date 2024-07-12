using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public interface IIngrodientsModerator
{
    IngrodientsAsset IngrodientsAsset { set; }
}
/// <summary>
/// ãÔçﬁ
/// </summary>
public class Ingrodients : MonoBehaviour,IIngrodientsModerator, ISwitchableGrabbableActive
{
    [SerializeField]
    private IngrodientsAsset ingrodientsAsset;
    private List<IngrodientsDetailInformation> ingrodientsDetailInformations = new List<IngrodientsDetailInformation>();
    public IngrodientsAsset IngrodientsAsset => ingrodientsAsset;
    private List<MonoBehaviour> interactables = new List<MonoBehaviour>();
    public struct TimeItTakesData
    {
        public readonly float MaxTimeItTakes;
        public readonly float NowTimeItTakes;
        public TimeItTakesData(float max, float now)
        {
            this.MaxTimeItTakes = max;
            this.NowTimeItTakes = now;
        }
    }
    private ReactiveProperty<TimeItTakesData> timeItTakesProperty = new ReactiveProperty<TimeItTakesData>();
    public IReadOnlyReactiveProperty<TimeItTakesData> TimeItTakesProperty => timeItTakesProperty;

    IngrodientsAsset IIngrodientsModerator.IngrodientsAsset
    {
        set
        {
            ingrodientsAsset = value;
        }
    }

    GameObject ISwitchableGrabbableActive.gameObject => throw new NotImplementedException();

    private CommodityFactory commodityFactory;

    private void Awake()
    {
        this.commodityFactory = GameObject.FindObjectOfType<CommodityFactory>();

        foreach (IngrodientsDetailInformation ingrodientsDetailInformation in ingrodientsAsset.IngrodientsDetailInformations)
        {
            ingrodientsDetailInformations.Add(new IngrodientsDetailInformation(ingrodientsDetailInformation.ProcessingType,ingrodientsDetailInformation.TimeItTakes,ingrodientsDetailInformation.Commodity));
        }

        interactables.Add(this.GetComponent<Grabbable>());
        foreach (MonoBehaviour item in this.gameObject.GetComponentsInChildren<DistanceHandGrabInteractable>())
        {
            interactables.Add(item);
        }
        foreach (MonoBehaviour item in this.gameObject.GetComponentsInChildren<DistanceGrabInteractable>())
        {
            interactables.Add(item);
        }
        foreach (MonoBehaviour item in this.gameObject.GetComponentsInChildren<HandGrabInteractable>())
        {
            interactables.Add(item);
        }
        foreach (MonoBehaviour item in this.gameObject.GetComponentsInChildren<GrabInteractable>())
        {
            interactables.Add(item);
        }
    }


    public void Inject(CommodityFactory commodityFactory)
    {
        this.commodityFactory = commodityFactory;
    }

    public bool SubToIngrodientsDetailInformationsTimeItTakes(ProcessingType processableType, float subValue)
    {
        foreach (IngrodientsDetailInformation information in ingrodientsDetailInformations)
        {
            if (information.ProcessingType == processableType)
            {
                information.SubToTimeItTakes(subValue);
                timeItTakesProperty.Value = new TimeItTakesData(information.MaxTimeItTakes, information.TimeItTakes);
                return information.IsProcessingFinish();
            }
        }

        Debug.Log("ProcessingTypeÇ™éwíËäO");
        return default;
    }


    public Commodity ProcessingStart(ProcessingType processingType,Transform machineTransform)
    {
        Commodity commodity = commodityFactory.Generate(this, processingType);
        Instantiate(commodity, this.transform.position, this.transform.rotation)/*.transform.parent = machineTransform*/;
        Destroy(this.gameObject);
        return commodity;
    }

    void ISwitchableGrabbableActive.Active()
    {
        foreach (MonoBehaviour item in interactables)
        {
            item.enabled = true;
        }
    }

    void ISwitchableGrabbableActive.Inactive()
    {
        foreach (MonoBehaviour item in interactables)
        {
            item.enabled = false;
        }
    }
}
