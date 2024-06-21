using UnityEngine;
using Oculus.Interaction.DistanceReticles;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;


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
