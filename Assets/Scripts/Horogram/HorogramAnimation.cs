using UnityEngine;

public class HorogramAnimation : MonoBehaviour
{
    private void Update()
    {
        this.transform.Rotate(0f, 180f * Time.deltaTime, 0f);
    }
}
