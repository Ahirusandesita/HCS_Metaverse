using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestViewTrackar : MonoBehaviour
{
    [SerializeField]
    Transform _view = default;
    void Update()
    {
        _view.transform.position = this.transform.position;
    }
}
