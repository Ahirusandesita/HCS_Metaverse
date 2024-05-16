using UnityEngine;
using Oculus.Interaction;
using TMPro;

public class FlickManager : MonoBehaviour
{
    private IFlickButtonClosure[] flickButtonClosures;

    [SerializeField]
    TextMeshProUGUI TextMeshProUGUI;

    private bool isPushScreen;
    public bool IsPushScreen 
    { 
        get
        {
            return isPushScreen;
        } 
    }

    private void Awake()
    {
        flickButtonClosures = this.transform.GetComponentsInChildren<IFlickButtonClosure>();
    }

    public void UnSelected()
    {
        Debug.Log($"UnitittiUntitti  UnSelect");
        isPushScreen = false;
    }
    public void Selected()
    {
        Debug.Log($"UnitittiUntitti  Select");
        isPushScreen = true;
    }

    public void StartFlickInput(IFlickButtonClosure flickButtonClosure)
    {
        foreach(IFlickButtonClosure closure in flickButtonClosures)
        {
            if(closure == flickButtonClosure)
            {
                continue;
            }

            closure.ButtonClose();
        }
    }

    public void FinishFlickInput(IFlickButtonClosure flickButtonClosure)
    {
        foreach(IFlickButtonClosure closure in flickButtonClosures)
        {
            closure.ButtonOpen();
        }
    }

    public void SendChar(char type)
    {
        TextMeshProUGUI.text += type;
    }
}
