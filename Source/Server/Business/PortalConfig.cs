using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Web;

using Mediachase.Ibn;
using Mediachase.IBN.Business.WebDAV.Common;
using Mediachase.IBN.Database;


namespace Mediachase.IBN.Business
{
	public class PortalConfig
	{
		#region Keys
		public const string keyAntiSpamFilter = "email.routing.antispamfilter";
		public const string keyAutoFillWhiteList = "email.routing.autofillwhitelist";
		public const string keyAutoFillBlackList = "email.routing.autofillblacklist";

		public const string keyWebDav = "filelibrary.webdav";
		public const string keyFullTextSearch = "filelibrary.fulltextsearch";

		public const string keyWorkTimeStart = "admin.worktime.start";
		public const string keyWorkTimeFinish = "admin.worktime.finish";
		public const string keyDefaultCalendar = "admin.defaultcalendar";

		//public const string keySmtpCheckUid = "email.smtp.checkuid";
		//public const string keySmtpChecked = "email.smtp.checked";
		//public const string keySmtpDefault = "email.smtp.default";
		//public const string keySmtpAllowOverride = "email.smtp.allowoverride"; 

		//public const string keySmtpServerName = "email.smtp.servername";
		//public const string keySmtpServerPort = "email.smtp.serverport";
		//public const string keySmtpSecure = "email.smtp.secure";
		//public const string keySmtpAuthenticate = "email.smtp.authenticate";
		//public const string keySmtpUser = "email.smtp.user";
		//public const string keySmtpPassword = "email.smtp.password";

		public const string keyUseWinLogin = "portal.usewinlogin";
		public const string keyUseIM = "portal.useim";

		public const string keyObjectOrder = "admin.objectorder";

		public const string keyOnlyOldListViews = "admin.listviews.onlyold";

		public const string keyWebDavSessionLifeTime = "filelibrary.webdav.lifetime";

		public const string keyAuditWebLogin = "audit.weblogin";
		public const string keyAuditIbnClientLogin = "audit.ibnclientlogin";

		public const string keySystemIsActive = "system.isactive";
		public const string keySystemScheme = "system.scheme";
		public const string keySystemHost = "system.host";
		public const string keySystemPort = "system.port";

		public const string keyCompanyMaxUsers = "company.max_users";
		public const string keyCompanyMaxExternalUsers = "company.max_external_users";
		public const string keyCompanyDabaseSize = "company.databasesize";
		public const string keyCompanyType = "company.type";
		public const string keyCompanyEndDate = "company.end_date";

		public const string keyPortalSupportName = "portal.support_name";
		public const string keyPortalSupportEmail = "portal.support_email";
		public const string keyPortalCompanyLogo = "portal.company_logo";
		public const string keyPortalCompanyLogoVersion = "portal.company_logo_version";
		public const string keyPortalEnableAlerts = "portal.enable_alerts";
		public const string keyPortalShowAdminWizard = "portal.show_admin_wizard";
		public const string keyPortalFirstDayOfWeek = "portal.firstdayofweek";
		public const string keyPortalLogUserStatus = "portal.log_user_status";
		public const string keyPortalDisableScheduleService = "portal.disablescheduleservice";
		public const string keyPortalAlertSubjectFormat = "portal.alert_subject_format";

		public const string keyPortalHomepageTitle1 = "portal.homepage.title1";
		public const string keyPortalHomepageTitle2 = "portal.homepage.title2";
		public const string keyPortalHomepageText1 = "portal.homepage.text1";
		public const string keyPortalHomepageText2 = "portal.homepage.text2";
		public const string keyPortalHomepageImage = "portal.homepage.image";

		public const string keyMessageDeliverySystemMaxDeliveryAttempts = "messagedeliverysystem.maxdeliveryattempts";
		public const string keyMdsDeliveryTimeout = "messagedeliverysystem.deliverytimeout";
		public const string keyMdsDeleteOlderMoreThan = "messagedeliverysystem.deleteoldermorethan";

		public const string keyIsListRssEnabled = "IsListRssEnabled";

		public const string keySmtpRequestTimeout = "system.smtp.requesttimeout";

		public const string keyShortInfoDescriptionLength = "portal.shortinfodescriptionlength";
		#endregion

		private static Guid _globalSettingsVersionId;

		private Guid _threadSettingsVersionId;
		private StringDictionary _innerDictionary = null;

		[ThreadStatic]
		private static PortalConfig _current;
		private static string _typeName = typeof(PortalConfig).FullName;
		#region Current
		public static PortalConfig Current
		{
			get
			{
				PortalConfig ret;

				if (HttpContext.Current != null)
					ret = HttpContext.Current.Items[_typeName] as PortalConfig;
				else
					ret = _current;

				if (ret == null)
				{
					ret = new PortalConfig();
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

		#region Commom Methods

		static PortalConfig()
		{
			_globalSettingsVersionId = Guid.NewGuid();
		}

		internal void Validate()
		{
			if (_globalSettingsVersionId != _threadSettingsVersionId)
				Init();
		}

		internal void Init()
		{
			_innerDictionary = new StringDictionary();

			foreach (PortalConfigRow row in PortalConfigRow.List())
			{
				_innerDictionary.Add(row.Key, row.Value);
			}

			_threadSettingsVersionId = _globalSettingsVersionId;
		}

		#region GetValue
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <returns></returns>
		public static string GetValue(string key)
		{
			return GetValue(key, null);
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="DefaultValue">The default value.</param>
		/// <returns></returns>
		public static string GetValue(string key, string defaultValue)
		{
			PortalConfig config = Current;

			if (config._innerDictionary != null && config._innerDictionary.ContainsKey(key))
				return config._innerDictionary[key];
			else
				return defaultValue;
		}
		#endregion

		#region SetValue
		public static void SetValue<T>(string key, T value)
		{
			if (value == null)
				SetValue(key, null);
			else
				SetValue(key, value.ToString());
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="Key">The key.</param>
		/// <param name="Value">The value.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void SetValue(string key, string value)
		{
			PortalConfig config = Current;

			_globalSettingsVersionId = Guid.NewGuid();
			config._threadSettingsVersionId = _globalSettingsVersionId;

			PortalConfigRow row = PortalConfigRow.Get(key);

			if (value != null)
			{
				config._innerDictionary[key] = value;

				if (row == null)
					row = new PortalConfigRow();

				row.Key = key;
				row.Value = value;

				row.Update();
			}
			else
			{
				config._innerDictionary.Remove(key);

				if (row != null)
					row.Delete();
			}
		}
		#endregion
		#endregion

		#region EnableAlerts
		public static bool EnableAlerts
		{
			get
			{
				return bool.Parse(GetValue(keyPortalEnableAlerts, bool.TrueString));
			}
			set
			{
				SetValue(keyPortalEnableAlerts, value);
			}
		}
		#endregion

		#region UseIM
		public static bool UseIM
		{
			get
			{
				return bool.Parse(GetValue(keyUseIM, bool.TrueString));
			}
			set
			{
				SetValue(keyUseIM, value);
			}
		}
		#endregion

		#region AlertSubjectFormat
		/// <summary>
		/// Gets or sets the alert subject format.
		/// </summary>
		/// <value>The alert subject format.</value>
		/// <remarks>Default value is "IBN 4.7 - [=EventSubject=]".</remarks>
		public static string AlertSubjectFormat
		{
			get
			{
				string value = GetValue(keyPortalAlertSubjectFormat);

				if (string.IsNullOrEmpty(value))
					value = IbnConst.ProductFamilyShort + " " + IbnConst.VersionMajorDotMinor + " - [=EventSubject=]";

				return value;
			}
			set
			{
				SetValue(keyPortalAlertSubjectFormat, value);
			}
		}
		#endregion

		#region WebDav Settings
		/// <summary>
		/// Gets or sets a value indicating whether [use web dav].
		/// </summary>
		/// <value><c>true</c> if [use web dav]; otherwise, <c>false</c>.</value>
		public static Nullable<bool> UseWebDav
		{
			get
			{
				string strValue = GetValue(keyWebDav);
				if (strValue == null)
					return null;

				return bool.Parse(strValue);
			}
			set
			{
				SetValue(keyWebDav, value);
			}
		}
		#endregion

		#region FullTextSearch Settings
		/// <summary>
		/// Gets a value indicating whether this instance is full text search service installed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is full text search service installed; otherwise, <c>false</c>.
		/// </value>
		public static bool IsFullTextSearchServiceInstalled
		{
			get
			{
				return FullTextSearch.IsInstalled();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [use full text search].
		/// </summary>
		/// <value><c>true</c> if [use full text search]; otherwise, <c>false</c>.</value>
		public static Nullable<bool> UseFullTextSearch
		{
			get
			{
				if (FullTextSearch.IsActive())
					return true;

				string strValue = GetValue(keyFullTextSearch);
				if (strValue == null)
					return null;

				return bool.Parse(strValue);
			}
			set
			{
				if (value.HasValue)
				{
					if (value.Value)
						FullTextSearch.Activate();
					else
						FullTextSearch.Deactivate();
				}

				SetValue(keyFullTextSearch, value);
			}
		}
		#endregion

		#region EMailRouter Settings
		/// <summary>
		/// Gets a value indicating whether [use anti spam filter].
		/// </summary>
		/// <value><c>true</c> if [use anti spam filter]; otherwise, <c>false</c>.</value>
		public static bool UseAntiSpamFilter
		{
			get
			{
				return bool.Parse(GetValue(keyAntiSpamFilter, bool.FalseString));
			}
			set
			{
				SetValue(keyAntiSpamFilter, value);

				if (!value)
				{
					EMail.EMailMessage.ApprovePending();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [auto fill white list].
		/// </summary>
		/// <value><c>true</c> if [auto fill white list]; otherwise, <c>false</c>.</value>
		public static bool AutoFillWhiteList
		{
			get
			{
				return bool.Parse(GetValue(keyAutoFillWhiteList, bool.TrueString));
			}
			set
			{
				SetValue(keyAutoFillWhiteList, value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [auto fill black list].
		/// </summary>
		/// <value><c>true</c> if [auto fill black list]; otherwise, <c>false</c>.</value>
		public static bool AutoFillBlackList
		{
			get
			{
				return bool.Parse(GetValue(keyAutoFillBlackList, bool.TrueString));
			}
			set
			{
				SetValue(keyAutoFillBlackList, value);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has external E mail box.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has external E mail box; otherwise, <c>false</c>.
		/// </value>
		public static bool HasExternalEMailBox
		{
			get
			{
				return EMail.EMailRouterPop3Box.ListExternal().Length > 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has internal E mail box.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has internal E mail box; otherwise, <c>false</c>.
		/// </value>
		public static bool HasInternalEMailBox
		{
			get
			{
				return EMail.EMailRouterPop3Box.ListInternal() != null;
			}
		}
		#endregion

		#region UseWinLogin
		/// <summary>
		/// Gets or sets a value indicating whether [use win login].
		/// </summary>
		/// <value><c>true</c> if [use win login]; otherwise, <c>false</c>.</value>
		public static bool UseWinLogin
		{
			get
			{
				return bool.Parse(GetValue(keyUseWinLogin, bool.FalseString));
			}
			set
			{
				SetValue(keyUseWinLogin, value);
			}
		}

		#endregion

		#region WorkTime Settings
		/// <summary>
		/// Gets or sets the work time start.
		/// </summary>
		/// <value>The work time start.</value>
		public static string WorkTimeStart
		{
			get
			{
				string strValue = GetValue(keyWorkTimeStart);
				if (strValue == null)
					return "09:00";

				return strValue;
			}
			set
			{
				SetValue(keyWorkTimeStart, value);
			}
		}

		/// <summary>
		/// Gets or sets the work time finish.
		/// </summary>
		/// <value>The work time finish.</value>
		public static string WorkTimeFinish
		{
			get
			{
				string strValue = GetValue(keyWorkTimeFinish);
				if (strValue == null)
					return "18:00";

				return strValue;
			}
			set
			{
				SetValue(keyWorkTimeFinish, value);
			}
		}
		#endregion

		#region WebDav Session Settings
		/// <summary>
		/// Gets or sets the work time start.
		/// </summary>
		/// <value>The work time start.</value>
		public static string WebDavSessionLifeTime
		{
			get
			{
				string strValue = GetValue(keyWebDavSessionLifeTime);
				if (strValue == null)
					return ConfigurationManager.AppSettings[WebDavAuthHelper.SETTING_SESSION_LIFE_TIME];

				return strValue;
			}
			set
			{
				SetValue(keyWebDavSessionLifeTime, value);
			}
		}
		#endregion

		#region ObjectOrder Settings
		public static string ObjectOrder
		{
			get
			{
				string strValue = GetValue(keyObjectOrder);
				if (strValue == null)
					return "5,6,7,16";	// Task, Todo, Incident, Document

				return strValue;
			}
			set
			{
				SetValue(keyObjectOrder, value);
			}
		}
		#endregion

		#region Audit settings
		#region AuditWebLogin
		public static bool AuditWebLogin
		{
			get
			{
				string value = GetValue(keyAuditWebLogin);
				if (!string.IsNullOrEmpty(value))
					return bool.Parse(value);
				else
					return false;
			}
			set
			{
				if (value)
					SetValue(keyAuditWebLogin, true);
				else
					SetValue(keyAuditWebLogin, null);
			}
		}
		#endregion

		#region AuditIbnClientLogin
		public static bool AuditIbnClientLogin
		{
			get
			{
				string value = GetValue(keyAuditIbnClientLogin);
				if (!string.IsNullOrEmpty(value))
					return bool.Parse(value);
				else
					return false;
			}
			set
			{
				if (value)
					SetValue(keyAuditIbnClientLogin, true);
				else
					SetValue(keyAuditIbnClientLogin, null);
			}
		}
		#endregion
		#endregion

		#region System settings

		#region SystemIsActive
		public static bool SystemIsActive
		{
			get
			{
				string value = GetValue(keySystemIsActive);
				if (!string.IsNullOrEmpty(value))
					return bool.Parse(value);
				else
					return false;
			}
		}
		#endregion

		#region SystemScheme
		public static string SystemScheme
		{
			get
			{
				return GetValue(keySystemScheme);
			}
		}
		#endregion
		#region SystemHost
		public static string SystemHost
		{
			get
			{
				return GetValue(keySystemHost);
			}
		}
		#endregion
		#region SystemPort
		public static string SystemPort
		{
			get
			{
				return GetValue(keySystemPort);
			}
		}
		#endregion

		#endregion

		#region Company settings
		#region CompanyMaxUsers
		public static int CompanyMaxUsers
		{
			get
			{
				string value = GetValue(keyCompanyMaxUsers);
				if (!string.IsNullOrEmpty(value))
					return int.Parse(value);
				else
					return -1;
			}
		}
		#endregion

		#region CompanyMaxExternalUsers
		public static int CompanyMaxExternalUsers
		{
			get
			{
				string value = GetValue(keyCompanyMaxExternalUsers);
				if (!string.IsNullOrEmpty(value))
					return int.Parse(value);
				else
					return -1;
			}
		}
		#endregion

		#region DatabaseSize
		public static int DatabaseSize
		{
			get
			{
				string value = GetValue(keyCompanyDabaseSize);
				if (!string.IsNullOrEmpty(value))
					return int.Parse(value);
				else
					return -1;
			}
		}
		#endregion

		#region CompanyType
		public static byte CompanyType
		{
			get
			{
				string value = GetValue(keyCompanyType);
				if (!string.IsNullOrEmpty(value))
					return byte.Parse(value);
				else
					return 1;
			}
		}
		#endregion

		#region CompanyEndDate
		public static DateTime? CompanyEndDate
		{
			get
			{
				string value = GetValue(keyCompanyEndDate);
				if (!string.IsNullOrEmpty(value))
					return DateTime.Parse(value, CultureInfo.InvariantCulture);
				else
					return null;
			}
		}
		#endregion
		#endregion

		#region * Portal settings *
		#region PortalSupportName
		public static string PortalSupportName
		{
			get
			{
				return GetValue(keyPortalSupportName, "");
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
					SetValue(keyPortalSupportName, value);
				else
					SetValue(keyPortalSupportName, null);
			}
		}
		#endregion

		#region PortalSupportEmail
		public static string PortalSupportEmail
		{
			get
			{
				return GetValue(keyPortalSupportEmail, "");
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
					SetValue(keyPortalSupportEmail, value);
				else
					SetValue(keyPortalSupportEmail, null);
			}
		}
		#endregion

		#region PortalCompanyLogo
		public static byte[] PortalCompanyLogo
		{
			get
			{
				byte[] retval = null;
				string value = GetValue(keyPortalCompanyLogo);
				if (!string.IsNullOrEmpty(value))
					retval = Convert.FromBase64String(value);
				return retval;
			}
			set
			{
				string newValue = null;
				if (value != null)
					newValue = Convert.ToBase64String(value);

				SetValue(keyPortalCompanyLogo, newValue);
				PortalCompanyLogoVersion = Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
			}
		}
		#endregion

		#region PortalCompanyLogoVersion
		public static string PortalCompanyLogoVersion
		{
			get
			{
				return GetValue(keyPortalCompanyLogoVersion);
			}
			set
			{
				SetValue(keyPortalCompanyLogoVersion, value);
			}
		}
		#endregion

		#region PortalHomepageTitle1
		public static string PortalHomepageTitle1
		{
			get
			{
				return GetValue(keyPortalHomepageTitle1, "");
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
					SetValue(keyPortalHomepageTitle1, value);
				else
					SetValue(keyPortalHomepageTitle1, null);
			}
		}
		#endregion

		#region PortalHomepageTitle2
		public static string PortalHomepageTitle2
		{
			get
			{
				return GetValue(keyPortalHomepageTitle2, "");
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
					SetValue(keyPortalHomepageTitle2, value);
				else
					SetValue(keyPortalHomepageTitle2, null);
			}
		}
		#endregion

		#region PortalHomepageText1
		public static string PortalHomepageText1
		{
			get
			{
				return GetValue(keyPortalHomepageText1, "");
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
					SetValue(keyPortalHomepageText1, value);
				else
					SetValue(keyPortalHomepageText1, null);
			}
		}
		#endregion

		#region PortalHomepageText2
		public static string PortalHomepageText2
		{
			get
			{
				return GetValue(keyPortalHomepageText2, "");
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
					SetValue(keyPortalHomepageText2, value);
				else
					SetValue(keyPortalHomepageText2, null);
			}
		}
		#endregion

		#region PortalHomepageImage
		public static byte[] PortalHomepageImage
		{
			get
			{
				byte[] retval = null;
				string value = GetValue(keyPortalHomepageImage);
				if (!string.IsNullOrEmpty(value))
					retval = Convert.FromBase64String(value);
				return retval;
			}
			set
			{
				string newValue = null;
				if (value != null)
					newValue = Convert.ToBase64String(value);

				SetValue(keyPortalHomepageImage, newValue);
			}
		}
		#endregion

		#region PortalShowAdminWizard
		public static bool PortalShowAdminWizard
		{
			get
			{
				string value = GetValue(keyPortalShowAdminWizard);
				if (!string.IsNullOrEmpty(value))
					return bool.Parse(value);
				else
					return false;
			}
			set
			{
				if (value)
					SetValue(keyPortalShowAdminWizard, true);
				else
					SetValue(keyPortalShowAdminWizard, null);
			}
		}
		#endregion

		#region PortalFirstDayOfWeek
		public static byte PortalFirstDayOfWeek
		{
			get
			{
				string value = GetValue(keyPortalFirstDayOfWeek);
				if (!string.IsNullOrEmpty(value))
					return byte.Parse(value);
				else
					return 1;
			}
			set
			{
				SetValue(keyPortalFirstDayOfWeek, value);
			}
		}
		#endregion

		#region PortalLogUserStatus
		public static bool PortalLogUserStatus
		{
			get
			{
				string value = GetValue(keyPortalLogUserStatus);
				if (!string.IsNullOrEmpty(value))
					return bool.Parse(value);
				else
					return false;
			}
			set
			{
				if (value)
					SetValue(keyPortalLogUserStatus, true);
				else
					SetValue(keyPortalLogUserStatus, null);
			}
		}
		#endregion

		#region PortalDisableScheduleService
		public static bool PortalDisableScheduleService
		{
			get
			{
				string value = GetValue(keyPortalDisableScheduleService);
				if (!string.IsNullOrEmpty(value))
					return bool.Parse(value);
				else
					return false;
			}
			set
			{
				if (value)
					SetValue(keyPortalDisableScheduleService, true);
				else
					SetValue(keyPortalDisableScheduleService, null);
			}
		}
		#endregion
		#endregion

		#region Message Delivery System
		/// <summary>
		/// Gets or sets the max delivery attempts.
		/// </summary>
		/// <value>The max delivery attempts.</value>
		/// <remarks>-1 - Unlimited</remarks>
		public static int MdsMaxDeliveryAttempts
		{
			get
			{
				string value = GetValue(keyMessageDeliverySystemMaxDeliveryAttempts);
				if (!string.IsNullOrEmpty(value))
					return int.Parse(value);
				else
					return -1;
			}
			set
			{
				SetValue(keyMessageDeliverySystemMaxDeliveryAttempts, value.ToString());
			}
		}

		/// <summary>
		/// Gets or sets the max delivery attempts. In minutes.
		/// </summary>
		/// <value>The max delivery attempts.</value>
		/// <remarks>In Minutes. -1 - Unlimited. Default Value is 2 days (2880 minutes).</remarks>
		public static int MdsDeliveryTimeout
		{
			get
			{
				string value = GetValue(keyMdsDeliveryTimeout);
				if (!string.IsNullOrEmpty(value))
					return int.Parse(value);
				else
					return 2*24*60;
			}
			set
			{
				SetValue(keyMdsDeliveryTimeout, value.ToString());
			}
		}

		/// <summary>
		/// Gets or sets the max delivery attempts. In minutes.
		/// </summary>
		/// <value>The max delivery attempts.</value>
		/// <remarks>In Minutes. -1 - No Delete. Default Value is 7 days (10080 minutes).</remarks>
		public static int MdsDeleteOlderMoreThan
		{
			get
			{
				string value = GetValue(keyMdsDeleteOlderMoreThan);
				if (!string.IsNullOrEmpty(value))
					return int.Parse(value);
				else
					return 7* 24 * 60;
			}
			set
			{
				SetValue(keyMdsDeleteOlderMoreThan, value.ToString());
			}
		}
		#endregion

		#region General Visibility Fields
		public const string keyGeneralAllowPriorityField = "portal.admin.general.allow.priority";
		#region GeneralAllowPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [general allow priority field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow priority field visibility]; otherwise, <c>false</c>.</value>
		public static bool GeneralAllowPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyGeneralAllowPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyGeneralAllowPriorityField, value);
			}
		}
		#endregion

		public const string keyGeneralAllowGeneralCategoriesField = "portal.admin.general.allow.generalcategories";
		#region GeneralAllowGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [general allow general categories field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow general categories field visibility]; otherwise, <c>false</c>.</value>
		public static bool GeneralAllowGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyGeneralAllowGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyGeneralAllowGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyGeneralAllowClientField = "portal.admin.general.allow.client";
		#region GeneralAllowClientField
		/// <summary>
		/// Gets or sets a value indicating whether [general allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool GeneralAllowClientField
		{
			get
			{
				return bool.Parse(GetValue(keyGeneralAllowClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyGeneralAllowClientField, value);
			}
		}
		#endregion

		public const string keyGeneralAllowTaskTimeField = "portal.admin.general.allow.tasktime";
		#region GeneralAllowTaskTimeField
		/// <summary>
		/// Gets or sets a value indicating whether [general allow task time field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow task time field visibility]; otherwise, <c>false</c>.</value>
		public static bool GeneralAllowTaskTimeField
		{
			get
			{
				return bool.Parse(GetValue(keyGeneralAllowTaskTimeField, bool.TrueString));
			}
			set
			{
				SetValue(keyGeneralAllowTaskTimeField, value);
			}
		}
		#endregion 
		#endregion

		#region CalendarEntry Visibility Fields
		public const string keyCEntryAllowEditClientField = "portal.admin.centry.allowedit.client";
		#region CEntryAllowEditClientField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool CEntryAllowEditClientField
		{
			get
			{
				return bool.Parse(GetValue(keyCEntryAllowEditClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyCEntryAllowEditClientField, value);
			}
		}
		#endregion

		public const string keyCEntryAllowEditGeneralCategoriesField = "portal.admin.centry.allowedit.generalcategories";
		#region CEntryAllowEditGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool CEntryAllowEditGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyCEntryAllowEditGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyCEntryAllowEditGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyCEntryAllowEditPriorityField = "portal.admin.centry.allowedit.priority";
		#region CEntryAllowEditPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool CEntryAllowEditPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyCEntryAllowEditPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyCEntryAllowEditPriorityField, value);
			}
		}
		#endregion

		public const string keyCEntryAllowEditAttachmentField = "portal.admin.centry.allowedit.attachment";
		#region CEntryAllowEditAttachmentField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool CEntryAllowEditAttachmentField
		{
			get
			{
				return bool.Parse(GetValue(keyCEntryAllowEditAttachmentField, bool.TrueString));
			}
			set
			{
				SetValue(keyCEntryAllowEditAttachmentField, value);
			}
		}
		#endregion

		public const string keyCEntryAllowViewClientField = "portal.admin.centry.allowview.client";
		#region CEntryAllowViewClientField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool CEntryAllowViewClientField
		{
			get
			{
				return bool.Parse(GetValue(keyCEntryAllowViewClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyCEntryAllowViewClientField, value);
			}
		}
		#endregion

		public const string keyCEntryAllowViewGeneralCategoriesField = "portal.admin.centry.allowview.generalcategories";
		#region CEntryAllowViewGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool CEntryAllowViewGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyCEntryAllowViewGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyCEntryAllowViewGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyCEntryAllowViewPriorityField = "portal.admin.centry.allowview.priority";
		#region CEntryAllowViewPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool CEntryAllowViewPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyCEntryAllowViewPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyCEntryAllowViewPriorityField, value);
			}
		}
		#endregion
		#endregion

		#region Document Visibility Fields
		public const string keyDocumentAllowEditClientField = "portal.admin.document.allowedit.client";
		#region DocumentAllowEditClientField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool DocumentAllowEditClientField
		{
			get
			{
				return bool.Parse(GetValue(keyDocumentAllowEditClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyDocumentAllowEditClientField, value);
			}
		}
		#endregion

		public const string keyDocumentAllowEditGeneralCategoriesField = "portal.admin.document.allowedit.generalcategories";
		#region DocumentAllowEditGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool DocumentAllowEditGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyDocumentAllowEditGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyDocumentAllowEditGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyDocumentAllowEditPriorityField = "portal.admin.document.allowedit.priority";
		#region DocumentAllowEditPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool DocumentAllowEditPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyDocumentAllowEditPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyDocumentAllowEditPriorityField, value);
			}
		}
		#endregion

		public const string keyDocumentAllowEditAttachmentField = "portal.admin.document.allowedit.attachment";
		#region DocumentAllowEditAttachmentField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool DocumentAllowEditAttachmentField
		{
			get
			{
				return bool.Parse(GetValue(keyDocumentAllowEditAttachmentField, bool.TrueString));
			}
			set
			{
				SetValue(keyDocumentAllowEditAttachmentField, value);
			}
		}
		#endregion

		public const string keyDocumentAllowEditTaskTimeField = "portal.admin.document.allowedit.tasktime";
		#region DocumentAllowEditTaskTimeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool DocumentAllowEditTaskTimeField
		{
			get
			{
				return bool.Parse(GetValue(keyDocumentAllowEditTaskTimeField, bool.TrueString));
			}
			set
			{
				SetValue(keyDocumentAllowEditTaskTimeField, value);
			}
		}
		#endregion

		public const string keyDocumentAllowViewClientField = "portal.admin.document.allowview.client";
		#region DocumentAllowViewClientField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool DocumentAllowViewClientField
		{
			get
			{
				return bool.Parse(GetValue(keyDocumentAllowViewClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyDocumentAllowViewClientField, value);
			}
		}
		#endregion

		public const string keyDocumentAllowViewGeneralCategoriesField = "portal.admin.document.allowview.generalcategories";
		#region DocumentAllowViewGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool DocumentAllowViewGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyDocumentAllowViewGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyDocumentAllowViewGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyDocumentAllowViewPriorityField = "portal.admin.document.allowview.priority";
		#region DocumentAllowViewPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool DocumentAllowViewPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyDocumentAllowViewPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyDocumentAllowViewPriorityField, value);
			}
		}
		#endregion

		public const string keyDocumentAllowViewTaskTimeField = "portal.admin.document.allowview.tasktime";
		#region DocumentAllowViewTaskTimeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool DocumentAllowViewTaskTimeField
		{
			get
			{
				return bool.Parse(GetValue(keyDocumentAllowViewTaskTimeField, bool.TrueString));
			}
			set
			{
				SetValue(keyDocumentAllowViewTaskTimeField, value);
			}
		}
		#endregion 
		#endregion

		#region Issue Visibility Fields
		public const string keyIncidentAllowEditClientField = "portal.admin.incident.allowedit.client";
		#region IncidentAllowEditClientField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditClientField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditClientField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowEditGeneralCategoriesField = "portal.admin.incident.allowedit.generalcategories";
		#region IncidentAllowEditGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowEditPriorityField = "portal.admin.incident.allowedit.priority";
		#region IncidentAllowEditPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditPriorityField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowEditAttachmentField = "portal.admin.incident.allowedit.attachment";
		#region IncidentAllowEditAttachmentField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditAttachmentField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditAttachmentField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditAttachmentField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowEditTaskTimeField = "portal.admin.incident.allowedit.tasktime";
		#region IncidentAllowEditTaskTimeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditTaskTimeField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditTaskTimeField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditTaskTimeField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowEditIncidentCategoriesField = "portal.admin.incident.allowedit.incidentcategories";
		#region IncidentAllowEditIncidentCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditIncidentCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditIncidentCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditIncidentCategoriesField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowEditExpAssDeadlineField = "portal.admin.incident.allowedit.expectedassigndeadline";
		#region IncidentAllowEditExpAssDeadlineField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditExpAssDeadlineField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditExpAssDeadlineField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditExpAssDeadlineField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowEditExpReplyDeadlineField = "portal.admin.incident.allowedit.expectedreplydeadline";
		#region IncidentAllowEditExpReplyDeadlineField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditExpReplyDeadlineField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditExpReplyDeadlineField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditExpReplyDeadlineField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowEditExpResolutionDeadlineField = "portal.admin.incident.allowedit.expectedresolutiondeadline";
		#region IncidentAllowEditExpResolutionDeadlineField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditExpResolutionDeadlineField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditExpResolutionDeadlineField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditExpResolutionDeadlineField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowEditTypeField = "portal.admin.incident.allowedit.type";
		#region IncidentAllowEditTypeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditTypeField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditTypeField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditTypeField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowEditSeverityField = "portal.admin.incident.allowedit.severity";
		#region IncidentAllowEditSeverityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowEditSeverityField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowEditSeverityField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowEditSeverityField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowViewClientField = "portal.admin.incident.allowview.client";
		#region IncidentAllowViewClientField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowViewClientField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowViewClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowViewClientField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowViewGeneralCategoriesField = "portal.admin.incident.allowview.generalcategories";
		#region IncidentAllowViewGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowViewGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowViewGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowViewGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowViewPriorityField = "portal.admin.incident.allowview.priority";
		#region IncidentAllowViewPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowViewPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowViewPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowViewPriorityField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowViewTaskTimeField = "portal.admin.incident.allowview.tasktime";
		#region IncidentAllowViewTaskTimeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowViewTaskTimeField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowViewTaskTimeField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowViewTaskTimeField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowViewIncidentCategoriesField = "portal.admin.incident.allowview.incidentcategories";
		#region IncidentAllowViewIncidentCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowViewIncidentCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowViewIncidentCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowViewIncidentCategoriesField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowViewExpAssDeadlineField = "portal.admin.incident.allowview.expectedassigndeadline";
		#region IncidentAllowViewExpAssDeadlineField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowViewExpAssDeadlineField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowViewExpAssDeadlineField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowViewExpAssDeadlineField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowViewExpReplyDeadlineField = "portal.admin.incident.allowview.expectedreplydeadline";
		#region IncidentAllowViewExpReplyDeadlineField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowViewExpReplyDeadlineField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowViewExpReplyDeadlineField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowViewExpReplyDeadlineField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowViewExpResolutionDeadlineField = "portal.admin.incident.allowview.expectedresolutiondeadline";
		#region IncidentAllowViewExpResolutionDeadlineField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowViewExpResolutionDeadlineField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowViewExpResolutionDeadlineField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowViewExpResolutionDeadlineField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowViewTypeField = "portal.admin.incident.allowview.type";
		#region IncidentAllowViewTypeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowViewTypeField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowViewTypeField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowViewTypeField, value);
			}
		}
		#endregion

		public const string keyIncidentAllowViewSeverityField = "portal.admin.incident.allowview.severity";
		#region IncidentAllowViewSeverityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool IncidentAllowViewSeverityField
		{
			get
			{
				return bool.Parse(GetValue(keyIncidentAllowViewSeverityField, bool.TrueString));
			}
			set
			{
				SetValue(keyIncidentAllowViewSeverityField, value);
			}
		}
		#endregion
		#endregion

		#region Task Visibility Fields
		public const string keyTaskAllowEditGeneralCategoriesField = "portal.admin.task.allowedit.generalcategories";
		#region TaskAllowEditGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowEditGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowEditGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowEditGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyTaskAllowEditPriorityField = "portal.admin.task.allowedit.priority";
		#region TaskAllowEditPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowEditPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowEditPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowEditPriorityField, value);
			}
		}
		#endregion

		public const string keyTaskAllowEditAttachmentField = "portal.admin.task.allowedit.attachment";
		#region TaskAllowEditAttachmentField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowEditAttachmentField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowEditAttachmentField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowEditAttachmentField, value);
			}
		}
		#endregion

		public const string keyTaskAllowEditTaskTimeField = "portal.admin.task.allowedit.tasktime";
		#region TaskAllowEditTaskTimeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowEditTaskTimeField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowEditTaskTimeField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowEditTaskTimeField, value);
			}
		}
		#endregion

		public const string keyTaskAllowEditActivationTypeField = "portal.admin.task.allowedit.activationtype";
		#region TaskAllowEditActivationTypeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowEditActivationTypeField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowEditActivationTypeField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowEditActivationTypeField, value);
			}
		}
		#endregion

		public const string keyTaskAllowEditCompletionTypeField = "portal.admin.task.allowedit.completiontype";
		#region TaskAllowEditCompletionTypeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowEditCompletionTypeField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowEditCompletionTypeField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowEditCompletionTypeField, value);
			}
		}
		#endregion

		public const string keyTaskAllowEditMustConfirmField = "portal.admin.task.allowedit.mustconfirm";
		#region TaskAllowEditMustConfirmField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowEditMustConfirmField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowEditMustConfirmField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowEditMustConfirmField, value);
			}
		}
		#endregion

		public const string keyTaskAllowViewGeneralCategoriesField = "portal.admin.task.allowview.generalcategories";
		#region TaskAllowViewGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowViewGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowViewGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowViewGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyTaskAllowViewPriorityField = "portal.admin.task.allowview.priority";
		#region TaskAllowViewPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowViewPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowViewPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowViewPriorityField, value);
			}
		}
		#endregion

		public const string keyTaskAllowViewTaskTimeField = "portal.admin.task.allowview.tasktime";
		#region TaskAllowViewTaskTimeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowViewTaskTimeField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowViewTaskTimeField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowViewTaskTimeField, value);
			}
		}
		#endregion

		public const string keyTaskAllowViewActivationTypeField = "portal.admin.task.allowview.activationtype";
		#region TaskAllowViewActivationTypeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowViewActivationTypeField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowViewActivationTypeField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowViewActivationTypeField, value);
			}
		}
		#endregion

		public const string keyTaskAllowViewCompletionTypeField = "portal.admin.task.allowview.completiontype";
		#region TaskAllowViewCompletionTypeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowViewCompletionTypeField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowViewCompletionTypeField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowViewCompletionTypeField, value);
			}
		}
		#endregion

		public const string keyTaskAllowViewMustConfirmField = "portal.admin.task.allowview.mustconfirm";
		#region TaskAllowViewMustConfirmField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool TaskAllowViewMustConfirmField
		{
			get
			{
				return bool.Parse(GetValue(keyTaskAllowViewMustConfirmField, bool.TrueString));
			}
			set
			{
				SetValue(keyTaskAllowViewMustConfirmField, value);
			}
		}
		#endregion
		#endregion

		#region ToDo Visibility Fields
		public const string keyToDoAllowEditClientField = "portal.admin.todo.allowedit.client";
		#region ToDoAllowEditClientField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowEditClientField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowEditClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowEditClientField, value);
			}
		}
		#endregion

		public const string keyToDoAllowEditGeneralCategoriesField = "portal.admin.todo.allowedit.generalcategories";
		#region ToDoAllowEditGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowEditGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowEditGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowEditGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyToDoAllowEditPriorityField = "portal.admin.todo.allowedit.priority";
		#region ToDoAllowEditPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowEditPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowEditPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowEditPriorityField, value);
			}
		}
		#endregion

		public const string keyToDoAllowEditAttachmentField = "portal.admin.todo.allowedit.attachment";
		#region ToDoAllowEditAttachmentField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowEditAttachmentField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowEditAttachmentField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowEditAttachmentField, value);
			}
		}
		#endregion

		public const string keyToDoAllowEditTaskTimeField = "portal.admin.todo.allowedit.tasktime";
		#region ToDoAllowEditTaskTimeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowEditTaskTimeField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowEditTaskTimeField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowEditTaskTimeField, value);
			}
		}
		#endregion

		public const string keyToDoAllowEditActivationTypeField = "portal.admin.todo.allowedit.activationtype";
		#region ToDoAllowEditActivationTypeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowEditActivationTypeField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowEditActivationTypeField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowEditActivationTypeField, value);
			}
		}
		#endregion

		public const string keyToDoAllowEditCompletionTypeField = "portal.admin.todo.allowedit.completiontype";
		#region ToDoAllowEditCompletionTypeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowEditCompletionTypeField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowEditCompletionTypeField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowEditCompletionTypeField, value);
			}
		}
		#endregion

		public const string keyToDoAllowEditMustConfirmField = "portal.admin.todo.allowedit.mustconfirm";
		#region ToDoAllowEditMustConfirmField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowEditMustConfirmField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowEditMustConfirmField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowEditMustConfirmField, value);
			}
		}
		#endregion

		public const string keyToDoAllowViewClientField = "portal.admin.todo.allowview.client";
		#region ToDoAllowViewClientField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowViewClientField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowViewClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowViewClientField, value);
			}
		}
		#endregion

		public const string keyToDoAllowViewGeneralCategoriesField = "portal.admin.todo.allowview.generalcategories";
		#region ToDoAllowViewGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowViewGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowViewGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowViewGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyToDoAllowViewPriorityField = "portal.admin.todo.allowview.priority";
		#region ToDoAllowViewPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowViewPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowViewPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowViewPriorityField, value);
			}
		}
		#endregion

		public const string keyToDoAllowViewTaskTimeField = "portal.admin.todo.allowview.tasktime";
		#region ToDoAllowViewTaskTimeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowViewTaskTimeField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowViewTaskTimeField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowViewTaskTimeField, value);
			}
		}
		#endregion

		public const string keyToDoAllowViewActivationTypeField = "portal.admin.todo.allowview.activationtype";
		#region ToDoAllowViewActivationTypeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowViewActivationTypeField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowViewActivationTypeField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowViewActivationTypeField, value);
			}
		}
		#endregion

		public const string keyToDoAllowViewCompletionTypeField = "portal.admin.todo.allowview.completiontype";
		#region ToDoAllowViewCompletionTypeField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowViewCompletionTypeField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowViewCompletionTypeField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowViewCompletionTypeField, value);
			}
		}
		#endregion

		public const string keyToDoAllowViewMustConfirmField = "portal.admin.todo.allowview.mustconfirm";
		#region ToDoAllowViewMustConfirmField
		/// <summary>
		/// Gets or sets a value indicating whether [centry allow client field visibility].
		/// </summary>
		/// <value><c>true</c> if [general allow client field visibility]; otherwise, <c>false</c>.</value>
		public static bool ToDoAllowViewMustConfirmField
		{
			get
			{
				return bool.Parse(GetValue(keyToDoAllowViewMustConfirmField, bool.TrueString));
			}
			set
			{
				SetValue(keyToDoAllowViewMustConfirmField, value);
			}
		}
		#endregion
		#endregion

		#region Project Visibility Fields
		public const string keyProjectAllowEditClientField = "portal.admin.project.allowedit.client";
		#region ProjectAllowEditClientField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow edit client field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow edit client field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowEditClientField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowEditClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowEditClientField, value);
			}
		}
		#endregion

		public const string keyProjectAllowEditGeneralCategoriesField = "portal.admin.project.allowedit.generalcategories";
		#region ProjectAllowEditGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow edit general categories field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow edit general categories field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowEditGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowEditGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowEditGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyProjectAllowEditPriorityField = "portal.admin.project.allowedit.priority";
		#region ProjectAllowEditPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow edit priority field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow edit priority field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowEditPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowEditPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowEditPriorityField, value);
			}
		}
		#endregion

		public const string keyProjectAllowEditProjectCategoriesField = "portal.admin.project.allowedit.projectcategories";
		#region ProjectAllowEditProjectCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow edit project categories field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow edit project categories field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowEditProjectCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowEditProjectCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowEditProjectCategoriesField, value);
			}
		}
		#endregion

		public const string keyProjectAllowEditGoalsField = "portal.admin.project.allowedit.goals";
		#region ProjectAllowEditGoalsField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow edit goals field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow edit goals field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowEditGoalsField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowEditGoalsField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowEditGoalsField, value);
			}
		}
		#endregion

		public const string keyProjectAllowEditDeliverablesField = "portal.admin.project.allowedit.deliverables";
		#region ProjectAllowEditDeliverablesField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow edit deliverables field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow edit deliverables field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowEditDeliverablesField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowEditDeliverablesField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowEditDeliverablesField, value);
			}
		}
		#endregion

		public const string keyProjectAllowEditScopeField = "portal.admin.project.allowedit.scope";
		#region ProjectAllowEditScopeField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow edit scope field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow edit scope field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowEditScopeField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowEditScopeField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowEditScopeField, value);
			}
		}
		#endregion

		public const string keyProjectAllowEditCurrencyField = "portal.admin.project.allowedit.currency";
		#region ProjectAllowEditCurrencyField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow edit currency field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow edit currency field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowEditCurrencyField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowEditCurrencyField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowEditCurrencyField, value);
			}
		}
		#endregion

	
		public const string keyProjectAllowViewClientField = "portal.admin.project.allowview.client";
		#region ProjectAllowViewClientField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow view client field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow view client field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowViewClientField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowViewClientField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowViewClientField, value);
			}
		}
		#endregion

		public const string keyProjectAllowViewGeneralCategoriesField = "portal.admin.project.allowview.generalcategories";
		#region ProjectAllowViewGeneralCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow view general categories field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow view general categories field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowViewGeneralCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowViewGeneralCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowViewGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyProjectAllowViewPriorityField = "portal.admin.project.allowview.priority";
		#region ProjectAllowViewPriorityField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow view priority field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow view priority field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowViewPriorityField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowViewPriorityField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowViewPriorityField, value);
			}
		}
		#endregion

		public const string keyProjectAllowViewProjectCategoriesField = "portal.admin.project.allowview.projectcategories";
		#region ProjectAllowViewProjectCategoriesField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow view project categories field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow view project categories field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowViewProjectCategoriesField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowViewProjectCategoriesField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowViewProjectCategoriesField, value);
			}
		}
		#endregion

		public const string keyProjectAllowViewGoalsField = "portal.admin.project.allowview.goals";
		#region ProjectAllowViewGoalsField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow view goals field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow view goals field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowViewGoalsField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowViewGoalsField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowViewGoalsField, value);
			}
		}
		#endregion

		public const string keyProjectAllowViewDeliverablesField = "portal.admin.project.allowview.deliverables";
		#region ProjectAllowViewDeliverablesField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow view deliverables field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow view deliverables field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowViewDeliverablesField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowViewDeliverablesField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowViewDeliverablesField, value);
			}
		}
		#endregion

		public const string keyProjectAllowViewScopeField = "portal.admin.project.allowview.scope";
		#region ProjectAllowViewScopeField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow view scope field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow view scope field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowViewScopeField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowViewScopeField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowViewScopeField, value);
			}
		}
		#endregion

		public const string keyProjectAllowViewCurrencyField = "portal.admin.project.allowview.currency";
		#region ProjectAllowViewCurrencyField
		/// <summary>
		/// Gets or sets a value indicating whether [project allow view currency field].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [project allow view currency field]; otherwise, <c>false</c>.
		/// </value>
		public static bool ProjectAllowViewCurrencyField
		{
			get
			{
				return bool.Parse(GetValue(keyProjectAllowViewCurrencyField, bool.TrueString));
			}
			set
			{
				SetValue(keyProjectAllowViewCurrencyField, value);
			}
		}
		#endregion
		#endregion

		#region Common Visibility Functions
		#region CommonDocumentAllowEditPriorityField
		public static bool CommonDocumentAllowEditPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && DocumentAllowEditPriorityField);
			}
		}
		#endregion

		#region CommonDocumentAllowViewPriorityField
		public static bool CommonDocumentAllowViewPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && DocumentAllowViewPriorityField);
			}
		}
		#endregion

		#region CommonCEntryAllowEditPriorityField
		public static bool CommonCEntryAllowEditPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && CEntryAllowEditPriorityField);
			}
		}
		#endregion

		#region CommonCEntryAllowViewPriorityField
		public static bool CommonCEntryAllowViewPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && CEntryAllowViewPriorityField);
			}
		}
		#endregion

		#region CommonIncidentAllowEditPriorityField
		public static bool CommonIncidentAllowEditPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && IncidentAllowEditPriorityField);
			}
		}
		#endregion

		#region CommonIncidentAllowViewPriorityField
		public static bool CommonIncidentAllowViewPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && IncidentAllowViewPriorityField);
			}
		}
		#endregion

		#region CommonTaskAllowEditPriorityField
		public static bool CommonTaskAllowEditPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && TaskAllowEditPriorityField);
			}
		}
		#endregion

		#region CommonTaskAllowViewPriorityField
		public static bool CommonTaskAllowViewPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && TaskAllowViewPriorityField);
			}
		}
		#endregion

		#region CommonToDoAllowEditPriorityField
		public static bool CommonToDoAllowEditPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && ToDoAllowEditPriorityField);
			}
		}
		#endregion

		#region CommonToDoAllowViewPriorityField
		public static bool CommonToDoAllowViewPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && ToDoAllowViewPriorityField);
			}
		}
		#endregion

		#region CommonProjectAllowEditPriorityField
		public static bool CommonProjectAllowEditPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && ProjectAllowEditPriorityField);
			}
		}
		#endregion

		#region CommonProjectAllowViewPriorityField
		public static bool CommonProjectAllowViewPriorityField
		{
			get
			{
				return (GeneralAllowPriorityField && ProjectAllowViewPriorityField);
			}
		}
		#endregion


		#region CommonDocumentAllowEditGeneralCategoriesField
		public static bool CommonDocumentAllowEditGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && DocumentAllowEditGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonDocumentAllowViewGeneralCategoriesField
		public static bool CommonDocumentAllowViewGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && DocumentAllowViewGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonCEntryAllowEditGeneralCategoriesField
		public static bool CommonCEntryAllowEditGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && CEntryAllowEditGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonCEntryAllowViewGeneralCategoriesField
		public static bool CommonCEntryAllowViewGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && CEntryAllowViewGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonIncidentAllowEditGeneralCategoriesField
		public static bool CommonIncidentAllowEditGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && IncidentAllowEditGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonIncidentAllowViewGeneralCategoriesField
		public static bool CommonIncidentAllowViewGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && IncidentAllowViewGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonTaskAllowEditGeneralCategoriesField
		public static bool CommonTaskAllowEditGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && TaskAllowEditGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonTaskAllowViewGeneralCategoriesField
		public static bool CommonTaskAllowViewGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && TaskAllowViewGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonToDoAllowEditGeneralCategoriesField
		public static bool CommonToDoAllowEditGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && ToDoAllowEditGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonToDoAllowViewGeneralCategoriesField
		public static bool CommonToDoAllowViewGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && ToDoAllowViewGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonProjectAllowEditGeneralCategoriesField
		public static bool CommonProjectAllowEditGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && ProjectAllowEditGeneralCategoriesField);
			}
		}
		#endregion

		#region CommonProjectAllowViewGeneralCategoriesField
		public static bool CommonProjectAllowViewGeneralCategoriesField
		{
			get
			{
				return (GeneralAllowGeneralCategoriesField && ProjectAllowViewGeneralCategoriesField);
			}
		}
		#endregion


		#region CommonDocumentAllowEditClientField
		public static bool CommonDocumentAllowEditClientField
		{
			get
			{
				return (GeneralAllowClientField && DocumentAllowEditClientField);
			}
		}
		#endregion

		#region CommonDocumentAllowViewClientField
		public static bool CommonDocumentAllowViewClientField
		{
			get
			{
				return (GeneralAllowClientField && DocumentAllowViewClientField);
			}
		}
		#endregion

		#region CommonCEntryAllowEditClientField
		public static bool CommonCEntryAllowEditClientField
		{
			get
			{
				return (GeneralAllowClientField && CEntryAllowEditClientField);
			}
		}
		#endregion

		#region CommonCEntryAllowViewClientField
		public static bool CommonCEntryAllowViewClientField
		{
			get
			{
				return (GeneralAllowClientField && CEntryAllowViewClientField);
			}
		}
		#endregion

		#region CommonIncidentAllowEditClientField
		public static bool CommonIncidentAllowEditClientField
		{
			get
			{
				return (GeneralAllowClientField && IncidentAllowEditClientField);
			}
		}
		#endregion

		#region CommonIncidentAllowViewClientField
		public static bool CommonIncidentAllowViewClientField
		{
			get
			{
				return (GeneralAllowClientField && IncidentAllowViewClientField);
			}
		}
		#endregion

		#region CommonToDoAllowEditClientField
		public static bool CommonToDoAllowEditClientField
		{
			get
			{
				return (GeneralAllowClientField && ToDoAllowEditClientField);
			}
		}
		#endregion

		#region CommonToDoAllowViewClientField
		public static bool CommonToDoAllowViewClientField
		{
			get
			{
				return (GeneralAllowClientField && ToDoAllowViewClientField);
			}
		}
		#endregion

		#region CommonProjectAllowEditClientField
		public static bool CommonProjectAllowEditClientField
		{
			get
			{
				return (GeneralAllowClientField && ProjectAllowEditClientField);
			}
		}
		#endregion

		#region CommonProjectAllowViewClientField
		public static bool CommonProjectAllowViewClientField
		{
			get
			{
				return (GeneralAllowClientField && ProjectAllowViewClientField);
			}
		}
		#endregion


		#region CommonDocumentAllowEditTaskTimeField
		public static bool CommonDocumentAllowEditTaskTimeField
		{
			get
			{
				return (GeneralAllowTaskTimeField && DocumentAllowEditTaskTimeField);
			}
		}
		#endregion

		#region CommonDocumentAllowViewTaskTimeField
		public static bool CommonDocumentAllowViewTaskTimeField
		{
			get
			{
				return (GeneralAllowTaskTimeField && DocumentAllowViewTaskTimeField);
			}
		}
		#endregion

		#region CommonIncidentAllowEditTaskTimeField
		public static bool CommonIncidentAllowEditTaskTimeField
		{
			get
			{
				return (GeneralAllowTaskTimeField && IncidentAllowEditTaskTimeField);
			}
		}
		#endregion

		#region CommonIncidentAllowViewTaskTimeField
		public static bool CommonIncidentAllowViewTaskTimeField
		{
			get
			{
				return (GeneralAllowTaskTimeField && IncidentAllowViewTaskTimeField);
			}
		}
		#endregion

		#region CommonTaskAllowEditTaskTimeField
		public static bool CommonTaskAllowEditTaskTimeField
		{
			get
			{
				return (GeneralAllowTaskTimeField && TaskAllowEditTaskTimeField);
			}
		}
		#endregion

		#region CommonTaskAllowViewTaskTimeField
		public static bool CommonTaskAllowViewTaskTimeField
		{
			get
			{
				return (GeneralAllowTaskTimeField && TaskAllowViewTaskTimeField);
			}
		}
		#endregion

		#region CommonToDoAllowEditTaskTimeField
		public static bool CommonToDoAllowEditTaskTimeField
		{
			get
			{
				return (GeneralAllowTaskTimeField && ToDoAllowEditTaskTimeField);
			}
		}
		#endregion

		#region CommonToDoAllowViewTaskTimeField
		public static bool CommonToDoAllowViewTaskTimeField
		{
			get
			{
				return (GeneralAllowTaskTimeField && ToDoAllowViewTaskTimeField);
			}
		}
		#endregion
		#endregion

		#region Document Fields Default Values
		public const string keyDocumentDefaultValueClientField = "portal.admin.document.defvalue.client";
		#region DocumentDefaultValueClientField
		public static string DocumentDefaultValueClientField
		{
			get
			{
				return GetValue(keyDocumentDefaultValueClientField, String.Empty);
			}
			set
			{
				SetValue(keyDocumentDefaultValueClientField, value);
			}
		}
		#endregion

		public const string keyDocumentDefaultValueGeneralCategoriesField = "portal.admin.document.defvalue.generalcategories";
		#region DocumentDefaultValueGeneralCategoriesField
		public static string DocumentDefaultValueGeneralCategoriesField
		{
			get
			{
				return GetValue(keyDocumentDefaultValueGeneralCategoriesField, String.Empty);
			}
			set
			{
				SetValue(keyDocumentDefaultValueGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyDocumentDefaultValuePriorityField = "portal.admin.document.defvalue.priority";
		#region DocumentDefaultValuePriorityField
		public static string DocumentDefaultValuePriorityField
		{
			get
			{
				return GetValue(keyDocumentDefaultValuePriorityField, ((int)Priority.Normal).ToString());
			}
			set
			{
				SetValue(keyDocumentDefaultValuePriorityField, value);
			}
		}
		#endregion

		public const string keyDocumentDefaultValueTaskTimeField = "portal.admin.document.defvalue.tasktime";
		#region DocumentDefaultValueTaskTimeField
		public static string DocumentDefaultValueTaskTimeField
		{
			get
			{
				return GetValue(keyDocumentDefaultValueTaskTimeField, "480");
			}
			set
			{
				SetValue(keyDocumentDefaultValueTaskTimeField, value);
			}
		}
		#endregion
		#endregion

		#region Task Fields Default Values
		public const string keyTaskDefaultValueGeneralCategoriesField = "portal.admin.task.defvalue.generalcategories";
		#region TaskDefaultValueGeneralCategoriesField
		public static string TaskDefaultValueGeneralCategoriesField
		{
			get
			{
				return GetValue(keyTaskDefaultValueGeneralCategoriesField, String.Empty);
			}
			set
			{
				SetValue(keyTaskDefaultValueGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyTaskDefaultValuePriorityField = "portal.admin.task.defvalue.priority";
		#region TaskDefaultValuePriorityField
		public static string TaskDefaultValuePriorityField
		{
			get
			{
				return GetValue(keyTaskDefaultValuePriorityField, ((int)Priority.Normal).ToString());
			}
			set
			{
				SetValue(keyTaskDefaultValuePriorityField, value);
			}
		}
		#endregion

		public const string keyTaskDefaultValueTaskTimeField = "portal.admin.task.defvalue.tasktime";
		#region TaskDefaultValueTaskTimeField
		public static string TaskDefaultValueTaskTimeField
		{
			get
			{
				return GetValue(keyTaskDefaultValueTaskTimeField, "480");
			}
			set
			{
				SetValue(keyTaskDefaultValueTaskTimeField, value);
			}
		}
		#endregion

		public const string keyTaskDefaultValueMustConfirmField = "portal.admin.task.defvalue.mustconfirm";
		#region TaskDefaultValueMustConfirmField
		public static string TaskDefaultValueMustConfirmField
		{
			get
			{
				return GetValue(keyTaskDefaultValueMustConfirmField, bool.FalseString);
			}
			set
			{
				SetValue(keyTaskDefaultValueMustConfirmField, value);
			}
		}
		#endregion

		public const string keyTaskDefaultValueActivationTypeField = "portal.admin.task.defvalue.activationtype";
		#region TaskDefaultValueActivationTypeField
		public static string TaskDefaultValueActivationTypeField
		{
			get
			{
				return GetValue(keyTaskDefaultValueActivationTypeField, ((int)ActivationTypes.AutoWithCheck).ToString());
			}
			set
			{
				SetValue(keyTaskDefaultValueActivationTypeField, value);
			}
		}
		#endregion

		public const string keyTaskDefaultValueCompetionTypeField = "portal.admin.task.defvalue.completiontype";
		#region TaskDefaultValueCompetionTypeField
		public static string TaskDefaultValueCompetionTypeField
		{
			get
			{
				return GetValue(keyTaskDefaultValueCompetionTypeField, ((int)CompletionType.All).ToString());
			}
			set
			{
				SetValue(keyTaskDefaultValueCompetionTypeField, value);
			}
		}
		#endregion
		#endregion

		#region ToDo Fields Default Values
		public const string keyToDoDefaultValueClientField = "portal.admin.todo.defvalue.client";
		#region ToDoDefaultValueClientField
		public static string ToDoDefaultValueClientField
		{
			get
			{
				return GetValue(keyToDoDefaultValueClientField, String.Empty);
			}
			set
			{
				SetValue(keyToDoDefaultValueClientField, value);
			}
		}
		#endregion

		public const string keyToDoDefaultValueGeneralCategoriesField = "portal.admin.todo.defvalue.generalcategories";
		#region ToDoDefaultValueGeneralCategoriesField
		public static string ToDoDefaultValueGeneralCategoriesField
		{
			get
			{
				return GetValue(keyToDoDefaultValueGeneralCategoriesField, String.Empty);
			}
			set
			{
				SetValue(keyToDoDefaultValueGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyToDoDefaultValuePriorityField = "portal.admin.todo.defvalue.priority";
		#region ToDoDefaultValuePriorityField
		public static string ToDoDefaultValuePriorityField
		{
			get
			{
				return GetValue(keyToDoDefaultValuePriorityField, ((int)Priority.Normal).ToString());
			}
			set
			{
				SetValue(keyToDoDefaultValuePriorityField, value);
			}
		}
		#endregion

		public const string keyToDoDefaultValueTaskTimeField = "portal.admin.todo.defvalue.tasktime";
		#region ToDoDefaultValueTaskTimeField
		public static string ToDoDefaultValueTaskTimeField
		{
			get
			{
				return GetValue(keyToDoDefaultValueTaskTimeField, "480");
			}
			set
			{
				SetValue(keyToDoDefaultValueTaskTimeField, value);
			}
		}
		#endregion

		public const string keyToDoDefaultValueMustConfirmField = "portal.admin.todo.defvalue.mustconfirm";
		#region ToDoDefaultValueMustConfirmField
		public static string ToDoDefaultValueMustConfirmField
		{
			get
			{
				return GetValue(keyToDoDefaultValueMustConfirmField, bool.FalseString);
			}
			set
			{
				SetValue(keyToDoDefaultValueMustConfirmField, value);
			}
		}
		#endregion

		public const string keyToDoDefaultValueActivationTypeField = "portal.admin.todo.defvalue.activationtype";
		#region ToDoDefaultValueActivationTypeField
		public static string ToDoDefaultValueActivationTypeField
		{
			get
			{
				return GetValue(keyToDoDefaultValueActivationTypeField, ((int)ActivationTypes.AutoWithCheck).ToString());
			}
			set
			{
				SetValue(keyToDoDefaultValueActivationTypeField, value);
			}
		}
		#endregion

		public const string keyToDoDefaultValueCompetionTypeField = "portal.admin.todo.defvalue.completiontype";
		#region ToDoDefaultValueCompetionTypeField
		public static string ToDoDefaultValueCompetionTypeField
		{
			get
			{
				return GetValue(keyToDoDefaultValueCompetionTypeField, ((int)CompletionType.All).ToString());
			}
			set
			{
				SetValue(keyToDoDefaultValueCompetionTypeField, value);
			}
		}
		#endregion
		#endregion

		#region CalendarEntry Fields Default Values
		public const string keyCEntryDefaultValueClientField = "portal.admin.centry.defvalue.client";
		#region CEntryDefaultValueClientField
		public static string CEntryDefaultValueClientField
		{
			get
			{
				return GetValue(keyCEntryDefaultValueClientField, String.Empty);
			}
			set
			{
				SetValue(keyCEntryDefaultValueClientField, value);
			}
		}
		#endregion

		public const string keyCEntryDefaultValueGeneralCategoriesField = "portal.admin.centry.defvalue.generalcategories";
		#region CEntryDefaultValueGeneralCategoriesField
		public static string CEntryDefaultValueGeneralCategoriesField
		{
			get
			{
				return GetValue(keyCEntryDefaultValueGeneralCategoriesField, String.Empty);
			}
			set
			{
				SetValue(keyCEntryDefaultValueGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyCEntryDefaultValuePriorityField = "portal.admin.centry.defvalue.priority";
		#region CEntryDefaultValuePriorityField
		public static string CEntryDefaultValuePriorityField
		{
			get
			{
				return GetValue(keyCEntryDefaultValuePriorityField, ((int)Priority.Normal).ToString());
			}
			set
			{
				SetValue(keyCEntryDefaultValuePriorityField, value);
			}
		}
		#endregion
		#endregion

		#region Incident Fields Default Values
		public const string keyIncidentDefaultValueClientField = "portal.admin.incident.defvalue.client";
		#region IncidentDefaultValueClientField
		public static string IncidentDefaultValueClientField
		{
			get
			{
				return GetValue(keyIncidentDefaultValueClientField, String.Empty);
			}
			set
			{
				SetValue(keyIncidentDefaultValueClientField, value);
			}
		}
		#endregion

		public const string keyIncidentDefaultValueGeneralCategoriesField = "portal.admin.incident.defvalue.generalcategories";
		#region IncidentDefaultValueGeneralCategoriesField
		public static string IncidentDefaultValueGeneralCategoriesField
		{
			get
			{
				return GetValue(keyIncidentDefaultValueGeneralCategoriesField, String.Empty);
			}
			set
			{
				SetValue(keyIncidentDefaultValueGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyIncidentDefaultValuePriorityField = "portal.admin.incident.defvalue.priority";
		#region IncidentDefaultValuePriorityField
		public static string IncidentDefaultValuePriorityField
		{
			get
			{
				return GetValue(keyIncidentDefaultValuePriorityField, ((int)Priority.Normal).ToString());
			}
			set
			{
				SetValue(keyIncidentDefaultValuePriorityField, value);
			}
		}
		#endregion

		public const string keyIncidentDefaultValueTaskTimeField = "portal.admin.incident.defvalue.tasktime";
		#region IncidentDefaultValueTaskTimeField
		public static string IncidentDefaultValueTaskTimeField
		{
			get
			{
				return GetValue(keyIncidentDefaultValueTaskTimeField, "180");
			}
			set
			{
				SetValue(keyIncidentDefaultValueTaskTimeField, value);
			}
		}
		#endregion

		public const string keyIncidentDefaultValueIncidentCategoriesField = "portal.admin.incident.defvalue.incidentcategories";
		#region IncidentDefaultValueIncidentCategoriesField
		public static string IncidentDefaultValueIncidentCategoriesField
		{
			get
			{
				return GetValue(keyIncidentDefaultValueIncidentCategoriesField, String.Empty);
			}
			set
			{
				SetValue(keyIncidentDefaultValueIncidentCategoriesField, value);
			}
		}
		#endregion

		public const string keyIncidentDefaultValueExpAssDeadlineField = "portal.admin.incident.defvalue.expectedassigndeadline";
		#region IncidentDefaultValueExpAssDeadlineField
		public static string IncidentDefaultValueExpAssDeadlineField
		{
			get
			{
				return GetValue(keyIncidentDefaultValueExpAssDeadlineField, "480");
			}
			set
			{
				SetValue(keyIncidentDefaultValueExpAssDeadlineField, value);
			}
		}
		#endregion

		public const string keyIncidentDefaultValueExpReplyDeadlineField = "portal.admin.incident.defvalue.expectedreplydeadline";
		#region IncidentDefaultValueExpReplyDeadlineField
		public static string IncidentDefaultValueExpReplyDeadlineField
		{
			get
			{
				return GetValue(keyIncidentDefaultValueExpReplyDeadlineField, "1440");
			}
			set
			{
				SetValue(keyIncidentDefaultValueExpReplyDeadlineField, value);
			}
		}
		#endregion

		public const string keyIncidentDefaultValueExpResolutionDeadlineField = "portal.admin.incident.defvalue.expectedresolutiondeadline";
		#region IncidentDefaultValueExpResolutionDeadlineField
		public static string IncidentDefaultValueExpResolutionDeadlineField
		{
			get
			{
				return GetValue(keyIncidentDefaultValueExpResolutionDeadlineField, "10080");
			}
			set
			{
				SetValue(keyIncidentDefaultValueExpResolutionDeadlineField, value);
			}
		}
		#endregion

		public const string keyIncidentDefaultValueTypeField = "portal.admin.incident.defvalue.type";
		#region IncidentDefaultValueTypeField
		public static string IncidentDefaultValueTypeField
		{
			get
			{
				return GetValue(keyIncidentDefaultValueTypeField, Common.GetDefaultIncidentType().ToString());
			}
			set
			{
				SetValue(keyIncidentDefaultValueTypeField, value);
			}
		}
		#endregion

		public const string keyIncidentDefaultValueSeverityField = "portal.admin.incident.defvalue.severity";
		#region IncidentDefaultValueSeverityField
		public static string IncidentDefaultValueSeverityField
		{
			get
			{
				return GetValue(keyIncidentDefaultValueSeverityField, Common.GetDefaultIncidentSeverity().ToString());
			}
			set
			{
				SetValue(keyIncidentDefaultValueSeverityField, value);
			}
		}
		#endregion

		#endregion

		#region Project Fields Default Values
		public const string keyProjectDefaultValueClientField = "portal.admin.project.defvalue.client";
		#region ProjectDefaultValueClientField
		public static string ProjectDefaultValueClientField
		{
			get
			{
				return GetValue(keyProjectDefaultValueClientField, String.Empty);
			}
			set
			{
				SetValue(keyProjectDefaultValueClientField, value);
			}
		}
		#endregion

		public const string keyProjectDefaultValueGeneralCategoriesField = "portal.admin.project.defvalue.generalcategories";
		#region ProjectDefaultValueGeneralCategoriesField
		public static string ProjectDefaultValueGeneralCategoriesField
		{
			get
			{
				return GetValue(keyProjectDefaultValueGeneralCategoriesField, String.Empty);
			}
			set
			{
				SetValue(keyProjectDefaultValueGeneralCategoriesField, value);
			}
		}
		#endregion

		public const string keyProjectDefaultValuePriorityField = "portal.admin.project.defvalue.priority";
		#region ProjectDefaultValuePriorityField
		public static string ProjectDefaultValuePriorityField
		{
			get
			{
				return GetValue(keyProjectDefaultValuePriorityField, ((int)Priority.Normal).ToString());
			}
			set
			{
				SetValue(keyProjectDefaultValuePriorityField, value);
			}
		}
		#endregion

		public const string keyProjectDefaultValueProjectCategoriesField = "portal.admin.project.defvalue.projectcategories";
		#region ProjectDefaultValueProjectCategoriesField
		public static string ProjectDefaultValueProjectCategoriesField
		{
			get
			{
				return GetValue(keyProjectDefaultValueProjectCategoriesField, String.Empty);
			}
			set
			{
				SetValue(keyProjectDefaultValueProjectCategoriesField, value);
			}
		}
		#endregion

		public const string keyProjectDefaultValueScopeField = "portal.admin.project.defvalue.scope";
		#region ProjectDefaultValueScopeField
		public static string ProjectDefaultValueScopeField
		{
			get
			{
				return GetValue(keyProjectDefaultValueScopeField, String.Empty);
			}
			set
			{
				SetValue(keyProjectDefaultValueScopeField, value);
			}
		}
		#endregion

		public const string keyProjectDefaultValueDeliverablesField = "portal.admin.project.defvalue.deliverables";
		#region ProjectDefaultValueDeliverablesField
		public static string ProjectDefaultValueDeliverablesField
		{
			get
			{
				return GetValue(keyProjectDefaultValueDeliverablesField, String.Empty);
			}
			set
			{
				SetValue(keyProjectDefaultValueDeliverablesField, value);
			}
		}
		#endregion

		public const string keyProjectDefaultValueGoalsField = "portal.admin.project.defvalue.goals";
		#region ProjectDefaultValueGoalsField
		public static string ProjectDefaultValueGoalsField
		{
			get
			{
				return GetValue(keyProjectDefaultValueGoalsField, String.Empty);
			}
			set
			{
				SetValue(keyProjectDefaultValueGoalsField, value);
			}
		}
		#endregion

		public const string keyProjectDefaultValueCurrencyField = "portal.admin.project.defvalue.currency";
		#region ProjectDefaultValueCurrencyField
		public static string ProjectDefaultValueCurrencyField
		{
			get
			{
				return GetValue(keyProjectDefaultValueCurrencyField, "1");
			}
			set
			{
				SetValue(keyProjectDefaultValueCurrencyField, value);
			}
		}
		#endregion
		#endregion


		#region DefaultCalendar
		/// <summary>
		/// Gets the default calendar.
		/// </summary>
		/// <value>The default calendar.</value>
		public static int DefaultCalendar
		{
			get
			{
				string value = GetValue(keyDefaultCalendar);
				if (!string.IsNullOrEmpty(value))
					return int.Parse(value);
				else
					return -1;
			}
			set
			{
				SetValue(keyDefaultCalendar, value);
			}
		} 
		#endregion

		#region IsListRssEnabled
		/// <summary>
		/// Gets or sets a value indicating whether this instance is list RSS enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is list RSS enabled; otherwise, <c>false</c>.
		/// </value>
		public static bool IsListRssEnabled
		{
			get
			{
				return bool.Parse(GetValue(keyIsListRssEnabled, "true"));
			}
			set
			{
				SetValue(keyIsListRssEnabled, value);
			}
		}
		#endregion

		#region IsListRssEnabled
		/// <summary>
		/// Gets or sets a value indicating whether this instance is list RSS enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is list RSS enabled; otherwise, <c>false</c>.
		/// </value>
		public static int SmtpRequestTimeout
		{
			get
			{
				return int.Parse(GetValue(keySmtpRequestTimeout, "30"));
			}
			set
			{
				SetValue(keySmtpRequestTimeout, value);
			}
		}
		#endregion

		#region UseIncidentDatesForProject
		//ak [2009-06-15] for IncidentEdit1 *softkey*
		public const string keyUseIncidentDatesForProject = "portal.admin.useincidentdatesforproject";
		public static bool UseIncidentDatesForProject
		{
			get
			{
				return bool.Parse(GetValue(keyUseIncidentDatesForProject, "false"));
			}
			set
			{
				SetValue(keyUseIncidentDatesForProject, value);
			}
		}
		#endregion

		#region UseIncidentEditAddon
		//ak [2009-06-15] for IncidentEdit1 *softkey*
		public const string keyUseIncidentEditAddon = "portal.admin.addon.useincidentedit";
		public static bool UseIncidentEditAddon
		{
			get
			{
				return bool.Parse(GetValue(keyUseIncidentEditAddon, "false"));
			}
			set
			{
				SetValue(keyUseIncidentEditAddon, value);
			}
		}
		#endregion

		#region ProjectEditControl
		// OR [2009-08-03]: path to ProjectEdit.ascx control
		public const string keyProjectEditControl = "portal.admin.projecteditcontrol";
		public const string ProjectEditControlDefaultValue = "~/Projects/Modules/ProjectEdit.ascx";
		public static string ProjectEditControl
		{
			get
			{
				return GetValue(keyProjectEditControl, ProjectEditControlDefaultValue);
			}
			set
			{
				SetValue(keyProjectEditControl, value);
			}
		}
		#endregion

		#region IncidentEditControl
		// OR [2009-08-03]: path to ProjectEdit.ascx control
		public const string keyIncidentEditControl = "portal.admin.incidenteditcontrol";
		public const string IncidentEditControlDefaultValue = "~/Incidents/Modules/IncidentEdit.ascx";
		public static string IncidentEditControl
		{
			get
			{
				return GetValue(keyIncidentEditControl, IncidentEditControlDefaultValue);
			}
			set
			{
				SetValue(keyIncidentEditControl, value);
			}
		}
		#endregion

		#region ProjectInfoControl
		// OR [2009-08-05]: path to ProjectInfo2.ascx control
		public const string keyProjectInfoControl = "portal.admin.projectinfocontrol";
		public const string ProjectInfoControlDefaultValue = "~/Projects/Modules/ProjectInfo2.ascx";
		public static string ProjectInfoControl
		{
			get
			{
				return GetValue(keyProjectInfoControl, ProjectInfoControlDefaultValue);
			}
			set
			{
				SetValue(keyProjectInfoControl, value);
			}
		}
		#endregion

		#region IssueShortInfoControl
		// OR [2009-08-05]: path to IssueShortInfo.ascx control
		public const string keyIssueShortInfoControl = "portal.admin.issueshortinfocontrol";
		public const string IssueShortInfoControlDefaultValue = "~/Incidents/Modules/IssueShortInfo.ascx";
		public static string IssueShortInfoControl
		{
			get
			{
				return GetValue(keyIssueShortInfoControl, IssueShortInfoControlDefaultValue);
			}
			set
			{
				SetValue(keyIssueShortInfoControl, value);
			}
		}
		#endregion

		#region ShowProjectCode
		// O.R. [2009-08-05]: if True then show ProjectCode instead of ProjectId
		public const string keyShowProjectCode = "portal.admin.showprojectcode";
		public static bool ShowProjectCode
		{
			get
			{
				return bool.Parse(GetValue(keyShowProjectCode, "false"));
			}
			set
			{
				SetValue(keyShowProjectCode, value);
			}
		}
		#endregion

		#region ProjectViewControl
		// OR [2009-08-07]: path to ProjectView2.ascx control
		public const string keyProjectViewControl = "portal.admin.projectviewcontrol";
		public const string ProjectViewControlDefaultValue = "~/Projects/Modules/ProjectView2.ascx";
		public static string ProjectViewControl
		{
			get
			{
				return GetValue(keyProjectViewControl, ProjectViewControlDefaultValue);
			}
			set
			{
				SetValue(keyProjectViewControl, value);
			}
		}
		#endregion

		#region ProjectSnapshotControl
		// OR [2009-09-07]: path to OverallProjectSnapshot.ascx control
		public const string keyProjectSnapshotControl = "portal.admin.projectsnapshotcontrol";
		public const string ProjectSnapshotControlDefaultValue = "~/Reports/Modules/OverallProjectSnapshot.ascx";
		public static string ProjectSnapshotControl
		{
			get
			{
				return GetValue(keyProjectSnapshotControl, ProjectSnapshotControlDefaultValue);
			}
			set
			{
				SetValue(keyProjectSnapshotControl, value);
			}
		}
		#endregion

		#region ShortInfoDescriptionLength
		public static int ShortInfoDescriptionLength
		{
			get
			{
				string value = GetValue(keyShortInfoDescriptionLength);
				if (!string.IsNullOrEmpty(value))
					return int.Parse(value);
				else
					return 200;
			}
			set
			{
				SetValue(keyShortInfoDescriptionLength, value.ToString());
			}
		}
		#endregion

		#region ManagementCenterDashboardControl
		// OR [2011-01-26]: path to dashboard.ascx control
		public const string keyManagementCenterDashboardControl = "portal.admin.managementcenter.dashboardcontrol";
		public const string ManagementCenterDashboardControlDefaultValue = "~/reports/modules/dashboard.ascx";
		public static string ManagementCenterDashboardControl
		{
			get
			{
				return GetValue(keyManagementCenterDashboardControl, ManagementCenterDashboardControlDefaultValue);
			}
			set
			{
				SetValue(keyManagementCenterDashboardControl, value);
			}
		}
		#endregion
	}
}
