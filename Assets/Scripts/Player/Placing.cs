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
		AAA();

		playerState.PlacingMode.Subscribe(isPlacingMode =>
		{
			if (isPlacingMode)
			{
				ghostModel.Spawn();
			}
			else
			{
				ghostModel.Despawn();
			}
		});
	}

	public void AAA()
	{
		ghostModel = new GhostModel().CreateModel(testOrigin, transform);
	}
}
