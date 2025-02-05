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
	[Header("�ʏ�̃v���n�u�i�\���݂̂̂��́j")]
	[SerializeField] private GameObject prefab = default;
	[Header("Grabbable�ȃv���n�u")]
	[SerializeField, InterfaceType(typeof(IDisplayItem))]
	private Object displayItem = default;
	[Header("Network���L����View�v���n�u")]
	[SerializeField]
	private NetworkView networkView = default;
	[Header("AnimationClip")]
	[SerializeField]
	private AnimationClip animation = default;
	[Space(10)]
	[Header("�f�[�^�x�[�X���珜�O����")]
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
			EditorGUILayout.HelpBox($"�A�C�e����͂�ŔC�ӂ̑�����s�������ꍇ�A{nameof(itemAsset.DisplayItem)}���g�p���邱�ƂŁA�\����p�̃I�u�W�F�N�g�Ǝ��ۂ̃I�u�W�F�N�g�𕪂��邱�Ƃ��ł��܂��B�i�������j\n\n" +
				$"�Ⴆ�΁A�u�N���b�J�[�v�Ƃ����A�C�e�����V���b�v�Ŏ�舵���ꍇ�A���ۂ̃v���n�u��\������ƃN���b�J�[�ɑ΂��ăC���^���N�g�ł��Ă��܂��܂��B�����ڂ݂̂̃v���n�u��ʓr�쐬���邱�Ƃł��̂悤�Ȗ����������邱�Ƃ��\�ł��B\n\n" +
				$"Grabbable�ȃA�C�e���ł���Ȃ���A�v���n�u�𕪂���K�v�̂Ȃ��ꍇ�́A{nameof(itemAsset.DisplayItem)}�̕����g�����Ƃ������߂��܂��B", MessageType.Info);
		}
	}
}
#endif