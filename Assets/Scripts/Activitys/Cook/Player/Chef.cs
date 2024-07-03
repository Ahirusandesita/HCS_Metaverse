using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : MonoBehaviour
{
    [SerializeField]
    private Commodity commodity;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MixProcessedGoods>().Mix(new Commodity[] { commodity, commodity });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
