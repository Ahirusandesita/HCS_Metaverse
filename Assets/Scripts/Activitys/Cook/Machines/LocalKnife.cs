using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalKnife : MonoBehaviour
{
    [SerializeField, Tooltip("見た目用オブジェクトのTransform")]
    private Transform _visualObjectTransform = default;

    [SerializeField, Tooltip("接触判定用Collider")]
    private Collider _knifeCollider = default;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
