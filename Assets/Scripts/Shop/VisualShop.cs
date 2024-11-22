using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisualShop : MonoBehaviour, ISelectedNotification, IDependencyInjector<PlayerBodyDependencyInformation>
{
	[ContextMenu("test")]
	private void Test()
	{
		
		shopCart.AddCart(_id);
	}

	private void Update()
	{
		//dev
		if (Input.GetKeyDown(KeyCode.RightShift)) { shopCart.AddCart(_id); }
	}
	[SerializeField] private int _id = 000;
	//カートクラスを作る
	[SerializeField] private ItemBundleAsset allItemAsset = default;
	[SerializeField] private BuyArea buyArea = default;
	[SerializeField] private List<Transform> viewPoints = default;
	[SerializeField] private List<ItemIDView> itemLineup = default;
	[SerializeField] private ShopCart shopCart = default;
	private List<GameObject> displayedItems = default;
	private IReadonlyPositionAdapter positionAdapter = default;

	public IReadOnlyList<ItemIDView> ItemLineup => itemLineup;

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	private void Reset()
	{
#if UNITY_EDITOR
		// Conditionalはメソッド内はコンパイルされてしまうので、仕方なく二重
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
	/// カートに入っているものを買う
	/// </summary>
	public void Buy()
	{
		//お金を減らす
		//店の収益にプラス？
		//所有権を移動
		foreach(KeyValuePair<int,int> pair in shopCart.InCarts)
		{
			XDebug.Log($"id:{pair.Key} count:{pair.Value}");
		}
	}

	private void InstanceShop()
	{
		//生成
		displayedItems = new List<GameObject>();

		for (int i = 0; i < itemLineup.Count; i++)
		{
			var asset = allItemAsset.GetItemAssetByID(itemLineup[i].ID);
			var position = viewPoints[i].position;
			var item = IDisplayItem.Instantiate(asset, position, Quaternion.identity, this);
			displayedItems.Add(item.gameObject);
		}
	}

	private void DestroyShop()
	{
		//削除
		foreach (var obj in displayedItems)
		{
			Destroy(obj);
		}
	}

	public void Select(SelectArgs selectArgs)
	{
		//つかまれた時に新しいものを生成する
		var itemSelectArgs = selectArgs as ItemSelectArgs;
		var asset = allItemAsset.GetItemAssetByID(itemSelectArgs.id);
		var position = itemSelectArgs.position;

		// 選択されたアイテムと同じものを生成する（コピーを表現）
		var item = IDisplayItem.Instantiate(asset, position, Quaternion.identity, this);
		displayedItems.Add(item.gameObject);

		//かごの表示
		buyArea.Display(positionAdapter.Position);
	}

	public void Unselect(SelectArgs selectArgs)
	{
		//つかんだものを離したときの処理
		var itemSelectArgs = selectArgs as ItemSelectArgs;
		var unselectedPosition = itemSelectArgs.gameObject.transform.position;

		// 掴んだアイテムを離したポイントが、購入エリアだったら購入
		if (buyArea.IsExist(unselectedPosition))
		{
			// Buy
			Debug.Log("BuyArea");
			//カートに入れる
			shopCart.AddCart(itemSelectArgs.id);
		}
		//とったやつを消している
		displayedItems.Remove(itemSelectArgs.gameObject);
		//展示されているオブジェクトを破棄
		Destroy(itemSelectArgs.gameObject);
		//かごを隠す
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
				// 要素ない状態でボタン押すと例外出る→うざいので握りつぶす
				catch (System.NullReferenceException) { }
			}
		}
	}
}
#endif