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
            EditorGUILayout.HelpBox($"アイテムを掴んで任意の操作を行いたい場合、{nameof(itemAsset.DisplayItem)}を使用することで、表示専用のオブジェクトと実際のオブジェクトを分けることができます。（仮実装）\n\n" +
                $"例えば、「クラッカー」というアイテムをショップで取り扱う場合、実際のプレハブを表示するとクラッカーに対してインタラクトできてしまいます。見た目のみのプレハブを別途作成することでそのような問題を解決することが可能です。\n\n" +
                $"Grabbableなアイテムでありながら、プレハブを分ける必要のない場合は、{nameof(itemAsset.DisplayItem)}の方を使うことをお勧めします。", MessageType.Info);
        }
    }
}
#endif