using UnityEngine;

public class PokeableCanvasProvider : MonoBehaviour, IDependencyProvider<PokeableCanvasInformation>
{
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Transform leftHandTransform;
    [SerializeField]
    private Transform rightHandTransform;
    [SerializeField]
    private Transform parent;

    public PokeableCanvasInformation Information => new PokeableCanvasInformation(cameraTransform, leftHandTransform, parent);
}
