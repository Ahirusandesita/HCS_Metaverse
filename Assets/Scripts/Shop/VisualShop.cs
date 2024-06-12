using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisualShop : MonoBehaviour, IInteraction, ISelectedNotification
{
    [SerializeField] private AllItemAsset allItemAsset = default;
    [SerializeField] private List<Transform> viewPoints = default;
    [SerializeField] private List<ItemIDViewer> itemLineup = default;
    private List<IDisplayItem> displayedItems = default;

    public IReadOnlyList<ItemIDViewer> ItemLineup => itemLineup;
    ISelectedNotification IInteraction.SelectedNotification => this;


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        allItemAsset = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(AllItemAsset)}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<AllItemAsset>)
                .First();
    }


    public void Open()
    {
        displayedItems = new List<IDisplayItem>();

        for (int i = 0; i < itemLineup.Count; i++)
        {
            var asset = allItemAsset.GetItemAssetByID(itemLineup[i]);
            var position = viewPoints[i].position;
            var item = IDisplayItem.Instantiate(asset, position, Quaternion.identity, this);
            displayedItems.Add(item);
        }
    }

    public void Close()
    {
        foreach (var item in displayedItems)
        {
            Destroy(item.gameObject);
        }
    }

    void ISelectedNotification.Select(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        IDisplayItem.Instantiate(allItemAsset.GetItemAssetByID(itemSelectArgs.id), this);
    }

    void ISelectedNotification.Unselect(SelectArgs selectArgs)
    {
        throw new System.NotImplementedException();
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