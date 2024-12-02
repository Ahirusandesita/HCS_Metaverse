using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    public void Push()
    {
        GameObject.FindObjectOfType<InventoryManager>().SendItem(20004);
    }
}
