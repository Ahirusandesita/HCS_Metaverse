using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface IDressUpEventSubscriber
{
    void OnDressUp(int id, string name);
}
public class DressUpEventPresenter : MonoBehaviour
{
    [SerializeField, Tooltip("‚½‚¾–¼‘O‚Â‚¯‚½‚ç•ª‚©‚è‚â‚·‚¢‚©‚ç’u‚¢‚Ä‚é‚¾‚¯")]
    private string dressUpName;

    [SerializeField, InterfaceType(typeof(IDressUpEventVendor))]
    private UnityEngine.Object IDressUpEventVendor;
    private IDressUpEventVendor DressUpEventVendor => IDressUpEventVendor as IDressUpEventVendor;
    [SerializeField, InterfaceType(typeof(IDressUpEventSubscriber))]
    private List<UnityEngine.Object> dressUpEventSubscribers = new List<UnityEngine.Object>();
    private List<IDressUpEventSubscriber> DressUpEventSubscribers => dressUpEventSubscribers.OfType<IDressUpEventSubscriber>().ToList();
    private IDisposable disposable;

    public void SubscribeEvent()
    {
        foreach (IDressUpEventSubscriber dressUpEventSubscriber in DressUpEventSubscribers)
        {
            disposable = DressUpEventVendor.SubscribeDressUpEvent(dressUpEventSubscriber.OnDressUp);
        }
    }

    private void OnDestroy()
    {
        disposable.Dispose();
    }
}
