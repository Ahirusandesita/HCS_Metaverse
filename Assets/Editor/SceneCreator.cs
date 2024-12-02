using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;

namespace BuildSettingScene
{
	public class ScenesCreator
	{
		/// <summary>
		/// namespace。
		/// </summary>
		static readonly string NAMESPACE = "BuildSettingScene";

		/// <summary>
		/// 改行。
		/// </summary>
		static readonly string NEWLINE = "\n";

		/// <summary>
		/// タブ。
		/// </summary>
		static readonly string TAB = "\t";

		/// <summary>
		/// スペース。
		/// </summary>
		static readonly string SPACE = " ";

		/// <summary>
		/// ファイルディレクトリパス。
		/// </summary>
		static readonly string FILE_DIRECTORY_PATH = "Assets/Scripts/Scene";

		/// <summary>
		/// Scenesファイル名。
		/// </summary>
		static readonly string SCENES_FILE_NAME = "Scenes";

		/// <summary>
		/// Scenesファイルパス。
		/// </summary>
		static readonly string SCENS_FILE_PATH = System.IO.Path.Combine(FILE_DIRECTORY_PATH, "Scenes.cs");

		/// <summary>
		/// ScenesHelperファイル名。
		/// </summary>
		static readonly string SCENESHELPER_FILE_NAME = "ScenesHelper";

		/// <summary>
		/// ScenesHelperファイルパス。
		/// </summary>
		static readonly string SCENESHELPER_FILE_PATH = System.IO.Path.Combine(FILE_DIRECTORY_PATH, "ScenesHelper.cs");

		/// <summary>
		/// ScenesHelper文字列変換メソッド名。
		/// </summary>
		static readonly string SCENESHELPER_TOSTRING_METHOD_NAME = "ScenesToString";

		[MenuItem("Menu/CreateScenes")]
		static void EditorCreateScenes()
		{
			CreateScenes();
			CreateScenesHelper();
		}

		/// <summary>
		/// シーン名一覧を取得。
		/// </summary>
		/// <returns>シーン名一覧。</returns>
		static List<string> LoadSceneNames()
		{
			var list = new List<string>();
			foreach (var scenes in EditorBuildSettings.scenes)
			{
				// 有効なもの。
				if (scenes.enabled)
				{
					// スラッシュからドットの間を取得。
					var slash = scenes.path.LastIndexOf("/");
					var dot = scenes.path.LastIndexOf(".");
					list.Add(scenes.path.Substring(slash + 1, dot - slash - 1));
				}
			}
			return list;
		}

		/// <summary>
		/// Scenes作成。
		/// </summary>
		static void CreateScenes()
		{
			var sceneNameList = LoadSceneNames();
			if (sceneNameList.Count <= 0)
			{
				return;
			}

			var codeSb = new StringBuilder();
			codeSb.Append("namespace" + SPACE + NAMESPACE + NEWLINE + "{" + NEWLINE);
			codeSb.Append(TAB + "///" + SPACE + "<summary>" + NEWLINE);
			codeSb.Append(TAB + "///" + SPACE + SCENES_FILE_NAME + "（自動生成クラス）。" + SPACE + NEWLINE);
			codeSb.Append(TAB + "///" + SPACE + "</summary>" + SPACE + NEWLINE);

			codeSb.Append(TAB + "public enum" + SPACE + SCENES_FILE_NAME + NEWLINE + TAB + "{" + NEWLINE);

			if (sceneNameList.Count > 0)
			{
				for (var idx = 0; idx < sceneNameList.Count; ++idx)
				{
					codeSb.Append(TAB + TAB);
					codeSb.Append(sceneNameList[idx] + "," + NEWLINE);
				}
			}

			codeSb.Append(TAB + "}");
			codeSb.Append(NEWLINE + "}");

			// ディレクトリがないときはディレクトリを作成。
			if (!System.IO.Directory.Exists(FILE_DIRECTORY_PATH))
			{
				System.IO.Directory.CreateDirectory(FILE_DIRECTORY_PATH);
			}

			System.IO.File.WriteAllText(SCENS_FILE_PATH, codeSb.ToString(), System.Text.Encoding.UTF8);
			AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
		}

		/// <summary>
		/// ScenesHelper作成。
		/// </summary>
		static void CreateScenesHelper()
		{
			var sceneNameList = LoadSceneNames();
			if (sceneNameList.Count <= 0)
			{
				return;
			}

			var codeSb = new StringBuilder();
			codeSb.Append("namespace" + SPACE + NAMESPACE + NEWLINE + "{" + NEWLINE);

			codeSb.Append(TAB + "///" + SPACE + "<summary>" + NEWLINE);
			codeSb.Append(TAB + "///" + SPACE + SCENES_FILE_NAME + "拡張クラス（自動生成クラス）。" + NEWLINE);
			codeSb.Append(TAB + "///" + SPACE + "</summary>" + SPACE + NEWLINE);

			codeSb.Append(TAB + "public static class" + SPACE + SCENESHELPER_FILE_NAME + NEWLINE + TAB + "{" + NEWLINE);

			codeSb.Append(TAB + TAB + "///" + SPACE + "<summary>" + SPACE + NEWLINE);
			codeSb.Append(TAB + TAB + "///" + SPACE + SCENES_FILE_NAME + "を文字列に変換するクラス。" + NEWLINE);
			codeSb.Append(TAB + TAB + "///" + SPACE + "</summary>" + SPACE + NEWLINE);

			codeSb.Append(TAB + TAB + "public static string" + SPACE + SCENESHELPER_TOSTRING_METHOD_NAME + "(this" + SPACE + SCENES_FILE_NAME + SPACE + "scenes)" + NEWLINE);
			codeSb.Append(TAB + TAB + "{" + NEWLINE);
			codeSb.Append(TAB + TAB + TAB + "switch(scenes)" + NEWLINE);
			codeSb.Append(TAB + TAB + TAB + "{" + NEWLINE);

			if (sceneNameList.Count > 0)
			{
				for (var idx = 0; idx < sceneNameList.Count; ++idx)
				{
					codeSb.Append(TAB + TAB + TAB + TAB + "case" + SPACE + SCENES_FILE_NAME + "." + sceneNameList[idx] + ":" + NEWLINE);
					codeSb.Append(TAB + TAB + TAB + TAB + TAB + "return" + SPACE + "\"" + sceneNameList[idx] + "\"" + ";" + NEWLINE);
				}
				codeSb.Append(TAB + TAB + TAB + TAB + "default:" + NEWLINE);
				codeSb.Append(TAB + TAB + TAB + TAB + TAB + "return" + SPACE + "\"\"" + ";" + NEWLINE);
			}

			codeSb.Append(TAB + TAB + TAB + "}" + NEWLINE);
			codeSb.Append(TAB + TAB + "}" + NEWLINE);
			codeSb.Append(TAB + "}" + NEWLINE);
			codeSb.Append("}");

			// ディレクトリがないときはディレクトリを作成。
			if (!System.IO.Directory.Exists(FILE_DIRECTORY_PATH))
			{
				System.IO.Directory.CreateDirectory(FILE_DIRECTORY_PATH);
			}

			System.IO.File.WriteAllText(SCENESHELPER_FILE_PATH, codeSb.ToString(), System.Text.Encoding.UTF8);
			AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
		}
	}
}