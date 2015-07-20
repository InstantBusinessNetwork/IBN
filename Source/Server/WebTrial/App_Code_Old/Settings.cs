using System;
using System.Configuration;

namespace Mediachase.Ibn.WebTrial
{
	public class Settings
	{
		public static string WebServiceUrl { get { return ConfigurationManager.AppSettings["WebServiceUrl"]; } }
		public static string UserName { get { return ConfigurationManager.AppSettings["UserName"]; } }
		public static string Password { get { return ConfigurationManager.AppSettings["Password"]; } }
		public static string Domain { get { return ConfigurationManager.AppSettings["Domain"]; } }
		public static string AuthType { get { return ConfigurationManager.AppSettings["AuthType"]; } }
		public static string AspPath { get { return ConfigurationManager.AppSettings["AspPath"]; } }
		public static string ConnectionString { get { return ConfigurationManager.AppSettings["ConnectionString"]; } }
		public static string UnknownResellerGuid { get { return ConfigurationManager.AppSettings["UnknownResellerGuid"]; } }
		public static string RuResellerGuid { get { return ConfigurationManager.AppSettings["ruResellerGuid"]; } }
		public static string EnResellerGuid { get { return ConfigurationManager.AppSettings["enResellerGuid"]; } }
	}
}
