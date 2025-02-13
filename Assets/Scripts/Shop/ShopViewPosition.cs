using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopViewPosition : MonoBehaviour
{
	[SerializeField]
	private ItemAsset.ItemSize _itemSize;
	[SerializeField]
	private bool _isRecommend;
	private Transform _myTransform;
	public Vector3 Postion => (_myTransform ??= transform).position;
	public ItemAsset.ItemSize ItemSize => _itemSize;
	public bool IsRecommend => _isRecommend;
	public Vector3 Direction => _myTransform.eulerAngles;
}
