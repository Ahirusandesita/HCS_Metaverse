using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCarCheckPoint : MonoBehaviour
{
    public enum pointType
    {
        load,
        end
    }

    public pointType type = default;

    [SerializeField]
    private CityCarCheckPoint[] nextPoint = default;

    public CityCarCheckPoint getNextPoint => nextPoint[Random.Range(0, nextPoint.Length)];
}
