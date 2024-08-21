using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using HCSMeta.Activity.Cook;
using HCSMeta.Activity.Cook.Interface;

namespace HCSMeta.Function.Initialize
{
#if UNITY_EDITOR
    [CustomEditor(typeof(AllCommodityAsset))]//�g������N���X���w��
    public class ExampleScriptEditor : Editor
    {

        /// <summary>
        /// Inspector��GUI���X�V
        /// </summary>
        public override void OnInspectorGUI()
        {
            //����Inspector������\��
            base.OnInspectorGUI();

            //�{�^����\��
            if (GUILayout.Button("�f�[�^����"))
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
                                Debug.LogError($"Commodity���d�����Ă���GameObject������܂��B�f�[�^�������L�����Z������CommodityID���ĕt�^���܂��B�@\n�d�����Ă���GameObject-{item.gameObject} {commodity.gameObject}\n�d�����Ă���CommodityID-{commodity.CommodityAsset.CommodityID}");
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