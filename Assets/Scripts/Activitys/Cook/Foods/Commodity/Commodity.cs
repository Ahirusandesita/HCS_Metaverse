using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Fusion;
using Cysharp.Threading.Tasks;

public class Commodity : MonoBehaviour, ICommodityModerator, IInject<ISwitchableGrabbableActive>, IGrabbableActiveChangeRequester
{
    [SerializeField]
    private CommodityAsset commodityAsset;
    [SerializeField]
    private GameObject mixParticle;
    public CommodityAsset CommodityAsset => this.commodityAsset;

    private IPutableOnDish putableOnDish = new NullPutableOnDish();
    private bool isOnDish;
    public bool IsOnDish => isOnDish;

    private bool canMix = true;
    public bool CanMix => canMix;

    private ISwitchableGrabbableActive switchableGrabbableActive;

    private PointableUnityEventWrapper _pointableUnityEventWrapper;
    public event PointableHandler OnPointable;
    private GrabObjectScale _grabObjectScale;

    private LocalView _localView = default;
    public LocalView LocalView => _localView;

    private CookActivitySound _sound = default;

    [SerializeField]
    private StateAuthorityData _stateAuthority;
    private NetworkRunner _networkRunner;

    private bool isGrab = false;

    private void Awake()
    {
        _pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();
        _localView = GetComponent<LocalView>();

        _pointableUnityEventWrapper.WhenSelect.AddListener((data) => OnPointable?.Invoke(new GrabEventArgs(GrabType.Grab)));
        _pointableUnityEventWrapper.WhenUnselect.AddListener((data) => OnPointable?.Invoke(new GrabEventArgs(GrabType.UnGrab)));

        if (!GetComponent<Ingrodients>())
        {
            _pointableUnityEventWrapper.WhenSelect.AddListener((data) => GetComponent<LocalView>().Grab());
            _pointableUnityEventWrapper.WhenUnselect.AddListener((data) => GetComponent<LocalView>().Release());

            _pointableUnityEventWrapper.WhenSelect.AddListener((data) => isGrab = true);
            _pointableUnityEventWrapper.WhenUnhover.AddListener((data) => isGrab = false);
        }


        _grabObjectScale = new GrabObjectScale();
        _grabObjectScale.StartSize = this.transform.lossyScale;

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

        _networkRunner = GateOfFusion.Instance.NetworkRunner;
        _sound = FindObjectOfType<CookActivitySound>();
    }

    public void Grab()
    {
        this.transform.parent = null;
        this.transform.localScale = _grabObjectScale.StartSize;
    }

    private void FixedUpdate()
    {
        // NetworkViewとの座標同期
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
        // 接続後かつホストの時 でなければ終了
        if (!GateOfFusion.Instance.IsActivityConnected || !GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
            return;
        }

        // 接触したCollisionがCommodityを持つかどうか
        if (collision.transform.root.transform.GetComponentInChildren<Commodity>())
        {
            // Commodityをキャッシュ
            Commodity collisionCommodity = collision.transform.root.transform.GetComponentInChildren<Commodity>();

            // 組み合わせ可能な状態かどうか
            if (canMix && collisionCommodity.CanMix)
            {
                // CommodityIDが大きい方を優先して実行
                if (CommodityAsset.CommodityID > collisionCommodity.CommodityAsset.CommodityID)
                {
                    // 組み合わせ後のCommodityの種類を取得
                    Commodity mixCommodity = MixCommodity.Mix(new Commodity[] { this, collisionCommodity });

                    // 組み合わせ後のCommodityが存在した場合のみ実行
                    if (!(mixCommodity is null))
                    {
                        _sound.RPC_PlayOneShotSE(CookActivitySound.SEName_Cook.mix, transform.position);

                        // commodityFactory
                        CommodityFactory commodityFactory = GameObject.FindObjectOfType<CommodityFactory>();
                        _localView.NetworkView.GetComponent<NetworkCommodity>().RPC_MixCommodity(collisionCommodity.LocalView.NetworkView.GetComponent<NetworkObject>(), commodityFactory.CommodityIndex(mixCommodity));
                        canMix = false;
                        collisionCommodity.canMix = false;
                    }
                }
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

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_MixCommodity(NetworkObject hitObject, int commodityID)
    {
        Instantiate(mixParticle, transform.position, transform.rotation);
        FoodSpawnManagerRPC foodSpawnManagerRPC = GameObject.FindObjectOfType<FoodSpawnManagerRPC>();
        NetworkObject networkObject = GetComponent<LocalView>().NetworkView.GetComponent<NetworkObject>();
        foodSpawnManagerRPC.RPC_CommoditySpawn(commodityID, transform.rotation.eulerAngles, transform.position);
        foodSpawnManagerRPC.RPC_Despawn(networkObject);
        foodSpawnManagerRPC.RPC_Despawn(hitObject);
    }

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_Destroy()
    {
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