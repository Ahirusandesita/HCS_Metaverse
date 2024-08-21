using HCSMeta.Activity;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
    public class GrabbableAutoAttach : EditorWindow
    {
        [SerializeField] private GameObject prefab = default;
        [SerializeField] private bool useHandGrab = default;
        [SerializeField] private bool useDistanceGrab = default;
        [SerializeField] private bool useDistanceHandGrab = default;
        [SerializeField] private int selectedIndex = default;

        private SerializedObject target = default;
        private Vector2 scrollPosition = default;
        private string[] displayOptions = default;


        [MenuItem("Meta/Grabbable Auto Attach")]
        [MenuItem("Window/Grabbable Auto Attach")]
        public static void OpenWindow()
        {
            var window = GetWindow<GrabbableAutoAttach>();
            window.titleContent = new GUIContent("Grabbable Auto Attach");
            window.Show();
        }

        private void OnEnable()
        {
            target = new SerializedObject(this);

            // 全アセット内からMonoBehaviourを一括取得し、IDisplayItemのみにフィルタリングする。
            // IDisplayItem型をクラス名に変換し、並び替えをしたのちstring配列にキャストする。
            // 配列の先頭には"None"を挿入
            List<string> tmpDisplayItems = new List<string>();
            tmpDisplayItems.Add("None");
            tmpDisplayItems.AddRange(
                Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                .Where(script => script is IDisplayItem)
                .Select(script => script.GetType().FullName)
                .OrderBy(name => name)
                .ToList()
                );
            displayOptions = tmpDisplayItems.ToArray();
        }

        private void OnGUI()
        {
            target.Update();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.Space(16);

            prefab = EditorGUILayout.ObjectField("Target Object", prefab, typeof(GameObject), false) as GameObject;

            EditorGUILayout.Space(16);

            useHandGrab = EditorGUILayout.Toggle("Use Hand Grab", useHandGrab);
            useDistanceGrab = EditorGUILayout.Toggle("Use Distance Grab", useDistanceGrab);
            useDistanceHandGrab = EditorGUILayout.Toggle("Use Distance Hand Grab", useDistanceHandGrab);

            EditorGUILayout.Space(16);

            selectedIndex = EditorGUILayout.Popup("Set IDisplayItem Script", selectedIndex, displayOptions);

            EditorGUILayout.Space(16);

            // Button押下で自動アタッチ実行
            if (GUILayout.Button("Auto Attach"))
            {
                if (prefab is null)
                {
                    Debug.Log("Target Objectにプレハブがアタッチされていません。");
                    return;
                }

                // Rigidbodyがアタッチされていなければ新たにアタッチ
                Rigidbody rigidbody;
                if (prefab.TryGetComponent(out Rigidbody rb))
                {
                    rigidbody = rb;
                }
                else
                {
                    rigidbody = prefab.AddComponent<Rigidbody>();
                }
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;

                // Grabbableがアタッチされていなければ新たにアタッチ
                Grabbable grabbable;
                if (prefab.TryGetComponent(out Grabbable grb))
                {
                    grabbable = grb;
                }
                else
                {
                    grabbable = prefab.AddComponent<Grabbable>();
                }
                grabbable.InjectOptionalRigidbody(rigidbody);

                if (useHandGrab)
                {
                    // HandGrabInteractableがアタッチされていなければ新たにアタッチ
                    HandGrabInteractable handGrab;
                    if (prefab.TryGetComponent(out HandGrabInteractable hgi))
                    {
                        handGrab = hgi;
                    }
                    else
                    {
                        handGrab = prefab.AddComponent<HandGrabInteractable>();
                    }
                    handGrab.InjectOptionalPointableElement(grabbable);
                    handGrab.InjectRigidbody(rigidbody);
                }
                // boolがfalseにもかかわらずアタッチされていた場合は消去する
                else
                {
                    if (prefab.TryGetComponent(out HandGrabInteractable hgi))
                    {
                        DestroyImmediate(hgi, true);
                    }
                }

                if (useDistanceGrab)
                {
                    // DistanceGrabInteractableがアタッチされていなければ新たにアタッチ
                    DistanceGrabInteractable distanceGrab;
                    if (prefab.TryGetComponent(out DistanceGrabInteractable dgi))
                    {
                        distanceGrab = dgi;
                    }
                    else
                    {
                        distanceGrab = prefab.AddComponent<DistanceGrabInteractable>();
                    }
                    distanceGrab.InjectOptionalPointableElement(grabbable);
                    distanceGrab.InjectRigidbody(rigidbody);
                }
                // boolがfalseにもかかわらずアタッチされていた場合は消去する
                else
                {
                    if (prefab.TryGetComponent(out DistanceGrabInteractable dgi))
                    {
                        DestroyImmediate(dgi, true);
                    }
                }

                if (useDistanceHandGrab)
                {
                    // DistanceHandGrabInteractableがアタッチされていなければ新たにアタッチ
                    DistanceHandGrabInteractable distanceHandGrab;
                    if (prefab.TryGetComponent(out DistanceHandGrabInteractable dhgi))
                    {
                        distanceHandGrab = dhgi;
                    }
                    else
                    {
                        distanceHandGrab = prefab.AddComponent<DistanceHandGrabInteractable>();
                    }
                    distanceHandGrab.InjectOptionalPointableElement(grabbable);
                    distanceHandGrab.InjectRigidbody(rigidbody);
                }
                // boolがfalseにもかかわらずアタッチされていた場合は消去する
                else
                {
                    if (prefab.TryGetComponent(out DistanceHandGrabInteractable dhgi))
                    {
                        DestroyImmediate(dhgi, true);
                    }
                }

                // PointableUnityEventWrapperがアタッチされていなければ新たにアタッチ
                PointableUnityEventWrapper pointableWrapper;
                if (prefab.TryGetComponent(out PointableUnityEventWrapper puew))
                {
                    pointableWrapper = puew;
                }
                else
                {
                    pointableWrapper = prefab.AddComponent<PointableUnityEventWrapper>();
                }
                pointableWrapper.InjectPointable(grabbable);

                // 選択したIDisplayItem型文字列配列が"None"以外のとき、そのスクリプトをアタッチ
                if (selectedIndex != 0)
                {
                    // 文字列からTypeを取得
                    // クラス名のみを指定するとなぜかNullが出力されるため、カンマを挟んでアセンブリ名を指定（そうすると正常に動作する）
                    // Assembly-CSharpは、アセンブリが切られてないスクリプトが自動的に割り振られるアセンブリ（自作クラスは指定しなければすべてここに入る）
                    Type displayItem = Type.GetType($"{displayOptions[selectedIndex]}, Assembly-CSharp");

                    Component component;
                    // Componentがアタッチされていなければ新たにアタッチ
                    if (prefab.TryGetComponent(displayItem, out Component cmp))
                    {
                        component = cmp;
                    }
                    else
                    {
                        component = prefab.AddComponent(displayItem);
                    }
                    (component as IDisplayItem).InjectPointableUnityEventWrapper(pointableWrapper);
                }

                Debug.Log("Attach Completed!");

                // Colliderが付いていなかったときに通知する
                if (!prefab.TryGetComponent(out Collider _))
                {
                    Debug.LogWarning("コライダーがアタッチされていません。手動でいずれかのコライダーをアタッチしてください。");
                }
            }

            EditorGUILayout.EndScrollView();
            target.ApplyModifiedProperties();
        }
    }
}
