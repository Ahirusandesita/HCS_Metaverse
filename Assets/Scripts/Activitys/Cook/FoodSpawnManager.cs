using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawnManager : MonoBehaviour, ISelectedNotification
{
    [SerializeField] private AllItemAsset allItemAsset = default;


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Reset()
    {
#if UNITY_EDITOR
        // Conditional�̓��\�b�h���̓R���p�C������Ă��܂��̂ŁA�d���Ȃ���d
        allItemAsset = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(AllItemAsset)}")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<AllItemAsset>)
                .First();
#endif
    }

    public void Select(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;
    }

    public void Unselect(SelectArgs selectArgs)
    {
        var itemSelectArgs = selectArgs as ItemSelectArgs;

    }
}
