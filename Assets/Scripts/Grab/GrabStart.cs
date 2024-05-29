using UnityEngine;

public class GrabStart : MonoBehaviour
{
    [SerializeField, InterfaceType(typeof(Grab.IGrabableSelect))]
    private UnityEngine.Object IGrabableSelect;
    private Grab.IGrabableSelect grabableSelect => IGrabableSelect as Grab.IGrabableSelect;

    public void Select()
    {
        if (grabableSelect.CanGrab)
        {
            grabableSelect.Select();
        }
    }
    public void UnSelect()
    {
        grabableSelect.UnSelect();
    }
}
