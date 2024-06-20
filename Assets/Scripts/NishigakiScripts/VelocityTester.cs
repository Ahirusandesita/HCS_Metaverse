using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityTester : MonoBehaviour
{
    [SerializeField]
    private Throwable _throwable = default;

    bool isMove = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // ThrowDataÇê∂ê¨
            _throwable.Select();

            isMove = true;

            StartCoroutine(coroutineA());
        }

        if (isMove)
        {
            _throwable._thisTransform.position += -Vector3.forward * 1f * Time.deltaTime;
        }
    }

    private IEnumerator coroutineA()
    {
        yield return new WaitForSeconds(1);

        isMove = false;

        _throwable.UnSelect();
    }
}
