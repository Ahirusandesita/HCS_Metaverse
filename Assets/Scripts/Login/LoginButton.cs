using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

public class LoginButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private LoginManager _loginManager;
    private void Reset()
    {
        _loginManager = FindAnyObjectByType<LoginManager>();
    }
    public void OnPointerClick(PointerEventData data)
    {
        _loginManager.ExecuteLogin().Forget();
    }
}
