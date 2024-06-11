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


        [MenuItem("Oculus/GrabbableAutoAttach")]
        [MenuItem("Window/GrabbableAutoAttach")]
        public static void OpenWindow()
        {
            var window = GetWindow<GrabbableAutoAttach>();
            window.titleContent = new GUIContent("Grabbable Auto Attach");
            window.Show();
        }

        private void OnEnable()
        {
            target = new SerializedObject(this);

            List<string> tmpDisplayItems = new List<string>();
            tmpDisplayItems.Add("None");
            tmpDisplayItems.AddRange(
                Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                .Where(script => script is IDisplayItem)
                .Select(script => script.GetType().Name)
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

            if (GUILayout.Button("Auto Attach"))
            {
                if (prefab is null)
                {
                    Debug.Log("Target Objectにプレハブがアタッチされていません。");
                    return;
                }

                Rigidbody rigidbody;
                if (prefab.TryGetComponent(out Rigidbody rb))
                {
                    rigidbody = rb;
                }
                else
                {
                    rigidbody = prefab.AddComponent<Rigidbody>();
                }
                rigidbody.isKinematic = true;

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
                    HandGrabInteractable handGrab;
                    if (prefab.TryGetComponent(out HandGrabInteractable hgi))
                    {
                        handGrab = hgi;
                    }
                    else
                    {
                        handGrab = prefab.AddComponent<HandGrabInteractable>();
                    }
                    handGrab.InjectRigidbody(rigidbody);
                }
                else
                {
                    if (prefab.TryGetComponent(out HandGrabInteractable hgi))
                    {
                        DestroyImmediate(hgi, true);
                    }
                }

                if (useDistanceGrab)
                {
                    DistanceGrabInteractable distanceGrab;
                    if (prefab.TryGetComponent(out DistanceGrabInteractable dgi))
                    {
                        distanceGrab = dgi;
                    }
                    else
                    {
                        distanceGrab = prefab.AddComponent<DistanceGrabInteractable>();
                    }
                    distanceGrab.InjectRigidbody(rigidbody);
                }
                else
                {
                    if (prefab.TryGetComponent(out DistanceGrabInteractable dgi))
                    {
                        DestroyImmediate(dgi, true);
                    }
                }

                if (useDistanceHandGrab)
                {
                    DistanceHandGrabInteractable distanceHandGrab;
                    if (prefab.TryGetComponent(out DistanceHandGrabInteractable dhgi))
                    {
                        distanceHandGrab = dhgi;
                    }
                    else
                    {
                        distanceHandGrab = prefab.AddComponent<DistanceHandGrabInteractable>();
                    }
                    distanceHandGrab.InjectRigidbody(rigidbody);
                }
                else
                {
                    if (prefab.TryGetComponent(out DistanceHandGrabInteractable dhgi))
                    {
                        DestroyImmediate(dhgi, true);
                    }
                }

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

                if (selectedIndex != 0)
                {
                    Type displayItem = Type.GetType($"{displayOptions[selectedIndex]}, Assembly-CSharp");
                    if (!prefab.TryGetComponent(displayItem, out Component _))
                    {
                        var component = prefab.AddComponent(displayItem);
                        (component as IDisplayItem).InjectPointableUnityEventWrapper(pointableWrapper);
                    }
                }

                Debug.Log("Attach Completed!");

                if (!prefab.TryGetComponent(out Collider col))
                {
                    Debug.LogWarning("コライダーをアタッチされていません。手動でいずれかのコライダーをアタッチしてください。");
                }
            }

            EditorGUILayout.EndScrollView();
            target.ApplyModifiedProperties();
        }
    }
}
