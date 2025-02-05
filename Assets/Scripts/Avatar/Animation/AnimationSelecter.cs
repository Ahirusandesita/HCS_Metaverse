using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;

public class AnimationSelecter : MonoBehaviour, IDressUpEventSubscriber
{
    [SerializeField]
    private ItemBundleAsset _animationBundleAsset = default;

    private Animator _playerAnimator = default;

    private void Start()
    {
        _playerAnimator = GetComponentInChildren<Animator>();
    }

    public void PlayAnimation(AnimationClip clip)
    {
        _playerAnimator.CrossFadeInFixedTime(clip.name, 0.25f);
    }

    public void OnDressUp(int id, string name)
    {
        ItemAsset animationAsset = _animationBundleAsset.GetItemAssetByID(id);

        Debug.LogError($"animationAsset = {animationAsset.Prefab.name} , {animationAsset.Animation.name}, {_playerAnimator.name}");

        if (animationAsset.Animation == null)
        {
            Debug.LogError("AnimationÇ™ê›íËÇ≥ÇÍÇƒÇ¢Ç»Ç¢");
            return;
        }

        AnimationClip clip = animationAsset.Animation;

        PlayAnimation(clip);
    }
}
