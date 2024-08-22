using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

using Fusion;

public interface IIngrodientsModerator
{
    IngrodientsAsset IngrodientsAsset { set; }
}

/// <summary>
/// ãÔçﬁ
/// </summary>
public class Ingrodients : MonoBehaviour, IIngrodientsModerator, ISwitchableGrabbableActive, IInject<ISwitchableGrabbableActive>
{
    [SerializeField]
    private IngrodientsAsset ingrodientsAsset;
    private List<IngrodientsDetailInformation> ingrodientsDetailInformations = new List<IngrodientsDetailInformation>();
    public IngrodientsAsset IngrodientsAsset => ingrodientsAsset;
    private ISwitchableGrabbableActive switchableGrabbableActive;
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

    private NetworkRunner networkRunner;

    private PointableUnityEventWrapper pointableUnityEventWrapper;
    private StateAuthorityData stateAuthority;
    private void Awake()
    {
        this.commodityFactory = GameObject.FindObjectOfType<CommodityFactory>();

        foreach (IngrodientsDetailInformation ingrodientsDetailInformation in ingrodientsAsset.IngrodientsDetailInformations)
        {
            ingrodientsDetailInformations.Add(new IngrodientsDetailInformation(ingrodientsDetailInformation.ProcessingType, ingrodientsDetailInformation.TimeItTakes, ingrodientsDetailInformation.Commodity));
        }

        pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();

        pointableUnityEventWrapper.WhenSelect.AddListener((data) => GateOfFusion.Instance.Grab(this.GetComponent<NetworkObject>()));
        pointableUnityEventWrapper.WhenUnselect.AddListener((data) => GateOfFusion.Instance.Release(this.GetComponent<NetworkObject>()));

        networkRunner = GateOfFusion.Instance.NetworkRunner;

        this.stateAuthority = this.GetComponent<StateAuthorityData>();
        stateAuthority.OnAuthrity += (data) =>
        {
            if (data.Authrity)
            {
                switchableGrabbableActive.Active();
            }
            else if (data.Authrity)
            {
                switchableGrabbableActive.Inactive();
            }
        };
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


    public Commodity ProcessingStart(ProcessingType processingType, Transform machineTransform)
    {
        Commodity commodity = commodityFactory.Generate(this, processingType);
        Commodity instanceCommodity = networkRunner.Spawn(commodity.gameObject, this.transform.position, this.transform.rotation).GetComponent<Commodity>();//NetworkRunnnerSpawn /*.transform.parent = machineTransform*/;
        networkRunner.Despawn(this.gameObject.GetComponent<NetworkObject>());
        return instanceCommodity;
    }

    void ISwitchableGrabbableActive.Active()
    {
        switchableGrabbableActive.Active();
    }

    void ISwitchableGrabbableActive.Inactive()
    {
        switchableGrabbableActive.Inactive();
    }

    void IInject<ISwitchableGrabbableActive>.Inject(ISwitchableGrabbableActive t)
    {
        this.switchableGrabbableActive = t;
    }
}
