using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// �v���C���[�̔z�u���[�h�i�n�E�W���O�j
/// </summary>
public class Placing : MonoBehaviour, IInputControllable
{
	[SerializeField] private PlayerState playerState = default;
	[Tooltip("�ݒu�i�n�E�W���O�j���[�h")]
	[SerializeField, HideAtPlaying] private PlaceableObject testOrigin;
	private GhostModel ghostModel = default;

	private int inventoryIndexTest = 0;
	[SerializeField] private ItemBundleAsset allItemAsset = default;

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	private void Reset()
	{
		try
		{
			playerState = GetComponent<PlayerState>();
		}
		catch (System.NullReferenceException) { }
	}

	private void Awake()
	{
		//playerState.PlacingMode.Subscribe(isPlacingMode =>
		//{
		//	if (isPlacingMode)
		//	{
		//		ghostModel.Spawn();
		//	}
		//	else
		//	{
		//		ghostModel.Despawn();
		//	}
		//});
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			CreateGhost(testOrigin);
		}
		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			TryDestroyGhost();
			inventoryIndexTest -= inventoryIndexTest == 0 ? 0 : 1;
			CreateGhost(allItemAsset.GetItemAssetByID(PlayerDontDestroyData.Instance.InventoryToList[inventoryIndexTest].ItemID).DisplayItem.gameObject.GetComponent<PlaceableObject>());

			if (!playerState.PlacingMode.Value)
			{
				playerState.ChangePlacingMode();
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad6))
		{
			TryDestroyGhost();
			inventoryIndexTest += inventoryIndexTest == 19 ? 0 : 1;
			CreateGhost(allItemAsset.GetItemAssetByID(PlayerDontDestroyData.Instance.InventoryToList[inventoryIndexTest].ItemID).DisplayItem.gameObject.GetComponent<PlaceableObject>());
			if (!playerState.PlacingMode.Value)
			{
				playerState.ChangePlacingMode();
			}
		}
	}
#endif

	public void CreateGhost(PlaceableObject origin)
	{
		ghostModel = new GhostModel().CreateModel(origin, transform);
		ghostModel.Spawn();
	}

	public bool TryDestroyGhost()
	{
		if (ghostModel != null)
		{
			ghostModel.DisposeModel();
			ghostModel = null;
			return true;
		}

		return false;
	}
}
