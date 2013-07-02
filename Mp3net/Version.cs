namespace Mp3net
{
	public class Version
	{
		private static readonly string VERSION = "0.8.2";

		private static readonly string URL = "http://github.com/mpatric/mp3agic";

		public static string AsString()
		{
			return GetVersion() + " - " + Version.GetUrl();
		}

		public static string GetVersion()
		{
			return VERSION;
		}

		public static string GetUrl()
		{
			return URL;
		}
	}
}
