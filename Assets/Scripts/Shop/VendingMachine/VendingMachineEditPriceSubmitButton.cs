using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;
using System.Threading;
using System;

public class VendingMachineEditPriceSubmitButton : MonoBehaviour, IPointerClickRegistrable
{
	[SerializeField]
	private VendingMachineEditPriceUI _vendingMachineEditPriceUI;
	private Tweener _positionTweener;
	private Tweener _colorTweener;
	private RectTransform _myTransform;
	private Vector3 _initPosition;
	private Color _initColor = default;
	private bool _isComplete = false;
	[Header("ƒpƒ‰ƒ[ƒ^")]
	[SerializeField]
	private Image _myImage;
	[SerializeField]
	private Color _incorrectColor = Color.red;
	[SerializeField]
	private float _incorrectShakeTime = 1;
	[SerializeField]
	private float _incorrectShakeStrength = 1;
	[SerializeField]
	private int _incorrectShakeVibrato = 10;
	[SerializeField]
	private float _incorrectShakeRandomness = 180;
	[SerializeField]
	private float _incorrectColorChangeTime = 1;
	[SerializeField]
	private float _incorrectColorChangeMerginTime = 0;
	[SerializeField]
	private float _incorrectColorReturnTime = 1;
	private void Start()
	{
		_myTransform = (transform as RectTransform);
		_initPosition = _myTransform.position;
		_initColor = _myImage.color;
	}
	public async void OnPointerClick(PointerEventData data)
	{
		bool result = await _vendingMachineEditPriceUI.Submit();
		//Ž¸”s‚µ‚½‚Æ‚«
		if (!result)
		{
			CancelEffect();
		}
	}

	private void CancelEffect()
	{
		StartShake();
		StartChangeColor();
	}

	private void StartShake()
	{
		// ‘O‰ñ‚Ìˆ—‚ªŽc‚Á‚Ä‚¢‚ê‚Î’âŽ~‚µ‚Ä‰ŠúˆÊ’u‚É–ß‚·
		if (_positionTweener != null)
		{
			_positionTweener.Kill();
			_myTransform.position = _initPosition;
		}
		// —h‚êŠJŽn
		_positionTweener = _myTransform.DOShakePosition(
			_incorrectShakeTime,
			_incorrectShakeStrength,
			_incorrectShakeVibrato,
			_incorrectShakeRandomness
			);
	}

	private async UniTaskVoid StartChangeColor()
	{
		CancellationTokenSource cts = new CancellationTokenSource();
		_isComplete = false;
		if (_colorTweener != null)
		{
			_colorTweener.Kill();
			_myImage.color = _initColor;
			cts.Cancel();

		}
		_colorTweener = _myImage.DOColor(_incorrectColor, _incorrectColorChangeTime)
			.OnComplete(() =>
			{
				_isComplete = true;
			});
		await UniTask.WaitUntil(() => _isComplete);
		try
		{
			await UniTask.WaitForSeconds(_incorrectColorChangeMerginTime,
				cancellationToken: cts.Token);
		}
		catch (OperationCanceledException) { }
		_colorTweener = _myImage.DOColor(_initColor, _incorrectColorReturnTime);
	}

	[ContextMenu("Click")]
	private async void Test()
	{
		bool result = await _vendingMachineEditPriceUI.Submit();
		//Ž¸”s‚µ‚½‚Æ‚«
		if (!result)
		{
			CancelEffect();
		}
	}
}
