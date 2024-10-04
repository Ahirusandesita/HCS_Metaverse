using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PinInformationView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMesh;
    [SerializeField]
    private Image image;

    private void Awake()
    {
        textMesh.text = "";
        this.image.enabled = false;
    }

    public void Display(string pinName)
    {
        textMesh.text = pinName;
    }
    public void Display(string pinName,Sprite sprite)
    {
        this.image.enabled = true;
        textMesh.text = pinName;
        this.image.sprite = sprite;
    }
    public void Initialize()
    {
        textMesh.text = "";
        this.image.enabled = false;
    }
}
