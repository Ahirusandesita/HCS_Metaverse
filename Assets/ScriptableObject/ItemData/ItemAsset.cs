using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemAsset/Item")]
public class ItemAsset : ScriptableObject
{
	private enum ItemSize
	{
		Large = 0,
		Small = 1
	}

	[SerializeField, Hide] private int itemID = default;
	[SerializeField] private string itemName = default;
	[SerializeField] private Sprite itemIcon = default;
	[SerializeField] private string itemText = default;
	[SerializeField] private ItemGenre itemGenre = default;
	[SerializeField] private ItemSize size = default;
	[Space(10)]
	[Header("通常のプレハブ（表示のみのもの）")]
	[SerializeField] private GameObject prefab = default;
	[Header("Grabbableなプレハブ")]
	[SerializeField, InterfaceType(typeof(IDisplayItem))]
	private Object displayItem = default;
	[Header("Network共有するViewプレハブ")]
	[SerializeField]
	private NetworkView networkView = default;
	[Header("AnimationClip")]
	[SerializeField]
	private AnimationClip animation = default;
	[Space(10)]
	[Header("データベースから除外する")]
	[SerializeField]
	private bool excludeDatabese = false;

	public int ID => itemID;
	public string Name => itemName;
	public Sprite ItemIcon
	{
		get
		{
			if (itemIcon == null)
			{
				itemIcon = Resources.Load<Sprite>("NotExistIcon");
			}
			return itemIcon;
		}
	}

	public string Text => itemText;
	public ItemGenre Genre => itemGenre;
	public int Size => (int)size;
	public GameObject Prefab => prefab;
	public IDisplayItem DisplayItem => displayItem as IDisplayItem;
	public NetworkView NetworkView => networkView;
	public AnimationClip Animation => animation;
	public bool ExcludeDatabase => excludeDatabese;

#if UNITY_EDITOR
	public void OnValidate()
	{
		if (itemName == string.Empty)
		{
			itemName = name;
		}
	}
#endif
}

public enum ItemGenre
{
	Interior = 1,
	Costume = 2,
	Usable = 3,
	Food = 4,
	Animation = 5,
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
	[CustomEditor(typeof(ItemAsset))]
	public class ItemAssetEditor : Editor
	{
		private ItemAsset itemAsset = default;

		private void OnEnable()
		{
			itemAsset = target as ItemAsset;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space(32f);
			EditorGUILayout.HelpBox($"アイテムを掴んで任意の操作を行いたい場合、{nameof(itemAsset.DisplayItem)}を使用することで、表示専用のオブジェクトと実際のオブジェクトを分けることができます。（仮実装）\n\n" +
				$"例えば、「クラッカー」というアイテムをショップで取り扱う場合、実際のプレハブを表示するとクラッカーに対してインタラクトできてしまいます。見た目のみのプレハブを別途作成することでそのような問題を解決することが可能です。\n\n" +
				$"Grabbableなアイテムでありながら、プレハブを分ける必要のない場合は、{nameof(itemAsset.DisplayItem)}の方を使うことをお勧めします。", MessageType.Info);
		}
	}
}
#endif