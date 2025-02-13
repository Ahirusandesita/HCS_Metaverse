using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
public class CartUIInfo
{
    public int row;
    public int column;
    public InCartItemUI inCartItemUI;
    public CartUIInfo(int row, int column, InCartItemUI inCartItemUI)
    {
        this.row = row;
        this.column = column;
        this.inCartItemUI = inCartItemUI;
    }
}
public class ShopCartUIManager : MonoBehaviour
{
    [SerializeField]
    private ItemBundleAsset _allItemAsset;
    [SerializeField]
    private InCartItemUI _itemIconPrefab;
    [SerializeField]
    private ShopCart _shopCart;
    [SerializeField]
    private Transform _shopUIParent;
    [SerializeField]
    private GameObject _shopCanvasObject;
    [SerializeField]
    private ProductUI _productUIPrefab;
    [SerializeField]
    private Transform _productParent;
    [SerializeField]
    private TextMeshProUGUI _totalPriceText;
    [SerializeField]
    private RectTransform _startPositionTransform;
    [SerializeField]
    private DragSystem _dragSystem = default;
    [SerializeField]
    private ScrollTransformInject _scrollTransformInject = default;
    [SerializeField]
    private Vector3 _priceCardPositionOffset = default;
    private Dictionary<int, ProductUI> _productUIs = new();
    private List<CartUIInfo> cartUIInfos = new List<CartUIInfo>();
    private float _offsetX = 100;
    private float _offsetY = 100;
    private int _horizontalLimit = 2;


    private void Start()
    {
        RectTransform iconTransform = _itemIconPrefab.transform as RectTransform;
        TatalPriceDisplay();
        _offsetX = iconTransform.sizeDelta.x;

        _offsetY = iconTransform.sizeDelta.y;
    }

    public void AddProductUI(int id, int price, int discountedPrice, int stock, float discount, Vector3 position)
    {
        ProductUI productUITemp = Instantiate(_productUIPrefab, _productParent);
        productUITemp.Init(price, discountedPrice, discount, stock);
        _productUIs.Add(id, productUITemp);

        productUITemp.transform.position = position + _priceCardPositionOffset;
    }

    public void AddCartUI(int id)
    {
        if (!_shopCanvasObject.activeSelf)
        {
            _shopCanvasObject.SetActive(true);
        }
        InCartItemUI uiIconTemp;
        if (!cartUIInfos.Any(info => info.inCartItemUI.ID == id))
        {
            ItemAsset itemAsset = _allItemAsset.GetItemAssetByID(id);
            int currentYPosition = (cartUIInfos.Count / _horizontalLimit);
            Vector2 popAnchoredPosition =
                _startPositionTransform.anchoredPosition
                + Vector2.right * (cartUIInfos.Count % _horizontalLimit) * _offsetX
                + Vector2.up * currentYPosition * -_offsetY;
            uiIconTemp = Instantiate(_itemIconPrefab, _shopUIParent);

            ///Ž„///
            Vector2 nowPosition = default;
            if (cartUIInfos.Count != 0)
            {
                if (cartUIInfos[cartUIInfos.Count - 1].row == cartUIInfos.Count / _horizontalLimit)
                {
                    if (cartUIInfos.Count % _horizontalLimit != 0)
                    {
                        nowPosition = cartUIInfos[cartUIInfos.Count - 1].inCartItemUI.GetComponent<RectTransform>().anchoredPosition;
                        nowPosition += Vector2.right * _offsetX;
                    }
                    else
                    {
                        nowPosition = cartUIInfos[cartUIInfos.Count - _horizontalLimit].inCartItemUI.GetComponent<RectTransform>().anchoredPosition;
                        nowPosition.y -= _offsetY;
                    }
                }
                else
                {
                    nowPosition = cartUIInfos[cartUIInfos.Count - _horizontalLimit].inCartItemUI.GetComponent<RectTransform>().anchoredPosition;
                    nowPosition.y -= _offsetY;
                }
            }
            else
            {
                nowPosition = popAnchoredPosition;
            }
            uiIconTemp.Init(itemAsset.ItemIcon, this, popAnchoredPosition, id, _dragSystem, _scrollTransformInject, currentYPosition, nowPosition);
            //////
            cartUIInfos.Add(new CartUIInfo(cartUIInfos.Count / _horizontalLimit, cartUIInfos.Count % _horizontalLimit, uiIconTemp));

            //////
            //if (cartUIInfos.Count % (_horizontalLimit + 1) == 0)
            //{
            //    foreach (CartUIInfo item in cartUIInfos)
            //    {
            //        item.inCartItemUI.UpdateLimit(_offsetY);
            //    }
            //    //_yPosition = currentYPosition;
            //}
            int s = 0;
            if(cartUIInfos[cartUIInfos.Count - 1].row + 1 - _horizontalLimit > 0)
            {
                s = cartUIInfos[cartUIInfos.Count - 1].row + 1 - _horizontalLimit;
            }
            foreach (CartUIInfo item in cartUIInfos)
            {
                item.inCartItemUI.UpdateLimit(_offsetY * (s));
            }
        }
        else
        {
            uiIconTemp = cartUIInfos.Where(info => info.inCartItemUI.ID == id).First().inCartItemUI;
        }
        uiIconTemp.UpdateCount(_shopCart.InCarts[id]);
        TatalPriceDisplay();
    }

    private void TatalPriceDisplay()
    {
        _totalPriceText.text = _shopCart.ClacTotalPrice().ToString("c");
    }

    public void BuyButtonPush()
    {
        Clear();
        _shopCart.Buy().Forget();
    }
    private void Clear()
    {
        foreach (CartUIInfo uiIcon in cartUIInfos)
        {
            Destroy(uiIcon.inCartItemUI.gameObject);
        }
        cartUIInfos.Clear();
        _shopCanvasObject.SetActive(false);
    }

    public void DestoryCartUI(int id)
    {
        _shopCart.RemoveCart(id);
        if (!_shopCart.InCarts.Keys.Contains(id))
        {
            cartUIInfos.Where(info => info.inCartItemUI.ID == id).First().inCartItemUI.UpdateCount(0);
            //Ž„//
            int index = cartUIInfos.IndexOf(cartUIInfos.Where(info => info.inCartItemUI.ID == id).First());
            //if (test_inCartItemUIs.Count > index)
            //{
            //    for (int i = 0; i < index; i++)
            //    {
            //        test_inCartItemUIs[i].UpdateLimit(-_offsetX);
            //    }
            //    for (int i = index; i < test_inCartItemUIs.Count; i++)
            //    {
            //        test_inCartItemUIs[i].UpdateDownLimit(_offsetY);
            //    }
            //    for (int i = test_inCartItemUIs.Count - 1; i > index; i--)
            //    {
            //        test_inCartItemUIs[i].GetComponent<RectTransform>().anchoredPosition = test_inCartItemUIs[i - 1].GetComponent<RectTransform>().anchoredPosition;
            //    }
            //}
            //test_inCartItemUIs.Remove(_itemIcons[id]);
            //////////
            //_itemIcons.Remove(id);

            //if (0 >= _shopCart.InCarts.Keys.Count)
            //{
            //    _shopCanvasObject.SetActive(false);
            //}
        }
        else
        {
            //_itemIcons[id].UpdateCount(_shopCart.InCarts[id]);
        }
    }
}
