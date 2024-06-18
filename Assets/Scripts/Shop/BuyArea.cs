using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OutlineManager))]
public class BuyArea : MonoBehaviour
{
    [SerializeField] private Collider myCollider = default;
    private OutlineManager outlineManager = default;


    private void Awake()
    {
        outlineManager = GetComponent<OutlineManager>();
    }

    private void Update()
    {
       
    }

    public bool IsExist(Vector3 targetPos)
    {
        if (myCollider.ClosestPoint(targetPos) == targetPos)
        {
            outlineManager.Outline.enabled = true;
            return true;
        }

        return false;
    }
}
