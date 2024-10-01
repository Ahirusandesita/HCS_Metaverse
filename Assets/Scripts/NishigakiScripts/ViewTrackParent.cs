using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTrackParent : MonoBehaviour
{
    [SerializeField]
    private Transform _parentTransform = default;

    private Rigidbody _myRigidbody = default;

    // Start is called before the first frame update
    void Start()
    {
        //_parentTransform = transform.parent;

        //_myRigidbody = GetComponent<Rigidbody>();

        //Physics.IgnoreCollision(_parentTransform.GetComponent<Collider>(), _myRigidbody.GetComponent<Collider>(), true);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = _parentTransform.rotation;
        //transform.position = _parentTransform.position;
    }
}
