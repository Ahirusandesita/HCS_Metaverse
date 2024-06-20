using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

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

public class DebutButton : MonoBehaviour, IInjectableSpecificType
{
    private IAvailableSpecificType availableSpecificType;
    private OVRCanvasManager OVRCanvasManager;

    public void Inject(IAvailableSpecificType availableSpecificType)
    {
        this.availableSpecificType = availableSpecificType;
    }

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
        OVRCanvasManager = await availableSpecificType.WaitForSpecificTypeAsync<OVRCanvasManager>();
    }
}
