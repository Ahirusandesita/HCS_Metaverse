using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using HCSMeta.Activity.Cook;
using HCSMeta.Activity.Cook.Interface;

namespace HCSMeta.Function.Initialize
{
#if UNITY_EDITOR
    [CustomEditor(typeof(AllCommodityAsset))]//拡張するクラスを指定
    public class ExampleScriptEditor : Editor
    {

        /// <summary>
        /// InspectorのGUIを更新
        /// </summary>
        public override void OnInspectorGUI()
        {
            //元のInspector部分を表示
            base.OnInspectorGUI();

            //ボタンを表示
            if (GUILayout.Button("データ注入"))
            {
                var guids = AssetDatabase.FindAssets("t:GameObject");
                var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
                List<GameObject> list = paths.Select(_ => AssetDatabase.LoadAssetAtPath<GameObject>(_)).ToList();

                List<Commodity> commodities = new List<Commodity>();


                foreach (GameObject gameObject in list)
                {
                    if (gameObject.TryGetComponent<Commodity>(out Commodity commodity))
                    {
                        foreach (Commodity item in commodities)
                        {
                            if (item.CommodityAsset.CommodityID == commodity.CommodityAsset.CommodityID)
                            {
                                Debug.LogError($"Commodityが重複しているGameObjectがあります。データ注入をキャンセルしてCommodityIDを再付与します。　\n重複しているGameObject-{item.gameObject} {commodity.gameObject}\n重複しているCommodityID-{commodity.CommodityAsset.CommodityID}");
                                GrantCommodityID.Initialize();
                                return;
                            }
                        }
                        commodities.Add(commodity);
                    }
                }
                var allCommodityAsset = target as IAllCommodityAsset;
                allCommodityAsset.Commodities = commodities;

                UnityEditor.EditorUtility.SetDirty(target as AllCommodityAsset);
            }
        }

    }
#endif
}