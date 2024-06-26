using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFrame : MonoBehaviour
{
    private void Awake()
    {
        this.transform.localScale = Vector3.zero;
    }
    public void GameStart()
    {
        StartCoroutine(FrameAnimationUp());
    }
    public void Close()
    {
        StartCoroutine(FrameAnimationDown());
    }

    IEnumerator FrameAnimationUp()
    {
        float size = 0f;
        while (size < 1f)
        {
            yield return new WaitForSeconds(0.01f);
            size += 0.04f;
            this.transform.localScale = new Vector3(size, size, size);
        }

        this.transform.localScale = new Vector3(1f, 1f, 1f);

        SceneManager.LoadScene("CookActivity");
    }
    IEnumerator FrameAnimationDown()
    {
        float size = 1f;
        while (size > 0f)
        {
            yield return new WaitForSeconds(0.01f);
            size -= 0.04f;
            this.transform.localScale = new Vector3(size, size, size);
        }

        this.transform.localScale = new Vector3(0f, 0f, 0f);
    }
}
