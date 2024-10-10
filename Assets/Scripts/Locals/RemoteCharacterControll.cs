using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteCharacterControll : MonoBehaviour, ICharacterControl
{
    private Animator animator;
    private Coroutine _coroutine;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void PlayAnimation(AnimationClip clip)
    {
        animator.CrossFadeInFixedTime(clip.name, 0.25f);
        if (_coroutine != null) StopCoroutine(_coroutine);
    }
}
