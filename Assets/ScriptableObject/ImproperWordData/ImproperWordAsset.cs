using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ImproperWordData", menuName = "ScriptableObjects/ImproperWordAsset")]
public class ImproperWordAsset : ScriptableObject
{
    [Header("�ȉ��̃��X�g�ɐݒ肳�ꂽ���[�h�́A<br>���[���h�`���b�g�@�\�ł̕�����u���ΏۂƂȂ�܂��B<br><br>" +
        " �u���㕶���F'*'<br>" +
        " �啶���Ə������F��ʂ��Ȃ�")]
    [Space, Space, Space]
    [SerializeField] private string[] improperWords = default;

    public IReadOnlyList<string> ImproperWords => improperWords;
    public const char MASKED_CHAR = '*';
}
