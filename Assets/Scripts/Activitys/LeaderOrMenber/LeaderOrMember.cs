using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class LOMInformation
{
    [SerializeField, InterfaceType(typeof(ILeader))]
    private MonoBehaviour leader;
    [SerializeField, InterfaceType(typeof(IMember))]
    private MonoBehaviour member;
    [SerializeField, InterfaceType(typeof(IRPCComponent))]
    private NetworkBehaviour rpcComponent;

    public MonoBehaviour Leader => leader;
    public MonoBehaviour Member => member;
    public NetworkBehaviour RPCComponent => rpcComponent;
    public IRPCComponent InterfaceRPCComponent => rpcComponent as IRPCComponent;
}
public class LeaderOrMember : MonoBehaviour
{
    [SerializeField]
    private List<LOMInformation> LOMInformations = new List<LOMInformation>();
    private bool canProsess = false;
    [SerializeField]
    private LeaderOrMemberRPC rpc;
    private void Awake()
    {
        GateOfFusion.Instance.OnActivityConnected += () =>
       {
           OnStart();
       };
    }
    private async void OnStart()
    {
        bool isLeader = true/*await GateOfFusion.Instance.GetIsLeader()*/;
        Debug.LogError(isLeader);
        for (int i = 0; i < LOMInformations.Count; i++)
        {
            LOMInformations[i].InterfaceRPCComponent.InstanceCode = i;
        }

        foreach (LOMInformation lOMInformation in LOMInformations)
        {
            if (isLeader)
            {
                NetworkBehaviour networkBehaviour = await GateOfFusion.Instance.SpawnAsync(lOMInformation.RPCComponent);
                IRPCComponent rPCComponent = networkBehaviour.GetComponent<IRPCComponent>();
                MonoBehaviour monoBehaviour = Instantiate(lOMInformation.Leader);
                ILeader participants = monoBehaviour.GetComponent<ILeader>();

                rPCComponent.LeaderInject(monoBehaviour);
                participants.Inject(networkBehaviour);
                Debug.LogError("¶¬");
            }
            else
            {
                await UniTask.WaitUntil(() => canProsess);
                foreach (IRPCComponent item in InterfaceUtils.FindObjectOfInterfaces<IRPCComponent>())
                {
                    if (item.InstanceCode == lOMInformation.InterfaceRPCComponent.InstanceCode)
                    {
                        MonoBehaviour member = Instantiate(lOMInformation.Member);
                        IMember memberInterface = member.GetComponent<IMember>();

                        memberInterface.Inject(item.NetworkBehaviour);
                        item.MemberInject(member);
                    }
                }
            }
        }

        if (isLeader)
        {
            rpc.RPC_ProsessComplete();
        }
    }

    public void ProcessComplete()
    {
        canProsess = true;
        OnStart();
    }
}