using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : MonoBehaviour
{
    [SerializeField]
    private Ingrodients tomato;

    [SerializeField]
    private Ingrodients rawMeat;

    [SerializeField]
    private Commodity banzu;
    [SerializeField]
    private Commodity cutTomato;
    [SerializeField]
    private Ingrodients cutMeat;
    [SerializeField]
    private Commodity fireMeat;
    
    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(MixCommodity.Mix(new Commodity[] { commodity, burger }));
        StartCoroutine(A());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            cutMeat.ProcessingStart(ProcessingType.Bake);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Instantiate(MixCommodity.Mix(new Commodity[] { fireMeat, cutTomato, banzu }));
        }
    }

    IEnumerator A()
    {
        yield return new WaitForSeconds(1f);

        tomato.ProcessingStart(ProcessingType.Cut);
        rawMeat.ProcessingStart(ProcessingType.Cut);
    }
}
