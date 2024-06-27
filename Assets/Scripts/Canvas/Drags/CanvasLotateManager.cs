using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CanvasLotateManager : MonoBehaviour, IPutCanvasInHand,ICanvasFixable
{
    private Transform cameraTransform = default;
    private Transform handTransform = default;

    [SerializeField]
    private float y;

    private bool isFixed = false;
    private bool isInject = false;

    void ICanvasFixable.Fixed(bool isFixed)
    {
        this.transform.LookAt(cameraTransform.position, this.transform.forward);
        Vector2 nowRotate = this.transform.rotation.eulerAngles;
        nowRotate.x = 0;
        this.transform.rotation = Quaternion.Euler(nowRotate);
        this.transform.Rotate(0f, 180f, 0f);

        this.transform.position = handTransform.position + new Vector3(0f, y, 0f);

        this.isFixed = isFixed;
    }

    void IPutCanvasInHand.Inject(Transform cameraTransform, Transform handTransform)
    {
        this.cameraTransform = cameraTransform;
        this.handTransform = handTransform;

        isInject = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isInject)
        {
            return;
        }

        if (isFixed)
        {
            return;
        }

        this.transform.LookAt(cameraTransform.position, this.transform.forward);
        Vector2 nowRotate = this.transform.rotation.eulerAngles;
        nowRotate.x = 0;
        this.transform.rotation = Quaternion.Euler(nowRotate);
        this.transform.Rotate(0f, 180f, 0f);

        this.transform.position = handTransform.position + new Vector3(0f, y, 0f);
    }
}
