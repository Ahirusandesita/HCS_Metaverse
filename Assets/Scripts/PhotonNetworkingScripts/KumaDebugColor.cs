using UnityEngine;

namespace KumaDebug
{
	public static class KumaDebugColor
	{
		public static Color ErrorColor => Color.red;
		public static Color WarningColor => Color.yellow;
		public static Color InformationColor => Color.cyan;
		public static Color MessageColor => Color.magenta;
		public static string SuccessColor => "lime";
		public static string RpcColor => "orange";
	}
}