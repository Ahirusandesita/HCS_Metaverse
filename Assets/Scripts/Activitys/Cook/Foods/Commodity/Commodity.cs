using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System;
using Fusion;
using Cysharp.Threading.Tasks;

using HCSMeta.Activity.Cook.Interface;
using HCSMeta.Player.VR.Interface;
using HCSMeta.Function.Injection;

namespace HCSMeta.Activity.Cook.Interface
{
    public interface ICommodityModerator
    {
        void SetCommodityAsset(CommodityAsset commodityAsset);
    }
}


public interface IPutable
{
    void Put();
}
public enum GrabType
{
    Grab,
    UnGrab
}
public class GrabEventArgs : System.EventArgs
{
    public readonly GrabType GrabType;
    public GrabEventArgs(GrabType grabType)
    {
        this.GrabType = grabType;
    }
}
public delegate void PointableHandler(GrabEventArgs grabEventArgs);

namespace HCSMeta.Activity.Cook
{
    public class Commodity : MonoBehaviour, ICommodityModerator, ISwitchableGrabbableActive, IInject<ISwitchableGrabbableActive>
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

            pointableUnityEventWrapper.WhenSelect.AddListener((data) => GateOfFusion.Instance.Grab(this.GetComponent<NetworkObject>()));
            pointableUnityEventWrapper.WhenUnselect.AddListener((data) => GateOfFusion.Instance.Release(this.GetComponent<NetworkObject>()));

            grabObjectScale = new GrabObjectScale();
            grabObjectScale.StartSize = this.transform.lossyScale;

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
                isOnDish = true;
                this.putableOnDish = putableOnDish;
                this.putableOnDish.PutCommodity(this);
            }
        }

        public void Active()
        {
            switchableGrabbableActive.Active();
        }

        public void Inactive()
        {
            switchableGrabbableActive.Inactive();
        }
        public void PutOnDish(IPutableOnDish putableOnDish, bool isOnDish)
        {
            this.isOnDish = isOnDish;
            this.putableOnDish = putableOnDish;
            this.putableOnDish.PutCommodity(this);
        }

        public void Inject(ISwitchableGrabbableActive t)
        {
            this.switchableGrabbableActive = t;
        }
    }
}