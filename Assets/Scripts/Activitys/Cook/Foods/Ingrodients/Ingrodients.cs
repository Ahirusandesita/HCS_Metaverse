using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Oculus.Interaction;
using Fusion;
using Cysharp.Threading.Tasks;


using UnityEditor;
using UnityEngine.UI;
/// <summary>
/// 具材
/// </summary>
public class Ingrodients : MonoBehaviour, IIngrodientsModerator, IInject<ISwitchableGrabbableActive>,IGrabbableActiveChangeRequester
{
    public bool IsGrabed = false;


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

    private CommodityFactory commodityFactory;

    private NetworkRunner networkRunner;

    private PointableUnityEventWrapper pointableUnityEventWrapper;
    private StateAuthorityData stateAuthority;

    private bool isGrab = false;
    private void Awake()
    {
        this.commodityFactory = GameObject.FindObjectOfType<CommodityFactory>();

        foreach (IngrodientsDetailInformation ingrodientsDetailInformation in ingrodientsAsset.IngrodientsDetailInformations)
        {
            ingrodientsDetailInformations.Add(new IngrodientsDetailInformation(ingrodientsDetailInformation.ProcessingType, ingrodientsDetailInformation.TimeItTakes, ingrodientsDetailInformation.Commodity));
        }

        pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();

        //pointableUnityEventWrapper.WhenSelect.AddListener((data) => GateOfFusion.Instance.Grab(this.GetComponent<NetworkObject>()).Forget());
        //pointableUnityEventWrapper.WhenUnselect.AddListener((data) => GateOfFusion.Instance.Release(this.GetComponent<NetworkObject>()));
        networkRunner = GateOfFusion.Instance.NetworkRunner;

        pointableUnityEventWrapper.WhenSelect.AddListener((data) => GetComponent<LocalView>().Grab());
        pointableUnityEventWrapper.WhenUnselect.AddListener((data) => GetComponent<LocalView>().Release());

        pointableUnityEventWrapper.WhenSelect.AddListener((data) => isGrab = true);
        pointableUnityEventWrapper.WhenUnhover.AddListener((data) => isGrab = false);
        //this.stateAuthority = this.GetComponent<StateAuthorityData>();
        //stateAuthority.OnAuthrity += (data) =>
        //{
        //    if (data.Authrity)
        //    {
        //        IsGrabed = true;
        //        switchableGrabbableActive.Active(this);
        //    }
        //    else if (!data.Authrity)
        //    {
        //        IsGrabed = true;
        //        switchableGrabbableActive.Inactive(this);
        //    }
        //};
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

        Debug.Log("ProcessingTypeが指定外");
        return default;
    }


    public Commodity ProcessingStart(ProcessingType processingType, Transform machineTransform)
    {
        Commodity commodity = commodityFactory.Generate(this, processingType);
        FoodSpawnManagerRPC foodSpawnManagerRPC = GameObject.FindObjectOfType<FoodSpawnManagerRPC>();

        if (GateOfFusion.Instance.NetworkRunner.IsSharedModeMasterClient)
        {
        /* ------------------------------- CommodityのID入れる！ ------------------------------------------------------------------------------- */
            foodSpawnManagerRPC.RPC_CommoditySpawn( 0 /* ←　ID */, transform.rotation.eulerAngles, transform.position);
        /* ------------------------------------------------------------------------------------------------------------------------------------ */
            foodSpawnManagerRPC.RPC_Despawn(GetComponent<LocalView>().NetworkView.GetComponent<NetworkObject>());
            networkRunner.Despawn(this.gameObject.GetComponent<NetworkObject>());
        }

        Commodity instanceCommodity = Instantiate(commodity.gameObject, this.transform.position, this.transform.rotation).GetComponent<Commodity>();//NetworkRunnnerSpawn /*.transform.parent = machineTransform*/;
        return instanceCommodity;
    }

    void IInject<ISwitchableGrabbableActive>.Inject(ISwitchableGrabbableActive t)
    {
        this.switchableGrabbableActive = t;
        this.switchableGrabbableActive.Regist(this);
    }

    public async void NonVRTest_GrabAndRelease()
    {
        GetComponent<DisplayItem>().WhenSelect(new PointerEvent());
        GateOfFusion.Instance.Grab(this.GetComponent<NetworkObject>()).Forget();
        this.transform.position = Vector3.zero;

        await UniTask.Delay(2000);

        GetComponent<DisplayItem>().WhenUnselect(new PointerEvent());
        GateOfFusion.Instance.Release(this.GetComponent<NetworkObject>());
    }

    private void FixedUpdate()
    {
        if (isGrab)
        {
            GetComponent<LocalView>().NetworkView.RPC_Position(this.transform.position, this.transform.rotation.eulerAngles);
        }
    }
}


#if UNITY_EDITOR
[InitializeOnLoad]
public class HierarchyColor
{
    static HierarchyColor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject != null && gameObject.GetComponent<Ingrodients>() != null)
        {
            if (!gameObject.GetComponent<Ingrodients>().IsGrabed)
            {
                EditorGUI.DrawRect(selectionRect, Color.red);
            }
            else
            {
                EditorGUI.DrawRect(selectionRect, Color.blue);
            }
            GUI.color = Color.black;
            EditorGUI.LabelField(selectionRect, gameObject.name, new GUIStyle()
            {
                fontStyle = FontStyle.Bold
            });
            GUI.color = Color.white;
        }
    }
}
#endif

