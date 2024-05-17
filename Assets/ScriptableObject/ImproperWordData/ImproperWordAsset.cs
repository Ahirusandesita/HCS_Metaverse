using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ImproperWordData", menuName = "ScriptableObjects/ImproperWordAsset")]
public class ImproperWordAsset : ScriptableObject
{
    [Header("以下のリストに設定されたワードは、<br>ワールドチャット機能での文字列置換対象となります。<br><br>" +
        " 置換後文字：'*'<br>" +
        " 大文字と小文字：区別しない")]
    [Space, Space, Space]
    [SerializeField] private string[] improperWords = default;

    public IReadOnlyList<string> ImproperWords => improperWords;
    public const char MASKED_CHAR = '*';
}
