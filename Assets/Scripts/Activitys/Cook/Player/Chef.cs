using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : MonoBehaviour
{
    [SerializeField]
    private Commodity commodity;
    [SerializeField]
    private Commodity burger;
    [SerializeField]
    private Ingrodients ingrodients;
    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(MixCommodity.Mix(new Commodity[] { commodity, burger }));

        ingrodients.ProcessingStart(ProcessingType.Bake);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
