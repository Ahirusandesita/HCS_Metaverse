using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(OutlineManager))]
public class BuyArea : MonoBehaviour
{
    [SerializeField] private BoxCollider myCollider = default;
    [SerializeField] private MeshRenderer meshRenderer = default;

    private Transform myTransform = default;
    private OutlineManager outlineManager = default;
    private Vector3 initScale = default;
    private Action UpdateAction = default;

    private readonly Collider[] resultNonAlloc = new Collider[4];


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        myCollider ??= GetComponent<BoxCollider>();
        meshRenderer ??= GetComponent<MeshRenderer>();
    }

    private void Awake()
    {
        myTransform = transform;
        outlineManager = GetComponent<OutlineManager>();
    }

    private void Start()
    {
        initScale = myTransform.localScale;
        meshRenderer.enabled = false;
    }

    private void Update()
    {
        UpdateAction?.Invoke();
    }

    /// <summary>
    /// エリア内に指定したターゲットが存在するかどうか
    /// </summary>
    /// <param name="targetPos">ターゲットの座標</param>
    public bool IsExist(Vector3 targetPos)
    {
        return myCollider.ClosestPoint(targetPos) == targetPos;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerPosition"></param>
    public void Display(Vector3 playerPosition)
    {
        myTransform.position = playerPosition + Vector3.right;// Debug
        myTransform.localScale = Vector3.zero;
        meshRenderer.enabled = true;
        UpdateAction += OverlapCheck;
        myTransform.DOScale(initScale, 0.25f);
    }

    public void Hide()
    {
        UpdateAction -= OverlapCheck;
        myTransform.DOScale(Vector3.zero, 0.25f).OnComplete(() => meshRenderer.enabled = false);
    }

    private void OverlapCheck()
    {
        var center = myTransform.position + myCollider.center;
        var halfExtents = myCollider.size / 2;
        int resultCount = Physics.OverlapBoxNonAlloc(center, halfExtents, resultNonAlloc);

        for (int i = 0; i < resultCount; i++)
        {
            if (resultNonAlloc[i].TryGetComponent(out IDisplayItem _))
            {
                outlineManager.Outline.enabled = true;
                return;
            }
        }

        outlineManager.Outline.enabled = false;
    }
}
