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
    private DM dm;
    [SerializeField]
    private RectTransform startTransform;

    [SerializeField]
    Participants participants;

    private List<DM> dms = new List<DM>();

    public void Initialize()
    {
        for(int i=0;i< participants.DeploymentParticipants().Participants.Count; i++)
        {
            dms.Add(Instantiate(dm, this.transform));
        }

        Vector3 position = startTransform.GetComponent<RectTransform>().localPosition;
        foreach(DM dm in dms)
        {
            dm.GetComponent<RectTransform>().localPosition = position;

            position.y -= 10f;
        }
    }
    public void Dispose()
    {
        foreach(DM dm in dms)
        {
            Destroy(dm);
        }
    }
}
