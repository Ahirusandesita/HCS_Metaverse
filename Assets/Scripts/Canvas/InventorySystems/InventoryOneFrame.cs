using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// ÉCÉìÉxÉìÉgÉäÇPògä«óù
/// </summary>
public class InventoryOneFrame : MonoBehaviour
{
    private NotExistIcon notExistIcon;
    [SerializeField]
    private TextMeshProUGUI textMesh;
    [SerializeField]
    private Image valueFrame;
    [SerializeField]
    private Image icon;

    private void Awake()
    {
        textMesh.text = "";
        valueFrame.enabled = false;
    }
    public void Inject(NotExistIcon notExistIcon)
    {
        this.notExistIcon = notExistIcon;
    }

    
    public void PutAway(ItemAsset itemAsset,int hasItemValue)
    {
        icon.sprite = itemAsset.ItemIcon == null ? notExistIcon.Icon : itemAsset.ItemIcon;
        textMesh.text = hasItemValue.ToString();
        valueFrame.enabled = true;
    }
    public void TakeOut()
    {
        icon.sprite = null;
        textMesh.text = "";
        valueFrame.enabled = false;
    }
}
