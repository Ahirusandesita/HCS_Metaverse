using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CookTimeUI : MonoBehaviour
{
    [SerializeField]
    private CookTimeManager cookTime;
    [SerializeField]
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh.text = "TIME : ???";
    }
    private void Start()
    {
        cookTime.OnTime += (time) =>
        {
            textMesh.text = "TIME : " + time.ToString();
        };
    }
}
