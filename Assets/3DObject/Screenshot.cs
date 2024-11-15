using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Screenshot : MonoBehaviour
{
    bool isCreatingScreenShot = false;
    string path;
    private string fileName = "NoName.png";

    public void IconName(string name)
    {
        this.fileName = name+".png";
    }
    public void Path(string path)
    {
        this.path = path+"/";
    }

    public void PrintScreen()
    {
        StartCoroutine(PrintScreenInternal());
    }

    IEnumerator PrintScreenInternal()
    {
        if (isCreatingScreenShot)
        {
            yield break;
        }

        isCreatingScreenShot = true;

        yield return null;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string filen = path + fileName;
        ScreenCapture.CaptureScreenshot(filen);

        yield return new WaitUntil(() => File.Exists(filen));
        Debug.Log($"{fileName}　スクリーンショット完了");
        isCreatingScreenShot = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            PrintScreen();
        }
    }

}