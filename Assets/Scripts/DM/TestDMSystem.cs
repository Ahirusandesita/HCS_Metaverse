using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDMSystem : MonoBehaviour
{
    [SerializeField]
    private DM dm;
    [SerializeField]
    DetailMenu detailMenu;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            detailMenu.Deployment();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            dm.Message(new MessageInformation("N", MessageSender.Other));
        }
    }
}
