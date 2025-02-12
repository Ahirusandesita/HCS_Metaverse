using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InteriorColliderCreate : EditorWindow
{
	private ItemBundleAsset allItemAsset = default;
	private SerializedObject target = default;
	private Vector2 scrollPosition = default;
	[SerializeField] private int[] ignoreIDs = default;
	private SerializedProperty ignoreIDsProperty = default;

	[MenuItem("Window/Interior Collider Create")]
	public static void OpenWindow()
	{
		var window = GetWindow<InteriorColliderCreate>();
		window.titleContent = new GUIContent("Interior Collider Create");
		window.Show();
	}

	private void OnEnable()
	{
		target = new SerializedObject(this);
		ignoreIDsProperty = target.FindProperty(nameof(ignoreIDs));

		// Conditionalはメソッド内はコンパイルされてしまうので、仕方なく二重
		allItemAsset = AssetDatabase.FindAssets($"t:{nameof(ItemBundleAsset)}")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<ItemBundleAsset>)
				.Where(itemBundleAsset => itemBundleAsset.name == "AllItemData")
				.First();
	}

	private void OnGUI()
	{
		target.Update();
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		EditorGUILayout.Space(16);

		EditorGUILayout.PropertyField(ignoreIDsProperty, new GUIContent("Ignore IDs"));
		EditorGUILayout.Space(16);

		if (GUILayout.Button("Execute Create Collider"))
		{
			foreach (var item in allItemAsset.Items)
			{
				bool isIgnore = false;

				foreach (var ignoreId in ignoreIDs)
				{
					if (item.ID == ignoreId)
					{
						isIgnore = true;
						break;
					}
				}

				if (isIgnore)
				{
					continue;
				}

				if (item.Genre == ItemGenre.Interior)
				{
					var collider = item.DisplayItem.gameObject.AddComponent<BoxCollider>();
					collider.size *= 1.5f;
					collider.isTrigger = true;
				}
			}
		}

		EditorGUILayout.EndScrollView();
		target.ApplyModifiedProperties();
	}
}
