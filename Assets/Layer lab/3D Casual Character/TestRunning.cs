using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TestRunning : MonoBehaviour
{
    public delegate void AnimationEventHandler();

    public event AnimationEventHandler animationEvent;

    [SerializeField]
    private Transform _nextPos = default;

    [SerializeField]
    private GameObject _camera = default;

    private float speed = 1;

    private void Start()
    {
        StartCoroutine(Run());
    }

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(3f);

        GetComponent<Animator>().SetBool("IsRun", true);
        speed = 1.5f;

       yield return new WaitForSeconds(3f);

        ChengeAnimation();
    }

    private void ChengeAnimation()
    {
        gameObject.SetActive(false);     
        animationEvent?.Invoke();

        _camera.transform.parent = null;
        _camera.transform.position = _nextPos.position;
        _camera.transform.rotation = _nextPos.rotation;
    }
}
