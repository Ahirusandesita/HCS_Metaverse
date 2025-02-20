using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Kuma;

public class VisualShop : MonoBehaviour, ISelectedNotification, IDependencyInjector<PlayerBodyDependencyInformation>
{
	//�J�[�g�N���X�����
	[SerializeField] private ItemBundleAsset allItemAsset = default;
	[SerializeField] private BuyArea buyArea = default;
	[SerializeField, HideAtPlaying] private List<ShopViewPosition> smallViewPoints = new();
	[SerializeField, HideAtPlaying] private List<ShopViewPosition> largeViewPoints = new();
	[SerializeField, HideAtPlaying] private List<ShopViewPosition> recommendViewPoints = new();
	[SerializeField] private ShopCart shopCart = default;
	[SerializeField] private ShopCartUIManager uiManager = default;
	[SerializeField] private int productId = 10962;
	private Dictionary<int, int> prices = new();
	private List<GameObject> displayedItems = new();
	private IReadonlyPositionAdapter positionAdapter = default;
	private int _shopID = 2;

	public int ShopID => _shopID;

	public int GetPrice(int id)
	{

		if (prices.Keys.Contains(id))
		{
			return prices[id];
		}

		XDebug.LogWarning($"{id}:����id�͌�����܂���ł���");
		return -1;
	}

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	private void Reset()
	{
#if UNITY_EDITOR
		// Conditional�̓��\�b�h���̓R���p�C������Ă��܂��̂ŁA�d���Ȃ���d
		allItemAsset = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(ItemBundleAsset)}")
				.Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
				.Select(UnityEditor.AssetDatabase.LoadAssetAtPath<ItemBundleAsset>)
				.First();
#endif
		buyArea = GetComponentInChildren<BuyArea>();
	}

	private void Awake()
	{
		PlayerInitialize.ConsignmentInject_static(this);
	}
	private void Start()
	{
		int? shopID = PlayerDontDestroyData.Instance.MovableShopID;
		if(shopID == null) 
		{
			XDebug.LogError("ShopID������܂���");
			return; 
		}
		_shopID = (int)shopID;
		ShopViewPosition[] shopViewPositions
			= FindObjectsByType<ShopViewPosition>(FindObjectsSortMode.None);

		foreach (ShopViewPosition viewPosition in shopViewPositions)
		{
			if (viewPosition.IsRecommend)
			{
				recommendViewPoints.Add(viewPosition);
				continue;
			}
			if (viewPosition.ItemSize == ItemAsset.ItemSize.Small)
			{
				smallViewPoints.Add(viewPosition);
				continue;
			}
			else if (viewPosition.ItemSize == ItemAsset.ItemSize.Large)
			{
				largeViewPoints.Add(viewPosition);
				continue;
			}
		}
		InstanceShop();
	}

	private void OnDisable()
	{
		DestroyShop();
	}

	private async void InstanceShop()
	{
		//����
		displayedItems = new List<GameObject>();
		WebAPIRequester webAPIRequester = new WebAPIRequester();

		var data = await webAPIRequester.PostShopEntry(_shopID);
		int smallItemCounter = 0;
		int largeItemCounter = 0;
		int recommendCounter = 0;
		for (int i = 0; i < data.ItemList.Count; i++)
		{
			InstantiateShopObject(data.ItemList[i], ref smallItemCounter, ref largeItemCounter);
		}
		var dataRecommend = await webAPIRequester.PostShopRecommend(_shopID);
		for (int i = 0; i < dataRecommend.ItemList.Count; i++)
		{
			InstantiateRecommendShopObject(dataRecommend.ItemList[i], ref recommendCounter);
		}
	}

	private void InstantiateRecommendShopObject(
		WebAPIRequester.ItemLineup itemLineup,
		ref int recommendCounter)
	{
		var asset = allItemAsset.GetItemAssetByID(itemLineup.ItemID);

		int discountedPrice = Mathf.FloorToInt(itemLineup.Price
			- (itemLineup.Price * itemLineup.Discount));

		int stock = itemLineup.Stock;
		ShopViewPosition shopViewPosition = recommendViewPoints[recommendCounter];
		recommendCounter++;

		var item = IDisplayItem.Instantiate(
			asset,
			shopViewPosition.TransformGetter.Position,
			Quaternion.Euler(shopViewPosition.TransformGetter.ForwardDirection),
			this);
		displayedItems.Add(item.gameObject);
		if (!prices.Keys.Contains(itemLineup.ItemID))
		{
			uiManager.AddProductUI(
			itemLineup.ItemID,
			itemLineup.Price,
			discountedPrice,
			stock,
			itemLineup.Discount,
			shopViewPosition.TransformGetter,
			item.gameObject.GetBounds().center
			);
			prices.Add(itemLineup.ItemID, discountedPrice);
		}

		Bounds bounds = item.gameObject.GetBounds();
		Vector3 underCenter = bounds.center - bounds.extents.y * Vector3.up;
		item.gameObject.transform.position += item.gameObject.transform.position - underCenter;
	}

	private void InstantiateShopObject(
		WebAPIRequester.ItemLineup itemLineup,
		ref int smallItemCounter,
		ref int largeItemCounter)
	{
		var asset = allItemAsset.GetItemAssetByID(itemLineup.ItemID);

		int discountedPrice = Mathf.FloorToInt(itemLineup.Price
			- (itemLineup.Price * itemLineup.Discount));

		int stock = itemLineup.Stock;
		ShopViewPosition shopViewPosition = default;
		if (itemLineup.Size == 1)
		{
			shopViewPosition = smallViewPoints[smallItemCounter];
			smallItemCounter++;
		}
		//�ق��̃T�C�Y���ǉ������\�������邽��else if�ɂ��Ă�
		else if (itemLineup.Size == 0)
		{
			shopViewPosition = largeViewPoints[largeItemCounter];
			largeItemCounter++;
		}
		var item = IDisplayItem.Instantiate(asset,
			shopViewPosition.TransformGetter.Position,
			Quaternion.LookRotation(shopViewPosition.TransformGetter.ForwardDirection),
			this);
		displayedItems.Add(item.gameObject);
		item.gameObject.layer = Layer.ITEM_LAYER_NUM;
		if (!prices.Keys.Contains(itemLineup.ItemID))
		{
			uiManager.AddProductUI(
			itemLineup.ItemID,
			itemLineup.Price,
			discountedPrice,
			stock,
			itemLineup.Discount,
			shopViewPosition.TransformGetter,
			item.gameObject.GetBounds().center
			);
			prices.Add(itemLineup.ItemID, discountedPrice);
		}
		Bounds bounds = item.gameObject.GetBounds();
		Vector3 underCenter = bounds.center - bounds.extents.y * Vector3.up;
		item.gameObject.transform.position += item.gameObject.transform.position - underCenter;
		buyArea.Display(bounds.center,new TransformGetter(item.gameObject.transform));
	}

	private void DestroyShop()
	{
		//�폜
		foreach (var obj in displayedItems)
		{
			Destroy(obj);
		}
	}

	public void Select(SelectArgs selectArgs)
	{
		//���܂ꂽ���ɐV�������̂𐶐�����
		var itemSelectArgs = selectArgs as ItemSelectArgs;
		var asset = allItemAsset.GetItemAssetByID(itemSelectArgs.id);
		var position = itemSelectArgs.position;

		// �I�����ꂽ�A�C�e���Ɠ������̂𐶐�����i�R�s�[��\���j
		var item = IDisplayItem.Instantiate(asset, position, Quaternion.identity, this);
		displayedItems.Add(item.gameObject);
		item.gameObject.layer = Layer.ITEM;

		//�����̕\��
		buyArea.Display(item.gameObject.GetBounds().center, new TransformGetter(item.gameObject.transform));
	}

	public void Unselect(SelectArgs selectArgs)
	{
		//���񂾂��̂𗣂����Ƃ��̏���
		var itemSelectArgs = selectArgs as ItemSelectArgs;
		var unselectedPosition = itemSelectArgs.gameObject.transform.position;

		// �͂񂾃A�C�e���𗣂����|�C���g���A�w���G���A��������w��
		if (buyArea.IsExist(unselectedPosition))
		{
			//�J�[�g�ɓ����
			shopCart.AddCart(itemSelectArgs.id);
		}
		//�Ƃ�����������Ă���
		displayedItems.Remove(itemSelectArgs.gameObject);
		//�W������Ă���I�u�W�F�N�g��j��
		Destroy(itemSelectArgs.gameObject);
		//�������B��
		buyArea.Hide();
	}

	void IDependencyInjector<PlayerBodyDependencyInformation>.Inject(PlayerBodyDependencyInformation information)
	{
		positionAdapter = information.PlayerBody;
	}
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
	[CustomEditor(typeof(VisualShop))]
	public class ShopEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Space(12f);

			if (GUILayout.Button("Update Display Options"))
			{
				try
				{
					ItemIDViewDrawer.UpdateDisplayOptions();
				}
				// �v�f�Ȃ���ԂŃ{�^�������Ɨ�O�o�遨�������̂ň���Ԃ�
				catch (System.NullReferenceException) { }
			}
		}
	}
}
#endif