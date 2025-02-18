using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kuma;

public class ShopViewPosition : MonoBehaviour
{
	[SerializeField]
	private ItemAsset.ItemSize _itemSize;
	[SerializeField]
	private bool _isRecommend;
	private TransformGetter _myTransformGetter;
	
	public ItemAsset.ItemSize ItemSize => _itemSize;
	public bool IsRecommend => _isRecommend;
	public TransformGetter TransformGetter => _myTransformGetter ??= new TransformGetter(transform);
}

namespace Kuma
{
	public class TransformGetter
	{
		private Transform _myTransform;
		public TransformGetter(Transform myTransform)
		{
			this._myTransform = myTransform;
		}
		public Vector3 Position => _myTransform.position;
		public Vector3 ForwardDirection => _myTransform.forward;
		public Vector3 RightDirection => _myTransform.right;
		public Vector3 UpDirection => _myTransform.up;
	}
}
