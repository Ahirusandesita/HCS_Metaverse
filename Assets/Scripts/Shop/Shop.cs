using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour, IInteraction, ISelectedNotification
{
    [SerializeField] private AllItemAsset allItemAsset = default;
    [SerializeField] private CatalogType catalogType = default;
    [SerializeField] private List<ItemIDViewer> itemLineup = default;

    public IReadOnlyList<ItemIDViewer> ItemLineup => itemLineup;
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
            Debug.Log(allItemAsset.GetItemAssetByID(id).ItemName);
            IDisplayItem.Instantiate(allItemAsset.GetItemAssetByID(id), vector, Quaternion.identity, this);
            vector += new Vector3(1.5f, 0f, 0f);

        }
    }

    public void Close()
    {

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
                    ItemIDViewerDrawer.UpdateDisplayOptions();
                }
                // �v�f�Ȃ���ԂŃ{�^�������Ɨ�O�o�遨�������̂ň���Ԃ�
                catch (System.NullReferenceException) { }
            }
        }
    }
}
#endif