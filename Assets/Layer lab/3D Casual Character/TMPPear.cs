using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMPPear : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaveHand());
    }

    private IEnumerator WaveHand()
    {
        yield return new WaitForSeconds(6f);

        GetComponent<Animator>().SetTrigger("OnAction");
    }
}
