using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerBodyDependencyInformation : DependencyInformation
{
    public readonly Transform PlayerBody;
    public PlayerBodyDependencyInformation(Transform playerBody)
    {
        this.PlayerBody = playerBody;
    }
}
public class PlayerHandDependencyInfomation : DependencyInformation
{
    public readonly Transform LeftHand;
    public readonly Transform RightHand;

    public PlayerHandDependencyInfomation(Transform leftHand, Transform rightHand)
    {
        this.LeftHand = leftHand;
        this.RightHand = rightHand;
    }
}

public class PlayerProvider : MonoBehaviour, IDependencyProvider<PlayerBodyDependencyInformation>, IDependencyProvider<PlayerHandDependencyInfomation>
{
    [SerializeField]
    private Transform playerBody;
    [SerializeField]
    private Transform playerLeftHand;
    [SerializeField]
    private Transform playerRightHand;


    PlayerHandDependencyInfomation IDependencyProvider<PlayerHandDependencyInfomation>.Information => new PlayerHandDependencyInfomation(playerLeftHand, playerRightHand);

    PlayerBodyDependencyInformation IDependencyProvider<PlayerBodyDependencyInformation>.Information => new PlayerBodyDependencyInformation(playerBody);
}
