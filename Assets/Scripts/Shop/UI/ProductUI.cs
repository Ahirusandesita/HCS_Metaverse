using UnityEngine;
using TMPro;

public class ProductUI : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _priceText;
	[SerializeField]
	private TextMeshProUGUI _discountedPriceText;
	[SerializeField]
	private TextMeshProUGUI _stockText;
	[SerializeField]
	private TextMeshProUGUI _discountText;

	public void Init(int price, int discountedPrice, float discout, int stock)
	{
		if (discout < 1 && discout > 0)
		{
			_discountedPriceText.text = discountedPrice.ToString("c");
			_discountText.text = discout.ToString("p0");
			_priceText.text =$"<s>{price.ToString("c")}</s>";
		}
		else
		{
			_discountedPriceText.color = Color.white;
			_discountedPriceText.text = price.ToString("c");
			_discountText.text = "";
			_priceText.text = "";
		}
		_stockText.text = $"Stock:{stock}";
	}

	public void UpdateStock(int stock)
	{
		_stockText.text = stock.ToString();
	}
}
