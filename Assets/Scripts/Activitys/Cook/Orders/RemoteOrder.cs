using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
namespace HCSMeta.Activity.Cook
{
    public class RemoteOrder : NetworkBehaviour
    {
        private Customer customer;
        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_Order(int index)
        {
            customer = GameObject.FindObjectOfType<Customer>();
            customer.RemoteOrder(index);
        }
    }
}
