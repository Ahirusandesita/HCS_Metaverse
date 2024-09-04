using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public interface IParticipantsInformation
{
    public PlayerRef Me { get; }
    public IReadOnlyList<PlayerRef> Participants { get; }
}
public class ParticipantsInformation:IParticipantsInformation
{
    private PlayerRef me;
    private List<PlayerRef> participants = new List<PlayerRef>();
    public PlayerRef Me => me;
    public IReadOnlyList<PlayerRef> Participants => participants;
    public ParticipantsInformation(PlayerRef me)
    {
        this.me = me;
    }

    public void AddPlayer(PlayerRef newPlayer)
    {
        participants.Add(newPlayer);
    }
}
public class Participants : MonoBehaviour
{
    private ParticipantsInformation participantsInformation;
    [SerializeField]
    private OwnInformation own;
    private void Awake()
    {
        StartCoroutine(AA());
    }

    IEnumerator AA()
    {
        yield return new WaitForSeconds(5f);
        BB();
    }

    async void BB()
    {
        await GateOfFusion.Instance.SpawnAsync(own);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            IParticipantsInformation i = DeploymentParticipants();

            Debug.LogError(i.Me);
            foreach(PlayerRef playerRef in i.Participants)
            {
                Debug.LogError(playerRef);
            }
        }
    }


    public IParticipantsInformation DeploymentParticipants()
    {
        if(participantsInformation == null)
        {
            participantsInformation = new ParticipantsInformation(GateOfFusion.Instance.NetworkRunner.LocalPlayer);
        }

        foreach(OwnInformation ownInformation in FindObjectsOfType<OwnInformation>())
        {
            participantsInformation.AddPlayer(ownInformation.MyPlayerRef);
        }

        return participantsInformation;
    }
}
