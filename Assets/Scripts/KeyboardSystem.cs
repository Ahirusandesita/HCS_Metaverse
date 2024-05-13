using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private TouchScreenKeyboard touchScreenKeyboard;
    void Start()
    {
        touchScreenKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
