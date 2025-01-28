using Fusion;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class GrabbableAutoAttach : EditorWindow
{
	[SerializeField] private GameObject prefab = default;
	[SerializeField] private GameObject[] prefabs = default;
	[SerializeField] private bool autoSearch = default;
	[SerializeField] private bool useHandGrab = default;
	[SerializeField] private bool useDistanceGrab = default;
	[SerializeField] private bool useDistanceHandGrab = default;
	[SerializeField] private int selectedIndex = default;
	[SerializeField] private bool createItemAsset = default;
	[SerializeField] private string folderName = default;
	[SerializeField] private bool createNetworkViewPrefab = default;
	[SerializeField] private string prefabFolderName = default;

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

		// �S�A�Z�b�g������MonoBehaviour���ꊇ�擾���AIDisplayItem�݂̂Ƀt�B���^�����O����B
		// IDisplayItem�^���N���X���ɕϊ����A���ёւ��������̂�string�z��ɃL���X�g����B
		// �z��̐擪�ɂ�"None"��}��
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
		autoSearch = EditorGUILayout.Toggle("Auto Search", autoSearch);

		EditorGUILayout.Space(16);

		useHandGrab = EditorGUILayout.Toggle("Use Hand Grab", useHandGrab);
		useDistanceGrab = EditorGUILayout.Toggle("Use Distance Grab", useDistanceGrab);
		useDistanceHandGrab = EditorGUILayout.Toggle("Use Distance Hand Grab", useDistanceHandGrab);

		EditorGUILayout.Space(16);

		selectedIndex = EditorGUILayout.Popup("Set IDisplayItem Script", selectedIndex, displayOptions);

		EditorGUILayout.Space(16);

		createItemAsset = EditorGUILayout.Toggle("Create Item Asset", createItemAsset);
		folderName = EditorGUILayout.TextField("Folder Name", folderName);

		EditorGUILayout.Space(16);

		createNetworkViewPrefab = EditorGUILayout.Toggle("Create NetworkView Prefab", createNetworkViewPrefab);
		prefabFolderName = EditorGUILayout.TextField("Prefab Folder Name", prefabFolderName);

		// Button�����Ŏ����A�^�b�`���s
		if (GUILayout.Button("Auto Attach"))
		{
			if (autoSearch)
			{
				// ���ׂĂ�Prefab��AssetPath���擾
				var assetPaths = AssetDatabase.FindAssets($"t:Prefab")
					.Select(AssetDatabase.GUIDToAssetPath)
					.ToArray();

				List<GameObject> gameObjects = new();
				for (int i = 0; i < assetPaths.Length; i++)
				{
					// AssetPath����GameObject���擾����B
					var loadAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPaths[i]);
					if (loadAsset.TryGetComponent(out PlaceableObject _))
					{
						gameObjects.Add(loadAsset);
					}
				}
				prefabs = gameObjects.ToArray();
			}
			else if (prefab is null)
			{
				Debug.Log("Target Object�Ƀv���n�u���A�^�b�`����Ă��܂���B");
				EditorGUILayout.EndScrollView();
				target.ApplyModifiedProperties();
				return;
			}
			else
			{
				prefabs = new GameObject[] { prefab };
			}

			foreach (var prefab in prefabs)
			{
				// Rigidbody���A�^�b�`����Ă��Ȃ���ΐV���ɃA�^�b�`
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

				// Grabbable���A�^�b�`����Ă��Ȃ���ΐV���ɃA�^�b�`
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
					// HandGrabInteractable���A�^�b�`����Ă��Ȃ���ΐV���ɃA�^�b�`
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
				// bool��false�ɂ�������炸�A�^�b�`����Ă����ꍇ�͏�������
				else
				{
					if (prefab.TryGetComponent(out HandGrabInteractable hgi))
					{
						DestroyImmediate(hgi, true);
					}
				}

				if (useDistanceGrab)
				{
					// DistanceGrabInteractable���A�^�b�`����Ă��Ȃ���ΐV���ɃA�^�b�`
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
				// bool��false�ɂ�������炸�A�^�b�`����Ă����ꍇ�͏�������
				else
				{
					if (prefab.TryGetComponent(out DistanceGrabInteractable dgi))
					{
						DestroyImmediate(dgi, true);
					}
				}

				if (useDistanceHandGrab)
				{
					// DistanceHandGrabInteractable���A�^�b�`����Ă��Ȃ���ΐV���ɃA�^�b�`
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
				// bool��false�ɂ�������炸�A�^�b�`����Ă����ꍇ�͏�������
				else
				{
					if (prefab.TryGetComponent(out DistanceHandGrabInteractable dhgi))
					{
						DestroyImmediate(dhgi, true);
					}
				}

				// PointableUnityEventWrapper���A�^�b�`����Ă��Ȃ���ΐV���ɃA�^�b�`
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

				// �I������IDisplayItem�^������z��"None"�ȊO�̂Ƃ��A���̃X�N���v�g���A�^�b�`
				if (selectedIndex != 0)
				{
					// �����񂩂�Type���擾
					// �N���X���݂̂��w�肷��ƂȂ���Null���o�͂���邽�߁A�J���}������ŃA�Z���u�������w��i��������Ɛ���ɓ��삷��j
					// Assembly-CSharp�́A�A�Z���u�����؂��ĂȂ��X�N���v�g�������I�Ɋ���U����A�Z���u���i����N���X�͎w�肵�Ȃ���΂��ׂĂ����ɓ���j
					Type displayItem = Type.GetType($"{displayOptions[selectedIndex]}, Assembly-CSharp");

					Component component;
					// Component���A�^�b�`����Ă��Ȃ���ΐV���ɃA�^�b�`
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

				if (!prefab.TryGetComponent(out SwitchableGrabbableActive _))
				{
					prefab.AddComponent<SwitchableGrabbableActive>();
				}

				DistanceInteractableActivatable distanceInteractableActivatable;
				if (prefab.TryGetComponent(out DistanceInteractableActivatable dia))
				{
					distanceInteractableActivatable = dia;
				}
				else
				{
					distanceInteractableActivatable = prefab.AddComponent<DistanceInteractableActivatable>();
				}
				distanceInteractableActivatable.SetActiveDistance(2f);

				LocalView localView;
				if (prefab.TryGetComponent(out LocalView lv))
				{
					localView = lv;
				}
				else
				{
					localView = prefab.AddComponent<LocalView>();
				}
				localView.SetMeshRenderers(prefab.GetComponents<MeshRenderer>().ToList());

				if (createItemAsset)
				{
					if (string.IsNullOrEmpty(folderName))
					{
						XDebug.Log($"{nameof(folderName)} �����͂���Ă��܂���B", "yellow");
						break;
					}

					var existingAsset = AssetDatabase.FindAssets($"t:{nameof(ItemAsset)}")
							.Select(AssetDatabase.GUIDToAssetPath)
							.Select(AssetDatabase.LoadAssetAtPath<ItemAsset>)
							.Where(asset => asset.name == prefab.name)
							.FirstOrDefault();

					ItemAsset itemAsset;
					if (existingAsset != null)
					{
						itemAsset = existingAsset;
					}
					else
					{
						itemAsset = CreateInstance<ItemAsset>();
					}

					var displayItemInfo = itemAsset.GetType()
						.GetField("displayItem", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					if (displayItemInfo != null)
					{
						if (selectedIndex != 0)
						{
							displayItemInfo.SetValue(itemAsset, prefab.GetComponent<IDisplayItem>());
						}
					}

					var itemNameInfo = itemAsset.GetType()
						.GetField("itemName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					if (itemNameInfo != null)
					{
						itemNameInfo.SetValue(itemAsset, prefab.name);
					}

					var genreInfo = itemAsset.GetType()
						.GetField("itemGenre", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					if (genreInfo != null)
					{
						genreInfo.SetValue(itemAsset, ItemGenre.Interior);
					}

					if (createNetworkViewPrefab)
					{
						if (string.IsNullOrEmpty(prefabFolderName))
						{
							XDebug.Log($"{nameof(prefabFolderName)} �����͂���Ă��܂���B", "yellow");
							break;
						}

						GameObject tmpObject = EditorUtility.CreateGameObjectWithHideFlags(
							$"Network_{prefab.name}",
							HideFlags.HideInHierarchy,
							typeof(MeshFilter), typeof(MeshRenderer), typeof(NetworkObject), typeof(NetworkTransform), typeof(NetworkView)
							);

						tmpObject.GetComponent<MeshFilter>().sharedMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
						tmpObject.GetComponent<MeshRenderer>().sharedMaterials = prefab.GetComponent<MeshRenderer>().sharedMaterials;
						tmpObject.GetComponent<NetworkObject>().Flags = NetworkObjectFlags.MasterClientObject;
						tmpObject.GetComponent<NetworkTransform>().AutoUpdateAreaOfInterestOverride = true;

						GameObject networkViewPrefab = PrefabUtility.CreatePrefab($"Assets/Prefabs/NetworkView/{prefabFolderName}/Network_{prefab.name}.prefab", tmpObject);
						DestroyImmediate(tmpObject);

						var networkViewInfo = itemAsset.GetType()
							.GetField("networkView", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
						if (networkViewInfo != null)
						{
							networkViewInfo.SetValue(itemAsset, networkViewPrefab.GetComponent<NetworkView>());
						}
					}

					EditorUtility.SetDirty(itemAsset);

					if (existingAsset == null)
					{
						string fileName = $"{prefab.name}.asset";
						string path = $"Assets/ScriptableObject/ItemData/{folderName}";

						AssetDatabase.CreateAsset(itemAsset, Path.Combine(path, fileName));
					}
				}

				// Collider���t���Ă��Ȃ������Ƃ��ɒʒm����
				if (!prefab.TryGetComponent(out Collider _))
				{
					Debug.LogWarning("�R���C�_�[���A�^�b�`����Ă��܂���B�蓮�ł����ꂩ�̃R���C�_�[���A�^�b�`���Ă��������B");
				}
			}

			XDebug.Log("Attach Completed!", "orange");
		}

		EditorGUILayout.EndScrollView();
		target.ApplyModifiedProperties();
	}
}