using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisualShop : SafetyInteractionObject, IDependencyInjector<PlayerBodyDependencyInformation>
{
    [SerializeField] private ItemBundleAsset allItemAsset = default;
    [SerializeField] private BuyArea buyArea = default;
    [SerializeField] private List<Transform> viewPoints = default;
    [SerializeField] private List<ItemIDViewer> itemLineup = default;
    private List<GameObject> displayedItems = default;
    private IReadonlyPositionAdapter positionAdapter = default;

    public IReadOnlyList<ItemIDViewer> ItemLineup => itemLineup;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
#if UNITY_EDITOR
        // Conditionalはメソッド内はコンパイルされてしまうので、仕方なく二重
        allItemAsset = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(ItemBundleAsset)}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<ItemBundleAsset>)
                .First();
#endif
        buyArea = GetComponentInChildren<BuyArea>();
    }

    protected override void Awake()
    {
        base.Awake();
        PlayerInitialize.ConsignmentInject_static(this);
    }

    protected override void SafetyOpen()
    {
        displayedItems = new List<GameObject>();

        for (int i = 0; i < itemLineup.Count; i++)
        {
            var asset = allItemAsset.GetItemAssetByID(itemLineup[i]);
            var position = viewPoints[i].position;
            var item = IDisplayItem.Instantiate(asset, position, Quaternion.identity, this);
            displayedItems.Add(item.gameObject);
        }
    }

    protected override void SafetyClose()
    {
        foreach (var obj in displayedItems)
        {
            Destroy(obj);
        }
    }

    public override void Select(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        var asset = allItemAsset.GetItemAssetByID(itemSelectArgs.id);
        var position = itemSelectArgs.position;

        // 選択されたアイテムと同じものを生成する（コピーを表現）
        var item = IDisplayItem.Instantiate(asset, position, Quaternion.identity, this);
        displayedItems.Add(item.gameObject);

        buyArea.Display(positionAdapter.Position);
    }

    public override void Unselect(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        var unselectedPosition = itemSelectArgs.gameObject.transform.position;

        // 掴んだアイテムを離したポイントが、購入エリアだったら購入
        if (buyArea.IsExist(unselectedPosition))
        {
            // Buy
            Debug.Log("BuyArea");
        }

        displayedItems.Remove(itemSelectArgs.gameObject);
        Destroy(itemSelectArgs.gameObject);
        buyArea.Hide();
    }

    public override void Hover(SelectArgs selectArgs)
    {

    }

    public override void Unhover(SelectArgs selectArgs)
    {

    }

    void IDependencyInjector<PlayerBodyDependencyInformation>.Inject(PlayerBodyDependencyInformation information)
    {
        positionAdapter = information.PlayerBody;
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomEditor(typeof(VisualShop))]
    public class ShopEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space(12f);

            if (GUILayout.Button("Update Display Options"))
            {
                try
                {
                    ItemIDViewerDrawer.UpdateDisplayOptions();
                }
                // 要素ない状態でボタン押すと例外出る→うざいので握りつぶす
                catch (System.NullReferenceException) { }
            }
        }
    }
}
#endif