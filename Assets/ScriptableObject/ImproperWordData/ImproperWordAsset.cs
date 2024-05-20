using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(fileName = "ImproperWordData", menuName = "ScriptableObjects/ImproperWordAsset")]
public class ImproperWordAsset : ScriptableObject
{
    [Header("以下のリストに設定されたワードは、ワールドチャット機能での文字列置換対象となります。\n" +
        " 置換後文字：'*'\n" +
        " 大文字と小文字：区別しない")]
    [Space(16)]
    [SerializeField] private string[] improperWords = default;
    
    public IReadOnlyList<string> ImproperWords => improperWords;
    public const char MASKED_CHAR = '*';

#if UNITY_EDITOR
    /// <summary>
    /// このプロパティは特定のEditorクラスからのみアクセスできます
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public string[] EditorImpropertyWords
    {
        get
        {
            var source = new StackFrame(1).GetMethod();
            if (source.ReflectedType != typeof(UnityEditor.ImproperWordAssetEditor))
            {
                throw new NotSupportedException($"{nameof(EditorImpropertyWords)}は特定のEditorクラスからのみのアクセスを想定しており、RunTimeでの利用はできません。");
            }

            return improperWords;
        }
        set
        {
            var source = new StackFrame(1).GetMethod();
            if (source.ReflectedType != typeof(UnityEditor.ImproperWordAssetEditor))
            {
                throw new NotSupportedException($"{nameof(EditorImpropertyWords)}は特定のEditorクラスからのみのアクセスを想定しており、RunTimeでの利用はできません。");
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

            if (GUILayout.Button("アルファベット順にソート（昇順）"))
            {
                Array.Sort(improperWordAsset.EditorImpropertyWords, StringComparer.OrdinalIgnoreCase);
            }
            if (GUILayout.Button("アルファベット順にソート（降順）"))
            {
                Array.Reverse(improperWordAsset.EditorImpropertyWords);
            }

            GUILayout.Space(16);
            EditorGUILayout.HelpBox("実行およびビルド前に、必ず文字数順にソートしてください。", MessageType.Info);

            if (GUILayout.Button("文字数順にソート"))
            {
                improperWordAsset.EditorImpropertyWords.SortLength();
            }
        }
    }
}
#endif