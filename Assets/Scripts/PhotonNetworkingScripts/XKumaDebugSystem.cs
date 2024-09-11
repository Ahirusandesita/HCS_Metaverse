using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KumaDebug
{
	public static class XKumaDebugSystem
	{
#if UNITY_EDITOR
		private static MasterServerConect masterServer;
		private static MasterServerConect MasterServer => masterServer ??= Object.FindObjectOfType<MasterServerConect>();
#endif
		public static void Log(object message, Color color)
		{
#if UNITY_EDITOR
			if (MasterServer.IsKumaDebug)
			{
				string rgb = ColorUtility.ToHtmlStringRGB(color);
				Debug.Log($"<color=#{rgb}>{message}</color>");
			}
#endif
		}

		/// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
		public static void Log(object message, string color = "white")
		{
#if UNITY_EDITOR
			if (MasterServer.IsKumaDebug)
			{
				Debug.Log($"<color={color}>{message}</color>");
			}
#endif
		}

		public static void LogWarning(object message, Color color)
		{
#if UNITY_EDITOR
			if (MasterServer.IsKumaDebug)
			{
				string rgb = ColorUtility.ToHtmlStringRGB(color);
				Debug.LogWarning($"<color=#{rgb}>{message}</color>");
			}
#endif
		}

		/// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
		public static void LogWarning(object message, string color = "white")
		{
#if UNITY_EDITOR
			if (MasterServer.IsKumaDebug)
			{
				Debug.LogWarning($"<color={color}>{message}</color>");
			}
#endif
		}

		public static void LogError(object message, Color color)
		{
#if UNITY_EDITOR
			if (MasterServer.IsKumaDebug)
			{
				string rgb = ColorUtility.ToHtmlStringRGB(color);
				Debug.LogError($"<color=#{rgb}>{message}</color>");
			}
#endif
		}

		/// <param name="color">「#」から始まるカラーコード、または「red」等HTMLのカラーネーム</param>
		public static void LogError(object message, string color = "white")
		{
#if UNITY_EDITOR
			if (MasterServer.IsKumaDebug)
			{
				Debug.LogError($"<color={color}>{message}</color>");
			}
#endif
		}
	}
}