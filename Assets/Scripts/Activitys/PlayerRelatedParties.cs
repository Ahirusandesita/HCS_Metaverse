using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class RelatedPartiesInformation
{
    [SerializeField]
    private GameObject @object;

    public enum RelatedPartiesObjectType
    {
        MeshRendererOff,
        Destroy
    }
    [SerializeField]
    private RelatedPartiesObjectType type;

    public GameObject Object => @object;
    public RelatedPartiesObjectType Type => type;
}
public class PlayerRelatedParties : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform participantsPosition;
    [SerializeField]
    private Transform spectatorsPosition;

    [SerializeField]
    private List<RelatedPartiesInformation> participants = new List<RelatedPartiesInformation>();
    [SerializeField]
    private List<RelatedPartiesInformation> spectators = new List<RelatedPartiesInformation>();
    private void Start()
    {
        switch (RelatedParties.Instance.ActivityRelatedPartiesState)
        {
            case ActivityRelatedPartiesState.Participants:
                player.position = participantsPosition.position;
                Setting(participants);
                break;
            case ActivityRelatedPartiesState.Spectators:
                player.position = spectatorsPosition.position;
                Setting(spectators);
                break;
        }
    }
    private void Setting(List<RelatedPartiesInformation> information)
    {
        foreach (RelatedPartiesInformation item in information)
        {
            switch (item.Type)
            {
                case RelatedPartiesInformation.RelatedPartiesObjectType.MeshRendererOff:
                    foreach (MeshRenderer meshRenderer in item.Object.GetComponentsInChildren<MeshRenderer>())
                    {
                        meshRenderer.enabled = false;
                    }
                    break;
                case RelatedPartiesInformation.RelatedPartiesObjectType.Destroy:
                    Destroy(item.Object);
                    break;
            }
        }
    }
}
