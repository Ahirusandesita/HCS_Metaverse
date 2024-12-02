namespace BuildSettingScene
{
	/// <summary>
	/// Scenes拡張クラス（自動生成クラス）。
	/// </summary> 
	public static class ScenesHelper
	{
		/// <summary> 
		/// Scenesを文字列に変換するクラス。
		/// </summary> 
		public static string ScenesToString(this Scenes scenes)
		{
			switch(scenes)
			{
				case Scenes.Test:
					return "Test";
				case Scenes.TestPhotonScene:
					return "TestPhotonScene";
				case Scenes.CookActivity:
					return "CookActivity";
				case Scenes.Playground:
					return "Playground";
				default:
					return "";
			}
		}
	}
}