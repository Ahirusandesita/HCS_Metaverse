using UnityEngine;

public class TransformAdapter : MonoBehaviour,IReadonlyPositionAdapter
{
    public Vector3 Position => transform.position;
}
