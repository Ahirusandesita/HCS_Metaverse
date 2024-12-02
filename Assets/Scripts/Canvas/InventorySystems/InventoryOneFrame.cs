using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ƒCƒ“ƒxƒ“ƒgƒŠ‚P˜gŠÇ—
/// </summary>
public class InventoryOneFrame : MonoBehaviour
{
    private NotExistIcon notExistIcon;
    public void Inject(NotExistIcon notExistIcon)
    {
        this.notExistIcon = notExistIcon;
    }

    [SerializeField]
    private Image icon;

    public void PutAway(ItemAsset itemAsset)
    {
        icon.sprite = itemAsset.ItemIcon;
    }
    public void TakeOut()
    {
        icon.sprite = null;
    }
}
