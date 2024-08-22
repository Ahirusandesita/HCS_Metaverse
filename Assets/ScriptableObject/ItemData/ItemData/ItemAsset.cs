using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemAsset/Item")]
public class ItemAsset : ScriptableObject
{
    [SerializeField] private int itemID = default;
    [SerializeField] private string itemName = default;
    [SerializeField] private string itemText = default;
    [SerializeField] private ItemGenre itemGenre = default;
    [SerializeField] private GameObject prefab = default;
    [Space(10)]
    [SerializeField] private bool isDisplayable = true;
    [SerializeField, InterfaceTypeMulti(typeof(IDisplayItem)), HideForBool(nameof(isDisplayable), false)]
    private Object displayItem = default;

    public int ID => itemID;
    public string Name => itemName;
    public string Text => itemText;
    public ItemGenre Genre => itemGenre;
    public GameObject Prefab => prefab;
    public bool IsDisplayable => isDisplayable;
    public IDisplayItem DisplayItem => displayItem as IDisplayItem;
}

public enum ItemGenre
{
    All,
    Usable,
    Food,
}

#if UNITY_EDITOR
namespace UnityEditor.HCSMeta
{
    [CustomEditor(typeof(ItemAsset))]
    public class ItemAssetEditor : Editor
    {
        private ItemAsset itemAsset = default;

        private void OnEnable()
        {
            itemAsset = target as ItemAsset;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(32f);
            EditorGUILayout.HelpBox($"�A�C�e����͂�ŔC�ӂ̑�����s�������ꍇ�A{nameof(itemAsset.DisplayItem)}���g�p���邱�ƂŁA�\����p�̃I�u�W�F�N�g�Ǝ��ۂ̃I�u�W�F�N�g�𕪂��邱�Ƃ��ł��܂��B�i�������j\n\n" +
                $"�Ⴆ�΁A�u�N���b�J�[�v�Ƃ����A�C�e�����V���b�v�Ŏ�舵���ꍇ�A���ۂ̃v���n�u��\������ƃN���b�J�[�ɑ΂��ăC���^���N�g�ł��Ă��܂��܂��B�����ڂ݂̂̃v���n�u��ʓr�쐬���邱�Ƃł��̂悤�Ȗ����������邱�Ƃ��\�ł��B\n\n" +
                $"Grabbable�ȃA�C�e���ł���Ȃ���A�v���n�u�𕪂���K�v�̂Ȃ��ꍇ�́A{nameof(itemAsset.DisplayItem)}�̕����g�����Ƃ������߂��܂��B", MessageType.Info);
        }
    }
}
#endif