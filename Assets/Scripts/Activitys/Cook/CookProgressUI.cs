using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

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

        activityProgressManagement.OnStart_All += async () =>
        {
            textMesh.text = "Go!";
            await UniTask.Delay(3000);
            textMesh.text = "";
        };

        activityProgressManagement.OnWaitFinish += async () =>
        {
            textMesh.text = "Finish!";
            await UniTask.Delay(5000);
        };
    }
}
