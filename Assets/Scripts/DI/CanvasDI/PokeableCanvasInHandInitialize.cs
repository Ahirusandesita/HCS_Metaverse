using UnityEngine;
using Cysharp.Threading.Tasks;

public class PokeableCanvasInHandInitialize : MonoBehaviour
{
    [SerializeField]
    private InitializeAsset initialize;

    [SerializeField, InterfaceType(typeof(IDependencyProvider<PokeableCanvasInformation>))]
    private UnityEngine.Object PokeableCanvasInHandDependencyProvider;

    private IDependencyProvider<PokeableCanvasInformation> pokeableCanvasDependencyProvider => PokeableCanvasInHandDependencyProvider as IDependencyProvider<PokeableCanvasInformation>;

    private IDependencyInjector<PokeableCanvasInformation>[] dependencyInjectors;

    private GameObject instance;
    private bool existInstance = false;
    private void Awake()
    {
        foreach (GameObject gameObject in initialize.InitializeObjects)
        {
            instance = Instantiate(gameObject);
            existInstance = true;

            dependencyInjectors = instance.GetComponentsInChildren<IDependencyInjector<PokeableCanvasInformation>>();

            foreach (IDependencyInjector<PokeableCanvasInformation> dependencyInjector in dependencyInjectors)
            {
                dependencyInjector.Inject(pokeableCanvasDependencyProvider.Information);
            }

            instance.transform.parent = pokeableCanvasDependencyProvider.Information.Parent;
        }
    }

    public async UniTask<T> WaitForSpecificTypeAsync<T>()
    {
        await UniTask.WaitUntil(() => existInstance);

        return instance.GetComponentInChildren<T>();
    }

}
