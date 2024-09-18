using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

public interface IParticipantsInformation
{
    public OwnInformation Me { get; }
    public IReadOnlyList<OwnInformation> Participants { get; }
}
public class ParticipantsInformation : IParticipantsInformation
{
    private OwnInformation me;
    private List<OwnInformation> participants = new List<OwnInformation>();
    public OwnInformation Me => me;
    public IReadOnlyList<OwnInformation> Participants => participants;
    public ParticipantsInformation(OwnInformation me)
    {
        this.me = me;
    }
    public void AddPlayer(OwnInformation ownInformation)
    {
        foreach (OwnInformation item in participants)
        {
            if (item == ownInformation)
            {
                return;
            }
        }
        participants.Add(ownInformation);
    }
}




public class Participants : MonoBehaviour
{
    private ParticipantsInformation participantsInformation;
    [SerializeField]
    private OwnInformation own;
    private void Awake()
    {
        GateOfFusion.Instance.OnConnect += () => PlayerInformaitonRegiser();
    }

    async void PlayerInformaitonRegiser()
    {
        OwnInformation ownInformation = await GateOfFusion.Instance.SpawnAsync(own);
        participantsInformation = new ParticipantsInformation(ownInformation);
    }
    public IParticipantsInformation DeploymentParticipants()
    {
        foreach (OwnInformation ownInformation in FindObjectsOfType<OwnInformation>())
        {
            if (GateOfFusion.Instance.NetworkRunner.LocalPlayer == ownInformation.MyPlayerRef)
            {
                continue;
            }
            participantsInformation.AddPlayer(ownInformation);
        }
        return participantsInformation;
    }
}
