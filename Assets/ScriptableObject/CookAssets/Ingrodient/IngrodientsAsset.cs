using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCSMeta.Activity.Cook.Interface;

namespace HCSMeta.Activity.Cook
{
    public enum ProcessingType
    {
        /// <summary>
        /// �Ă�
        /// </summary>
        Bake,
        /// <summary>
        /// �؂�
        /// </summary>
        Cut,
        /// <summary>
        /// �g����
        /// </summary>
        Fry,
        /// <summary>
        /// �U��
        /// </summary>
        Shake,
        /// <summary>
        /// ����
        /// </summary>
        Boil,
        /// <summary>
        /// ������
        /// </summary>
        Mix
    }
}

namespace HCSMeta.Activity.Cook.Interface
{
    public interface IIngrodientAsset
    {
        void SetUp(List<IngrodientsDetailInformation> ingrodientsDetailInformation);
    }
}

namespace HCSMeta.Activity.Cook
{
    [CreateAssetMenu(fileName = "IngrodientAsset", menuName = "ScriptableObjects/Foods/IngrodientsAsset")]
    public class IngrodientsAsset : ScriptableObject, IIngrodientAsset
    {

        /// <summary>
        /// ��ނ̖��O
        /// </summary>
        [SerializeField]
        private IngrodientsType ingrodientsType;
        [SerializeField]
        private List<IngrodientsDetailInformation> ingrodientsDetailInformations = new List<IngrodientsDetailInformation>();

        public IngrodientsType IngrodientsType => ingrodientsType;
        public IReadOnlyList<IngrodientsDetailInformation> IngrodientsDetailInformations => ingrodientsDetailInformations;

        void IIngrodientAsset.SetUp(List<IngrodientsDetailInformation> ingrodientsDetailInformations)
        {
            this.ingrodientsDetailInformations = ingrodientsDetailInformations;
        }
    }

    /// <summary>
    /// ��ނ̏ڍ׏��
    /// </summary>
    [System.Serializable]
    public class IngrodientsDetailInformation
    {
        /// <summary>
        /// ���H�\�ȃ^�C�v
        /// </summary>
        [SerializeField]
        private ProcessingType processableType;
        /// <summary>
        /// ���H�Ɋ|���鎞��
        /// </summary>
        [SerializeField]
        private float timeItTakes;

        /// <summary>
        /// ���H��̊����i
        /// </summary>
        [SerializeField]
        private Commodity commodity;

        public ProcessingType ProcessingType => processableType;
        public float TimeItTakes => timeItTakes;

        [System.NonSerialized]
        public readonly float MaxTimeItTakes;

        public Commodity Commodity => commodity;

        public IngrodientsDetailInformation(ProcessingType processableType, float timeItTakes, Commodity commodity)
        {
            this.processableType = processableType;
            this.timeItTakes = timeItTakes;
            this.commodity = commodity;
            MaxTimeItTakes = timeItTakes;
        }

        public void SubToTimeItTakes(float subValue)
        {
            timeItTakes -= subValue;
        }

        public bool IsProcessingFinish()
        {
            if (timeItTakes <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
