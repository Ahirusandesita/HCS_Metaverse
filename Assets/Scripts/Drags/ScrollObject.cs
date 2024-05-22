using UnityEngine;

public class ScrollObject : MonoBehaviour, IScrollable
{
    public void Scroll(Vector2 moveValue, float sensitivity)
    {
        this.transform.position += -(Vector3)moveValue / (1500f / sensitivity);
    }
}
