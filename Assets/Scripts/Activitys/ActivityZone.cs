using UnityEngine;
using Fusion;
public class ActivityZone : MonoBehaviour
{
    private string sessionName;
    public string SessionName => sessionName;

    [Rpc]
    private void A(string sessionName)
    {
        
    }
}
