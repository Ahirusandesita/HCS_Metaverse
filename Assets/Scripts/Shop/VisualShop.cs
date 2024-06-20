using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisualShop : SafetyInteractionObject
{
    [SerializeField] private AllItemAsset allItemAsset = default;
    [SerializeField] private BuyArea buyArea = default;
    [SerializeField] private List<Transform> viewPoints = default;
    [SerializeField] private List<ItemIDViewer> itemLineup = default;
    private List<IDisplayItem> displayedItems = default;

    public IReadOnlyList<ItemIDViewer> ItemLineup => itemLineup;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
#if UNITY_EDITOR
        // �Ȃ���Conditional�t���Ă�AssetDatabase�^���r���h���ɃG���[�N�����̂ŁA�d���Ȃ���d
        allItemAsset = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(AllItemAsset)}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<AllItemAsset>)
                .First();
#endif
    }

    protected override void SafetyOpen()
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

    protected override void SafetyClose()
    {
        foreach (var item in displayedItems)
        {
            Destroy(item.gameObject);
        }
    }

    public override void Select(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        var asset = allItemAsset.GetItemAssetByID(itemSelectArgs.id);
        var position = itemSelectArgs.position;
        var item =�@IDisplayItem.Instantiate(asset, position, Quaternion.identity, this);
        displayedItems.Add(item);

        buyArea.Display(itemSelectArgs.position);
    }

    public override void Unselect(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
        var unselectedPosition = itemSelectArgs.gameObject.transform.position;

        // �͂񂾃A�C�e���𗣂����|�C���g���A�w���G���A��������w��
        if (buyArea.IsExist(unselectedPosition))
        {
            // Buy
            Debug.Log("BuyArea");
        }

        Destroy(itemSelectArgs.gameObject);
        buyArea.Hide();
    }

    public override void Hover(SelectArgs selectArgs)
    {

    }

    public override void Unhover(SelectArgs selectArgs)
    {

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
                // �v�f�Ȃ���ԂŃ{�^�������Ɨ�O�o�遨�������̂ň���Ԃ�
                catch (System.NullReferenceException) { }
            }
        }
    }
}
#endif