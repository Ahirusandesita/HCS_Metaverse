using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressUpTest : MonoBehaviour
{
    [SerializeField]
    private List<int> ids = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PartsView>().InjectItemAssetID(ids);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
