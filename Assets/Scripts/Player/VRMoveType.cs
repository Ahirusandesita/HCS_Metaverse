using UniRx;

namespace HCSMeta.Player
{
    /// <summary>
    /// VR�̃v���C���[�̈ړ�����
    /// </summary>
    public enum VRMoveType
    {
        /// <summary>
        /// �X�^���_�[�h�Ńi�`�������Ȉړ������B�X�e�B�b�N��|���C�ӂ̕����ɘA���I�Ɉړ����邱�Ƃ��\�B
        /// </summary>
        Natural,
        /// <summary>
        /// �����΍�Ɍ��ʓI�ȃ��[�v�����B�X�e�B�b�N��|���Ă���ԃ��[�v�悪�\������A�����Ǝw�肵���ʒu�ɏu�ԓI�Ɉړ��B
        /// </summary>
        Warp,
    }

    [System.Serializable]
    public class MoveTypeReactiveProperty : ReactiveProperty<VRMoveType>
    {
        public MoveTypeReactiveProperty() : base() { }
        public MoveTypeReactiveProperty(VRMoveType initialValue) : base(initialValue) { }
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(HCSMeta.Player.MoveTypeReactiveProperty))]
    public class MoveTypeReactivePropertyDrawer : InspectorDisplayDrawer { }
}
#endif
