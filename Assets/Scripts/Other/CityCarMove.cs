using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCarMove : MonoBehaviour
{
    [SerializeField]
    CityCarCheckPoint point = default;

    float speed = 10f;

    float timecof = default;

    Vector3 startPos = default;

    float v = 0f;

    // Update is called once per frame
    void Update()
    {
        if (false)
        {
            //Ç¢Ç¬Ç©Ç±Ç±Ç≈è’ìÀîªíËÇµÇΩÇ¢Ç»
        }

        v += Time.deltaTime;

        transform.position = Vector3.Lerp(startPos, point.transform.position, v / timecof);

        if (Vector3.Distance(transform.position, point.transform.position) <= 0.1f)
        {
            switch (point.type)
            {
                case CityCarCheckPoint.pointType.load:
                    NextPoint(point.getNextPoint);
                    break;
                case CityCarCheckPoint.pointType.end:
                    Destroy(gameObject);
                    break;
                default:
                    return;
            }
        }
    }

    public void NextPoint(CityCarCheckPoint point)
    {
        this.point = point;
        transform.LookAt(point.transform);
        Vector3 rotation_Y = transform.rotation.eulerAngles;
        rotation_Y.x = 0;
        rotation_Y.z = 0;
        transform.rotation = Quaternion.Euler(rotation_Y);

        startPos = transform.position;

        v = 0;

        timecof = Vector3.Distance(point.transform.position, startPos) / speed;
    }
}
