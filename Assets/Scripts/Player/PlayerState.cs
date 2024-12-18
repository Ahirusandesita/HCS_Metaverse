using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerState : MonoBehaviour, IInputControllable
{
    [SerializeField] private BoolReactiveProperty placingMode = new BoolReactiveProperty();
    public IReadOnlyReactiveProperty<bool> PlacingMode => placingMode;

	private void Start()
	{
        placingMode.Subscribe(_ =>
        {
            if (placingMode.Value)
            {
                // Debug
                Inputter.ChangeInputPreset(this, Inputter.InputActionPreset.Placing);
            }
            else
            {
                // Debug
                Inputter.ChangeInputPreset(this, Inputter.InputActionPreset.Default);
            }
        }).AddTo(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            placingMode.Value = !placingMode.Value;
        }
    }

}
