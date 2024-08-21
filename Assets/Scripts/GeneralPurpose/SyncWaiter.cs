using System;
using UnityEngine;

namespace HCSMeta.Function
{
    /// <summary>
    /// ����(Update)�Ŏ��Ԃ��v������N���X
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
        /// ���̃R���X�g���N�^�̓V���O���g����GameObject�𐶐����邽�߁A�ÓI�ȃt�B�[���h�ŃC���X�^���X�����Ȃ�����
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
        /// �w��b���v������
        /// </summary>
        /// <returns>�w��b�����o�߂�����true��Ԃ�</returns>
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
        /// �v�����ꎞ��~
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
        /// �v�����ĊJ
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
        /// Wait�������Ɋ�������
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
        /// ���C���X�^���X��2��ȏ�g�p����ꍇ�̓��Z�b�g���K�v
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