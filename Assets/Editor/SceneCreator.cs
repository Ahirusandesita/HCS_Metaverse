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
		/// namespace�B
		/// </summary>
		static readonly string NAMESPACE = "BuildSettingScene";

		/// <summary>
		/// ���s�B
		/// </summary>
		static readonly string NEWLINE = "\n";

		/// <summary>
		/// �^�u�B
		/// </summary>
		static readonly string TAB = "\t";

		/// <summary>
		/// �X�y�[�X�B
		/// </summary>
		static readonly string SPACE = " ";

		/// <summary>
		/// �t�@�C���f�B���N�g���p�X�B
		/// </summary>
		static readonly string FILE_DIRECTORY_PATH = "Assets/Scripts/Scene";

		/// <summary>
		/// Scenes�t�@�C�����B
		/// </summary>
		static readonly string SCENES_FILE_NAME = "Scenes";

		/// <summary>
		/// Scenes�t�@�C���p�X�B
		/// </summary>
		static readonly string SCENS_FILE_PATH = System.IO.Path.Combine(FILE_DIRECTORY_PATH, "Scenes.cs");

		/// <summary>
		/// ScenesHelper�t�@�C�����B
		/// </summary>
		static readonly string SCENESHELPER_FILE_NAME = "ScenesHelper";

		/// <summary>
		/// ScenesHelper�t�@�C���p�X�B
		/// </summary>
		static readonly string SCENESHELPER_FILE_PATH = System.IO.Path.Combine(FILE_DIRECTORY_PATH, "ScenesHelper.cs");

		/// <summary>
		/// ScenesHelper������ϊ����\�b�h���B
		/// </summary>
		static readonly string SCENESHELPER_TOSTRING_METHOD_NAME = "ScenesToString";

		[MenuItem("Menu/CreateScenes")]
		static void EditorCreateScenes()
		{
			CreateScenes();
			CreateScenesHelper();
		}

		/// <summary>
		/// �V�[�����ꗗ���擾�B
		/// </summary>
		/// <returns>�V�[�����ꗗ�B</returns>
		static List<string> LoadSceneNames()
		{
			var list = new List<string>();
			foreach (var scenes in EditorBuildSettings.scenes)
			{
				// �L���Ȃ��́B
				if (scenes.enabled)
				{
					// �X���b�V������h�b�g�̊Ԃ��擾�B
					var slash = scenes.path.LastIndexOf("/");
					var dot = scenes.path.LastIndexOf(".");
					list.Add(scenes.path.Substring(slash + 1, dot - slash - 1));
				}
			}
			return list;
		}

		/// <summary>
		/// Scenes�쐬�B
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
			codeSb.Append(TAB + "///" + SPACE + SCENES_FILE_NAME + "�i���������N���X�j�B" + SPACE + NEWLINE);
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

			// �f�B���N�g�����Ȃ��Ƃ��̓f�B���N�g�����쐬�B
			if (!System.IO.Directory.Exists(FILE_DIRECTORY_PATH))
			{
				System.IO.Directory.CreateDirectory(FILE_DIRECTORY_PATH);
			}

			System.IO.File.WriteAllText(SCENS_FILE_PATH, codeSb.ToString(), System.Text.Encoding.UTF8);
			AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
		}

		/// <summary>
		/// ScenesHelper�쐬�B
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
			codeSb.Append(TAB + "///" + SPACE + SCENES_FILE_NAME + "�g���N���X�i���������N���X�j�B" + NEWLINE);
			codeSb.Append(TAB + "///" + SPACE + "</summary>" + SPACE + NEWLINE);

			codeSb.Append(TAB + "public static class" + SPACE + SCENESHELPER_FILE_NAME + NEWLINE + TAB + "{" + NEWLINE);

			codeSb.Append(TAB + TAB + "///" + SPACE + "<summary>" + SPACE + NEWLINE);
			codeSb.Append(TAB + TAB + "///" + SPACE + SCENES_FILE_NAME + "�𕶎���ɕϊ�����N���X�B" + NEWLINE);
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

			// �f�B���N�g�����Ȃ��Ƃ��̓f�B���N�g�����쐬�B
			if (!System.IO.Directory.Exists(FILE_DIRECTORY_PATH))
			{
				System.IO.Directory.CreateDirectory(FILE_DIRECTORY_PATH);
			}

			System.IO.File.WriteAllText(SCENESHELPER_FILE_PATH, codeSb.ToString(), System.Text.Encoding.UTF8);
			AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
		}
	}
}