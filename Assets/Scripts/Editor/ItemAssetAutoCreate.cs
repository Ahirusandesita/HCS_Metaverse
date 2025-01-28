using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ItemAssetAutoCreate : EditorWindow
{
	[SerializeField] private GameObject prefab = default;
	[SerializeField] private GameObject[] prefabs = default;
	[SerializeField] private bool autoSearch = default;
	[SerializeField] private SearchOption searchOption = SearchOption.AllDirectories;
	[SerializeField] private string prefabFolderPath = default;
	[SerializeField] private string createdAssetfolderPath = default;
	[SerializeField] private ItemGenre itemGenre = ItemGenre.Interior;

	private SerializedObject target = default;
	private Vector2 scrollPosition = default;

	[MenuItem("Window/Item Asset Auto Create")]
	public static void OpenWindow()
	{
		var window = GetWindow<ItemAssetAutoCreate>();
		window.titleContent = new GUIContent("Item Asset Auto Create");
		window.Show();
	}

	private void OnEnable()
	{
		target = new SerializedObject(this);
	}

	private void OnGUI()
	{
		target.Update();
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		EditorGUILayout.Space(16);

		autoSearch = EditorGUILayout.Toggle("Auto Search", autoSearch);
		EditorGUI.BeginDisabledGroup(autoSearch);
		prefab = EditorGUILayout.ObjectField("Target Prefab", prefab, typeof(GameObject), false) as GameObject;
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space(16);

		EditorGUI.BeginDisabledGroup(!autoSearch);
		prefabFolderPath = EditorGUILayout.TextField("Prefab Folder Path (Origin)", prefabFolderPath);
		searchOption = (SearchOption)EditorGUILayout.EnumPopup("Search Option", searchOption);
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space(16);

		createdAssetfolderPath = EditorGUILayout.TextField("Created Asset Folder Path (Save)", createdAssetfolderPath);

		EditorGUILayout.Space(16);

		itemGenre = (ItemGenre)EditorGUILayout.EnumPopup("Item Genre", itemGenre);

		EditorGUILayout.Space(16);

		if (GUILayout.Button("Auto Create"))
		{
			if (string.IsNullOrEmpty(createdAssetfolderPath))
			{
				Debug.LogError("Pathが入力されていません。");
				EditorGUILayout.EndScrollView();
				target.ApplyModifiedProperties();
				return;
			}

			if (autoSearch)
			{
				if (string.IsNullOrEmpty(prefabFolderPath))
				{
					Debug.LogError("Pathが入力されていません。");
					EditorGUILayout.EndScrollView();
					target.ApplyModifiedProperties();
					return;
				}

				var filePaths = Directory.GetFiles(prefabFolderPath, "*", searchOption);

				List<GameObject> gameObjects = new();
				foreach (var filePath in filePaths)
				{
					// FilePathからGameObjectを取得する。
					var loadAsset = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
					if (loadAsset != null)
					{
						gameObjects.Add(loadAsset);
					}
				}

				prefabs = gameObjects.ToArray();
			}
			else if (prefab is null)
			{
				Debug.LogError("Target Objectにプレハブがアタッチされていません。");
				EditorGUILayout.EndScrollView();
				target.ApplyModifiedProperties();
				return;
			}
			else
			{
				prefabs = new GameObject[] { prefab };
			}

			foreach (var prefab in prefabs)
			{
				var existingAsset = AssetDatabase.FindAssets($"t:{nameof(ItemAsset)}")
					.Select(AssetDatabase.GUIDToAssetPath)
					.Select(AssetDatabase.LoadAssetAtPath<ItemAsset>)
					.Where(asset => asset.name == prefab.name)
					.FirstOrDefault();

				ItemAsset itemAsset;
				if (existingAsset != null)
				{
					itemAsset = existingAsset;
				}
				else
				{
					itemAsset = CreateInstance<ItemAsset>();
				}

				var itemNameInfo = itemAsset.GetType()
					.GetField("itemName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (itemNameInfo != null)
				{
					itemNameInfo.SetValue(itemAsset, prefab.name);
				}

				var genreInfo = itemAsset.GetType()
					.GetField("itemGenre", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (genreInfo != null)
				{
					genreInfo.SetValue(itemAsset, itemGenre);
				}

				var prefabInfo = itemAsset.GetType()
					.GetField("prefab", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (prefabInfo != null)
				{
					prefabInfo.SetValue(itemAsset, prefab);
				}

				var sizeInfo = itemAsset.GetType()
					.GetField("size", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (sizeInfo != null)
				{
					sizeInfo.SetValue(itemAsset, 1);
				}

				if (existingAsset == null)
				{
					string fileName = $"{prefab.name}.asset";
					AssetDatabase.CreateAsset(itemAsset, Path.Combine(createdAssetfolderPath, fileName));
				}

				EditorUtility.SetDirty(itemAsset);
			}

			XDebug.Log($"Create or Update Completed : {prefabs.Length}assets!");
		}

		EditorGUILayout.EndScrollView();
		target.ApplyModifiedProperties();
	}
}
