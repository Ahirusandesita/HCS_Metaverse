using UnityEngine;
public class CanvasDeploymentAndConvergence : MonoBehaviour, ICanvasDeploymentAndConvergence
{

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    bool ICanvasDeploymentAndConvergence.IsDeployment => throw new System.NotImplementedException();

    void ICanvasDeploymentAndConvergence.Convergence()
    {
        this.gameObject.SetActive(false);
    }

    void ICanvasDeploymentAndConvergence.Deployment()
    {
        this.gameObject.SetActive(true);
    }
}