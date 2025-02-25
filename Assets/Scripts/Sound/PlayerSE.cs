using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerSE : MonoBehaviour
{
    [System.Serializable]
    public struct Footsteps
    {
        public AudioClip footsteps_1;
        public AudioClip footsteps_2;
        public AudioClip footsteps_3;
        public AudioClip footsteps_4;
        public AudioClip footsteps_5;
        public AudioClip footsteps_6;
    }

    [SerializeField]
    private Footsteps _footsteps = default;

    private Vector2 _direction;

    private float _footstepsInterval;

    private const float FOOTSTEPS_INTERVAL = 0.8f;

    private AudioClip GetRandomFootsteps => UnityEngine.Random.Range(1, 7) switch
    {
        1 => _footsteps.footsteps_1,
        2 => _footsteps.footsteps_2,
        3 => _footsteps.footsteps_3,
        4 => _footsteps.footsteps_4,
        5 => _footsteps.footsteps_5,
        6 => _footsteps.footsteps_6,
        _ => _footsteps.footsteps_1
    };

    private void Start()
    {
        Inputter.Player.Move.performed += PaformedAction;

        Inputter.Player.Move.canceled += CanseledAction;
    }

    private void Update()
    {
        _footstepsInterval -= Time.deltaTime;

        if (_footstepsInterval > 0) return;

        if (_direction != Vector2.zero)
        {
            PlayFootStep();
            _footstepsInterval = FOOTSTEPS_INTERVAL;
        }
    }

    public void PlayFootStep()
    {
        AudioSource.PlayClipAtPoint(GetRandomFootsteps, transform.position);
    }

    private void OnDestroy()
    {
        Inputter.Player.Move.performed -= PaformedAction;

        Inputter.Player.Move.canceled -= CanseledAction;
    }

    private void PaformedAction(InputAction.CallbackContext dir)
    {
        _direction = dir.ReadValue<Vector3>();
    }

    private void CanseledAction(InputAction.CallbackContext dir)
    {
        _direction = Vector2.zero;
    }
}
