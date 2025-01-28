using System.Collections;
using UnityEngine;

public class TestMovieAnimation : MonoBehaviour
{
    private Animator animator = default;

    [SerializeField]
    Transform startPos = default;

    [SerializeField]
    Transform endPos = default;

    private bool doMove = false;

    private float time = default;

    private void Start()
    {
        animator = GetComponent<Animator>();

        StartCoroutine(StartWalking());
    }

    private void Update()
    {
        if (doMove)
        {
            transform.position = Vector3.Lerp(startPos.position, endPos.position, time);

            time += Time.deltaTime / 5f;
        }
    }

    private IEnumerator StartWalking()
    {
        animator.SetBool("IsWalk", true);
        doMove = true;
         
        yield return new WaitForSeconds(5f);

        animator.SetBool("IsWalk", false);
        doMove = false;

        StartCoroutine(WaveHand());
    }

    private IEnumerator WaveHand()
    {
        yield return new WaitForSeconds(1f);

        animator.SetTrigger("OnAction");
    }
}
