using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : MonoBehaviour
{
    [SerializeField]
    private Commodity commodity;
    [SerializeField]
    private ProcessedGoods processedGoods;
    [SerializeField]
    private ProcessedGoods processed;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MixProcessedGoods>().Mix(processed, processedGoods);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
