using UnityEngine;

public class TestGun : MonoBehaviour
{
    [SerializeField, InterfaceType(typeof(IBullet))]
    private UnityEngine.Object IBullet;
    private IBullet bullet => IBullet as IBullet;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Instantiate(IBullet,this.transform.position,this.transform.rotation);
        }
    }
}
