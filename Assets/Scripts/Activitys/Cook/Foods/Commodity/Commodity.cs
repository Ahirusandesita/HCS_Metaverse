using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Fusion;
using Cysharp.Threading.Tasks;

public class Commodity : MonoBehaviour, ICommodityModerator, IInject<ISwitchableGrabbableActive>, IGrabbableActiveChangeRequester
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

    private LocalView _localView = default;
    public LocalView LocalView => _localView;

    [SerializeField]
    private StateAuthorityData stateAuthority;
    private NetworkRunner networkRunner;

    private bool isGrab = false;

    private void Awake()
    {
        pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();

        pointableUnityEventWrapper.WhenSelect.AddListener((data) => OnPointable?.Invoke(new GrabEventArgs(GrabType.Grab)));
        pointableUnityEventWrapper.WhenUnselect.AddListener((data) => OnPointable?.Invoke(new GrabEventArgs(GrabType.UnGrab)));

        if (!GetComponent<Ingrodients>())
        {
            pointableUnityEventWrapper.WhenSelect.AddListener((data) => GetComponent<LocalView>().Grab());
            pointableUnityEventWrapper.WhenUnselect.AddListener((data) => GetComponent<LocalView>().Release());

            pointableUnityEventWrapper.WhenSelect.AddListener((data) => isGrab = true);
            pointableUnityEventWrapper.WhenUnhover.AddListener((data) => isGrab = false);
        }


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

    private void FixedUpdate()
    {
        if (isGrab)
        {
            GetComponent<LocalView>().NetworkView.RPC_Position(this.transform.position, this.transform.rotation.eulerAngles);
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (!GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            return;
        }

        if (collision.transform.root.transform.GetComponentInChildren<Commodity>())
        {
            Commodity collisionCommodity = collision.transform.root.transform.GetComponentInChildren<Commodity>();
            FoodSpawnManagerRPC foodSpawnManagerRPC = GameObject.FindObjectOfType<FoodSpawnManagerRPC>();
            if (CommodityAsset.CommodityID > collisionCommodity.CommodityAsset.CommodityID)
            {
                

                _localView.NetworkView.GetComponent<NetworkCommodity>().RPC_MixCommodity(collision.gameObject.GetComponent<NetworkObject>(), 1);
            }
        }

        if (collision.gameObject.TryGetComponent<SubmisionTable>(out SubmisionTable table))
        {
            table.Submit(this);
        }
        

        //if (collision.transform.root.gameObject.TryGetComponent<IPutableOnDish>(out IPutableOnDish putableOnDish))
        //{
        //    Debug.Log("put:commodity");

        //    isOnDish = true;
        //    this.putableOnDish = putableOnDish;
        //    this.putableOnDish.Rpc_PutCommodity(GetComponent<NetworkObject>());

        //    //RPCEvents.RPC_Event<Commodity>(this.GetComponent<NetworkObject>(), collision.transform.root.gameObject.GetComponent<NetworkObject>());
        //}
    }

    [Rpc]
    public void RPC_MixCommodity(NetworkObject hitObject, int commodityID)
    {
            FoodSpawnManagerRPC foodSpawnManagerRPC = GameObject.FindObjectOfType<FoodSpawnManagerRPC>();
            NetworkObject networkObject = GetComponent<LocalView>().NetworkView.GetComponent<NetworkObject>();
            // ----------------------------- ID ------------------------------------------
            foodSpawnManagerRPC.RPC_CommoditySpawn(commodityID, transform.rotation.eulerAngles, transform.position);
            // ---------------------------------------------------------------------------
            foodSpawnManagerRPC.RPC_Despawn(networkObject);
            Destroy(gameObject);
    }

    //public void PutOnDish(IPutableOnDish putableOnDish, bool isOnDish)
    //{
    //    this.isOnDish = isOnDish;
    //    this.putableOnDish = putableOnDish;
    //    this.putableOnDish.Rpc_PutCommodity(GetComponent<NetworkObject>());
    //}

    public void Inject(ISwitchableGrabbableActive t)
    {
        this.switchableGrabbableActive = t;
        this.switchableGrabbableActive.Regist(this);
    }
}