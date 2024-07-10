using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcessDegreeOfProgressView : MonoBehaviour
{
    [SerializeField]
    private Image progressPercentImage;

    public void View(float percent)
    {
        progressPercentImage.fillAmount = percent;
    }
}
