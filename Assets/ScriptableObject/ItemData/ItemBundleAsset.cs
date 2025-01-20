using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Reflection;

/// <summary>
/// このInterfaceはエディタクラスからのみアクセスすること
/// </summary>
public interface IEditorItemBundleAsset
{
	List<ItemAsset> EditorItems { set; }
}

[CreateAssetMenu(fileName = "ItemBundleData", menuName = "ScriptableObjects/ItemAsset/Bundle")]
public class ItemBundleAsset : ScriptableObject, IEditorItemBundleAsset
{
	[Header("全アイテムのリスト\n※ジャンルごとに整列")]
	[SerializeField] private List<ItemAsset> items = default;
	public IReadOnlyList<ItemAsset> Items => items;
	List<ItemAsset> IEditorItemBundleAsset.EditorItems { set => items = value; }

	public ItemAsset GetItemAssetByID(int id)
	{
		return items.Where(item => item.ID == id).FirstOrDefault();
	}
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
	[CustomEditor(typeof(ItemBundleAsset))]
	public class ItemBundleAssetEditor : Editor
	{
		private const string PATH = "Assets/ScriptableObject/ItemData/LastIDData.txt";

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Space(12f);

			if (GUILayout.Button("Auto Set"))
			{
				try
				{
					var itemBundleAsset = target as IEditorItemBundleAsset;
					List<ItemAsset> itemAsset = default;

					itemAsset = AssetDatabase.FindAssets($"t:{nameof(ItemAsset)}")
						.Select(AssetDatabase.GUIDToAssetPath)
						.Select(AssetDatabase.LoadAssetAtPath<ItemAsset>)
						.OrderBy(asset => asset.Genre)
						.ToList();

					itemBundleAsset.EditorItems = itemAsset;
				}
				// 要素ない状態でボタン押すと例外出る→うざいので握りつぶす
				catch (System.NullReferenceException) { }
			}

			EditorGUILayout.Space(12f);

			if (GUILayout.Button("Allocate ID"))
			{
				var itemBundleAsset = target as ItemBundleAsset;
				StringBuilder sb1 = new StringBuilder(EditorSaveSystem.Load(PATH));
				StringBuilder sb2 = new StringBuilder(EditorSaveSystem.Load(PATH));
				StringBuilder sb3 = new StringBuilder(EditorSaveSystem.Load(PATH));
				StringBuilder sb4 = new StringBuilder(EditorSaveSystem.Load(PATH));
				int id1 = int.Parse(sb1.Remove(5, 18).ToString());
				int id2 = int.Parse(sb2.Remove(0, 6).Remove(5, 12).ToString());
				int id3 = int.Parse(sb3.Remove(0, 12).Remove(5, 6).ToString());
				int id4 = int.Parse(sb4.Remove(0, 18).ToString());

				foreach (var itemAsset in itemBundleAsset.Items)
				{
					var itemIDInfo = itemAsset.GetType()
						.GetField("itemID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					if (itemIDInfo != null && itemAsset.ID == 0)
					{
						switch (itemAsset.Genre)
						{
							case ItemGenre.Interior:
								itemIDInfo.SetValue(itemAsset, id1);
								id1++;
								break;

							case ItemGenre.Costume:
								itemIDInfo.SetValue(itemAsset, id2);
								id2++;
								break;

							case ItemGenre.Usable:
								itemIDInfo.SetValue(itemAsset, id3);
								id3++;
								break;

							case ItemGenre.Food:
								itemIDInfo.SetValue(itemAsset, id4);
								id4++;
								break;

							default:
								throw new System.InvalidOperationException();
						}

					}
				}

				AssetDatabase.SaveAssets();
				EditorSaveSystem.Save(PATH, $"{id1}/{id2}/{id3}/{id4}");
			}

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(target);
			}
		}
	}
}
#endif