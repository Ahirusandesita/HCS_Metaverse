using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface IDressUpEventSubscriber
{
    void OnDressUp(int id);
}
public class DressUpEventPresenter : MonoBehaviour
{
    [SerializeField,Tooltip("�������O�����番����₷������u���Ă邾��")]
    private string dressUpName;

    [SerializeField, InterfaceType(typeof(IDressUpEventVendor))]
    private UnityEngine.Object IDressUpEventVendor;
    private IDressUpEventVendor DressUpEventVendor => IDressUpEventVendor as IDressUpEventVendor;
    [SerializeField, InterfaceType(typeof(IDressUpEventSubscriber))]
    private List<UnityEngine.Object> dressUpEventSubscribers = new List<UnityEngine.Object>();
    private List<IDressUpEventSubscriber> DressUpEventSubscribers => dressUpEventSubscribers.OfType<IDressUpEventSubscriber>().ToList();

    private void Start()
    {
        foreach (IDressUpEventSubscriber dressUpEventSubscriber in DressUpEventSubscribers)
        {
            DressUpEventVendor.SubscribeDressUpEvent(dressUpEventSubscriber.OnDressUp);
        }
    }
}
