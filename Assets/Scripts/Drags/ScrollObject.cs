using UnityEngine;

public class ScrollObject : MonoBehaviour, IScrollable
{
    void IScrollable.Scroll(Vector2 move)
    {
        this.transform.position += -(Vector3)move / 1000f;
    }
}
