using Cysharp.Threading.Tasks;
using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Layer_lab._3D_Casual_Character
{
    public class CharacterControl : MonoBehaviour
    {
        public static CharacterControl Instance;
        public CharacterBase CharacterBase { get; set; }
        private Animator animator;
        [SerializeField] private TMP_Text textAnimationName;
        private Coroutine _coroutine;

        [SerializeField]
        private bool isLocal = false;
        private AnimationControl animationControl;

        private RemoteView remoteView;
        void Awake()
        {
            //local‚Ì‚Ý
            if (isLocal)
            {
                Instance = this;
                FindAsyncRemoteView().Forget();
            }
            animationControl = FindObjectOfType<AnimationControl>();
            //textAnimationName.text = "Stand_Idle1";
            animator = GetComponentInChildren<Animator>();
            CharacterBase = GetComponentInChildren<CharacterBase>();
        }


        private async UniTaskVoid FindAsyncRemoteView()
        {
            remoteView = await FindObjectOfType<LocalRemoteSeparation>().ReceiveRemoteView();
        }
        public void PlayAnimation(AnimationControl.AnimType animType, int index)
        {
            PlayAnimation(animationControl.GetAnimation(new AnimationControl.AnimData(animType, index)));
        }
        public void PlayAnimation(AnimationClip clip)
        {
            if (isLocal)
            {
                AnimationControl.AnimData animData = animationControl.GetAnimData(clip);
                FindObjectOfType<CharacterRPCManager>().Rpc_PlayEmote(animData.AnimType, animData.Index, remoteView.GetComponent<NetworkObject>());
            }

            //textAnimationName.text = clip.name;
            animator.CrossFadeInFixedTime(clip.name, 0.25f);
            if (_coroutine != null) StopCoroutine(_coroutine);

            if (!clip.isLooping)
            {
                StartCoroutine(ChangeIdleText(clip.length));
            }

        }

        IEnumerator ChangeIdleText(float duration)
        {
            yield return new WaitForSeconds(duration);
            //textAnimationName.text = "Stand_Idle1";
        }
    }
}
