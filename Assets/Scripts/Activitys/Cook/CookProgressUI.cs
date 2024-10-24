using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CookProgressUI : MonoBehaviour
{
    [SerializeField]
    private ActivityProgressManagement activityProgressManagement;
    [SerializeField]
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh.text = "";
    }
    private void Start()
    {
        activityProgressManagement.OnReady += () =>
        {
            textMesh.text = "Ready?";
        };

        activityProgressManagement.OnStart += () =>
        {
            textMesh.text = "Go!";
        };
    }
}
