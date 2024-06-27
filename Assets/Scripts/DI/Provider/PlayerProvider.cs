using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IReadonlyPositionAdapter
{
    Vector3 Position { get; }
}
public class PlayerBodyDependencyInformation : DependencyInformation
{
    public readonly IReadonlyPositionAdapter PlayerBody;
    public PlayerBodyDependencyInformation(IReadonlyPositionAdapter playerBody)
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

public class PlayerVisualHandDependencyInformation : DependencyInformation
{
    public readonly Transform VisualLeftHand;
    public readonly Transform VisualRightHand;

    public PlayerVisualHandDependencyInformation(Transform visualLeftHand, Transform visualRightHand)
    {
        this.VisualLeftHand = visualLeftHand;
        this.VisualRightHand = visualRightHand;
    }
}

public class PlayerProvider : MonoBehaviour, IDependencyProvider<PlayerBodyDependencyInformation>, IDependencyProvider<PlayerHandDependencyInfomation>,IDependencyProvider<PlayerVisualHandDependencyInformation>
{
    [SerializeField, InterfaceType(typeof(IReadonlyPositionAdapter))]
    private UnityEngine.Object IReadonlyPositionAdapter;

    private IReadonlyPositionAdapter playerBody => IReadonlyPositionAdapter as IReadonlyPositionAdapter;
    [SerializeField]
    private Transform playerLeftHand;
    [SerializeField]
    private Transform playerRightHand;

    [SerializeField]
    private Transform playerVisualLeftHand;
    [SerializeField]
    private Transform playerVisualRightHand;


    PlayerHandDependencyInfomation IDependencyProvider<PlayerHandDependencyInfomation>.Information => new PlayerHandDependencyInfomation(playerLeftHand, playerRightHand);

    PlayerBodyDependencyInformation IDependencyProvider<PlayerBodyDependencyInformation>.Information => new PlayerBodyDependencyInformation(playerBody);

    PlayerVisualHandDependencyInformation IDependencyProvider<PlayerVisualHandDependencyInformation>.Information => new PlayerVisualHandDependencyInformation(playerVisualLeftHand, playerVisualRightHand);
}
