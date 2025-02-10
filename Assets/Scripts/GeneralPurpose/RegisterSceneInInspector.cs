using System;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;

namespace UnityEditor.HCSMeta
{
	/// <summary>
	/// RegisterSceneInInspector�N���X��SerializedProperty���g������N���X
	/// </summary>
	[CustomPropertyDrawer(typeof(RegisterSceneInInspector))]
	public class RegisterSceneDrawer : PropertyDrawer
	{
		private string[] sceneNames = default;
		private bool isExecuted = false;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label = EditorGUI.BeginProperty(position, label, property);

			// ����̂ݎ��s
			if (!isExecuted)
			{
				isExecuted = true;

				// �\�����镶���񃊃X�g������������
				UpdateSceneNames();
				// BuildSettings�̃V�[�����X�g�X�V���ɍĂю��s�����悤�A�C�x���g�ɓo�^
				EditorBuildSettings.sceneListChanged += UpdateSceneNames;
			}
			EditorGUI.BeginChangeCheck();

			// SerializedPorperty���擾
			var nameProperty = property.FindPropertyRelative("name");
			var seletedIndexProperty = property.FindPropertyRelative("selectedIndex");

			// Inspector�̕\�����g��
			int newValue = EditorGUI.Popup(position, label.text, seletedIndexProperty.intValue, sceneNames);
			if (EditorGUI.EndChangeCheck())
			{
				seletedIndexProperty.intValue = newValue;
				nameProperty.stringValue = sceneNames[seletedIndexProperty.intValue];
			}
			EditorGUI.EndProperty();
		}

		/// <summary>
		/// �V�[���ꗗ���X�V����
		/// </summary>
		public void UpdateSceneNames()
		{
			sceneNames = new string[EditorBuildSettings.scenes.Length];

			for (int i = 0; i < sceneNames.Length; i++)
			{
				// BuildSettings����V�[���ꗗ���擾���A�p�X���當������擾
				var scene = EditorBuildSettings.scenes[i];
				sceneNames[i] = Path.GetFileNameWithoutExtension(scene.path);
			}
		}
	}
}
#endif

/// <summary>
/// Inspector�ŃV�[������ݒ肷��N���X
/// <br>- �K���ϐ����V���A���C�Y�����邱��</br>
/// </summary>
[Serializable]
public class RegisterSceneInInspector
{
	// Editor����̂݃A�N�Z�X������
	[SerializeField] private string name;
	[SerializeField] private int selectedIndex;

	/// <summary>
	/// �V�[����
	/// <br>- BuildSettings�ɓo�^����Ă���V�[�������v���_�E���œo�^</br>
	/// </summary>
	public string Name => name;

	public static implicit operator string(RegisterSceneInInspector rsi)
	{
		return rsi.Name;
	}
}