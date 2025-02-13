using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcessDegreeOfProgressView : MonoBehaviour
{
    [SerializeField]
    private Image progressPercentImage;
    [SerializeField]
    private GameObject frame;

    private void Start()
    {
        progressPercentImage.fillAmount = 0f;
        frame.SetActive(false);
    }
    public void View(float percent)
    {
        progressPercentImage.fillAmount = percent;
        frame.SetActive(true);
    }
}
