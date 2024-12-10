using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenLocalAvatar : MonoBehaviour
{
    [SerializeField]
    private Transform _localBodyParent = default;

    [SerializeField]
    private Transform _localPartParent = default;

    // 
    private const int LAYER_NUMBER_LOCAL_AVATAR = 10;

    private void Start()
    {
        ChangeLayer();
    }

    private void ChangeLayer()
    {
        Transform[] localBodys = _localBodyParent.GetComponentsInChildren<Transform>(true);
        Transform[] localParts = _localPartParent.GetComponentsInChildren<Transform>(true);

        foreach (Transform body in localBodys)
        {
            body.gameObject.layer = LAYER_NUMBER_LOCAL_AVATAR;
        }

        foreach(Transform part in localParts)
        {
            part.gameObject.layer = LAYER_NUMBER_LOCAL_AVATAR;
        }
    }
}
