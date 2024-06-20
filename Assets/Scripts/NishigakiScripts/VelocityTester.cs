using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityTester : MonoBehaviour
{
    [SerializeField]
    private Throwable _throwable = default;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // ThrowDataÇê∂ê¨
            _throwable.Select();

            for (int i = 0; i < 11; i++)
            {
                // 
                _throwable._throwData.SetOrbitPosition(_throwable._thisTransform.position + -Vector3.forward * 0.15f * i);
            }

            _throwable.UnSelect();
        }


    }
}
