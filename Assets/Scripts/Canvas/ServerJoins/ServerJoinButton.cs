using UnityEngine;
using UnityEngine.EventSystems;
using HCSMeta.Network;

public class ServerJoinButton : MonoBehaviour,IPointerUpRegistrable
{
    [SerializeField, InterfaceType(typeof(IMasterServerConectable))]
    private UnityEngine.Object IMasterServerConectable;
    private bool isJoin = false;
    private IMasterServerConectable conectable => IMasterServerConectable as IMasterServerConectable;
    void IPointerUpRegistrable.OnPointerUp(PointerEventData data)
    {
        if (isJoin)
        {
            return;
        }
        conectable.Connect("Room");
        Debug.LogError("ルーム名適当だよ");
        isJoin = true;
    }
}
