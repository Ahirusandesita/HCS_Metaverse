using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの配置モード（ハウジング）
/// </summary>
public class Placing : MonoBehaviour
{
    [Tooltip("設置（ハウジング）モード")]
    [SerializeField] private bool placingMode = default;
    [SerializeField, HideAtPlaying] private GameObject testOrigin;
    private GhostModel ghostModel = default;


    private void Awake()
    {
        //Inputter.PlacingMode.Place.performed += 
        AAA();
    }

    private void Update()
    {
        if (placingMode)
        {
            ghostModel.Spawn();
        }
        else
        {
            ghostModel.Despawn();
        }
    }

    public void AAA()
    {
        ghostModel = new GhostModel().CreateModel(testOrigin).AddPlacingFunction(transform);
    }
}
