using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour, IInteraction, ISelectedNotification
{
    [SerializeField] private AllItemAsset allItemAsset = default;
    [SerializeField] private List<ItemID> itemLineup = default;

    public IReadOnlyList<ItemID> ItemLineup => itemLineup;

    ISelectedNotification IInteraction.SelectedNotification => this;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
        allItemAsset = allItemAsset = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(AllItemAsset)}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<AllItemAsset>)
                .First();
    }

    private void Start()
    {
        Open();
    }

    public void Open()
    {
        Vector3 vector = transform.position;
        foreach (var id in itemLineup)
        {
            Debug.Log(allItemAsset.ItemDictionary[id].Name);
            IDisplayItem.Instantiate(allItemAsset.ItemDictionary[id], vector, Quaternion.identity, this);
            vector += new Vector3(1.5f, 0f, 0f);

        }
    }

    public void Close()
    {

    }

    void ISelectedNotification.Select(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        IDisplayItem.Instantiate(allItemAsset.ItemDictionary[itemSelectArgs.id], this);
    }

    void ISelectedNotification.Unselect(SelectArgs selectArgs)
    {
        throw new System.NotImplementedException();
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomEditor(typeof(Shop))]
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
                    ItemIDDrawer.UpdateDisplayOptions();
                }
                // 要素ない状態でボタン押すと例外出る→うざいので握りつぶす
                catch (System.NullReferenceException) { }
            }
        }
    }
}
#endif