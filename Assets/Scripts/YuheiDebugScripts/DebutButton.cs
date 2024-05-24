using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    [SerializeField]
    Transform centerAye;

    [SerializeField]
    private OVRCanvasManager OVRCanvasManager;

    public void Selected()
    {
        OVRCanvasManager.ChangeCanvasDeployment();
    }
}
