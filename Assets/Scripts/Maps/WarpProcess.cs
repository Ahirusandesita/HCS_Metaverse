using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class WarpProcess : MonoBehaviour, IMarkProcess
{
    private WhiteVignetteManager whiteVignette;
    private Transform player;
    public void Process(MarkViewEventArgs markEventArgs, MarkData markData)
    {
        whiteVignette = FindObjectOfType<WhiteVignetteManager>();
        Warp(markData.MarkPosition).Forget();
    }

    private async UniTaskVoid Warp(Vector3 position)
    {
        await whiteVignette.WhiteOut();
        player = FindObjectOfType<PlayerInteraction>().transform;
        player.transform.position = position;
    }
}
