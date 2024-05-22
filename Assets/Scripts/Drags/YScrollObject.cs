using UnityEngine;

public class YScrollObject : MonoBehaviour, IVerticalOnlyScrollable
{
    public void Scroll(Vector2 move, float sensitivity)
    {
        this.transform.position += -new Vector3(0f, move.y, 0f) / (1500f / sensitivity);
    }
}
