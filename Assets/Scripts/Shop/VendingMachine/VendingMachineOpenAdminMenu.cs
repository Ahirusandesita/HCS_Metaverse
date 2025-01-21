using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class VendingMachineOpenAdminMenu : MonoBehaviour, IPointerClickRegistrable
{
	[SerializeField]
	private VendingMachineUIManager _vendingMachineUI;
	[SerializeField]
	private TextMeshProUGUI _text;
	private const string _OPEN_ADMIN_MENU_MSG = "OpenAdminMenu";
	private const string _OPEN_BUY_MENU_MSG = "OpenBuyMenu";
	private bool _isOpenAdminMenu;

	private void Start()
	{
		_text.text = _OPEN_ADMIN_MENU_MSG;
		_isOpenAdminMenu = true;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		_vendingMachineUI.OpenAdminMenuUI();
		ToggleOpenMenu();
	}
	[ContextMenu("Click")]
	private void OnPointerClickTest()
	{
		_vendingMachineUI.OpenAdminMenuUI();
		ToggleOpenMenu();
	}

	private void ToggleOpenMenu()
	{
		if (_isOpenAdminMenu)
		{
			_isOpenAdminMenu = false;
			_text.text = _OPEN_BUY_MENU_MSG;
		}
		else
		{
			_isOpenAdminMenu = true;
			_text.text = _OPEN_ADMIN_MENU_MSG;
		}
	}
}
