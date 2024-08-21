using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HCSMeta.Activity.Shop
{
    [RequireComponent(typeof(OutlineManager))]
    public class BuyArea : MonoBehaviour
    {
        [SerializeField] private BoxCollider myCollider = default;
        [SerializeField] private MeshRenderer meshRenderer = default;

        private Transform myTransform = default;
        private OutlineManager outlineManager = default;
        private Vector3 initScale = default;
        private Action UpdateAction = default;

        private readonly Collider[] resultNonAlloc = new Collider[4];
        private const float DOSCALE_DURATION = 0.2f;


        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void Reset()
        {
            myCollider ??= GetComponent<BoxCollider>();
            meshRenderer ??= GetComponent<MeshRenderer>();
        }

        private void Awake()
        {
            myTransform = transform;
            outlineManager = GetComponent<OutlineManager>();
        }

        private void Start()
        {
            initScale = myTransform.localScale;
            meshRenderer.enabled = false;
        }

        private void Update()
        {
            UpdateAction?.Invoke();
        }

        /// <summary>
        /// �G���A���Ɏw�肵���^�[�Q�b�g�����݂��邩�ǂ���
        /// </summary>
        /// <param name="targetPos">�^�[�Q�b�g�̍��W</param>
        public bool IsExist(Vector3 targetPos)
        {
            return myCollider.ClosestPoint(targetPos) == targetPos;
        }

        /// <summary>
        /// �G���A��\������
        /// </summary>
        /// <param name="playerPosition"></param>
        public void Display(Vector3 playerPosition)
        {
            myTransform.position = playerPosition + Vector3.right;// Debug
            myTransform.localScale = Vector3.zero;
            meshRenderer.enabled = true;
            UpdateAction += OverlapCheck;
            myTransform.DOScale(initScale, DOSCALE_DURATION);
        }

        /// <summary>
        /// �G���A���\������
        /// </summary>
        public void Hide()
        {
            UpdateAction -= OverlapCheck;
            myTransform.DOScale(Vector3.zero, DOSCALE_DURATION).OnComplete(() => meshRenderer.enabled = false);
        }

        private void OverlapCheck()
        {
            var center = myTransform.position + myCollider.center;
            var halfExtents = myCollider.size / 2;
            int resultCount = Physics.OverlapBoxNonAlloc(center, halfExtents, resultNonAlloc);

            for (int i = 0; i < resultCount; i++)
            {
                if (resultNonAlloc[i].TryGetComponent(out IDisplayItem _))
                {
                    outlineManager.Outline.enabled = true;
                    return;
                }
            }

            outlineManager.Outline.enabled = false;
        }
    }
}