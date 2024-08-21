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
            /// 浮きを投げる
            /// </summary>
            Start,
            /// <summary>
            /// ヒット！
            /// </summary>
            Hit,
            /// <summary>
            /// 暴れた
            /// </summary>
            MoveAround,
            /// <summary>
            /// 疲れた
            /// </summary>
            Tired,
            /// <summary>
            /// 釣り上げた！
            /// </summary>
            Pullup,
            /// <summary>
            /// 逃がした…
            /// </summary>
            LetEscape,
            /// <summary>
            /// 釣りをやめる
            /// </summary>
            Cancel,
        }


        [System.Serializable]
        private class FishInfo
        {
            [SerializeField] private FishAsset fishAsset = default;
            [Tooltip("確率を比率で入力")]
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
                throw new System.InvalidOperationException($"{nameof(FishingManager)} の {nameof(fishPool)} 内の確率の合計が100を超えています。");
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

            // StateMachineを起動
            stateMachine.Update();
        }

        private void Update()
        {
            // StateMachineを更新
            stateMachine.Update();
        }

        /// <summary>
        /// 魚を抽選する
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