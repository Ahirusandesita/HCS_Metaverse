using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectTouch : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textMesh;
    private void Awake()
    {
        textMesh = GameObject.Find("TouchObject").GetComponent<TextMeshProUGUI>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        textMesh.text = other.gameObject.name;
        this.transform.root.transform.position = new Vector3(100f, 100f, 100f);
    }
}
