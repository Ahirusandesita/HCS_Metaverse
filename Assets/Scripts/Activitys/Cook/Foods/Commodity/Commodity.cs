using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Fusion;
using Cysharp.Threading.Tasks;

public class Commodity : MonoBehaviour, ICommodityModerator, IInject<ISwitchableGrabbableActive>,IGrabbableActiveChangeRequester
{
    [SerializeField]
    private CommodityAsset commodityAsset;
    public CommodityAsset CommodityAsset => this.commodityAsset;

    private IPutableOnDish putableOnDish = new NullPutableOnDish();
    private bool isOnDish;
    public bool IsOnDish => isOnDish;

    private ISwitchableGrabbableActive switchableGrabbableActive;

    private PointableUnityEventWrapper pointableUnityEventWrapper;
    public event PointableHandler OnPointable;
    private GrabObjectScale grabObjectScale;

    [SerializeField]
    private StateAuthorityData stateAuthority;
    private NetworkRunner networkRunner;

    private void Awake()
    {
        pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();

        pointableUnityEventWrapper.WhenSelect.AddListener((data) => OnPointable?.Invoke(new GrabEventArgs(GrabType.Grab)));
        pointableUnityEventWrapper.WhenUnselect.AddListener((data) => OnPointable?.Invoke(new GrabEventArgs(GrabType.UnGrab)));

        pointableUnityEventWrapper.WhenSelect.AddListener((data) => GateOfFusion.Instance.Grab(this.GetComponent<NetworkObject>()).Forget());
        pointableUnityEventWrapper.WhenUnselect.AddListener((data) => GateOfFusion.Instance.Release(this.GetComponent<NetworkObject>()));


        grabObjectScale = new GrabObjectScale();
        grabObjectScale.StartSize = this.transform.lossyScale;

        //this.stateAuthority = this.GetComponent<StateAuthorityData>();
        //stateAuthority.OnAuthrity += (data) =>
        //{
        //    if (data.Authrity)
        //    {
        //        switchableGrabbableActive.Active(this);
        //    }
        //    else if (!data.Authrity)
        //    {
        //        switchableGrabbableActive.Inactive(this);
        //    }
        //};

        networkRunner = GateOfFusion.Instance.NetworkRunner;
    }

    public void Grab()
    {
        this.transform.parent = null;
        this.transform.localScale = grabObjectScale.StartSize;
    }

    public void InjectPutableOnDish(IPutableOnDish putableOnDish)
    {
        isOnDish = false;
        this.putableOnDish = putableOnDish;
    }

    void ICommodityModerator.SetCommodityAsset(CommodityAsset commodityAsset)
    {
        this.commodityAsset = commodityAsset;
    }
    public bool IsMatchCommodity(CommodityAsset commodityAsset)
    {
        if (this.commodityAsset.CommodityID == commodityAsset.CommodityID)
        {
            return true;
        }
        return false;
    }

    public bool CanInstanceCommodity(Commodity[] commodities)
    {
        if (commodityAsset.Commodities.Count != commodities.Length)
        {
            return false;
        }

        List<Commodity> targetCommodity = new List<Commodity>();

        foreach (Commodity commodity in commodities)
        {
            targetCommodity.Add(commodity);
        }

        foreach (Commodity commodity in commodityAsset.Commodities)
        {
            foreach (Commodity target in targetCommodity)
            {
                if (commodity.CommodityAsset.CommodityID == target.CommodityAsset.CommodityID)
                {
                    targetCommodity.Remove(target);
                    break;
                }
            }
        }
        if (targetCommodity.Count == 0)
        {
            return true;
        }
        return false;
    }

    private async void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.transform.GetComponentInChildren<Commodity>())
        {
            Commodity collisionCommodity = collision.transform.root.transform.GetComponentInChildren<Commodity>();
            if (CommodityAsset.CommodityID > collisionCommodity.CommodityAsset.CommodityID)
            {
                Commodity mixCommodity = MixCommodity.Mix(new Commodity[] { this, collisionCommodity });
                if (!(mixCommodity is null))
                {
                    if (collisionCommodity.IsOnDish)
                    {
                        this.putableOnDish = collisionCommodity.putableOnDish;
                    }
                    this.putableOnDish.CommodityReset();
                    NetworkObject networkObject = await networkRunner.SpawnAsync(mixCommodity.gameObject, this.transform.position, this.transform.rotation);
                    Commodity createCommodity = networkObject.GetComponent<Commodity>();
                    createCommodity.PutOnDish(this.putableOnDish, isOnDish);
                    createCommodity.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }

        if (collision.gameObject.TryGetComponent<SubmisionTable>(out SubmisionTable table))
        {
            //table.Sub(this);
        }

        if (collision.transform.root.gameObject.TryGetComponent<IPutableOnDish>(out IPutableOnDish putableOnDish))
        {
            Debug.Log("put:commodity");

            isOnDish = true;
            this.putableOnDish = putableOnDish;
            this.putableOnDish.Rpc_PutCommodity(GetComponent<NetworkObject>());

            //RPCEvents.RPC_Event<Commodity>(this.GetComponent<NetworkObject>(), collision.transform.root.gameObject.GetComponent<NetworkObject>());
        }
    }

    public void PutOnDish(IPutableOnDish putableOnDish, bool isOnDish)
    {
        this.isOnDish = isOnDish;
        this.putableOnDish = putableOnDish;
        this.putableOnDish.Rpc_PutCommodity(GetComponent<NetworkObject>());
    }
    public void Inject(ISwitchableGrabbableActive t)
    {
        this.switchableGrabbableActive = t;
        this.switchableGrabbableActive.Regist(this);
    }
}