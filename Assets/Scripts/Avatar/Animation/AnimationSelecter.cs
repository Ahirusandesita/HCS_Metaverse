using System;
using UnityEngine;

public class AnimationSelecter : MonoBehaviour, IDressUpEventSubscriber
{
    [SerializeField]
    private ItemBundleAsset _animationBundleAsset = default;

    private Animator _playerAnimator = default;
    private CharacterControlRPCManager characterControl;

    private string _playingAnimationName = default;
    private bool _isPlaying = false;

    public event Action animationStart;
    public event Action animationEnd;

    private async void Start()
    {
        _playerAnimator = GetComponentInChildren<Animator>();
        RemoteView remoteView = await FindObjectOfType<LocalRemoteSeparation>().ReceiveRemoteView();
        characterControl = remoteView.GetComponentInChildren<CharacterControlRPCManager>();

        animationStart += () =>
        {
            _playerAnimator.enabled = true;
        };

        animationEnd += () =>
        {
            _playerAnimator.enabled = false;
        };
    }
    public void StartedMove()
    {
        if (_playerAnimator != null)
        {
            _playerAnimator.SetBool("Walk", true);
        }
    }
    public void EndMove()
    {
        if (_playerAnimator != null)
        {
            _playerAnimator.SetBool("Walk", false);
        }
    }
    private void Update()
    {
        if (!_isPlaying)
        {
            return;
        }

        AnimatorStateInfo animationInfo = _playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (animationInfo.IsName(_playingAnimationName) && animationInfo.normalizedTime >= 0.95f)
        {
            _isPlaying = false;
            _playingAnimationName = default;

            animationEnd?.Invoke();
        }
    }

    public void PlayAnimation(AnimationClip clip)
    {
        _playerAnimator.Play(clip.name);
        _playingAnimationName = clip.name;
        _isPlaying = true;
    }

    public void OnDressUp(int id, string name)
    {
        ItemAsset animationAsset = _animationBundleAsset.GetItemAssetByID(id);

        if (animationAsset.Animation == null)
        {
            Debug.LogError("AnimationÇ™ê›íËÇ≥ÇÍÇƒÇ¢Ç»Ç¢");
            return;
        }

        AnimationClip clip = animationAsset.Animation;

        animationStart?.Invoke();
        PlayAnimation(clip);
        characterControl.RPC_AnimationSelect(id, name);
    }
    public void RPCDressUP(int id, string name)
    {
        ItemAsset animationAsset = _animationBundleAsset.GetItemAssetByID(id);

        if (animationAsset.Animation == null)
        {
            Debug.LogError("AnimationÇ™ê›íËÇ≥ÇÍÇƒÇ¢Ç»Ç¢");
            return;
        }

        AnimationClip clip = animationAsset.Animation;

        animationStart?.Invoke();
        PlayAnimation(clip);
    }

}
