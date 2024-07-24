using UnityEngine;

public static class XDebug
{
    public static void LogColor(object message, Color color)
    {
#if UNITY_EDITOR
        string rgb = ColorUtility.ToHtmlStringRGB(color);
        Debug.Log($"<color=#{rgb}>{message}</color>");
#endif
    }

    /// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
    public static void LogColor(object message, string color = "white")
    {
#if UNITY_EDITOR
        Debug.Log($"<color={color}>{message}</color>");
#endif
    }

    public static void Print(this object message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    public static void PrintColor(this object message, Color color)
    {
#if UNITY_EDITOR
        LogColor(message, color);
#endif
    }

    /// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
    public static void PrintColor(this object message, string color = "white")
    {
#if UNITY_EDITOR
        LogColor(message, color);
#endif
    }
}
