using System.Collections.Generic;
using UnityEngine;

namespace Layer_lab._3D_Casual_Character
{
    public class AnimationControl : MonoBehaviour
    {
        [SerializeField] private AnimationClip[] animDance;
        [SerializeField] private AnimationClip[] animIdle;
        [SerializeField] private AnimationClip[] animReaction;
        [SerializeField] private AnimationClip[] animInteraction;
        [SerializeField] private AnimationClip[] animEmoji;
        [SerializeField] private AnimationClip[] animAction;
        public enum AnimType
        {
            Dance,
            Idle,
            Reaction,
            Interaction,
            Emoji,
            Action
        }
        public struct AnimData
        {
            public readonly AnimType AnimType;
            public readonly int Index;
            public AnimData(AnimType animType,int index)
            {
                this.AnimType = animType;
                this.Index = index;
            }
        }

        [SerializeField] private ButtonAnimation button;
        [SerializeField] private Transform content;

        [SerializeField] private Sprite[] spriteIcons;

        private void Start()
        {
            SpawnAnimationButton(animIdle, "idle");
            SpawnAnimationButton(animAction, "action");
            SpawnAnimationButton(animReaction, "reaction");
            SpawnAnimationButton(animInteraction, "interaction");
            SpawnAnimationButton(animEmoji, "emotion");
            SpawnAnimationButton(animDance, "dance");
            
            

            button.gameObject.SetActive(false);
        }


        private Sprite GetSprite(string name)
        {
            foreach (var sprite in spriteIcons)
            {
                if (sprite.name.Contains(name))
                {
                    return sprite;
                }
            }

            return null;
        }
    
        public void SpawnAnimationButton(AnimationClip[] animationClips, string name)
        {
            for (int i = 0; i < animationClips.Length; i++)
            {
                ButtonAnimation buttonAnimation = Instantiate(button, content, false);
                buttonAnimation.SetButton(animationClips[i], GetSprite(name));
            }
        }
        
        public AnimData GetAnimData(AnimationClip animationClip)
        {
            for(int i = 0; i < animDance.Length; i++)
            {
                if(animDance[i] == animationClip)
                {
                    return new AnimData(AnimType.Dance, i);
                }
            }

            for (int i = 0; i < animIdle.Length; i++)
            {
                if (animIdle[i] == animationClip)
                {
                    return new AnimData(AnimType.Idle, i);
                }
            }

            for (int i = 0; i < animReaction.Length; i++)
            {
                if (animReaction[i] == animationClip)
                {
                    return new AnimData(AnimType.Reaction, i);
                }
            }

            for (int i = 0; i < animInteraction.Length; i++)
            {
                if (animInteraction[i] == animationClip)
                {
                    return new AnimData(AnimType.Reaction, i);
                }
            }

            for (int i = 0; i < animEmoji.Length; i++)
            {
                if (animEmoji[i] == animationClip)
                {
                    return new AnimData(AnimType.Emoji, i);
                }
            }

            for (int i = 0; i < animAction.Length; i++)
            {
                if (animAction[i] == animationClip)
                {
                    return new AnimData(AnimType.Action, i);
                }
            }
            return new AnimData(AnimType.Dance, 0);

        }
    
    }
}
