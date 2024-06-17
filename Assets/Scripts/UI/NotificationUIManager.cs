using TMPro;
using UnityEngine;

public class NotificationUIManager : MonoBehaviour
{
    private static NotificationUIManager s_instance = default;
    public static NotificationUIManager Instance => s_instance;

    [SerializeField] private TextMeshProUGUI text = default;


    private void Awake()
    {
        if (s_instance is not null)
        {
            Destroy(this);
            return;
        }

        s_instance = this;
    }

    public void DisplayInteraction()
    {
        text.text = "Interact";
    }

    public void HideInteraction()
    {
        text.text = string.Empty;
    }
}
