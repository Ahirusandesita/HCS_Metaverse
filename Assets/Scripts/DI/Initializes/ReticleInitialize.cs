using UnityEngine;

public class ReticleInitialize : MonoBehaviour
{
    [SerializeField]
    private HandType handType;

    [SerializeField]
    private InitializeAsset initialize;

    [SerializeField, InterfaceType(typeof(IDependencyProvider<ReticleDependencyInformation>))]
    private UnityEngine.Object ReticleDependencyProvider;

    private IDependencyProvider<ReticleDependencyInformation> reticleDependencyProvider => ReticleDependencyProvider as IDependencyProvider<ReticleDependencyInformation>;

    private IDependencyInjector<ReticleDependencyInformation>[] dependencyInjectors;


    private void Awake()
    {
        foreach (GameObject gameObject in initialize.InitializeObjects)
        {
            GameObject instance = Instantiate(gameObject);

            dependencyInjectors = instance.GetComponentsInChildren<IDependencyInjector<ReticleDependencyInformation>>();

            foreach (IDependencyInjector<ReticleDependencyInformation> dependencyInjector in dependencyInjectors)
            {
                dependencyInjector.Inject(reticleDependencyProvider.Information);
            }
        }
    }

    public void Inject()
    {
        ReticleDependencyProvider[] reticleDependencyProviders = GameObject.FindObjectsOfType<ReticleDependencyProvider>();

        foreach(ReticleDependencyProvider @object in reticleDependencyProviders)
        {
            if(@object.HandType == handType)
            {
                ReticleDependencyProvider = @object;
            }
        }
    }
}

