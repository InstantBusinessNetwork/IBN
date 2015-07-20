using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Xml;

namespace Mediachase.Ibn
{
	internal class License
	{
		[ThreadStatic]
		private static License _License;

		private DateTime _LoadedDate;
		private bool _Expired;
		private DateTime _ExpirationDate = DateTime.MinValue;
		private NameValueCollection _Properties = new NameValueCollection();

		private int _ActiveUsersCount;
		private int _ExternalUsersCount;
		private int _PortalsCount;
		private bool _MailIssuesEnabled;
		private bool _PartnersEnabled;
		private int _PortfolioCount;
		private bool _MSProjectIntegration;
		private bool _MSProjectSynchronization;
		private bool _WebGanttChart;
		private bool _ActiveDirectoryImport;
		private bool _RealTimeMonitoring;
		private bool _ProjectManagement;
		private bool _HelpDesk;
		private bool _TimeTrackingModule;
		private bool _TimeTrackingCustomization;
		private DateTime _UpdatesExpirationDate;
		private bool _WorkflowModule;

		internal static bool Expired{ get{ return CurrentLicense._Expired; } }
		internal static DateTime ExpirationDate{ get{ return CurrentLicense._ExpirationDate; } }
		internal static NameValueCollection Properties{ get{ return new NameValueCollection(CurrentLicense._Properties); } }

		internal static int ActiveUsersCount{ get{ return CurrentLicense._ActiveUsersCount; } }
		internal static int ExternalUsersCount{ get{ return CurrentLicense._ExternalUsersCount; } }
		internal static int PortalsCount{ get{ return CurrentLicense._PortalsCount; } }
		internal static bool MailIssuesEnabled{ get{ return CurrentLicense._MailIssuesEnabled; } }
		internal static bool PartnersEnabled{ get{ return CurrentLicense._PartnersEnabled; } }
		internal static int PortfolioCount{ get{ return CurrentLicense._PortfolioCount; } }
		internal static bool MsProjectIntegration{ get{ return CurrentLicense._MSProjectIntegration; } }
		internal static bool MSProjectSynchronization { get { return CurrentLicense._MSProjectSynchronization; } }
		internal static bool WebGanttChart { get { return CurrentLicense._WebGanttChart; } }
		internal static bool ActiveDirectoryImport{ get{ return CurrentLicense._ActiveDirectoryImport; } }
		internal static bool RealTimeMonitoring{ get{ return CurrentLicense._RealTimeMonitoring; } }
		internal static bool ProjectManagement{ get{ return CurrentLicense._ProjectManagement; } }
		internal static bool HelpDesk{ get{ return CurrentLicense._HelpDesk; } }
		internal static bool TimeTrackingModule { get { return CurrentLicense._TimeTrackingModule; } }
		internal static bool TimeTrackingCustomization { get { return CurrentLicense._TimeTrackingCustomization; } }
		internal static DateTime UpdatesExpirationDate { get { return CurrentLicense._UpdatesExpirationDate; } }
		internal static bool WorkflowModule { get { return CurrentLicense._WorkflowModule; } }

		private License()
		{
			try
			{
				UInt32 errSize1=0, errData1=0, errSize2=0, errData2=0;
				string licenseFile = string.Format(CultureInfo.InvariantCulture, "{0}bin\\{1}.lic", AppDomain.CurrentDomain.BaseDirectory, IbnConst.ProductGuid);
				string data = GetLicenseData(licenseFile, IbnConst.ProductGuid, AppDomain.CurrentDomain.FriendlyName + WindowsIdentity.GetCurrent().Name, out errSize1, out errData1, out _Expired, out _ExpirationDate);
				if(data == null)
					data = GetLicenseData(null, IbnConst.ProductGuid, AppDomain.CurrentDomain.FriendlyName + WindowsIdentity.GetCurrent().Name, out errSize2, out errData2, out _Expired, out _ExpirationDate);
				if(data != null)
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(data);

					_Properties.Add("LicenseName", doc.SelectSingleNode("/license/name").InnerXml);
					_Properties.Add("ExpirationDate", (_ExpirationDate != DateTime.MinValue ? _ExpirationDate.ToString(CultureInfo.InvariantCulture) : string.Empty));
					_Properties.Add("ClientName", doc.SelectSingleNode("/license/clientinformation/name").InnerXml);
					_Properties.Add("ClientEmail", doc.SelectSingleNode("/license/clientinformation/email").InnerXml);

					foreach(XmlNode p in doc.SelectNodes("/license/params/param"))
					{
						XmlAttribute aName = p.Attributes["name"];
						if(aName != null)
							_Properties.Add(aName.Value, p.InnerXml);
					}

					_ActiveUsersCount = GetLicensePropertyInteger(doc, "ActiveUsersCount", 0);
					_ExternalUsersCount = GetLicensePropertyInteger(doc, "ExternalUsersCount", 0);
					_PortalsCount = GetLicensePropertyInteger(doc, "PortalsCount", 0);
					_MailIssuesEnabled = GetLicensePropertyBoolean(doc, "MailIssuesEnabled", false);
					_PartnersEnabled = GetLicensePropertyBoolean(doc, "PartnersEnabled", false);
					_PortfolioCount = GetLicensePropertyInteger(doc, "PortfolioCount", 0);
					_MSProjectIntegration = GetLicensePropertyBoolean(doc, "MsProjectIntegration", false);
					_MSProjectSynchronization = GetLicensePropertyBoolean(doc, "MSProjectSynchronization", false);
					_WebGanttChart = GetLicensePropertyBoolean(doc, "WebGanttChart", false);
					_ActiveDirectoryImport = GetLicensePropertyBoolean(doc, "ActiveDirectoryImport", false);
					_RealTimeMonitoring = GetLicensePropertyBoolean(doc, "RealTimeMonitoring", false);
					_ProjectManagement = GetLicensePropertyBoolean(doc, "ProjectManagement", false);
					_HelpDesk = GetLicensePropertyBoolean(doc, "HelpDesk", false);
					_TimeTrackingModule = GetLicensePropertyBoolean(doc, "TimeTrackingModule", false);
					_TimeTrackingCustomization = GetLicensePropertyBoolean(doc, "TimeTrackingCustomization", false);
					_UpdatesExpirationDate = GetLicensePropertyDateTime(doc, "UpdatesExpirationDate", _ExpirationDate);
					_WorkflowModule = GetLicensePropertyBoolean(doc, "WorkflowModule", false);
				}
				else
				{
					Log.WriteWarning(string.Format(CultureInfo.InvariantCulture, "Lisense data is null.\nErrSize1 = {0}\nErrData1 = {1}\nErrSize2 = {2}\nErrData2 = {3}", errSize1, errData1, errSize2, errData2));
					_Properties.Add("Warning", "Lisense data is null.");
					_Properties.Add("ErrorSize1", errSize1.ToString(CultureInfo.InvariantCulture));
					_Properties.Add("ErrorData1", errData1.ToString(CultureInfo.InvariantCulture));
					_Properties.Add("ErrorSize2", errSize2.ToString(CultureInfo.InvariantCulture));
					_Properties.Add("ErrorData2", errData2.ToString(CultureInfo.InvariantCulture));
				}
			}
			catch(Exception ex)
			{
				Log.WriteError(ex.ToString());
				_Properties.Clear();
				_Properties.Add("Exception", ex.ToString());
			}
			_LoadedDate = DateTime.UtcNow;
		}

		private DateTime GetLicensePropertyDateTime(XmlDocument doc, string name, DateTime defaultValue)
		{
			DateTime value = defaultValue;

			string stringValue = GetLicensePropertyString(doc, name);
			if (!string.IsNullOrEmpty(stringValue))
				value = DateTime.Parse(stringValue, CultureInfo.InvariantCulture);

			return value;
		}

		private static bool GetLicensePropertyBoolean(XmlDocument doc, string name, bool defaultValue)
		{
			bool value = defaultValue;

			string stringValue = GetLicensePropertyString(doc, name);
			if (!string.IsNullOrEmpty(stringValue))
				value = bool.Parse(stringValue);

			return value;
		}

		private static int GetLicensePropertyInteger(XmlDocument doc, string name, int defaultValue)
		{
			int value = defaultValue;

			string stringValue = GetLicensePropertyString(doc, name);
			if (!string.IsNullOrEmpty(stringValue))
				value = int.Parse(stringValue, CultureInfo.InvariantCulture);

			return value;
		}

		private static string GetLicensePropertyString(XmlDocument doc, string name)
		{
			string value = null;

			XmlNode node = doc.SelectSingleNode("/license/params/param[@name='"+name+"']");
			if (node != null)
			{
				value = node.InnerXml;
			}

			return value;
		}

		private static string GetLicenseData(string LicenseFile, string ProductGuid, string CallerName, out UInt32 errSize, out UInt32 errData, out bool Expired, out DateTime ExpirationDate)
		{
			string ret = null;
			Expired = false;
			ExpirationDate = DateTime.MinValue; // Unknown

			UInt32 size, expirationDate;
			errSize = ExtGetLicenseDataSize(LicenseFile, ProductGuid, out size);
			if(0 == errSize)
			{
				byte[] data = new byte[size];
				errData = ExtGetLicenseData(LicenseFile, ProductGuid, CallerName, data, ref size, out expirationDate);
				if(errData == 0)
					ret = Encoding.UTF8.GetString(data);
				else if(errData == 0x800900ff)
					Expired = true;

				if(expirationDate == 0)
					ExpirationDate = DateTime.MinValue; // Unknown
				else if(expirationDate == UInt32.MaxValue)
					ExpirationDate = DateTime.MaxValue; // Infinite
				else
					ExpirationDate = new DateTime(Convert.ToInt32((expirationDate&0xFFFFFE00)>>9), Convert.ToInt32((expirationDate&0x1E0)>>5), Convert.ToInt32(expirationDate&0x1F));
			}
			else
				errData = 0;

			return ret;
		}

		private static UInt32 ExtGetLicenseDataSize(
			string szLicenseFile
			, string szProductGuid
			, out UInt32 pcbLicenseData
			)
		{
			if (IntPtr.Size == 8)
				return NativeMethods.GetLicenseDataSize64(szLicenseFile, szProductGuid, out pcbLicenseData);
			else
				return NativeMethods.GetLicenseDataSize(szLicenseFile, szProductGuid, out pcbLicenseData);
		}
		private static UInt32 ExtGetLicenseData(
			string szLicenseFile
			, string szProductGuid
			, string szContainer
			, byte[] pLicenseData
			, ref UInt32 pcbLicenseData
			, out UInt32 pExpirationDate
			)
		{
			if (IntPtr.Size == 8)
				return NativeMethods.GetLicenseData64(szLicenseFile, szProductGuid, szContainer, pLicenseData, ref pcbLicenseData, out pExpirationDate);
			else
				return NativeMethods.GetLicenseData(szLicenseFile, szProductGuid, szContainer, pLicenseData, ref pcbLicenseData, out pExpirationDate);
		}

		private static License CurrentLicense
		{
			get
			{
				if(_License == null)
					_License = new License();
				else
				{
					TimeSpan span = DateTime.UtcNow - _License._LoadedDate;
					if(span.TotalHours > 1)
						_License = new License();
				}
				if(_License._Expired)
					throw new LicenseExpiredException();
				return _License;
			}
		}
	}

	internal static class NativeMethods
	{
		[DllImport("McLicenseVerify2.dll", EntryPoint = "GetLicenseDataSize", SetLastError = true,
			 CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern UInt32 GetLicenseDataSize(
			string szLicenseFile,
			string szProductGuid,
			out UInt32 pcbLicenseData);

		[DllImport("McLicenseVerify2.dll", EntryPoint = "GetLicenseData2", SetLastError = true,
			 CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern UInt32 GetLicenseData(
			string szLicenseFile,
			string szProductGuid,
			string szContainer,
			byte[] pLicenseData,
			ref UInt32 pcbLicenseData,
			out UInt32 pExpirationDate
			);

		[DllImport("McLicenseVerify64.dll", EntryPoint = "GetLicenseDataSize", SetLastError = true,
			 CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern UInt32 GetLicenseDataSize64(
			string szLicenseFile,
			string szProductGuid,
			out UInt32 pcbLicenseData);

		[DllImport("McLicenseVerify64.dll", EntryPoint = "GetLicenseData2", SetLastError = true,
			 CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern UInt32 GetLicenseData64(
			string szLicenseFile,
			string szProductGuid,
			string szContainer,
			byte[] pLicenseData,
			ref UInt32 pcbLicenseData,
			out UInt32 pExpirationDate
			);
	}
}
