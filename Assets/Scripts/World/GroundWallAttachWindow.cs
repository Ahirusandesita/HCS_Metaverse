using System;
using UnityEngine;

namespace UnityEditor.HCSMeta
{
    public class GroundWallAttachWindow : EditorWindow
    {
        private enum OperationMode
        {
            Attach,
            Remove
        }

        [SerializeField] private GameObject prefab = default;
        [SerializeField] private OperationMode mode = default;
        [SerializeField] private int selectedIndex = default;
        [SerializeField] private string[] types = new string[3];
        [SerializeField] private bool containsRoot = default;
        private GUIStyle style = default;

        [MenuItem("Window/Ground And Wall Attach Window")]
        public static void OpenWindow()
        {
            var window = GetWindow<GroundWallAttachWindow>();
            window.titleContent = new GUIContent("Ground and Wall Attach Window");
            window.Show();
        }

        private void OnEnable()
        {
            types[0] = nameof(Ground);
            types[1] = nameof(Wall);
            types[2] = nameof(Pedestal);
            style = new GUIStyle { richText = true, fontSize = 16 };
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(16);

            GUILayout.Label("<b><color=#f5f5f5>Target Rootに設定されたオブジェクトの子オブジェクトに、" +
                "指定したコンポーネントをアタッチします。</color></b>", style);

            EditorGUILayout.Space(16);

            mode = (OperationMode)EditorGUILayout.Popup("Mode", (int)mode, Enum.GetNames(typeof(OperationMode)));
            selectedIndex = EditorGUILayout.Popup("Attach Component", selectedIndex, types);
            prefab = EditorGUILayout.ObjectField("Target Root", prefab, typeof(GameObject), true) as GameObject;
            containsRoot = EditorGUILayout.Toggle("Contains Root", containsRoot);

            EditorGUILayout.Space(32);

            if (GUILayout.Button("Execute Attach"))
            {
                Type type = default;
                switch (types[selectedIndex])
                {
                    case nameof(Ground):
                        type = typeof(Ground);
                        break;

                    case nameof(Wall):
                        type = typeof(Wall);
                        break;

                    case nameof(Pedestal):
                        type = typeof(Pedestal);
                        break;
                }

                var childs = prefab.GetComponentsInChildren<Transform>();
                for (int i = 0; i < childs.Length; i++)
                {
                    if (i == 0 && !containsRoot)
                    {
                        continue;
                    }

                    if (mode == OperationMode.Attach)
                    {
                        childs[i].gameObject.AddComponent(type);
                    }
                    else
                    {
                        childs[i].TryGetComponent(type, out var component);
                        DestroyImmediate(component);
                    }
                }

                if (mode == OperationMode.Attach)
                {
                    Debug.Log($"Completed: Attach [{type.Name}]!");
                }
                else
                {
                    Debug.Log($"Completed: Remove [{type.Name}]!");
                }
            }
        }
    }
}
