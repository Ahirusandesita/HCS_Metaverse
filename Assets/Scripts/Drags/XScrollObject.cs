using UnityEngine;

public class XScrollObject : MonoBehaviour, IHorizontalOnlyScrollable
{
    public void Scroll(Vector2 moveValue, float sensitivity)
    {
        this.transform.position += -new Vector3(moveValue.x,0f, 0f) / (1500f / sensitivity);
    }
}
