using System;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : SafetyInteractionObject
{
	public class PlacingInteractionInfo : IInteraction.InteractionInfo
	{
		
	}

	[Header("�z�u�\�ȒI���X�g�i�V�܂ށj\n�����̕��̒I����ݒ肷��̂���������")]
	[SerializeField] private List<BoxCollider> shelfBoard = default;
	private int focusBoardIndex = default;
	private Action UpdateAction = default;


	private void Update()
	{
		UpdateAction?.Invoke();
	}

	protected override void SafetyOpen()
	{
		XDebug.Log("Shelf Access", "green");
		focusBoardIndex = 0;
		interactionInfoSubject.OnNext(new PlacingInteractionInfo());
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
