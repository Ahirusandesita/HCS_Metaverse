using UnityEngine;
public class ActivityZone : MonoBehaviour
{
	private string sessionName;
	public string SessionName => sessionName;


	public void SetSessionName(string sessionName)
	{
		this.sessionName = sessionName;
		Debug.LogWarning("set:" + sessionName);
	}
}
