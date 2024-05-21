using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGetItem : MonoBehaviour
{
    InventoryManager inventoryManager;

    [SerializeField]
    private Item_Cube Item_Cube;
    private void Awake()
    {
        inventoryManager = this.GetComponent<InventoryManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            inventoryManager.SendItem(Instantiate(Item_Cube));
        }
    }
}
