using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconPath : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField inputField;

    [SerializeField]
    private List<GameObject> gameObjects = new List<GameObject>();

    public void InputText()
    {
        //�e�L�X�g��inputField�̓��e�𔽉f
        FindObjectOfType<Screenshot>().Path(inputField.text);


        foreach(GameObject obj in gameObjects)
        {
            obj.SetActive(false);
        }
    }


}
