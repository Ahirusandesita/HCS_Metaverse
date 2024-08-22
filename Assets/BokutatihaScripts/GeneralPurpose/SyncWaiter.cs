using System;
using UnityEngine;

namespace HCSMeta.Function
{
    /// <summary>
    /// 同期(Update)で時間を計測するクラス
    /// </summary>
    public class SyncWaiter
    {
        private class SyncTimeCounter : MonoBehaviour
        {
            public event Action UpdateAction = default;

            private void Update()
            {
                UpdateAction?.Invoke();
            }
        }

        private static SyncTimeCounter syncTimeCounter = default;

        private float waitTime = default;
        private bool isFirst = true;
        private bool isCompleted = false;
        private bool isWaiting = false;
        private bool isPausing = false;


        /// <summary>
        /// このコンストラクタはシングルトンでGameObjectを生成するため、静的なフィールドでインスタンスを作らないこと
        /// </summary>
        public SyncWaiter()
        {
            if (syncTimeCounter is null)
            {
                GameObject empty = new GameObject("SyncTimeCounter");
                UnityEngine.Object.DontDestroyOnLoad(empty);
                syncTimeCounter = empty.AddComponent<SyncTimeCounter>();
            }
        }

        /// <summary>
        /// 指定秒数計測する
        /// </summary>
        /// <returns>指定秒数が経過したらtrueを返す</returns>
        public bool WaitSecounds(float waitTime)
        {
            if (isFirst)
            {
                isFirst = false;
                isWaiting = true;

                this.waitTime = waitTime;
                syncTimeCounter.UpdateAction += TimeCount;
            }

            return isCompleted;
        }

        /// <summary>
        /// 計測を一時停止
        /// </summary>
        public void Pause()
        {
            if (!isWaiting)
            {
                return;
            }

            isPausing = true;
            syncTimeCounter.UpdateAction -= TimeCount;
        }

        /// <summary>
        /// 計測を再開
        /// </summary>
        public void Restart()
        {
            if (!isPausing)
            {
                return;
            }

            isPausing = false;
            syncTimeCounter.UpdateAction += TimeCount;
        }

        /// <summary>
        /// Waitが直ちに完了する
        /// </summary>
        public void Skip()
        {
            if (!isWaiting)
            {
                return;
            }

            waitTime = 0f;
        }

        /// <summary>
        /// 同インスタンスで2回以上使用する場合はリセットが必要
        /// </summary>
        public void Reset()
        {
            isFirst = true;
            isCompleted = false;
        }

        private void TimeCount()
        {
            waitTime -= Time.deltaTime;

            if (waitTime <= 0f)
            {
                isCompleted = true;
                isWaiting = false;
                syncTimeCounter.UpdateAction -= TimeCount;
            }
        }
    }
}