using UniRx;

namespace HCSMeta.Player
{
    /// <summary>
    /// VRのプレイヤーの移動方式
    /// </summary>
    public enum VRMoveType
    {
        /// <summary>
        /// スタンダードでナチュラルな移動方式。スティックを倒し任意の方向に連続的に移動することが可能。
        /// </summary>
        Natural,
        /// <summary>
        /// 酔い対策に効果的なワープ方式。スティックを倒している間ワープ先が表示され、離すと指定した位置に瞬間的に移動。
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
