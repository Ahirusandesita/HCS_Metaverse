using System;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : SafetyInteractionObject
{
	public class ShelfInteractionInfo : SafetyInteractionInfo
	{
		public class OnShelfInteractionInfo : OnSafetyActionInfo
		{
			public OnShelfInteractionInfo(GameObject shelf, IReadOnlyList<BoxCollider> shelfBoards)
			{
				this.shelf = shelf;
				this.shelfBoards = shelfBoards;
			}

			public GameObject shelf { get; private set; }
			public IReadOnlyList<BoxCollider> shelfBoards { get; private set; }
		}
	}

	[Header("配置可能な棚板リスト（天板含む）\n※下の方の棚板から設定するのがおすすめ")]
	[SerializeField] private List<BoxCollider> shelfBoards = default;
	private int focusBoardIndex = default;
	private Action UpdateAction = default;
	private readonly ShelfInteractionInfo shelfInteractionInfo = new ShelfInteractionInfo();


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
		shelfInteractionInfo.InvokeOpen(this, new ShelfInteractionInfo.OnShelfInteractionInfo(gameObject, shelfBoards));
		UpdateAction += () =>
		{
			// Debug
			if (Input.GetKeyDown(KeyCode.UpArrow)) { focusBoardIndex++; }
			if (Input.GetKeyDown(KeyCode.DownArrow)) { focusBoardIndex--; }
		};
	}

	protected override void SafetyClose()
	{
		XDebug.Log("Shelf Exit", "green");
		UpdateAction = null;
		shelfInteractionInfo.InvokeClose(this, new SafetyInteractionInfo.NullOnSafetyActionInfo());
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
}
