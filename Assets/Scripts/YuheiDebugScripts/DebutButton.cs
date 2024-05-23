using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface ICanvasDeploymentAndConvergence
{
    /// <summary>
    /// �W�J
    /// </summary>
    void Deployment();
    /// <summary>
    /// ����
    /// </summary>
    void Convergence();
    /// <summary>
    /// �W�J����
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
