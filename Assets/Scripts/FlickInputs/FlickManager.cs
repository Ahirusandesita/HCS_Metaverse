using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlickManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;

    private IFlickButtonOpeningAndClosing[] flickButtonOpeningAndClosings;

    private void Awake()
    {
        flickButtonOpeningAndClosings = this.GetComponentsInChildren<IFlickButtonOpeningAndClosing>(true);
    }

    public void StartFlick(IFlickButtonOpeningAndClosing flickButtonOpeningAndClosing)
    {
        foreach(IFlickButtonOpeningAndClosing item in flickButtonOpeningAndClosings)
        {
            if(item == flickButtonOpeningAndClosing)
            {
                continue;
            }

            item.Close();
        }
    }
    public void EndFlick(IFlickButtonOpeningAndClosing flickButtonOpeningAndClosing)
    {
        foreach(IFlickButtonOpeningAndClosing item in flickButtonOpeningAndClosings)
        {
            item.Open();
        }
    }

    public void SendMessage(char keyChar)
    {
        textMeshProUGUI.text += keyChar;
    }
}
