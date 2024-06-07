using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class HttpRequest : MonoBehaviour
{
    private string url = "http://10.11.39.210:8080/login";

    [System.Obsolete]
    private void Start()
    {
        StartCoroutine(GetData());
    }

    [System.Obsolete]
    private IEnumerator GetData()
    {
        //UnityWebRequest req = UnityWebRequest.Get(url);
        //yield return req.SendWebRequest();

        //if (req.isNetworkError || req.isHttpError)
        //{
        //    Debug.Log(req.error);
        //}
        //else if (req.responseCode == 200)
        //{

        //    Debug.Log(req.downloadHandler.text);
        //    //LoginData loginData = new LoginData("goro@xxx.co.jp", "hcs5511");
        //    //byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(loginData));
        //    //var request = new UnityWebRequest(url, "POST");

        //    //request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        //    //request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        //    //request.SetRequestHeader("Content-Type", "application/json");
        //    //yield return request.Send();

        //    //Debug.Log(req.downloadHandler.text);
        //    //Debug.Log(request.downloadHandler.text);
        //    //Debug.Log(request.GetRequestHeader("Authorization"));
        //    //ëóêM

        //}




        UnityWebRequest req = new UnityWebRequest(url, "Post");
        LoginData loginData = new LoginData("goro@xxx.co.jp", "hcs5511");
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(loginData));

        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();

        if (req.isNetworkError || req.isHttpError)
        {
            Debug.Log(req.error);
        }
        else if (req.responseCode == 200)
        {

            Debug.Log(req.GetResponseHeader("Authorization"));

            string reqUrl = "http://10.11.39.210:8080/user";
            UnityWebRequest request = UnityWebRequest.Get(reqUrl);
            request.SetRequestHeader("Authorization",req.GetResponseHeader("Authorization"));
            yield return request.Send();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }

            Debug.Log(request.downloadHandler.text);
            //LoginData loginData = new LoginData("goro@xxx.co.jp", "hcs5511");
            //byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(loginData));
            //var request = new UnityWebRequest(url, "POST");

            //request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
            //request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            //request.SetRequestHeader("Content-Type", "application/json");
            //yield return request.Send();

            //Debug.Log(req.downloadHandler.text);
            //Debug.Log(request.downloadHandler.text);
            //Debug.Log(request.GetRequestHeader("Authorization"));
            //ëóêM

        }
    }
}

public class Data
{
    public int age;
    public string name;
    public Data(string name,int age)
    {
        this.age = age;
        this.name = name;
    }
}

public class LoginData
{
    public string userId;
    public string password;
    public LoginData(string userId,string password)
    {
        this.userId = userId;
        this.password = password;
    }
}
