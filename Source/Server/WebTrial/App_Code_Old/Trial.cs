using System;
using System.Data;
using System.Net;
using System.Xml;

namespace Mediachase.Ibn.WebTrial
{
	public class TrialHelper
	{
		#region GetProxy
		static WebTrial.localhost.Trial GetProxy()
		{
			WebTrial.localhost.Trial proxy = new WebTrial.localhost.Trial();
			proxy.Timeout = 5*60*1000;
			proxy.Url = Settings.WebServiceUrl;

			CredentialCache cache = new CredentialCache();
			cache.Add(new Uri(proxy.Url), Settings.AuthType, new NetworkCredential(Settings.UserName, Settings.Password, Settings.Domain)); 
			proxy.Credentials = cache;

			return proxy;
		}
		#endregion


		#region Request
		public static WebTrial.localhost.TrialResult Request(
			string CompanyName
			, string Domain
			, string FirstName
			, string LastName
			, string Email
			, string Phone
			, string Country
			, string Login
			, string Password
			, string ResellerGuid
			, string Locale
			, int TimeZoneId
			, string referrer
			, out int RequestId
			, out string RequestGuid
			)
		{
			WebTrial.localhost.Trial proxy = GetProxy();
			return proxy.CompanyRequestTrial2(
				CompanyName
				, string.Empty
				, string.Empty
				, Domain
				, FirstName
				, LastName
				, Email
				, Phone
				, Country
				/*, string.Empty*/
				, Login
				, Password
				, ResellerGuid
				, null
				/*, true*/
				, Locale
				/*, TimeZoneId*/
				, referrer
				, out RequestId
				, out RequestGuid
				);
		}
		#endregion

		#region Activate
		public static string Activate(int RequestId, string RequestGuid, out string login, out string password)
		{
			string ret;

			WebTrial.localhost.Trial proxy = GetProxy();
			proxy.ActivateTrialCompany(RequestId, RequestGuid, out ret, out login, out password);

			return ret;
		}
		#endregion

		#region DomainExists
		public static bool DomainExists(string domain)
		{
			WebTrial.localhost.Trial proxy = GetProxy();
			return proxy.DomainExists(domain);
		}
		#endregion

		#region GetTrialPeriod()
		public static int GetTrialPeriod()
		{
			WebTrial.localhost.Trial proxy = GetProxy();
			return proxy.GetTrialPeriod();
		}
		#endregion
		#region GetParentDomain()
		public static string GetParentDomain()
		{
			WebTrial.localhost.Trial proxy = GetProxy();
			return proxy.GetParentDomain();
		}
		#endregion
		#region GetTimeZones()
		public static DataTable GetTimeZones(string locale)
		{
			WebTrial.localhost.Trial proxy = GetProxy();
			string xml = string.Empty; //proxy.GetTimeZones(locale);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			
			string[] columns = new string[]{"TimeZoneId", "DisplayName", "Bias"};

			DataTable dt = new DataTable("TimeZones");

			dt.Columns.Add(columns[0], typeof(int));
			dt.Columns.Add(columns[1], typeof(string));
			dt.Columns.Add(columns[2], typeof(int));

			foreach(XmlNode nd in doc.SelectNodes("/dictionary/item"))
			{
				DataRow row = dt.NewRow();
				foreach(string columnName in columns)
					row[columnName] = nd.Attributes[columnName].InnerText;
				dt.Rows.Add(row);
			}
			return dt;

		}
		#endregion
	}
}
