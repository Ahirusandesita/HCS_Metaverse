using Oculus.Interaction;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Grabbable����Clone�ȃI�u�W�F�N�g�B��������VR���L�̒ʒm�iHover, Select�j�𑗐M��������B
/// </summary>
[RequireComponent(typeof(OutlineManager))]
public class DisplayItem : MonoBehaviour, IDisplayItem
{
	[SerializeField] private PointableUnityEventWrapper onGrabbed = default;
	private ItemSelectArgs itemSelectArgs = default;
	private ISelectedNotification sn = default;

	[SerializeField]
	private List<RegisterSceneInInspector> availableScenes = default;
	[SerializeField]
	private bool canUseAtStart = false;
	[SerializeField]
	private int maxInventoryCapacity = 1;

	public int MaxInventoryCapacity => maxInventoryCapacity;

	public bool CanUseAtStart { get; set; }

	public bool IsAvailable()
	{
		foreach (string item in availableScenes)
		{
			if (item == SceneManager.GetActiveScene().name)
			{
				return true;
			}
		}
		return false;
	}


	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	private void Reset()
	{
		onGrabbed ??= GetComponent<PointableUnityEventWrapper>();
	}

	void IDisplayItem.Inject_ItemSelectArgsAndSelectedNotification(ItemSelectArgs itemSelectArgs, ISelectedNotification sn)
	{
		this.itemSelectArgs = itemSelectArgs;
		this.sn = sn;

		onGrabbed.WhenSelect.AddListener(WhenSelect);
		onGrabbed.WhenUnselect.AddListener(WhenUnselect);
		onGrabbed.WhenHover.AddListener(WhenHover);
		onGrabbed.WhenUnhover.AddListener(WhenUnhover);
	}

	void IDisplayItem.InjectPointableUnityEventWrapper(PointableUnityEventWrapper puew)
	{
		onGrabbed = puew;
	}

	public void WhenSelect(PointerEvent pointerEvent)
	{
		sn.Select(itemSelectArgs);

		onGrabbed.WhenSelect.RemoveListener(WhenSelect);
		onGrabbed.WhenHover.RemoveListener(WhenHover);
		onGrabbed.WhenUnhover.RemoveListener(WhenUnhover);
	}

	public void WhenUnselect(PointerEvent pointerEvent)
	{
		sn.Unselect(itemSelectArgs);
		onGrabbed.WhenUnselect.RemoveListener(WhenUnselect);
	}

	private void WhenHover(PointerEvent pointerEvent)
	{
		sn.Hover(itemSelectArgs);
	}

	private void WhenUnhover(PointerEvent pointerEvent)
	{
		sn.Unhover(itemSelectArgs);
	}



	public void Use()
	{
		// �݌v�~�X�ɂ��A����ł����Ƌ�ǂ�������ł��܂���B���߂񂿂�
		if (TryGetComponent(out PlaceableObject placeableObject))
		{
			// �v���C���[��Ghost�𐶐�
			var playerPlacing = FindAnyObjectByType<Placing>();
			playerPlacing.CreateGhost(placeableObject);
		}
	}

	public void CleanUp()
	{
		GateOfFusion.Instance.Despawn(this);
	}

	public void TakeOut(Vector3 position)
	{

	}
}