using System;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;

namespace UnityEditor.HCSMeta
{
	/// <summary>
	/// RegisterSceneInInspectorクラスのSerializedPropertyを拡張するクラス
	/// </summary>
	[CustomPropertyDrawer(typeof(RegisterSceneInInspector))]
	public class RegisterSceneDrawer : PropertyDrawer
	{
		private string[] sceneNames = default;
		private bool isExecuted = false;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label = EditorGUI.BeginProperty(position, label, property);

			// 初回のみ実行
			if (!isExecuted)
			{
				isExecuted = true;

				// 表示する文字列リストを初期化する
				UpdateSceneNames();
				// BuildSettingsのシーンリスト更新時に再び実行されるよう、イベントに登録
				EditorBuildSettings.sceneListChanged += UpdateSceneNames;
			}
			EditorGUI.BeginChangeCheck();

			// SerializedPorpertyを取得
			var nameProperty = property.FindPropertyRelative("name");
			var seletedIndexProperty = property.FindPropertyRelative("selectedIndex");

			// Inspectorの表示を拡張
			int newValue = EditorGUI.Popup(position, label.text, seletedIndexProperty.intValue, sceneNames);
			if (EditorGUI.EndChangeCheck())
			{
				seletedIndexProperty.intValue = newValue;
				nameProperty.stringValue = sceneNames[seletedIndexProperty.intValue];
			}
			EditorGUI.EndProperty();
		}

		/// <summary>
		/// シーン一覧を更新する
		/// </summary>
		public void UpdateSceneNames()
		{
			sceneNames = new string[EditorBuildSettings.scenes.Length];

			for (int i = 0; i < sceneNames.Length; i++)
			{
				// BuildSettingsからシーン一覧を取得し、パスから文字列を取得
				var scene = EditorBuildSettings.scenes[i];
				sceneNames[i] = Path.GetFileNameWithoutExtension(scene.path);
			}
		}
	}
}
#endif

/// <summary>
/// Inspectorでシーン情報を設定するクラス
/// <br>- 必ず変数をシリアライズ化すること</br>
/// </summary>
[Serializable]
public class RegisterSceneInInspector
{
	// Editorからのみアクセスさせる
	[SerializeField] private string name;
	[SerializeField] private int selectedIndex;

	/// <summary>
	/// シーン名
	/// <br>- BuildSettingsに登録されているシーン名をプルダウンで登録</br>
	/// </summary>
	public string Name => name;

	public static implicit operator string(RegisterSceneInInspector rsi)
	{
		return rsi.Name;
	}
}