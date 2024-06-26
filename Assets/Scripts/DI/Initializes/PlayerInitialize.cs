using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
public class DependencyInjectionException : Exception
{
    public DependencyInjectionException()
    {

    }
    public DependencyInjectionException(string message)
    {
        Debug.LogError(message);
    }
    public DependencyInjectionException(string message, Exception innerException)
    {

    }
}
public class PlayerInitialize : InitializeBase
{
    [SerializeField]
    private PlayerProvider playerProvider;

    [SerializeField, InterfaceType(typeof(IDependencyInjector<PlayerBodyDependencyInformation>))]
    private List<UnityEngine.Object> IDependencyInjectorsPlayerBody;
    private List<IDependencyInjector<PlayerBodyDependencyInformation>> dependencyInjectorsPlayerBody => IDependencyInjectorsPlayerBody.OfType<IDependencyInjector<PlayerBodyDependencyInformation>>().ToList();

    [SerializeField, InterfaceType(typeof(IDependencyInjector<PlayerHandDependencyInfomation>))]
    private List<UnityEngine.Object> IDependencyInjectorsPlayerHand;
    private List<IDependencyInjector<PlayerHandDependencyInfomation>> dependencyInjectorsPlayerHand => IDependencyInjectorsPlayerHand.OfType<IDependencyInjector<PlayerHandDependencyInfomation>>().ToList();

    private static PlayerProvider playerProvider_static;

    private void Awake()
    {
        IDependencyProvider<PlayerBodyDependencyInformation> providerPlayerBody = playerProvider;
        foreach (IDependencyInjector<PlayerBodyDependencyInformation> dependencyInjector in dependencyInjectorsPlayerBody)
        {
            dependencyInjector.Inject(providerPlayerBody.Information);
        }

        IDependencyProvider<PlayerHandDependencyInfomation> providerPlayerHand = playerProvider;
        foreach (IDependencyInjector<PlayerHandDependencyInfomation> dependencyInjector in dependencyInjectorsPlayerHand)
        {
            dependencyInjector.Inject(providerPlayerHand.Information);
        }

        playerProvider_static = playerProvider;
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

    public static void ConsignmentInject_static(IDependencyInjector<PlayerBodyDependencyInformation> dependencyInjector)
    {
        if(playerProvider_static is null)
        {
            throw new DependencyInjectionException($"PlayerProvider is NULL");
        }
        IDependencyProvider<PlayerBodyDependencyInformation> providerPlayerBody = playerProvider_static;
        dependencyInjector.Inject(providerPlayerBody.Information);
    }

    public static void ConsignmentInject_static(IDependencyInjector<PlayerHandDependencyInfomation> dependencyInjector)
    {
        if(playerProvider_static is null)
        {
            throw new DependencyInjectionException($"PlayerProvider is NULL");
        }
        IDependencyProvider<PlayerHandDependencyInfomation> providerPlayerHand = playerProvider_static;
        dependencyInjector.Inject(providerPlayerHand.Information);
    }

    public override void Initialize()
    {
        playerProvider = InterfaceUtils.FindObjectOfInterfaces<PlayerProvider>()[0];
    }
}
