using UnityEngine;

namespace HCSMeta.Activity
{
    public class StoperObject : MonoBehaviour
    {
        [SerializeField, Tooltip("�ڐG������s��Collider")]
        private Collider _stoperColliter = default;

        // 
        private Vector3 _hitBoxCenter = default;

        // 
        private Vector3 _hitBoxSize = default;

        // 
        private Quaternion _hitBoxRotation = default;

        // 
        private string _stoppableObjectsTag = "StoppableObject";

        // 
        private enum DebugMode_StoperObject
        {
            FixedUpdate,
            Update,
            LateUpdate
        }

        [SerializeField]
        DebugMode_StoperObject mode = DebugMode_StoperObject.Update;

        private void Start()
        {
            // 
            _hitBoxCenter = _stoperColliter.bounds.center;

            // 
            _hitBoxSize = _stoperColliter.bounds.size / 2;

            // 
            _hitBoxRotation = this.transform.rotation;
        }

        private void Update()
        {
            // 
            Collider[] hitColliders = Physics.OverlapBox(_hitBoxCenter, _hitBoxSize, _hitBoxRotation);

            bool onFlag = false;

            if (hitColliders is null)
            {
                Debug.Log($"�Ȃɂ��������ĂȂ����");
                return;
            }

            // 
            foreach (Collider hitCollider in hitColliders)
            {
                // 
                if (!hitCollider.transform.root.TryGetComponent<Stoppable>(out var tmp))
                {
                    // 
                    continue;
                }

                onFlag = true;

                // 
                if (hitCollider.transform.root.TryGetComponent<StopData>(out var stopData))
                {
                    // 
                    stopData.SetIsHitStopper(true);

                    Debug.Log($"{hitCollider.gameObject.name} is Stopping now");
                }
                // 
                else
                {
                    // 
                    hitCollider.transform.root.gameObject.AddComponent<StopData>();

                    tmp.StoppingEvent();

                    Debug.Log($"{hitCollider.gameObject.name} �� StopData�ǉ��������");
                }
            }
        }
    }

}