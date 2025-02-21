using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class IconAttacher : EditorWindow
{
	private ItemBundleAsset allItemAsset = default;
	private SerializedObject target = default;
	private Vector2 scrollPosition = default;
	[SerializeField] private string iconPath = default;

	[MenuItem("Window/Icon Attacher")]
	public static void OpenWindow()
	{
		var window = GetWindow<IconAttacher>();
		window.titleContent = new GUIContent("Icon Attacher");
		window.Show();
	}

	private void OnEnable()
	{
		target = new SerializedObject(this);

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

		iconPath = EditorGUILayout.TextField("Icon Path", iconPath);
		EditorGUILayout.Space(16);

		if (GUILayout.Button("Execute Icon Attach"))
		{
			var filePaths = Directory.GetFiles(iconPath, "*", SearchOption.AllDirectories);

			List<Sprite> sprites = new();
			foreach (var filePath in filePaths)
			{
				var loadAsset = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
				if (loadAsset != null)
				{
					sprites.Add(loadAsset);
				}
			}

			foreach (var itemAsset in allItemAsset.Items)
			{
				foreach (var sprite in sprites)
				{
					if (itemAsset.Name == sprite.name)
					{
						itemAsset.ItemIcon = sprite;
						break;
					}
				}

				EditorUtility.SetDirty(itemAsset);
			}
		}

		EditorGUILayout.EndScrollView();
		target.ApplyModifiedProperties();
	}

}
