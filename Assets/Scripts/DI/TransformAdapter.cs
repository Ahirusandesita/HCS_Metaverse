using UnityEngine;

public class TransformAdapter : MonoBehaviour,IReadonlyPositionAdapter,IReadonlyTransformAdapter
{
    public Vector3 Position => transform.position;

    public Quaternion Rotation => transform.rotation;

    public Vector3 Size => transform.localScale;
}
