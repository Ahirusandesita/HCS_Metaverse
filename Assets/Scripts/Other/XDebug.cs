using UnityEngine;

public static class XDebug
{
    public static void Log(object message, Color color)
    {
#if UNITY_EDITOR
        string rgb = ColorUtility.ToHtmlStringRGB(color);
        Debug.Log($"<color=#{rgb}>{message}</color>");
#endif
    }

    /// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
    public static void Log(object message, string color = "white")
    {
#if UNITY_EDITOR
        Debug.Log($"<color={color}>{message}</color>");
#endif
    }

    public static void LogWarning(object message, Color color)
    {
#if UNITY_EDITOR
        string rgb = ColorUtility.ToHtmlStringRGB(color);
        Debug.LogWarning($"<color=#{rgb}>{message}</color>");
#endif
    }

    /// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
    public static void LogWarning(object message, string color = "white")
    {
#if UNITY_EDITOR
        Debug.LogWarning($"<color={color}>{message}</color>");
#endif
    }

    public static void LogError(object message, Color color)
    {
#if UNITY_EDITOR
        string rgb = ColorUtility.ToHtmlStringRGB(color);
        Debug.LogError($"<color=#{rgb}>{message}</color>");
#endif
    }

    /// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
    public static void LogError(object message, string color = "white")
    {
#if UNITY_EDITOR
        Debug.LogError($"<color={color}>{message}</color>");
#endif
    }

    public static void Print(this object message, Color color)
    {
#if UNITY_EDITOR
        Log(message, color);
#endif
    }

    /// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
    public static void Print(this object message, string color = "white")
    {
#if UNITY_EDITOR
        Log(message, color);
#endif
    }

    public static void PrintWarning(this object message, Color color)
    {
#if UNITY_EDITOR
        LogWarning(message, color);
#endif
    }

    /// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
    public static void PrintWarning(this object message, string color = "white")
    {
#if UNITY_EDITOR
        LogWarning(message, color);
#endif
    }

    public static void PrintError(this object message, Color color)
    {
#if UNITY_EDITOR
        LogError(message, color);
#endif
    }

    /// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
    public static void PrintError(this object message, string color = "white")
    {
#if UNITY_EDITOR
        LogError(message, color);
#endif
    }
}
