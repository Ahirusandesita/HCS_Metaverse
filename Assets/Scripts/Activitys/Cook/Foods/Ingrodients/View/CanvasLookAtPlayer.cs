using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAtPlayer : MonoBehaviour, IDependencyInjector<PokeableCanvasInformation>
{
    private Transform cameraTransform = default;

    private bool isInject = false;

    public void Inject(PokeableCanvasInformation information)
    {
        cameraTransform = information.CameraTransform;
        isInject = true;
    }

    void Start()
    {
        PokeableCanvasInHandInitialize.ConsignmentInject_static(this);
    }

    void Update()
    {
        if (!isInject)
        {
            return;
        }

        this.transform.LookAt(cameraTransform.position, this.transform.forward);
        Vector2 nowRotate = this.transform.rotation.eulerAngles;
        nowRotate.x = 0;
        this.transform.rotation = Quaternion.Euler(nowRotate);
        this.transform.Rotate(0f, 180f, 0f);

        //this.transform.position = new Vector3(0f, this.transform.position.y, 0f);
    }
}
