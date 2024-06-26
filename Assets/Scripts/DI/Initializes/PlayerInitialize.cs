using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInitialize : MonoBehaviour
{
    [SerializeField]
    private PlayerProvider playerProvider;

    [SerializeField, InterfaceType(typeof(IDependencyInjector<PlayerBodyDependencyInformation>))]
    private List<UnityEngine.Object> IDependencyInjectorsPlayerBody;
    private List<IDependencyInjector<PlayerBodyDependencyInformation>> dependencyInjectorsPlayerBody => IDependencyInjectorsPlayerBody.OfType<IDependencyInjector<PlayerBodyDependencyInformation>>().ToList();

    [SerializeField, InterfaceType(typeof(IDependencyInjector<PlayerHandDependencyInfomation>))]
    private List<UnityEngine.Object> IDependencyInjectorsPlayerHand;
    private List<IDependencyInjector<PlayerHandDependencyInfomation>> dependencyInjectorsPlayerHand => IDependencyInjectorsPlayerHand.OfType<IDependencyInjector<PlayerHandDependencyInfomation>>().ToList();

    private void Awake()
    {
        IDependencyProvider<PlayerBodyDependencyInformation> providerPlayerBody = playerProvider;
        foreach (IDependencyInjector<PlayerBodyDependencyInformation> dependencyInjector in dependencyInjectorsPlayerBody)
        {
            dependencyInjector.Inject(providerPlayerBody.Information);
        }

        IDependencyProvider<PlayerHandDependencyInfomation> providerPlayerHand = playerProvider;
        foreach(IDependencyInjector<PlayerHandDependencyInfomation> dependencyInjector in dependencyInjectorsPlayerHand)
        {
            dependencyInjector.Inject(providerPlayerHand.Information);
        }
    }

    public void ConsignmentInject(IDependencyInjector<PlayerBodyDependencyInformation> dependencyInjector)
    {
        IDependencyProvider<PlayerBodyDependencyInformation> providerPlayerBody = playerProvider;
        dependencyInjector.Inject(providerPlayerBody.Information);
    }
    public void ConsignmentInject(IDependencyInjector<PlayerHandDependencyInfomation> dependencyInjector)
    {
        IDependencyProvider<PlayerHandDependencyInfomation> providerPlayerHand = playerProvider;
        dependencyInjector.Inject(providerPlayerHand.Information);
    }
}
