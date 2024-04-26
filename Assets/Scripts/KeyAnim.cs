using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyAnim : MonoBehaviour
{
    Vector3 pushPosition;
    Vector3 position;

    private TextMeshProUGUI textMesh;

    [SerializeField]
    private Canvas canvas;

    Vector3 canvasPushPosition;
    Vector3 canvasPosition;

    private void Awake()
    {
        position = this.transform.localPosition;
        pushPosition = this.transform.localPosition - new Vector3(0f, 0.3f, 0f);
        canvasPushPosition = canvas.transform.localPosition - new Vector3(0f, 0.3f, 0f);
        canvasPosition = canvas.transform.localPosition;

        textMesh = GameObject.Find("TouchObject").GetComponent<TextMeshProUGUI>();
        textMesh.text = "";
    }
    private void OnCollisionStay(Collision collision)
    {
        this.transform.localPosition = pushPosition;
        canvas.transform.localPosition = canvasPushPosition;
    }

    private void OnCollisionExit(Collision collision)
    {
        this.transform.localPosition = position;
        this.canvas.transform.localPosition = canvasPosition;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        textMesh.text += "A";
    }
}
