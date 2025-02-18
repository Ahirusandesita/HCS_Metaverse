using UniRx;

[System.Serializable]
public class MoveTypeReactiveProperty : ReactiveProperty<VRMoveType>
{
    public MoveTypeReactiveProperty() : base() { }
    public MoveTypeReactiveProperty(VRMoveType initialValue) : base(initialValue) { }
}

[System.Serializable]
public class RotateTypeReactiveProperty : ReactiveProperty<VRRotateType>
{
    public RotateTypeReactiveProperty() : base() { }
    public RotateTypeReactiveProperty(VRRotateType initialValue) : base(initialValue) { }
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
    [CustomPropertyDrawer(typeof(MoveTypeReactiveProperty))]
    public class MoveTypeReactivePropertyDrawer : InspectorDisplayDrawer { }

    [CustomPropertyDrawer(typeof(RotateTypeReactiveProperty))]
    public class RotateTypeReactivePropertyDrawer : InspectorDisplayDrawer { }
}
#endif
