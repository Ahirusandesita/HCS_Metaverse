using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public interface ICanvasDeploymentAndConvergence
{
    /// <summary>
    /// “WŠJ
    /// </summary>
    void Deployment();
    /// <summary>
    /// Žû‘©
    /// </summary>
    void Convergence();
    /// <summary>
    /// “WŠJ’†‚©
    /// </summary>
    bool IsDeployment { get; }
}

public class DebutButton : MonoBehaviour
{
    private OVRCanvasManager OVRCanvasManager;

    private void Awake()
    {
        FindOVRCanvasManager().Forget();
    }

    public void Selected()
    {     
        OVRCanvasManager.ChangeCanvasDeployment();
    }

    private async UniTaskVoid FindOVRCanvasManager()
    {
        OVRCanvasManager = await GameObject.FindObjectOfType<PokeableCanvasInHandInitialize>().WaitForSpecificTypeAsync<OVRCanvasManager>();
    }
}
