using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPoiPoi : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(YScrollObject yScrollObject in this.GetComponentsInChildren<YScrollObject>())
        {
            yScrollObject.InjectDownLimit(yScrollObject.GetComponent<RectTransform>().localPosition.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
