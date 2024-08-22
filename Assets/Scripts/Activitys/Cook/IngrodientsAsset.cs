using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCSMeta.Activity.Cook.Interface;

namespace HCSMeta.Activity.Cook
{
    public enum ProcessingType
    {
        /// <summary>
        /// èƒÇ≠
        /// </summary>
        Bake,
        /// <summary>
        /// êÿÇÈ
        /// </summary>
        Cut,
        /// <summary>
        /// ógÇ∞ÇÈ
        /// </summary>
        Fry,
        /// <summary>
        /// êUÇÈ
        /// </summary>
        Shake,
        /// <summary>
        /// êÜÇ≠
        /// </summary>
        Boil,
        /// <summary>
        /// ç¨Ç∫ÇÈ
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
        /// ãÔçﬁÇÃñºëO
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
    /// ãÔçﬁÇÃè⁄ç◊èÓïÒ
    /// </summary>
    [System.Serializable]
    public class IngrodientsDetailInformation
    {
        /// <summary>
        /// â¡çHâ¬î\Ç»É^ÉCÉv
        /// </summary>
        [SerializeField]
        private ProcessingType processableType;
        /// <summary>
        /// â¡çHÇ…ä|Ç©ÇÈéûä‘
        /// </summary>
        [SerializeField]
        private float timeItTakes;

        /// <summary>
        /// â¡çHå„ÇÃäÆê¨ïi
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
