using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KumaDebug;

public class VisualShop : MonoBehaviour, ISelectedNotification, IDependencyInjector<PlayerBodyDependencyInformation>
{
	//�J�[�g�N���X�����
	[SerializeField] private ItemBundleAsset allItemAsset = default;
	[SerializeField] private BuyArea buyArea = default;
	[SerializeField] private List<Transform> smallViewPoints = default;
	[SerializeField] private List<Transform> largeViewPoints = default;
	[SerializeField] private ShopCart shopCart = default;
	[SerializeField] private ShopCartUIManager uiManager = default;
	private Dictionary<int, int> prices = new();
	private List<GameObject> displayedItems = default;
	private IReadonlyPositionAdapter positionAdapter = default;
	[SerializeField]
	private int id = 20004;
	private void Update()
	{

		if (Input.GetKeyDown(KeyCode.N))
		{
			shopCart.AddCart(id);
			id++;
		}
	}
	public int GetPrice(int id)
	{

		if (prices.Keys.Contains(id))
		{
			return prices[id];
		}

		XKumaDebugSystem.LogWarning($"{id}:����id�͌�����܂���ł���");
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
		//NotificationUIManager.Instance.DisplayInteraction();
		PlayerInitialize.ConsignmentInject_static(this);
		InstanceShop();
	}

	private void OnDisable()
	{
		//NotificationUIManager.Instance.HideInteraction();
		DestroyShop();
	}

	/// <summary>
	/// �J�[�g�ɓ����Ă�����̂𔃂�
	/// </summary>
	public void Buy()
	{
		//���������炷
		//�X�̎��v�Ƀv���X�H
		//���L�����ړ�
		foreach (KeyValuePair<int, int> pair in shopCart.InCarts)
		{
			XDebug.Log($"id:{pair.Key} count:{pair.Value}");
		}
	}

	private async void InstanceShop()
	{
		//����
		displayedItems = new List<GameObject>();
		WebAPIRequester webAPIRequester = new WebAPIRequester();

		var data = await webAPIRequester.PostShopEntry(0);
		int smallItemCounter = 0;
		int largeItemCounter = 0;
		for (int i = 0; i < data.GetBody.ItemList.Count; i++)
		{
			var asset = allItemAsset.GetItemAssetByID(data.GetBody.ItemList[i].ItemID);

			int discountedPrice = Mathf.FloorToInt(data.GetBody.ItemList[i].Price
				- (data.GetBody.ItemList[i].Price * data.GetBody.ItemList[i].Discount));

			int stock = data.GetBody.ItemList[i].Stock;
			Vector3 position = default;
			if (data.GetBody.ItemList[i].Size == 0)
			{
				position = smallViewPoints[smallItemCounter].position;
				smallItemCounter++;
			}
			//�ق��̃T�C�Y���ǉ������\�������邽��else if�ɂ��Ă�
			else if (data.GetBody.ItemList[i].Size == 1)
			{
				position = largeViewPoints[largeItemCounter].position;
				largeItemCounter++;
			}
			var item = IDisplayItem.Instantiate(asset, position, Quaternion.identity, this);
			displayedItems.Add(item.gameObject);
			uiManager.AddProductUI(
				data.GetBody.ItemList[i].ItemID,
				data.GetBody.ItemList[i].Price,
				discountedPrice,
				stock,
				data.GetBody.ItemList[i].Discount,
				position);
			prices.Add(data.GetBody.ItemList[i].ItemID, discountedPrice);
		}
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

		//�����̕\��
		buyArea.Display(positionAdapter.Position);
	}

	public void Unselect(SelectArgs selectArgs)
	{
		//���񂾂��̂𗣂����Ƃ��̏���
		var itemSelectArgs = selectArgs as ItemSelectArgs;
		var unselectedPosition = itemSelectArgs.gameObject.transform.position;

		// �͂񂾃A�C�e���𗣂����|�C���g���A�w���G���A��������w��
		if (buyArea.IsExist(unselectedPosition))
		{
			// Buy
			Debug.Log("BuyArea");
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