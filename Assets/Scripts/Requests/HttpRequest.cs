using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpRequest : MonoBehaviour
{
    private string url = "http://localhost:18080/foo/bar/baz";

    [System.Obsolete]
    private void Start()
    {
        StartCoroutine(GetData());
    }

    [System.Obsolete]
    private IEnumerator GetData()
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.isNetworkError || req.isHttpError)
        {
            Debug.Log(req.error);
        }
        else if (req.responseCode == 200)
        {
            Data data = new Data("MetaMan", 20);
            byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();


            Debug.Log(req.downloadHandler.text);

            //ëóêM
            //    byte[] postData = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(req.downloadHandler.text));
            //    var request = new UnityWebRequest(url, "POST");
            //    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
            //    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            //    request.SetRequestHeader("Content-Type", "application/json");
            //    yield return request.Send();
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
