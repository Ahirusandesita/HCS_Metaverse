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
        try
        {
            myCollider ??= GetComponent<BoxCollider>();
        }
        catch (NullReferenceException) { }
        try
        {
            meshRenderer ??= GetComponent<MeshRenderer>();
        }
        catch (NullReferenceException) { }
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

    public bool IsExist(Vector3 targetPos)
    {
        return myCollider.ClosestPoint(targetPos) == targetPos;
    }

    public void Display(Vector3 itemPosition)
    {
        myTransform.position = itemPosition + Vector3.right;
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
