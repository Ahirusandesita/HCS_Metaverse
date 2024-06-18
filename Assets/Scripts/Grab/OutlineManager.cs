using Oculus.Interaction;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    private Outline outline = default;
    public Outline Outline => outline;


    private void Awake()
    {
        // ’è”‚ğ‘ã“ü
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = new Color32(255, 163, 0, 255);
        outline.OutlineWidth = 10f;
        outline.enabled = false;

        if (TryGetComponent(out PointableUnityEventWrapper puew))
        {
            puew.WhenHover.AddListener(_ =>
            {
                outline.enabled = true;
            });
            puew.WhenUnhover.AddListener(_ =>
            {
                outline.enabled = false;
            });
        }
    }


}
