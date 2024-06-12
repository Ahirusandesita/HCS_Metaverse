using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
public class Item_Cube : MonoBehaviour, IItem, IInventoryRetractable, ISelectedNotificationInjectable
{
    [SerializeField]
    private GameObject test;

    private AppearanceInfo_Mesh appearanceInfo_Mesh;
    private ISelectedNotification selectedNotification = new NullSelectedNotification();

    private bool isSelect = false;

    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        appearanceInfo_Mesh = new AppearanceInfo_Mesh(
            meshFilter.mesh,
            meshRenderer.materials
        );
    }
    AppearanceInfo_Mesh IInventoryRetractable.Appearance()
    {
        return appearanceInfo_Mesh;
    }

    void IItem.CleanUp()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<BoxCollider>().enabled = false;
    }

    void IItem.TakeOut(Vector3 position)
    {
        this.GetComponent<MeshRenderer>().enabled = true;
        this.GetComponent<BoxCollider>().enabled = true;
        this.transform.position = position;
    }


    void IItem.Use()
    {
        selectedNotification.Select(SelectArgs.Empty);
    }

    public void UnSelect()
    {
        //FindObjectOfType<InventoryManager>().SendItem(this);
        isSelect = false;
        //test.SetActive(false);
    }
    public void Select()
    {
        isSelect = true;
        //test.SetActive(true);
        
    }

    public void Inject(ISelectedNotification selectedNotification)
    {
        this.selectedNotification = selectedNotification;
    }
}
