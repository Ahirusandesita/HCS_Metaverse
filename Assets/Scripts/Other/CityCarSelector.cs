using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCarSelector : MonoBehaviour
{
    [SerializeField]
    GameObject[] car = default;

    public GameObject getCar => car[Random.Range(0, car.Length)];
}
