using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutAreaLine : MonoBehaviour, IDependencyInjector<PlayerBodyDependencyInformation>
{
    private float _fadeDistance = 5f;

    private Image _areaLineImage = default;

    private PlayerBodyDependencyInformation _information = default;

    [SerializeField]
    private Direction _direction = Direction.x;

    private enum Direction
    {
        x,
        z
    }

    private void Start()
    {
        _areaLineImage = GetComponentInChildren<Image>();
    }

    private void FixedUpdate()
    {
        float distanceToPlayer = _direction == Direction.x ? Mathf.Abs(_information.PlayerBody.Position.x - transform.position.x) : Mathf.Abs(_information.PlayerBody.Position.z - transform.position.z);

        float alpha = 1 - Mathf.Clamp(distanceToPlayer / _fadeDistance, 0f, 1f);

        Color newColor = _areaLineImage.color;

        newColor.a = alpha;

        _areaLineImage.color = newColor;
    }

    public void Inject(PlayerBodyDependencyInformation information)
    {
        this._information = information;
    }
}
