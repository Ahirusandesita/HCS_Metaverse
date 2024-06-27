using Oculus.Interaction;
using UnityEngine;

public class OutlineManager : MonoBehaviour, IDependencyInjector<PlayerBodyDependencyInformation>
{
    private Outline outline = default;
    private IReadonlyPositionAdapter playerPositionAdapter = default;
    private Transform myTransform = default;
    public Outline Outline => outline;


    private void Awake()
    {
        myTransform = transform;

        // ’è”‚ğ‘ã“ü
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = new Color32(255, 163, 0, 255);
        outline.OutlineWidth = 10f;
        outline.enabled = false;

        PlayerInitialize.ConsignmentInject_static(this);
    }

    void IDependencyInjector<PlayerBodyDependencyInformation>.Inject(PlayerBodyDependencyInformation information)
    {
        playerPositionAdapter = information.PlayerBody;
        Subscription();
    }

    private void Subscription()
    {
        if (TryGetComponent(out PointableUnityEventWrapper puew))
        {
            puew.WhenHover.AddListener(_ =>
            {
                float distance = Vector3.Distance(myTransform.position, playerPositionAdapter.Position);
                float width = distance > 10f ? 10f : distance;
                outline.OutlineWidth = width;
                outline.enabled = true;
            });
            puew.WhenUnhover.AddListener(_ =>
            {
                outline.enabled = false;
            });
        }
    }
}
