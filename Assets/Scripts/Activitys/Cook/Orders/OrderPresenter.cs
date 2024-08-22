using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OrderPresenter : MonoBehaviour
{

    [SerializeField]
    private OrderManager orderManager;
    [SerializeField]
    private OrderView orderView;

    private void Awake()
    {
        orderManager.OnOrderInitialize += orderView.OrderInitializeHandler;
        orderManager.OnOrder += orderView.OrderHandler;
        orderManager.OnResetOrder += orderView.ResetOrderArrayHandler;
    }
}
