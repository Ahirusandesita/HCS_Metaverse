using UnityEngine;
using Oculus.Interaction;

public class FlickManager : MonoBehaviour
{
    private IFlickButtonClosure[] flickButtonClosures;


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
        isPushScreen = false;
    }
    public void Selected()
    {
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

            flickButtonClosure.ButtonClose();
        }
    }

    public void SendChar(char type)
    {

    }
}
