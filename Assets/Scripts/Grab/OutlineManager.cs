using Oculus.Interaction;
using UnityEngine;
using System;

public class OutlineManager : MonoBehaviour, IDependencyInjector<PlayerBodyDependencyInformation>
{
    [SerializeField] private bool hide = false;

    private Outline outline = default;
    private IReadonlyPositionAdapter playerPositionAdapter = default;
    private Transform myTransform = default;
    private Action FixedUpdateAction = default;
    private const float MAX_WIDTH = 10f;
    private const float MIN_WIDTH = 1f;

    public Outline Outline => outline;


    private void Awake()
    {
        myTransform = transform;

        // íËêîÇë„ì¸
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = new Color32(255, 163, 0, 255);
        outline.OutlineWidth = MAX_WIDTH;
        outline.enabled = false;

        PlayerInitialize.ConsignmentInject_static(this);
    }

    private void FixedUpdate()
    {
        FixedUpdateAction?.Invoke();
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
                if (hide)
                {
                    return;
                }

                outline.enabled = true;
                FixedUpdateAction += OutlineControl;
            });
            puew.WhenUnhover.AddListener(_ =>
            {
                if (hide)
                {
                    return;
                }

                outline.enabled = false;
                FixedUpdateAction -= OutlineControl;
            });
        }
    }

    private void OutlineControl()
    {
        float distance = Vector3.Distance(myTransform.position, playerPositionAdapter.Position);
        float width = distance < MAX_WIDTH - MIN_WIDTH
            ? MAX_WIDTH - distance
            : MIN_WIDTH;
        outline.OutlineWidth = width;
    }
}