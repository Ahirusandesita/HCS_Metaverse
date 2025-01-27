using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GrabbableSceneRegister : MonoBehaviour, IGrabbableActiveChangeRequester
{
    [SerializeField]
    private List<RegisterSceneInInspector> registerSceneInInspectors = new List<RegisterSceneInInspector>();

    void Start()
    {
        foreach (RegisterSceneInInspector item in registerSceneInInspectors)
        {
            if (item == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                return;
            }
        }

        if (registerSceneInInspectors.Count != 0)
        {
            this.GetComponent<ISwitchableGrabbableActive>().Regist(this);
            this.GetComponent<ISwitchableGrabbableActive>().Inactive(this);
        }
    }
}
