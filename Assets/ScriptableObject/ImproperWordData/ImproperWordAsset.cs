using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "ImproperWordData", menuName = "ScriptableObjects/ImproperWordAsset")]
public class ImproperWordAsset : ScriptableObject
{
    [Header("�ȉ��̃��X�g�ɐݒ肳�ꂽ���[�h�́A���[���h�`���b�g�@�\�ł̕�����u���ΏۂƂȂ�܂��B\n" +
        " �u���㕶���F'*'\n" +
        " �啶���Ə������F��ʂ��Ȃ�")]
    [Space(16)]
    [SerializeField] private string[] improperWords = default;
    
    public IReadOnlyList<string> ImproperWords => improperWords;
    public const char MASKED_CHAR = '*';

#if UNITY_EDITOR
    /// <summary>
    /// ���̃v���p�e�B�͓����Editor�N���X����̂݃A�N�Z�X�ł��܂�
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public string[] EditorImpropertyWords
    {
        get
        {
            var source = new StackFrame(1).GetMethod();
            if (source.ReflectedType != typeof(UnityEditor.ImproperWordAssetEditor))
            {
                throw new NotSupportedException($"{nameof(EditorImpropertyWords)}�͓����Editor�N���X����݂̂̃A�N�Z�X��z�肵�Ă���ARunTime�ł̗��p�͂ł��܂���B");
            }

            return improperWords;
        }
        set
        {
            var source = new StackFrame(1).GetMethod();
            if (source.ReflectedType != typeof(UnityEditor.ImproperWordAssetEditor))
            {
                throw new NotSupportedException($"{nameof(EditorImpropertyWords)}�͓����Editor�N���X����݂̂̃A�N�Z�X��z�肵�Ă���ARunTime�ł̗��p�͂ł��܂���B");
            }

            improperWords = value;
        }
    }
#endif
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomEditor(typeof(ImproperWordAsset))]
    public class ImproperWordAssetEditor : Editor
    {
        private ImproperWordAsset improperWordAsset = default;


        private void OnEnable()
        {
            improperWordAsset = target as ImproperWordAsset;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("�A���t�@�x�b�g���Ƀ\�[�g�i�����j"))
            {
                Array.Sort(improperWordAsset.EditorImpropertyWords, StringComparer.OrdinalIgnoreCase);
            }
            if (GUILayout.Button("�A���t�@�x�b�g���Ƀ\�[�g�i�~���j"))
            {
                Array.Reverse(improperWordAsset.EditorImpropertyWords);
            }

            GUILayout.Space(16);
            EditorGUILayout.HelpBox("���s����уr���h�O�ɁA�K�����������Ƀ\�[�g���Ă��������B", MessageType.Info);

            if (GUILayout.Button("���������Ƀ\�[�g"))
            {
                improperWordAsset.EditorImpropertyWords.SortLength();
            }
        }
    }
}
#endif