using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjectDressUpComponent : MonoBehaviour
{
    [SerializeField]
    private CharacterControlRPCManager manager;
    [SerializeField]
    private ChangeSkins skin;
    [SerializeField]
    private AnimationSelecter anim;

    private void Awake()
    {
        manager.InjectChangeSkin(skin);
        manager.InjectAnimationSelect(anim);
    }
}
