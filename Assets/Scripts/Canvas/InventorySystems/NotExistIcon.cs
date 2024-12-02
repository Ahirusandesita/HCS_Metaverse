using UnityEngine;

public class NotExistIcon : MonoBehaviour
{
    [SerializeField]
    private Sprite sprite;
    public Sprite Icon
    {
        get => sprite;
        private set
        {
            sprite = value;
        }
    }
}
