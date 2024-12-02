using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// �C���x���g���P�g�Ǘ�
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
