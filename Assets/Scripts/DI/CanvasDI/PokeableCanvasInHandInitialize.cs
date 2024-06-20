using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeableCanvasInHandInitialize : MonoBehaviour
{
    [SerializeField]
    private InitializeAsset initialize;

    [SerializeField, InterfaceType(typeof(IDependencyProvider<PokeableCanvasInformation>))]
    private UnityEngine.Object PokeableCanvasInHandDependencyProvider;

    private IDependencyProvider<PokeableCanvasInformation> pokeableCanvasDependencyProvider => PokeableCanvasInHandDependencyProvider as IDependencyProvider<PokeableCanvasInformation>;

    private IDependencyInjector<PokeableCanvasInformation>[] dependencyInjectors;


    private void Awake()
    {
        foreach (GameObject gameObject in initialize.InitializeObjects)
        {
            GameObject instance = Instantiate(gameObject);

            dependencyInjectors = instance.GetComponentsInChildren<IDependencyInjector<PokeableCanvasInformation>>();

            foreach (IDependencyInjector<PokeableCanvasInformation> dependencyInjector in dependencyInjectors)
            {
                dependencyInjector.Inject(pokeableCanvasDependencyProvider.Information);
            }

            instance.transform.parent = pokeableCanvasDependencyProvider.Information.Parent;
        }
    }
}
