using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using System.Threading.Tasks;

namespace HCSMeta.Activity.Cook
{
    public class Puttable : MonoBehaviour
    {
        private LockedCuttingBoard _parentLockedCuttingObject = default;

        // 掴んだ時や離した時にイベントを実行するクラス
        private PointableUnityEventWrapper _pointableUnityEventWrapper;

        private void Awake()
        {
            _pointableUnityEventWrapper = this.GetComponentInChildren<PointableUnityEventWrapper>();
            _pointableUnityEventWrapper.WhenSelect.AddListener((action) => { Select(); });

        }

        public void SetLockedCuttingObject(LockedCuttingBoard lockedCuttingObject)
        {
            // 
            _parentLockedCuttingObject = lockedCuttingObject;
        }

        public void Select()
        {
            DestroyThis();
        }

        public void DestroyThis()
        {
            Destroy(this);
        }

        private void OnDestroy()
        {
            _pointableUnityEventWrapper.WhenSelect.RemoveListener((action) => { Select(); });
            _parentLockedCuttingObject.CanselCutting();
        }
    }
}