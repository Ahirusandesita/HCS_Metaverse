using UnityEngine;
using Oculus.Interaction.DistanceReticles;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;

public class ReticleDependencyInformation : DependencyInformation
{
    public readonly DistanceHandGrabInteractor DistanceHandGrabInteractor;
    public readonly DistanceGrabInteractor DistanceGrabInteractor;
    public readonly HandRef HandRef;
    public readonly Hand Hand;
    public readonly Transform CenterAye;

    public ReticleDependencyInformation(DistanceHandGrabInteractor distanceHandGrabInteractor, DistanceGrabInteractor distanceGrabInteractor, HandRef handRef, Hand hand,Transform centerAye)
    {
        this.DistanceHandGrabInteractor = distanceHandGrabInteractor;
        this.DistanceGrabInteractor = distanceGrabInteractor;
        this.HandRef = handRef;
        this.Hand = hand;
        this.CenterAye = centerAye;
    }
}
public class ReticleDI : MonoBehaviour, IDependencyInjector<ReticleDependencyInformation>
{
    private ReticleDependencyInformation dependencyInformation;
    [SerializeField]
    private HandType handType;

    [SerializeField]
    private ReticleIconDrawer handReticleIconDrawer;
    [SerializeField]
    private ReticleIconDrawer controllerReticleIconDrawer;

    [SerializeField]
    private DistantInteractionTubeVisual controllerDistantInteractionTubeVisual;
    [SerializeField]
    private DistantInteractionTubeVisual handDistantInteractionTubeVisual;

    [SerializeField]
    private ReticleGhostDrawer handReticleGhostDrawer;
    [SerializeField]
    private ActiveStateTracker activeStateTracker;
    [SerializeField]
    private SyntheticHand syntheticHand;
    [SerializeField]
    private ReticleMeshDrawer reticleMeshDrawer;

    public HandType HandType => handType;
    public void Inject(ReticleDependencyInformation reticleDependencyInformation)
    {
        this.dependencyInformation = reticleDependencyInformation;

        Initialize();
    }

    private void Initialize()
    {
        handReticleIconDrawer.InjectDistanceInteractor(dependencyInformation.DistanceHandGrabInteractor);
        controllerReticleIconDrawer.InjectDistanceInteractor(dependencyInformation.DistanceGrabInteractor);

        handReticleIconDrawer.InjectCenterEye(dependencyInformation.CenterAye);
        controllerReticleIconDrawer.InjectCenterEye(dependencyInformation.CenterAye);


        handDistantInteractionTubeVisual.InjectDistanceInteractor(dependencyInformation.DistanceHandGrabInteractor);
        controllerDistantInteractionTubeVisual.InjectDistanceInteractor(dependencyInformation.DistanceGrabInteractor);

        handReticleGhostDrawer.InjectHandGrabInteractor(dependencyInformation.DistanceHandGrabInteractor);
        activeStateTracker.InjectActiveState(dependencyInformation.HandRef);
        syntheticHand.InjectModifyDataFromSource(dependencyInformation.Hand);

        reticleMeshDrawer.InjectHandGrabInteractor(dependencyInformation.DistanceHandGrabInteractor);
    }
}
