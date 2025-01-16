using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Interface�̓G�f�B�^�N���X����̂݃A�N�Z�X���邱��
/// </summary>
public interface IEditorItemBundleAsset
{
    List<ItemAsset> EditorItems { set; }
}

[CreateAssetMenu(fileName = "ItemBundleData", menuName = "ScriptableObjects/ItemAsset/Bundle")]
public class ItemBundleAsset : ScriptableObject, IEditorItemBundleAsset
{
    [Header("�S�A�C�e���̃��X�g\n���W���������Ƃɐ���")]
    [SerializeField] private List<ItemAsset> items = default;
    public IReadOnlyList<ItemAsset> Items => items;
    List<ItemAsset> IEditorItemBundleAsset.EditorItems { set => items = value; }

    public ItemAsset GetItemAssetByID(int id)
    {
        return items.Where(item => item.ID == id).FirstOrDefault();
    }
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
    [CustomEditor(typeof(ItemBundleAsset))]
    public class ItemBundleAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space(12f);

            if (GUILayout.Button("Auto Set"))
            {
                try
                {
                    var itemBundleAsset = target as IEditorItemBundleAsset;
                    List<ItemAsset> itemAsset = default;

                    itemAsset = AssetDatabase.FindAssets($"t:{nameof(ItemAsset)}")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<ItemAsset>)
                        .OrderBy(asset => asset.Genre)
                        .ToList();

                    itemBundleAsset.EditorItems = itemAsset;
                }
                // �v�f�Ȃ���ԂŃ{�^�������Ɨ�O�o�遨�������̂ň���Ԃ�
                catch (System.NullReferenceException) { }
            }

            if (EditorGUI.EndChangeCheck())
			{
                EditorUtility.SetDirty(target);
			}
        }
    }
}
#endif