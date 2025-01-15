using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class WarpProcess : MonoBehaviour, IMarkProcess
{
    private WhiteVignetteManager whiteVignette;
    private Transform player;

    public void CanvasTransformInject(Transform canvasTransform)
    {

    }

    public void Process(MarkViewEventArgs markEventArgs, MarkData markData)
    {
        if (markEventArgs.MarkProcessType == MarkProcessType.Select)
        {
            //注意！！　たかやなぎの危険領域に侵入！
            FindObjectOfType<VRPlayerController>().Warp(markData.MarkPosition).Forget();
        }
    }

    private async UniTaskVoid Warp(Vector3 position)
    {
        await whiteVignette.WhiteOut();
        player = FindObjectOfType<PlayerInteraction>().transform;
        player.transform.position = position;
    }
}
