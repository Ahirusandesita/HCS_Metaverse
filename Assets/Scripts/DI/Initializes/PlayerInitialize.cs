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
public interface ISingletonInitializer<T>
{
    T Provider { get; }
}

public class PlayerInitialize : InitializeBase, ISingletonInitializer<PlayerProvider>
{
    [SerializeField]
    private PlayerProvider playerProvider;

    [SerializeField, InterfaceType(typeof(IDependencyInjector<PlayerBodyDependencyInformation>))]
    private List<UnityEngine.Object> IDependencyInjectorsPlayerBody;
    private List<IDependencyInjector<PlayerBodyDependencyInformation>> dependencyInjectorsPlayerBody =>
        IDependencyInjectorsPlayerBody.OfType<IDependencyInjector<PlayerBodyDependencyInformation>>().ToList();

    [SerializeField, InterfaceType(typeof(IDependencyInjector<PlayerHandDependencyInfomation>))]
    private List<UnityEngine.Object> IDependencyInjectorsPlayerHand;
    private List<IDependencyInjector<PlayerHandDependencyInfomation>> dependencyInjectorsPlayerHand =>
        IDependencyInjectorsPlayerHand.OfType<IDependencyInjector<PlayerHandDependencyInfomation>>().ToList();

    PlayerProvider ISingletonInitializer<PlayerProvider>.Provider => playerProvider;
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

        IDependencyProvider<PlayerVisualHandDependencyInformation> providerPlayerVisualHandAndController = playerProvider;

        foreach(IDependencyInjector<PlayerBodyDependencyInformation> injector in InterfaceUtils.FindObjectOfInterfaces<IDependencyInjector<PlayerBodyDependencyInformation>>())
        {
            injector.Inject(providerPlayerBody.Information);
        }
        foreach(IDependencyInjector<PlayerHandDependencyInfomation> injector in InterfaceUtils.FindObjectOfInterfaces<IDependencyInjector<PlayerHandDependencyInfomation>>())
        {
            injector.Inject(providerPlayerHand.Information);
        }
        foreach(IDependencyInjector<PlayerVisualHandDependencyInformation> injector in InterfaceUtils.FindObjectOfInterfaces<IDependencyInjector<PlayerVisualHandDependencyInformation>>())
        {
            injector.Inject(providerPlayerVisualHandAndController.Information);
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

    public static void ConsignmentInject_static(IDependencyInjector<PlayerBodyDependencyInformation> dependencyInjector)
    {
        if (playerProvider_static is null)
        {
            playerProvider_static = InterfaceUtils.FindObjectOfInterfaces<ISingletonInitializer<PlayerProvider>>()[0].Provider;
        }
        IDependencyProvider<PlayerBodyDependencyInformation> providerPlayerBody = playerProvider_static;
        dependencyInjector.Inject(providerPlayerBody.Information);
    }

    public static void ConsignmentInject_static(IDependencyInjector<PlayerHandDependencyInfomation> dependencyInjector)
    {
        if (playerProvider_static is null)
        {
            playerProvider_static = InterfaceUtils.FindObjectOfInterfaces<ISingletonInitializer<PlayerProvider>>()[0].Provider;
        }
        IDependencyProvider<PlayerHandDependencyInfomation> providerPlayerHand = playerProvider_static;
        dependencyInjector.Inject(providerPlayerHand.Information);
    }

    public static void ConsignmentInject_static(IDependencyInjector<PlayerVisualHandDependencyInformation> dependencyInjector)
    {
        if (playerProvider_static is null)
        {
            playerProvider_static = InterfaceUtils.FindObjectOfInterfaces<ISingletonInitializer<PlayerProvider>>()[0].Provider;
        }
        IDependencyProvider<PlayerVisualHandDependencyInformation> providerPlayerHandAndController = playerProvider_static;
        dependencyInjector.Inject(providerPlayerHandAndController.Information);
    }

    public override void Initialize()
    {
        playerProvider = InterfaceUtils.FindObjectOfInterfaces<PlayerProvider>()[0];
        UnityEditor.EditorUtility.SetDirty(this);
    }

    private void OnDestroy()
    {
        playerProvider_static = null;
    }
}
