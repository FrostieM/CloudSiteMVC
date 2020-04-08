using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;

namespace cloudSiteMvc
{
	internal static class SpreadsheetConfigReader
	{
		private static readonly JObject Config = JObject.Parse(File.ReadAllText(HttpContext.Current.Server.MapPath("~") + "configs/spreadsheet_config.json"));


		public static string ApiKey => Config["ApiKey"].ToString();
		public static string ApplicationName => Config["ApplicationName"].ToString();
		public static string ServiceAccount => Config["ServiceAccount"].ToString();
		public static string SpreadsheetId => Config["SpreadsheetId"].ToString();

		public static bool UseProxy => Config["UseProxy"].ToObject<bool>();
		public static bool UseCredentials => Config["UseCredentials"].ToObject<bool>();

		public static string ProxyPassword => Config["ProxyPassword"].ToString();
		public static string ProxyUsername => Config["ProxyUsername"].ToString();
		public static string ProxyAddress  => Config["ProxyAddress"].ToString();
		public static int ProxyPort => Config["ProxyPort"].ToObject<int>();

	}
}
