using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IceMilkTea.StateMachine;

namespace HCSMeta.Activity.Fishing
{
    public partial class FishingManager : MonoBehaviour
    {
        private enum EventID
        {
            /// <summary>
            /// �����𓊂���
            /// </summary>
            Start,
            /// <summary>
            /// �q�b�g�I
            /// </summary>
            Hit,
            /// <summary>
            /// �\�ꂽ
            /// </summary>
            MoveAround,
            /// <summary>
            /// ��ꂽ
            /// </summary>
            Tired,
            /// <summary>
            /// �ނ�グ���I
            /// </summary>
            Pullup,
            /// <summary>
            /// ���������c
            /// </summary>
            LetEscape,
            /// <summary>
            /// �ނ����߂�
            /// </summary>
            Cancel,
        }


        [System.Serializable]
        private class FishInfo
        {
            [SerializeField] private FishAsset fishAsset = default;
            [Tooltip("�m����䗦�œ���")]
            [SerializeField, Min(0f)] private float percent = default;

            public FishAsset Asset => fishAsset;
            public float Percent => percent;
        }


        [SerializeField] private List<FishInfo> fishPool = default;
        [SerializeField, Min(0f)] private float minTimeToHit = 5f;
        [SerializeField, Min(0f)] private float maxTimeToHit = 10f;

        private ImtStateMachine<FishingManager, EventID> stateMachine = default;
        private float[] fishPercents = default;
        private FishAsset targetFish = default;


        private void Awake()
        {
            #region Editor Check
#if UNITY_EDITOR
            float allPercent = 0f;
            foreach (var item in fishPool)
            {
                allPercent += item.Percent;
            }

            if (allPercent > 100f)
            {
                throw new System.InvalidOperationException($"{nameof(FishingManager)} �� {nameof(fishPool)} ���̊m���̍��v��100�𒴂��Ă��܂��B");
            }
#endif
            #endregion

            stateMachine = new ImtStateMachine<FishingManager, EventID>(this);
            stateMachine.AddTransition<NonInteracted, Swim>(EventID.Start);
            stateMachine.AddTransition<Swim, MoveAround>(EventID.Hit);
            stateMachine.AddTransition<MoveAround, Tired>(EventID.Tired);
            stateMachine.AddTransition<Tired, MoveAround>(EventID.MoveAround);
            stateMachine.AddTransition<Tired, Pullup>(EventID.Pullup);
            stateMachine.AddTransition<MoveAround, LetEscape>(EventID.LetEscape);
            stateMachine.SetStartState<NonInteracted>();
        }

        private void Start()
        {
            #region Percent Initialize
            fishPercents = new float[fishPool.Count];
            float percentCount = 0f;
            for (int i = 0; i < fishPercents.Length; i++)
            {
                percentCount += fishPool[i].Percent;
                fishPercents[i] = percentCount;
            }
            #endregion

            // StateMachine���N��
            stateMachine.Update();
        }

        private void Update()
        {
            // StateMachine���X�V
            stateMachine.Update();
        }

        /// <summary>
        /// ���𒊑I����
        /// </summary>
        private void LotteryForFish()
        {
            float rnd = Random.Range(0f, fishPercents[^1]);
            int index;
            for (index = 0; index < fishPercents.Length; index++)
            {
                if (rnd < fishPercents[index])
                {
                    break;
                }
            }

            targetFish = fishPool[index].Asset;
        }
    }
}