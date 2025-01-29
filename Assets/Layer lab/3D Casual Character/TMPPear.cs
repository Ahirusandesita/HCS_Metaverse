using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMPPear : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<TestRunning>().animationEvent += AnimationChenge;
    }

    // Start is called before the first frame update
    void AnimationChenge()
    {
        StartCoroutine(WaveHand());
    }

    private IEnumerator WaveHand()
    {
        yield return new WaitForSeconds(5.5f);

        GetComponent<Animator>().SetTrigger("OnAction");
    }
}
