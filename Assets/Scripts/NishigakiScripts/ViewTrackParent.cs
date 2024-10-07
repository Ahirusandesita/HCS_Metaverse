using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTrackParent : MonoBehaviour
{
    [SerializeField]
    private Transform _parentTransform = default;

    [SerializeField]
    private BoxCollider _myCollider = default;

    [SerializeField]
    private BoxCollider _hitCollider = default;

    private ViewMoveDepth _viewMoveDepth = default;

    // Start is called before the first frame update
    void Start()
    {
        _viewMoveDepth = new ViewMoveDepth(_myCollider, _hitCollider, transform) ;

        _viewMoveDepth.HitCollider();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = _parentTransform.rotation;

        _viewMoveDepth.MoveDepth();
        //transform.position = _parentTransform.position;
    }
}
