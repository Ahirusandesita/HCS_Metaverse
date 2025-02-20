using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PageButton : MonoBehaviour, IPointerClickHandler
{
	private enum PageButtonMode
	{
		Next,
		Previous
	}
	[SerializeField]
	private Image _image;
	[SerializeField,HideAtPlaying]
	private PageButtonMode _mode;
	[SerializeField, InterfaceType(typeof(IPageController))]
	private Object _pageController = default;
	private IPageController _pageControllerTemp;
	private IPageController PageController => _pageControllerTemp ??= _pageController as IPageController;
	private void Start()
	{
		if (_mode == PageButtonMode.Next)
		{
			_image.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
		else if (_mode == PageButtonMode.Previous)
		{
			_image.transform.rotation = Quaternion.Euler(0, 0, 180);
		}
	}


	public void OnPointerClick(PointerEventData eventData)
	{
		if (_mode == PageButtonMode.Next)
		{
			PageController.NextPage();
		}
		else if (_mode == PageButtonMode.Previous)
		{
			PageController.PreviousPage();
		}
	}

	[ContextMenu("click")]
	private void OnPointerClickTest()
	{
		if (_mode == PageButtonMode.Next)
		{
			PageController.NextPage();
		}
		else if (_mode == PageButtonMode.Previous)
		{
			PageController.PreviousPage();
		}
	}
}


public interface IPageController
{
	void NextPage();
	void PreviousPage();
}