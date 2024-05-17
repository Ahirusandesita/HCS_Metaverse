using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRInputManager : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private Transform cameraTransform;

    private void Update()
    {
        //if (OVRInput.Get(OVRInput.Button.One,OVRInput.Controller.RTouch))
        //{
        //    transform.position += cameraTransform.up * Time.deltaTime * speed;
        //}

        if (OVRInput.Get(OVRInput.Button.Two,OVRInput.Controller.RTouch))
        {
            transform.position += -cameraTransform.up * Time.deltaTime * speed;
        }

        Vector2 tmp = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

        Vector3 direction = new Vector3(tmp.x, 0f, tmp.y);


        transform.position += cameraTransform.forward * direction.z * speed * Time.deltaTime;

        transform.position += cameraTransform.right * direction.x * speed * Time.deltaTime;

        //Vector2 cameraDirection = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);

        //transform.Rotate(new Vector3(cameraDirection.y, cameraDirection.x, 0f));

        //if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        //{
        //    transform.Rotate(new Vector3(0f, 90f, 0f));
        //}
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            transform.Rotate(new Vector3(0f, -90f, 0f));
        }


        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -cameraTransform.right * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += cameraTransform.right * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += cameraTransform.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -cameraTransform.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += cameraTransform.up * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += -cameraTransform.up * speed * Time.deltaTime;
        }
    }
}
