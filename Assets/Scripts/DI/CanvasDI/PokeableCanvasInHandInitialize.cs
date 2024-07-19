using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction.HandGrab;
public interface IAvailableSpecificType
{
    UniTask<T> WaitForSpecificTypeAsync<T>();
}
public interface IInjectableSpecificType
{
    void Inject(IAvailableSpecificType availableSpecificType);
}
public class PokeableCanvasInHandInitialize : InitializeBase, IAvailableSpecificType,ISingletonInitializer<PokeableCanvasProvider>
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

    PokeableCanvasProvider ISingletonInitializer<PokeableCanvasProvider>.Provider => pokeableCanvasDependencyProvider as PokeableCanvasProvider;
    private static PokeableCanvasProvider pokeableCanvasProvider_static;
    private void Awake()
    {
        foreach (IInjectableSpecificType injectableSpecificType in injectableSpecificTypes)
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
#if UNITY_EDITOR
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
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public static void ConsignmentInject_static(IDependencyInjector<PokeableCanvasInformation> dependencyInjector)
    {
        if (pokeableCanvasProvider_static is null)
        {
            pokeableCanvasProvider_static = InterfaceUtils.FindObjectOfInterfaces<ISingletonInitializer<PokeableCanvasProvider>>()[0].Provider;
        }
        IDependencyProvider<PokeableCanvasInformation> pokeableCanvasProvider = pokeableCanvasProvider_static;
        dependencyInjector.Inject(pokeableCanvasProvider.Information);
    }
}