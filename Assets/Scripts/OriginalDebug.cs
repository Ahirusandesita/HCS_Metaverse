using System.Diagnostics;
using UnityEngine;
using System;
using System.ComponentModel;
public static class Debugexpansion
{
    public static void Log(this IDebug debug,object message)
    {
        debug.RegularLog(message);
    }
    public static void LogError(this IDebug debug,object message)
    {
        debug.RegularLogError(message);
    }
    public static void LogWarning(this IDebug debug,object message)
    {
        debug.RegularLogWarning(message);
    }
}
public interface IDebug
{
    void RegularLog(object message);
    void RegularLogError(object message);
    void RegularLogWarning(object message);
}
public class Debug : IDebug
{
    public static void Log(object message) { }
    void IDebug.RegularLog(object message)
    {
        UnityEngine.Debug.Log(message);
    }
    public static void LogError(object message) { }
    void IDebug.RegularLogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }
    public static void LogWarning(object message) { }
    void IDebug.RegularLogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Debug.logger is obsolete. Please use Debug.unityLogger instead (UnityUpgradable) -> unityLogger")]
    public static ILogger logger { get; }
    public static bool developerConsoleVisible { get; set; }
    public static bool developerConsoleEnabled { get; set; }
    public static ILogger unityLogger { get; }
    public static bool isDebugBuild { get; }

    public static void Log(object message, UnityEngine.Object context)
    {

    }
    [Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertion(object message, UnityEngine.Object context)
    {

    }
    [Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertion(object message)
    {

    }
    [Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertionFormat(UnityEngine.Object context, string format, params object[] args)
    {

    }
    [Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertionFormat(string format, params object[] args)
    {

    }
    public static void LogError(object message, UnityEngine.Object context)
    {

    }
    public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
    {

    }
    public static void LogErrorFormat(string format, params object[] args)
    {

    }
    public static void LogException(Exception exception)
    {

    }
    public static void LogException(Exception exception, UnityEngine.Object context)
    {

    }
    public static void LogFormat(string format, params object[] args)
    {

    }
    public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
    {

    }
    public static void LogFormat(LogType logType, LogOption logOptions, UnityEngine.Object context, string format, params object[] args)
    {

    }

    public static void LogWarning(object message, UnityEngine.Object context)
    {

    }

    public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
    {

    }
    public static void LogWarningFormat(string format, params object[] args)
    {

    }
}
