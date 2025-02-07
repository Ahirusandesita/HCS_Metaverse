using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class DefaultDeployment : MonoBehaviour
{
    private async void Start()
    {

        await UniTask.Delay(1000);
        GetComponent<MenuButtonDeploymentDetailMenu>().OnPointerClick(null);
    }
}
