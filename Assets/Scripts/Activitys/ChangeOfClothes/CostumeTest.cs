using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostumeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<ChangeOfClothes>().InjectItemAsset(new int[] { 20001,20160,20321,20322,20288, 20134 });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
