using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PutCanvasInHand : MonoBehaviour
{
    public enum HandType
    {
        LeftHand,
        RightHand
    }
    private HandType handType;

    [SerializeField]
    private GameObject button;

    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Transform leftHandTransform;
    [SerializeField]
    private Transform rightHandTransform;

    [SerializeField]
    private HandType startHandType;

    [SerializeField]
    private MasterServerConect masterServerConect;

    private void Awake()
    {
        handType = startHandType;
        PutCanvas(handType);
    }

    public void CangeHandType(HandType handType)
    {
        this.handType = handType;
        PutCanvas(this.handType);
    }
    private void PutCanvas(HandType handType)
    {
        foreach (IPutCanvasInHand putCanvasInHand in GetComponentsInChildren<IPutCanvasInHand>(true))
        {
            Transform handTransform = handType == HandType.LeftHand ? leftHandTransform : rightHandTransform;
            putCanvasInHand.Inject(cameraTransform, handTransform);

            button.transform.parent = handTransform;

            Vector3 position = handType == HandType.LeftHand ? new Vector3(0f, -0.04f, 0f) : new Vector3(0f, 0.04f, 0f);
            Quaternion quaternion = handType == HandType.LeftHand ? Quaternion.Euler(180f, 0f, 0f) : Quaternion.Euler(0f, 0f, 0f);
            button.transform.localPosition = position;
            button.transform.localRotation = quaternion;
        }
    }


    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch)|| Input.GetKeyDown(KeyCode.W))
        {
            Debug.LogError("EEE");
            GateOfFusion.Instance.ActivityStart("CookActivity");
        }
    }
}
