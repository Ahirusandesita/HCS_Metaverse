using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                //  アクセスされたらまずは、インスタンスがあるか調べる
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    //  なかったら作る
                    SetupInstance();
                }
                else
                {
                    //  既に会った時のデバッグログ　特に意味はない
                    string typeName = typeof(T).Name;

                    Debug.Log("[Singleton] " + typeName + " instance already created: " +
                        _instance.gameObject.name);
                }
            }

            return _instance;
        }
    }

    public virtual void Awake()
    {
        //  重複回避のためのチェック
        RemoveDuplicates();

    }

    //  シングルトン初期化
    private static void SetupInstance()
    {
        _instance = (T)FindObjectOfType(typeof(T));

        if (_instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = typeof(T).Name;

            _instance = gameObj.AddComponent<T>();
            DontDestroyOnLoad(gameObj);
        }
    }

    private void RemoveDuplicates()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public class RelatedParties : Singleton<RelatedParties>
{
    private ActivityRelatedPartiesState activityRelatedPartiesState;
    public ActivityRelatedPartiesState ActivityRelatedPartiesState
    {
        get
        {
            return activityRelatedPartiesState;
        }
        set
        {
            activityRelatedPartiesState = value;
        }
    }
}
