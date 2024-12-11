using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CookTimeUI : MonoBehaviour,INetworkTimeInjectable
{
    [SerializeField]
    private TextMeshProUGUI textMesh;

    private TimeNetwork timeNetwork;
    public void Inject(TimeNetwork timeNetwork)
    {
        this.timeNetwork = timeNetwork;
        this.timeNetwork.OnTime += (time) =>
        {
            textMesh.text = "TIME : " + time.ToString();
        };
    }

    private void Awake()
    {
        textMesh.text = "TIME : ???";
    }
}
