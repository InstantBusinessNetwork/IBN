using System;
using System.Collections;
using System.Data;
using System.Web;

using Mediachase.Ibn;
using Mediachase.Ibn.Data;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	#region public enum SqlServerEdition
	public enum SqlServerEdition
	{
		Unknown,
		PersonalOrDesktopEngine,
		Standard,
		EnterpriseOrDeveloper,
		Express
	}
	#endregion

	public enum DatabaseState
	{
		Undefined = 0,
		Locked = 1,
		Initialize = 2,
		Initializing = 3,
		Update = 4,
		Updating = 5,
		Ready = 6
	}

	#region class IbnUser
	class IbnUser
	{
		public int UserId;
		public bool IsAdmin = false;
	}
	#endregion

	#region class Configuration
	public class Configuration
	{
		private const string ConstInstallDir = "INSTALLDIR";
		private const string ConstDisableCustomSqlReport = "DisableCustomSqlReport";

		private bool _initialized;
		private string _serverLink;
		private string _portalLink;
		private string _host;
		private byte _companyType;
		private DateTime _endDate;
		private SqlServerEdition _sqlServerEdition;
		private string _defaultLocale;

		private static System.Timers.Timer _timer;
		private static int _interval;

		[ThreadStatic]
		private static Configuration _current;
		private static string _typeName = typeof(Configuration).FullName;
		#region Current
		public static Configuration Current
		{
			get
			{
				Configuration ret;

				if (HttpContext.Current != null)
					ret = HttpContext.Current.Items[_typeName] as Configuration;
				else
					ret = _current;

				if (ret == null)
				{
					ret = new Configuration();
					Current = ret;
				}

				return ret;
			}
			set
			{
				if (HttpContext.Current != null)
					HttpContext.Current.Items[_typeName] = value;
				else
					_current = value;
			}
		}
		#endregion

		static Configuration()
		{
			if (HttpRuntime.AppDomainAppId != null)
			{
				_interval = 600000;
				_timer = new System.Timers.Timer();
				_timer.AutoReset = false;
				_timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerElapsed);
				SetTimer();
			}
		}

		#region Unload
		public static void Uninitialize()
		{
			// OZ Relese Company Web Site Semaphore
			CompanyWebSiteSemaphore.Uninitialize();
		}

		#endregion

		#region public static void Init()
		public static void Init()
		{
			Configuration config = Configuration.Current;

			if (config._initialized)
			{
				PortalConfig.Current.Validate();
				DataContext.Current = new DataContext(DbContext.Current.PortalConnectionString);
			}
			else
			{
				config.Init3();
			}
		}
		#endregion
		#region public static void Init2()
		public static void Init2()
		{
			Configuration.Current.Init3();
		}
		#endregion

		#region private void Init3()
		private void Init3()
		{
			// OZ Initialize Company Web Site Semaphore
			CompanyWebSiteSemaphore.Initialize();

			DbHelper2.Init2();

			ValidateSqlServerVesion();
			_sqlServerEdition = (SqlServerEdition)DBCommon.GetSqlServerEdition();

			DatabaseState state = (DatabaseState)DBCommon.GetDatabaseState();
			if (state != DatabaseState.Ready)
				throw new DatabaseStateException(state);

			Alerts2.Init();
			PortalConfig.Current.Init();

			_host = PortalConfig.SystemHost;
			string scheme = PortalConfig.SystemScheme;
			string port = PortalConfig.SystemPort;
			_companyType = PortalConfig.CompanyType;

			if (PortalConfig.CompanyEndDate.HasValue)
				_endDate = PortalConfig.CompanyEndDate.Value;
			else
				_endDate = DateTime.MaxValue;

			_portalLink = string.Format("{0}://{1}{2}", scheme, _host, !string.IsNullOrEmpty(port) ? (":" + port) : "");
			_serverLink = _portalLink + "/";

			_defaultLocale = DBCommon.GetDefaultLanguageName();

			
			_initialized = true;
		}
		#endregion

		public static string ServerLink { get { return Current._serverLink; } }
		public static string PortalLink { get { return Current._portalLink; } }
		public static string Domain { get { return Current._host; } }
		public static int CompanyType { get { return (int)Current._companyType; } }
		public static SqlServerEdition SqlEdition { get { return Current._sqlServerEdition; } }
		public static bool IsASP { get { return DisableCustomSqlReport; } }
		internal static DateTime EndDate { get { return Current._companyType == 2 ? Current._endDate : DateTime.MaxValue; } }
		public static string DefaultLocale { get { return Current._defaultLocale; } }
		public static bool DisableCustomSqlReport { get { return GetConfigurationString(ConstDisableCustomSqlReport) == "True"; } }

		#region GetToolsDir()
		public static string GetToolsDir()
		{
			return string.Format("{0}Tools\\", GetConfigurationString(ConstInstallDir));
		}
		#endregion

		#region GetConfigurationString()
		private static string GetConfigurationString(string name)
		{
			return RegistrySettings.ReadString(name);
		}
		#endregion

		#region SetTimer
		static void SetTimer()
		{
			_timer.Interval = _interval;
			_timer.Start();
		}
		#endregion

		#region OnTimerElapsed
		static void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				Init2();

				int count = User.GetRemainingActiveUsers(false);
				if (count < 0)
					DeactivateUsers(false, -count);

				count = User.GetRemainingActiveUsers(true);
				if (count < 0)
					DeactivateUsers(true, -count);
			}
			catch { }

			SetTimer();
		}
		#endregion

		#region DeactivateUsers
		static void DeactivateUsers(bool External, int ExcessCount)
		{
			ArrayList users = GetActiveUsers(External);
			IbnUser user;

			// Shuffle users
			int curCount = users.Count;
			System.Random rnd = new System.Random();
			object o;
			for (int i = 0; i < curCount; i++)
			{
				int j = rnd.Next(curCount);
				// Swap i-th and j-th users
				o = users[i];
				users[i] = users[j];
				users[j] = o;
			}

			// Leave at least one admin
			if (!External)
			{
				for (int i = 0; i < users.Count; i++)
				{
					user = (IbnUser)users[i];
					if (user.IsAdmin)
					{
						users.RemoveAt(i);
						break;
					}
				}
			}

			// Calculate number of users to deactivate
			int n = Math.Min(ExcessCount, users.Count);

			// Deactivate first n users
			for (int i = 0; i < n; i++)
			{
				user = (IbnUser)users[i];
				User.UpdateActivity(user.UserId, false);
			}
		}
		#endregion

		#region GetActiveUsers
		static ArrayList GetActiveUsers(bool External)
		{
			ArrayList users = new ArrayList();

			using (IDataReader r = User.GetListUsersBySubstring(String.Empty))
			{
				while (r.Read())
				{
					bool IsPending = (bool)r["IsPending"];
					if (!IsPending)
					{
						bool IsExternal = (bool)r["IsExternal"];
						if (IsExternal == External)
						{
							IbnUser user = new IbnUser();
							user.UserId = (int)r["UserId"];

							if (!IsExternal)
								user.IsAdmin = Security.IsUserInGroup(user.UserId, InternalSecureGroups.Administrator);
							users.Add(user);
						}
					}
				}
			}
			return users;
		}
		#endregion

		#region private static void ValidateSqlServerVesion()
		private static void ValidateSqlServerVesion()
		{
			// SQL Server 2005: 9.00.1399.06
			// SQL Server 2000 SP4: 8.00.2039
			// SQL Server 2000 SP3: 8.00.760

			bool success = false;
			string productVesion = DBCommon.GetSqlServerVersion();

			try
			{
				int firstDotPos = productVesion.IndexOf(".");
				int lastDotPos = productVesion.LastIndexOf(".");
				int majorVersion = int.Parse(productVesion.Substring(0, firstDotPos));
				int build = int.Parse(productVesion.Substring(lastDotPos + 1));
				if (majorVersion > 8 || (majorVersion == 8 && build >= 760))
					success = true;
			}
			catch
			{
			}

			if (!success)
				throw new UnsupportedSqlServerVersionException();
		}
		#endregion

		#region LicenseExpired
		public static bool LicenseExpired
		{
			get
			{
				return License.Expired;
			}
		}
		#endregion

		#region HelpDeskEnabled
		public static bool HelpDeskEnabled
		{
			get
			{
				return License.HelpDesk;
			}
		}
		#endregion
		#region ProjectManagementEnabled
		public static bool ProjectManagementEnabled
		{
			get
			{
				return License.ProjectManagement;
			}
		}
		#endregion
		#region TimeTrackingModule
		public static bool TimeTrackingModule
		{
			get
			{
				return License.TimeTrackingModule;
			}
		}
		#endregion
		#region TimeTrackingCustomization
		public static bool TimeTrackingCustomization
		{
			get
			{
				return License.TimeTrackingCustomization;
			}
		}
		#endregion
		#region WorkflowModule
		public static bool WorkflowModule
		{
			get
			{
				return License.WorkflowModule;
			}
		}
		#endregion
	}
	#endregion
}
