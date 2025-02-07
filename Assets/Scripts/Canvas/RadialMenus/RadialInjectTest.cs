using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialInjectTest : MonoBehaviour
{
    [SerializeField]
    private List<ItemAsset> itemAssets = new List<ItemAsset>();
    // Start is called before the first frame update
    private void Start()
    {
        FindObjectOfType<RadialMenuManager>().InjectItemAssets(itemAssets);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
