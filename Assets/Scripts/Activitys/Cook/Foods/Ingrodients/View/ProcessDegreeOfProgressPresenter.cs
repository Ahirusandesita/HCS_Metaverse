using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class ProcessDegreeOfProgressPresenter : MonoBehaviour
{
    [SerializeField]
    private Ingrodients ingrodients;
    [SerializeField]
    ProcessDegreeOfProgressView processDegreeOfProgressView;

    private void Awake()
    {
        ingrodients.TimeItTakesProperty.Subscribe((data) =>
        {
            processDegreeOfProgressView.View(1f - (data.NowTimeItTakes / data.MaxTimeItTakes));
        }).AddTo(this);
    }
}