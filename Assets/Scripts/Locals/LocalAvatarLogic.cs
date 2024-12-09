using System;
using UnityEngine;
using VContainer;

public class LocalAvatarLogic : MonoBehaviour
{
    [SerializeField]
    private Transform _rightControllerTransform = default;

    [SerializeField]
    private Transform _leftControllerTransform = default;

    private RemoteView remoteView = default;

    private Action action = default;

    private AvatarHandTracker _avatarHandTracker = null;

    public AvatarHandTracker setAvatarHandTracker { set => _avatarHandTracker = value; }
    [Inject]
    public void Inject(RemoteView remoteView)
    {
        this.remoteView = remoteView;
        Vector3 position = this.transform.position;
        position.z -= 1f;
        remoteView.transform.position = position;

        remoteView.GetComponent<MeshRenderer>().enabled = false;

        Debug.Log($"<color=green>LAL:Inject</color>");

        //action += () => remoteView.SetController(_rightControllerTransform, _leftControllerTransform);
    }

    private void HandTracking()
    {
        // 
        if (_avatarHandTracker == null)
        {
            return;
        }
        Debug.Log($"<color=red>HandTracking:{_avatarHandTracker == null}</color>");
        // 
        _avatarHandTracker.RightHandTracking(_rightControllerTransform);

        // 
        _avatarHandTracker.LeftHandTracking(_leftControllerTransform);
    }

    private void LateUpdate()
    {
        action?.Invoke();

        HandTracking();
    }
}
