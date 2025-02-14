using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class SatueiyouAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimationClip animationClip;
    [SerializeField]
    private float delay;
    // Start is called before the first frame update
    public async void Play()
    {
        await UniTask.WaitForSeconds(delay);
        animator.SetTrigger("Action");
    }
}
