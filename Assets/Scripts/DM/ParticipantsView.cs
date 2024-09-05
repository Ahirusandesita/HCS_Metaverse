using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IDetailMenuInitialize
{
    void Initialize();
    void Dispose();
}
public class ParticipantsView : MonoBehaviour,IDetailMenuInitialize
{
    [SerializeField]
    private ContactAddress contactAddress;
    [SerializeField]
    private RectTransform startTransform;

    [SerializeField]
    Participants participants;
    [SerializeField]
    private DM testDM;

    [SerializeField]
    private TestDMInjector testInjector;
    
    private List<ContactAddress> contactAddressList = new List<ContactAddress>();

    public void Initialize()
    {
        for(int i=0;i< participants.DeploymentParticipants().Participants.Count; i++)
        {
            contactAddressList.Add(Instantiate(contactAddress, this.transform));
            testInjector.InjectTest(contactAddressList[contactAddressList.Count - 1]);
        }

        Vector3 position = startTransform.GetComponent<RectTransform>().localPosition;
        for (int i = 0; i < contactAddressList.Count; i++)
        {
            contactAddressList[i].GetComponent<RectTransform>().localPosition = position;

            position.y -= 100f;

            contactAddressList[i].InjectOwinInformation(participants.DeploymentParticipants().Participants[i]);
            contactAddressList[i].InjectDM(testDM);
        }
    }
    public void Dispose()
    {
        foreach(ContactAddress contactAddress in contactAddressList)
        {
            Destroy(contactAddress);
        }
        contactAddressList.Clear();
    }
}
