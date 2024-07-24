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

    public static void LogColor(object message, string color = "white")
    {
#if UNITY_EDITOR
        Debug.Log($"<color={color}>{message}</color>");
#endif
    }
}
