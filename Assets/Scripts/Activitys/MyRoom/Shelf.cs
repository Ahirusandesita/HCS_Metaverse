using System;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : SafetyInteractionObject
{
	public class ShelfInteractionInfo : SafetyInteractionInfo { }

	[Header("配置可能な棚板リスト（天板含む）\n※下の方の棚板から設定するのがおすすめ")]
	[SerializeField] private List<BoxCollider> shelfBoard = default;
	private int focusBoardIndex = default;
	private Action UpdateAction = default;
	private ShelfInteractionInfo shelfInteractionInfo = new ShelfInteractionInfo();


	private void Update()
	{
		UpdateAction?.Invoke();
	}

	public override IInteraction.InteractionInfo Open()
	{
		base.Open();
		return shelfInteractionInfo;
	}

	protected override void SafetyOpen()
	{
		XDebug.Log("Shelf Access", "green");
		focusBoardIndex = 0;
		shelfInteractionInfo.InvokeOpen(this);
		Spawn();
		UpdateAction += () =>
		{
			// Debug
			if (Input.GetKeyDown(KeyCode.UpArrow)) { focusBoardIndex++; Spawn(); }
			if (Input.GetKeyDown(KeyCode.DownArrow)) { focusBoardIndex--; Spawn(); }
		};
	}

	protected override void SafetyClose()
	{
		XDebug.Log("Shelf Exit", "green");
		UpdateAction = null;
		shelfInteractionInfo.InvokeClose(this);
		shelfInteractionInfo.ClearOpen(this);
		shelfInteractionInfo.ClearClose(this);
	}

	public override void Select(SelectArgs selectArgs)
	{
		throw new System.NotImplementedException();
	}

	public override void Unselect(SelectArgs selectArgs)
	{
		throw new System.NotImplementedException();
	}

	private void Spawn()
	{
		FindObjectOfType<PlacingTarget_Shelf>(true).transform.position = shelfBoard[focusBoardIndex].transform.position + Vector3.up * (shelfBoard[focusBoardIndex].size.y + 0.01f);
	}
}
