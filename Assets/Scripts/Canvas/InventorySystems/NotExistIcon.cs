using UnityEngine;

public class NotExistIcon : MonoBehaviour
{
    [SerializeField]
    private Sprite sprite;
    public Sprite Sprite
    {
        get => sprite;
        private set
        {
            sprite = value;
        }
    }
}
