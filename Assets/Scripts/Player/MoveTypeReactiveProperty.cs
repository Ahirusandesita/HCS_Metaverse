using UniRx;

[System.Serializable]
public class MoveTypeReactiveProperty : ReactiveProperty<VRMoveType>
{
    public MoveTypeReactiveProperty() : base() { }
    public MoveTypeReactiveProperty(VRMoveType initialValue) : base(initialValue) { }
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
    [CustomPropertyDrawer(typeof(MoveTypeReactiveProperty))]
    public class MoveTypeReactivePropertyDrawer : InspectorDisplayDrawer { }
}
#endif
