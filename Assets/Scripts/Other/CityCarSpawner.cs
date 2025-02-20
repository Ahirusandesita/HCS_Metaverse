using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCarSpawner : MonoBehaviour
{
    [SerializeField]
    CityCarSelector carSelector = default;

    [SerializeField]
    Transform spawnPoint = default;

    [SerializeField]
    CityCarCheckPoint point = default;

    float spawnInterval = 0f;

    float minInterval = 1f;

    float maxInterval = 4f;

    private void Update()
    {
        spawnInterval -= Time.deltaTime;

        if (spawnInterval <= 0)
        {
            GameObject ins = Instantiate(carSelector.getCar, spawnPoint.position, Quaternion.identity);
            ins.GetComponent<CityCarMove>().NextPoint(point);

            spawnInterval += Random.Range(minInterval, maxInterval);
        }
    }
}
