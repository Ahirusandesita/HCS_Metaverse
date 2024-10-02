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

    public void ChengeProgressPresenter(float t)
    {
        processDegreeOfProgressView.View(1f - t);
    }

    private void Awake()
    {
        ingrodients.TimeItTakesProperty.Skip(1).Subscribe((data) =>
        {
            ChengeProgressPresenter(data.NowTimeItTakes / data.MaxTimeItTakes);
        }).AddTo(this);
    }
    private void Start()
    {
        //RPCSpawner.GetRPCSpawner().InjectAsync(this.gameObject);
    }
}