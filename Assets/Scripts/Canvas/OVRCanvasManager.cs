using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OVRCanvasManager : MonoBehaviour
{
    [SerializeField, InterfaceType(typeof(ICanvasDeploymentAndConvergence))]
    private List<UnityEngine.Object> objects;
    private ICanvasFixable[] canvasFixables;
    private List<ICanvasDeploymentAndConvergence> canvasDeploymentAndConvergences => objects.OfType<ICanvasDeploymentAndConvergence>().ToList();
    private int count = 0;

    [SerializeField, InterfaceType(typeof(ICanvasFixedHandler))]
    private UnityEngine.Object ICanvasFixedHandler;
    private ICanvasFixedHandler canvasFixedHandler => ICanvasFixedHandler as ICanvasFixedHandler;

    private void Awake()
    {
        canvasFixables = this.GetComponentsInChildren<ICanvasFixable>(true);


        canvasFixedHandler.OnFixed += (eventArgs) =>
        {
            foreach (ICanvasFixable canvasFixable in canvasFixables)
            {
                canvasFixable.Fixed(eventArgs.IsFixed);
            }
        };
    }

    public void ChangeCanvasDeployment()
    {
        if (count == canvasDeploymentAndConvergences.Count)
        {
            foreach (ICanvasDeploymentAndConvergence item in canvasDeploymentAndConvergences)
            {
                item.Convergence();
            }
            count = 0;
            return;
        }

        canvasDeploymentAndConvergences[count].Deployment();

        for (int i = 0; i < canvasDeploymentAndConvergences.Count; i++)
        {
            if (i == count)
            {
                continue;
            }

            canvasDeploymentAndConvergences[i].Convergence();
        }

        count++;
    }
}