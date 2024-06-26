using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public interface IAvailableSpecificType
{
    UniTask<T> WaitForSpecificTypeAsync<T>();
}
public interface IInjectableSpecificType
{
    void Inject(IAvailableSpecificType availableSpecificType);
}
public class PokeableCanvasInHandInitialize : InitializeBase, IAvailableSpecificType
{
    [SerializeField]
    private InitializeAsset initialize;

    [SerializeField, InterfaceType(typeof(IDependencyProvider<PokeableCanvasInformation>))]
    private UnityEngine.Object PokeableCanvasInHandDependencyProvider;

    [SerializeField, InterfaceType(typeof(IInjectableSpecificType))]
    private List<UnityEngine.Object> IInjectableSpecificTypes;
    private List<IInjectableSpecificType> injectableSpecificTypes => IInjectableSpecificTypes.OfType<IInjectableSpecificType>().ToList();

    private IDependencyProvider<PokeableCanvasInformation> pokeableCanvasDependencyProvider => PokeableCanvasInHandDependencyProvider as IDependencyProvider<PokeableCanvasInformation>;

    private IDependencyInjector<PokeableCanvasInformation>[] dependencyInjectors;

    private GameObject instance;
    private bool existInstance = false;
    private void Awake()
    {
        foreach(IInjectableSpecificType injectableSpecificType in injectableSpecificTypes)
        {
            injectableSpecificType.Inject(this);
        }

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
        await UniTask.WaitUntil(() => instance.GetComponentInChildren<T>() != null);

        return instance.GetComponentInChildren<T>();
    }

    public override void Initialize()
    {
        PokeableCanvasInHandDependencyProvider = GameObject.FindObjectOfType<PokeableCanvasProvider>();

        string[] guids = InitializeAssetDatabase.Find();
        foreach (string guid in guids)
        {
            InitializeAsset asset = InitializeAssetDatabase.LoadAssetAtPathFromGuid(guid);

            if (asset.InitializeType == InitializeType.PokeableCanvas)
            {
                initialize = asset;
            }
        }
    }
}