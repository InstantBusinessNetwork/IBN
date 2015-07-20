using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

using Mediachase.Ibn.Configuration;
using Mediachase.Net.Mail;

namespace Mediachase.Ibn.WebAsp
{
	#region enum EmailType
	public enum EmailType
	{
		TrialNewRequest,
		UserActivation,
		UserActivated,
		TrialActivated,
		UserAfterOneWeek,
		UserOneWeekBefore,
		UserOneDayBefore,
		TrialOneDayBefore,
		TrialDeactivated,
		UserAfterOneDayIM,
		OperatorCompanyDeactivatedDayBefore,
		OperatorTariffRequest,
		OperatorCompanyDeactivated,
		Client1DayZeroBalance,
		Client3DayZeroBalance,
		Client7DayZeroBalance,
		ClientZeroBalance,
		ClientBalanceUp
	}
	#endregion

	#region enum CompanyCategory
	public enum CompanyCategory
	{
		Outdated = 1,
		Expired = 2,
		OneDayBeforeEnd = 3,
		OneWeekBeforeEnd = 4,
		OneWeekAfterStart = 5,
		OneDayNoIM = 6,
		BillableNdaysBefore = 7,
		BillableNegativeBalance = 8,
		BillableForDeactivate = 9
	}
	#endregion

	#region class UserInfo
	public class UserInfo
	{
		public string Login;
		public string Password;
		public string Name;
		public string Email;
	}
	#endregion

	#region enum CompanyType
	public enum CompanyType
	{
		Billable = 1,
		Trial = 2
	}
	#endregion

	/// <summary>
	/// Summary description for CManage.
	/// </summary>
	public class CManage
	{
		private const int TimeInterval = 25;

		public const string keyCompanyMaxUsers = "company.max_users";
		public const string keyCompanyMaxExternalUsers = "company.max_external_users";
		public const string keyCompanyDatabaseSize = "company.databasesize";
		public const string keyCompanyType = "company.type";
		public const string keyCompanyEndDate = "company.end_date";

		#region GetCompany
		/// <summary>
		/// Reader returns fields:
		///		company_uid, company_name, domain, creation_date, company_type, end_date, is_active,
		///		contact_name, contact_phone, contact_email, send_spam, tariffId, tariffName, dailyCost30, symbol,
		///		balance, discount, creditLimit, tariffStartDate, alertThreshold
		/// </summary>
		public static IDataReader GetCompany(Guid company_uid)
		{
			return DBHelper.RunSPReturnDataReader("ASP_COMPANY_GET",
				DBHelper.mp("@company_type", SqlDbType.Int, 0),
				DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, company_uid));
		}
		#endregion

		#region GetCompaniesDataTable
		/// <summary>
		/// DataTable returns fields:
		///		company_uid, company_name, domain, creation_date, company_type, end_date, is_active,
		///		contact_name, contact_phone, contact_email, send_spam, tariffId, tariffName, symbol,
		///		balance, discount, creditLimit, tariffStartDate, alertThreshold, monthlyCost
		/// </summary>
		/// <returns></returns>
		public static DataTable GetCompaniesDataTable()
		{
			return DBHelper.RunSPReturnDataTable("ASP_COMPANY_GET",
				DBHelper.mp("@company_type", SqlDbType.Int, 0),
				DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, Guid.Empty));
		}
		#endregion

		#region UpdateTrialCompany
		/// <summary>
		/// Updates company information.
		/// </summary>
		/// <returns></returns>
		public static void UpdateTrialCompany(Guid companyUid,
			string CompanyName, string DomainName, string scheme, string port,
			byte companyType, DateTime startDate, DateTime deactivationDate, bool isActive,
			string contactName, string contactPhone, string contactEmail,
			int maxUsers, int maxExternalUsers, int maxDiskSpace,
			bool sendSpam, bool checkChangeDomain)
		{

			bool newTran = DBHelper.BeginTransaction();
			try
			{
				DBHelper.RunSP("ASP_COMPANY_ADD",
					DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, companyUid),
					DBHelper.mp("@company_name", SqlDbType.NVarChar, 100, CompanyName),
					DBHelper.mp("@domain", SqlDbType.NVarChar, 255, DomainName),
					DBHelper.mp("@company_type", SqlDbType.TinyInt, companyType),
					DBHelper.mp("@start_date", SqlDbType.DateTime, startDate),
					DBHelper.mp("@end_date", SqlDbType.DateTime, deactivationDate),
					DBHelper.mp("@is_active", SqlDbType.Bit, isActive),
					DBHelper.mp("@contact_name", SqlDbType.NVarChar, 100, contactName),
					DBHelper.mp("@contact_phone", SqlDbType.NVarChar, 100, contactPhone),
					DBHelper.mp("@contact_email", SqlDbType.NVarChar, 100, contactEmail),
					DBHelper.mp("@send_spam", SqlDbType.Bit, sendSpam),
					DBHelper.mp("@tariffId", SqlDbType.Int, -1),
					DBHelper.mp("@balance", SqlDbType.Money, 0m),
					DBHelper.mp("@discount", SqlDbType.Int, 0),
					DBHelper.mp("@creditLimit", SqlDbType.Money, 0m),
					DBHelper.mp("@alertThreshold", SqlDbType.Money, 0m),
					DBHelper.mp("@tariffStartDate", SqlDbType.DateTime, null)
					);

				IConfigurator config = Configurator.Create();
				ICompanyInfo company = config.GetCompanyInfo(companyUid.ToString());
				if (company.IsActive != isActive)
					config.ActivateCompany(companyUid.ToString(), isActive, false);

				if (checkChangeDomain && (company.Host != DomainName || company.Scheme != scheme || company.Port != port))
					config.ChangeCompanyAddress(companyUid.ToString(), scheme, DomainName, port, false);

				config.SetCompanyPropertyValue(companyUid.ToString(), keyCompanyMaxUsers, maxUsers.ToString());
				config.SetCompanyPropertyValue(companyUid.ToString(), keyCompanyMaxExternalUsers, maxExternalUsers.ToString());
				config.SetCompanyPropertyValue(companyUid.ToString(), keyCompanyDatabaseSize, maxDiskSpace.ToString());
				config.SetCompanyPropertyValue(companyUid.ToString(), keyCompanyType, companyType.ToString());
				config.SetCompanyPropertyValue(companyUid.ToString(), keyCompanyEndDate, deactivationDate.ToString(CultureInfo.InvariantCulture));

				DBHelper.Commit(newTran);
			}
			catch (Exception)
			{
				DBHelper.Rollback(newTran);
				throw;
			}
		}
		#endregion

		#region UpdateCompany
		/// <summary>
		/// Updates company information.
		/// </summary>
		/// <returns></returns>
		public static void UpdateCompany(Guid companyUid,
			string CompanyName, string DomainName, string scheme, string port,
			byte companyType, bool isActive,
			string contactName, string contactPhone, string contactEmail,
			int maxUsers, int maxExternalUsers, int maxDiskSpace, bool checkChangeDomain,
			int tariffId, decimal balance, int discount, decimal creditLimit, decimal alertThreshold,
			bool sendSpam)
		{
			DateTime? tariffStartDate = null;
			object endDate = DBNull.Value;
			int oldTariffId = -1;
			bool oldIsActive = false;

			// Read information about company
			using (IDataReader reader = GetCompany(companyUid))
			{
				if (reader.Read())
				{
					endDate = reader["end_date"];

					if (reader["tariffStartDate"] != DBNull.Value)
						tariffStartDate = (DateTime)reader["tariffStartDate"];

					if (reader["tariffId"] != DBNull.Value)
						oldTariffId = (int)reader["tariffId"];

					oldIsActive = (bool)reader["is_active"];
				}
			}

			if (tariffId > 0
				&& companyType == (byte)CompanyType.Billable
				&& isActive
				&& (tariffId != oldTariffId || tariffStartDate == null || !oldIsActive))
				tariffStartDate = DateTime.Now.Date;

			if (isActive)
				endDate = DBNull.Value;
			else if (endDate == DBNull.Value)
				endDate = DateTime.UtcNow;

			bool newTran = DBHelper.BeginTransaction();
			try
			{
				DBHelper.RunSP("ASP_COMPANY_ADD",
					DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, companyUid),
					DBHelper.mp("@company_name", SqlDbType.NVarChar, 100, CompanyName),
					DBHelper.mp("@domain", SqlDbType.NVarChar, 255, DomainName),
					DBHelper.mp("@company_type", SqlDbType.TinyInt, companyType),
					DBHelper.mp("@start_date", SqlDbType.DateTime, DBNull.Value),
					DBHelper.mp("@end_date", SqlDbType.DateTime, endDate),
					DBHelper.mp("@is_active", SqlDbType.Bit, isActive),
					DBHelper.mp("@contact_name", SqlDbType.NVarChar, 100, contactName),
					DBHelper.mp("@contact_phone", SqlDbType.NVarChar, 100, contactPhone),
					DBHelper.mp("@contact_email", SqlDbType.NVarChar, 100, contactEmail),
					DBHelper.mp("@send_spam", SqlDbType.Bit, sendSpam),
					DBHelper.mp("@tariffId", SqlDbType.Int, tariffId),
					DBHelper.mp("@balance", SqlDbType.Money, balance),
					DBHelper.mp("@discount", SqlDbType.Int, discount),
					DBHelper.mp("@creditLimit", SqlDbType.Money, creditLimit),
					DBHelper.mp("@alertThreshold", SqlDbType.Money, alertThreshold),
					DBHelper.mp("@tariffStartDate", SqlDbType.DateTime, tariffStartDate));

				IConfigurator config = Configurator.Create();
				ICompanyInfo company = config.GetCompanyInfo(companyUid.ToString());

				if (company.IsActive != isActive)
					config.ActivateCompany(companyUid.ToString(), isActive, false);

				if (checkChangeDomain && (company.Host != DomainName || company.Scheme != scheme || company.Port != port.ToString()))
					config.ChangeCompanyAddress(companyUid.ToString(), scheme, DomainName, port, false);

				config.SetCompanyPropertyValue(companyUid.ToString(), keyCompanyMaxUsers, maxUsers.ToString());
				config.SetCompanyPropertyValue(companyUid.ToString(), keyCompanyMaxExternalUsers, maxExternalUsers.ToString());
				config.SetCompanyPropertyValue(companyUid.ToString(), keyCompanyDatabaseSize, maxDiskSpace.ToString());
				config.SetCompanyPropertyValue(companyUid.ToString(), keyCompanyType, companyType.ToString());

				DBHelper.Commit(newTran);
			}
			catch (Exception)
			{
				DBHelper.Rollback(newTran);
				throw;
			}
		}
		#endregion

		#region Class Usr
		public class Usr
		{
			public int PrincipalId;
			public string Login;
			public string Password;
			public string FirstName;
			public string LastName;
			public string Email;
			public string Salt;
			public string Hash;
			public int OldIMGroupId;
			public int NewIMGroupId;
			public int OldOriginalId;
			public int NewOriginalId;
			public bool IsActive;
			public int PortalLoginsCount;

			public Usr()
			{
			}
		}
		#endregion

		#region CompanyExists
		public static bool CompanyExists(string domain)
		{
			return Configurator.Create().CheckIfHostIsRegistered(domain);
		}
		#endregion


		#region ASPCreateTrialCompany
		public static Guid ASPCreateTrialCompany(int RequestID)
		{
			string companyName;
			string domain;
			string firstName;
			string lastName;
			string email;
			string phone;
			string login;
			string password;
			string defaultLanguage;

			// Load request info
			using (IDataReader r = DBTrialRequest.Get(RequestID))
			{
				r.Read();
				companyName = r["CompanyName"].ToString();
				domain = r["Domain"].ToString();
				firstName = r["FirstName"].ToString();
				lastName = r["LastName"].ToString();
				email = r["EMail"].ToString();
				phone = r["Phone"].ToString();
				login = r["Login"].ToString();
				password = r["Password"].ToString();
				defaultLanguage = r["Locale"].ToString();
			}

			AspSettings settings = AspSettings.Load();

			DateTime startDate = DateTime.UtcNow;
			DateTime endDate = startDate.AddDays(settings.TrialPeriod);

			Guid companyUid;
			bool newTran = DBHelper.BeginTransaction();
			try
			{
				string id = Configurator.Create().CreateCompany(
					companyName,
					domain,
					defaultLanguage,
					true,
					settings.IisIpAddress,
					settings.IisPort,
					settings.DefaultTrialPool,
					login,
					password,
					firstName,
					lastName,
					email);

				companyUid = new Guid(id);

				UpdateTrialCompany(companyUid, companyName, domain, "http", "", (int)CompanyType.Trial, startDate, endDate, true, String.Concat(firstName, " ", lastName), phone, email, settings.MaxUsers, settings.MaxExternalUsers, settings.MaxHDD, true, false);

				DBTrialRequest.SetActive(RequestID, companyUid);

				DBHelper.Commit(newTran);
			}
			catch (Exception)
			{
				DBHelper.Rollback(newTran);
				throw;
			}

			return companyUid;
		}
		#endregion

		#region CompanyGetVariables
		public static TemplateVariables CompanyGetVariables(Guid CompanyUid)
		{
			TemplateVariables vars = new TemplateVariables();
			string dbName = string.Empty;
			DateTime dtEnd = DateTime.MinValue;

			ICompanyInfo company = Configurator.Create().GetCompanyInfo(CompanyUid.ToString());
			int port = (string.IsNullOrEmpty(company.Port) ? -1 : int.Parse(company.Port, CultureInfo.InvariantCulture));
			UriBuilder uri = new UriBuilder(company.Scheme, company.Host, port);

			vars["Domain"] = company.Host;
			vars["PortalLink"] = uri.ToString();
			vars["Locale"] = company.DefaultLanguage.Locale;

			using (IDataReader r = GetCompany(CompanyUid))
			{
				if (r != null)
				{
					if (r.Read())
					{
						vars["CompanyName"] = r["company_name"].ToString();
						vars["ContactName"] = r["contact_name"].ToString();
						vars["ContactPhone"] = r["contact_phone"].ToString();
						vars["ContactEmail"] = r["contact_email"].ToString();

						if (r["tariffName"] != DBNull.Value)
						{
							vars["Tariff"] = r["tariffName"].ToString();

							decimal credit = (decimal)r["CreditLimit"];
							decimal balance = (decimal)r["balance"];
							decimal dailyCost = (decimal)r["dailyCost30"];
							int discount = (int)r["discount"];

							vars["Balance"] = balance.ToString("f");
							vars["Credit"] = credit.ToString("f");
							int daysBeforeEnd = (int)((credit + balance) / (dailyCost - dailyCost * discount / 100m));
							if (daysBeforeEnd < 0)
								daysBeforeEnd = 0;
							vars["DaysBeforeEnd"] = daysBeforeEnd.ToString();
						}
						else
						{
							vars["Tariff"] = string.Empty;

							vars["Balance"] = string.Empty;
							vars["Credit"] = string.Empty;
							vars["DaysBeforeEnd"] = string.Empty;
						}

						if (r["symbol"] != DBNull.Value)
							vars["CurrencySymbol"] = r["symbol"].ToString();
						else
							vars["CurrencySymbol"] = "";

						object o = r["end_date"];
						if (o != null && o != DBNull.Value)
							dtEnd = (DateTime)o;
					}
				}
			}

			vars["EndDate"] = dtEnd.ToString("D", new CultureInfo(company.DefaultLanguage.Locale, true));

			return vars;
		}
		#endregion

		#region * E-Mail *
		#region SendEmail
		public static void SendEmail(string recipient, EmailType type, TemplateVariables vars)
		{
			string subject;
			string body = "";

			FileInfo[] attachments = null;

			switch (type)
			{
				case EmailType.UserActivation:
					body = LoadMessage("UserActivation.htm", vars, out subject);
					if (subject == null)
						subject = "Mediachase IBN Trial - Instructions for Activation.";
					attachments = LoadEmailAttachments("UserActivation", vars);
					break;
				case EmailType.UserActivated:
					body = LoadMessage("UserActivated.htm", vars, out subject);
					if (subject == null)
						subject = "Mediachase IBN Trial – Getting Started.";
					attachments = LoadEmailAttachments("UserActivated", vars);
					break;
				case EmailType.UserAfterOneDayIM:
					body = LoadMessage("UserAfterOneDayIM.htm", vars, out subject);
					if (subject == null)
						subject = "Additional Tips and Information for Your IBN Trial.";
					attachments = LoadEmailAttachments("UserAfterOneDayIM", vars);
					break;
				case EmailType.UserAfterOneWeek:
					body = LoadMessage("UserAfterOneWeek.htm", vars, out subject);
					if (subject == null)
						subject = "Additional Tips and Information for Your IBN Trial.";
					attachments = LoadEmailAttachments("UserAfterOneWeek", vars);
					break;
				case EmailType.UserOneWeekBefore:
					body = LoadMessage("UserOneWeekBefore.htm", vars, out subject);
					if (subject == null)
						subject = "IBN Trial - One Week Left.";
					attachments = LoadEmailAttachments("UserOneWeekBefore", vars);
					break;
				case EmailType.UserOneDayBefore:
					body = LoadMessage("UserOneDayBefore.htm", vars, out subject);
					if (subject == null)
						subject = "IBN Trial - Trial Period has Completed.";
					attachments = LoadEmailAttachments("UserOneDayBefore", vars);
					break;
				case EmailType.TrialNewRequest:
					body = LoadMessage("TrialNewRequest.htm", vars, out subject);
					if (subject == null)
						subject = "Mediachase IBN Trial - New Trial Request.";
					attachments = LoadEmailAttachments("TrialNewRequest", vars);
					break;
				case EmailType.TrialActivated:
					body = LoadMessage("TrialActivated.htm", vars, out subject);
					if (subject == null)
						subject = "IBN Trial Alert – Customer has Activated Trial.";
					attachments = LoadEmailAttachments("TrialActivated", vars);
					break;
				case EmailType.TrialOneDayBefore:
					body = LoadMessage("TrialOneDayBefore.htm", vars, out subject);
					if (subject == null)
						subject = "IBN Trial - Trial Period has Completed.";
					attachments = LoadEmailAttachments("TrialOneDayBefore", vars);
					break;
				case EmailType.TrialDeactivated:
					body = LoadMessage("TrialDeactivated.htm", vars, out subject);
					if (subject == null)
						subject = "IBN Trial - Account Deactivated.";
					attachments = LoadEmailAttachments("TrialDeactivated", vars);
					break;
				case EmailType.OperatorCompanyDeactivatedDayBefore:
					body = LoadMessage("OperatorCompanyDeactivatedDayBefore.htm", vars, out subject);
					if (subject == null)
						subject = "Balance is too low.";
					attachments = LoadEmailAttachments("OperatorCompanyDeactivatedDayBefore", vars);
					break;
				case EmailType.OperatorCompanyDeactivated:
					body = LoadMessage("OperatorCompanyDeactivated.htm", vars, out subject);
					if (subject == null)
						subject = "A company has been deactivated.";
					attachments = LoadEmailAttachments("OperatorCompanyDeactivated", vars);
					break;
				case EmailType.OperatorTariffRequest:
					body = LoadMessage("OperatorTariffRequest.htm", vars, out subject);
					if (subject == null)
						subject = "Customer has requested a new Tariff.";
					attachments = LoadEmailAttachments("OperatorTariffRequest", vars);
					break;
				case EmailType.Client1DayZeroBalance:
					body = LoadMessage("Client1DayZeroBalance.htm", vars, out subject);
					if (subject == null)
						subject = "Balance is too low. 1 day left.";
					attachments = LoadEmailAttachments("Client1DayZeroBalance", vars);
					break;
				case EmailType.Client3DayZeroBalance:
					body = LoadMessage("Client3DayZeroBalance.htm", vars, out subject);
					if (subject == null)
						subject = "Balance is too low. 3 days left.";
					attachments = LoadEmailAttachments("Client3DayZeroBalance", vars);
					break;
				case EmailType.Client7DayZeroBalance:
					body = LoadMessage("Client7DayZeroBalance.htm", vars, out subject);
					if (subject == null)
						subject = "Balance is too low. 7 days left.";
					attachments = LoadEmailAttachments("Client7DayZeroBalance", vars);
					break;
				case EmailType.ClientZeroBalance:
					body = LoadMessage("ClientZeroBalance.htm", vars, out subject);
					if (subject == null)
						subject = "Balance is negative.";
					attachments = LoadEmailAttachments("ClientZeroBalance", vars);
					break;
				case EmailType.ClientBalanceUp:
					body = LoadMessage("ClientBalanceUp.htm", vars, out subject);
					if (subject == null)
						subject = "Payment was received.";
					attachments = LoadEmailAttachments("ClientBalanceUp", vars);
					break;
				default:
					subject = "";
					break;
			}
			SendEmail(recipient, subject, body, attachments);
		}

		public static void SendEmail(string to, string subject, string body, FileInfo[] attachments)
		{
			AspSettings settings = AspSettings.Load();

			SmtpClient smtpClient = new SmtpClient(settings.SmtpServer, settings.SmtpPort);
			smtpClient.SecureConnectionType = settings.SmtpSecureConnection;
			smtpClient.Authenticate = settings.SmtpAuthenticate;
			smtpClient.User = settings.SmtpUser;
			smtpClient.Password = settings.SmtpPassword;

			MailMessage message = new MailMessage();
			message.From = new MailAddress(settings.EmailFrom);
			message.To.Add(new MailAddress(to));
			message.Subject = subject;
			message.Body = body;
			message.IsBodyHtml = true;

			if (attachments != null)
			{
				foreach (FileInfo fi in attachments)
				{
					message.Attachments.Add(new Attachment("application/octet-stream", fi.FullName));
				}
			}

			smtpClient.Send(message);

			if (!string.IsNullOrEmpty(message.ErrorMessage))
				Log.WriteError(message.ErrorMessage);
		}
		#endregion

		#region LoadEmailAttach
		private static FileInfo[] LoadEmailAttachments(string Name, TemplateVariables vars)
		{
			FileInfo[] ret = null;

			string dirPath = string.Format("{0}email\\{1}\\{2}",
				HttpRuntime.AppDomainAppPath, vars["Locale"], Name);
			try
			{
				if (Directory.Exists(dirPath))
				{
					DirectoryInfo di = new DirectoryInfo(dirPath);
					ret = di.GetFiles();
				}
			}
			catch { }

			return ret;
		}
		#endregion

		#region LoadMessage
		private static string LoadMessage(string fileName, TemplateVariables vars, out string subject)
		{
			string filePath = string.Format("{0}email\\{1}\\{2}",
				HttpRuntime.AppDomainAppPath, vars["Locale"], fileName);
			TextReader tr = File.OpenText(filePath);
			string text = tr.ReadToEnd();
			tr.Close();

			// Replace variables names with their values.
			int cur = 0;
			StringBuilder sb = new StringBuilder();
			while (cur < text.Length)
			{
				int i1 = text.IndexOf("[=", cur);
				if (-1 == i1)
					break;

				sb.Append(text, cur, i1 - cur);
				cur = i1;

				int i2 = text.IndexOf("=]", i1);
				if (-1 == i2)
					break;

				int length = i2 - i1 - 2;
				if (length < 0)
					break;

				string name = text.Substring(i1 + 2, length);
				string val = vars[name];
				if (val != null)
					sb.Append(val);
				cur = i2 + 2;
			}
			sb.Append(text, cur, text.Length - cur);

			string retVal = sb.ToString();

			subject = null;

			int titleStart = retVal.IndexOf("<title>", StringComparison.OrdinalIgnoreCase);
			int titleEnd = retVal.IndexOf("</title>", StringComparison.OrdinalIgnoreCase);
			if (titleStart != -1 && titleStart < titleEnd)
			{
				titleStart += 7; // length of <title>
				subject = retVal.Substring(titleStart, titleEnd - titleStart);
			}

			return retVal;
		}

		private static string LoadMessage(string fileName, TemplateVariables vars)
		{
			string subject;
			return LoadMessage(fileName, vars, out subject);
		}
		#endregion
		#endregion

		#region * Resellers *
		#region ResellerGet
		/// <summary>
		/// Reader returns fields:
		/// ResellerId, Title, Guid, ContactName, ContactEmail, ContactPhone, 
		/// CommissionPercentage
		/// </summary>
		public static IDataReader ResellerGet(int ResellerId)
		{
			return DBHelper.RunSPReturnDataReader("ASP_TRIAL_RESELLER_GET",
				DBHelper.mp("@ResellerId", SqlDbType.Int, ResellerId)
				);
		}
		#endregion

		#region ResellerGetDataTable
		/// <summary>
		/// DataTable returns fields:
		/// ResellerId, Title, Guid, ContactName, ContactEmail, ContactPhone, 
		/// </summary>
		public static DataTable ResellerGetDataTable()
		{
			return DBHelper.RunSPReturnDataTable("ASP_TRIAL_RESELLER_GET",
				DBHelper.mp("@ResellerId", SqlDbType.Int, 0)
				);
		}
		#endregion

		#region ResellerCreateUpdate
		public static int ResellerCreateUpdate(
			int ResellerId,
			string Title,
			string ContactName,
			string ContactEmail,
			string ContactPhone,
			int CommissionPercentage
			)
		{
			object oContactName = ContactName != null ? (object)ContactName : DBNull.Value;
			object oContactEmail = ContactEmail != null ? (object)ContactEmail : DBNull.Value;
			object oContactPhone = ContactPhone != null ? (object)ContactPhone : DBNull.Value;

			return DBHelper.RunSPReturnInteger("ASP_TRIAL_RESELLER_CREATE_UPDATE",
				DBHelper.mp("@ResellerId", SqlDbType.Int, ResellerId),
				DBHelper.mp("@Title", SqlDbType.NVarChar, 255, Title),
				DBHelper.mp("@ContactName", SqlDbType.NVarChar, 255, oContactName),
				DBHelper.mp("@ContactEmail", SqlDbType.NVarChar, 100, oContactEmail),
				DBHelper.mp("@ContactPhone", SqlDbType.NVarChar, 50, oContactPhone),
				DBHelper.mp("@CommPerc", SqlDbType.Int, CommissionPercentage)
				);
		}
		#endregion
		#endregion

		#region GetTotalStatistic
		/// <summary>
		/// Reader returns fields:
		/// CompaniesCount, ActiveCount, BillableCount, ActiveBillableCount, TrialCount,
		/// ActiveTrialCount, PendingTrialsCount
		/// </summary>
		public static IDataReader GetTotalStatistic()
		{
			return DBHelper.RunSPReturnDataReader("ASP_REP_GET_TOTAL_STATISTICS");
		}
		#endregion

		#region GetUsersDataTable
		/// <summary>
		/// PrincipalId, Login, FirstName, LastName, Email, IMGroupId, OriginalId
		/// </summary>
		public static DataTable GetUsersDataTable(Guid companyUid)
		{
			DBHelper.SwitchToPortalDatabase(GetDbNameByCompanyId(companyUid));
			try
			{
				if (IsPortalInitialized())
					return DBHelper.RunSPReturnDataTable("UsersGetActive",
						DBHelper.mp("@Keyword", SqlDbType.NVarChar, 100, ""));
				else
					return null;
			}
			finally
			{
				DBHelper.SwitchToAspDatabase();
			}
		}
		#endregion

		#region GetPortalUsers
		internal static UserInfo[] GetPortalUsers(string dbName)
		{
			ArrayList users = new ArrayList();

			DBHelper.SwitchToPortalDatabase(dbName);
			try
			{
				if (IsPortalInitialized())
				{
					using (IDataReader reader = DBHelper.RunSPReturnDataReader("UsersGetActive",
						DBHelper.mp("@Keyword", SqlDbType.NVarChar, 100, "")))
					{
						while (reader.Read())
						{
							UserInfo ui = new UserInfo();

							ui.Login = reader["Login"].ToString();
							ui.Password = "";
							ui.Name = string.Concat(reader["LastName"].ToString(), " ", reader["FirstName"].ToString());
							ui.Email = reader["Email"].ToString();

							users.Add(ui);
						}
					}
				}
			}
			finally
			{
				DBHelper.SwitchToAspDatabase();
			}
			return users.ToArray(typeof(UserInfo)) as UserInfo[];
		}
		#endregion

		#region ExportExcel
		public static void ExportExcel(System.Web.UI.Control ctrl, string filename)
		{
			HttpResponse Response = HttpContext.Current.Response;
			Response.Clear();
			Response.Charset = "utf-8";
			Response.AddHeader("Content-Type", "application/octet-stream");
			Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", filename));
			System.IO.StringWriter stringWrite = new System.IO.StringWriter();
			System.Web.UI.HtmlTextWriter htmlWrite = new System.Web.UI.HtmlTextWriter(stringWrite);
			ctrl.RenderControl(htmlWrite);
			string Header = "<html><head><meta http-equiv=\"Content-Type\" content=\"application/octet-stream; charset=utf-8\"></head><body>";
			string Footer = "</body></html>";
			Response.Write(String.Concat(Header, stringWrite.ToString(), Footer));
			Response.End();
		}
		#endregion

		#region Search
		/// <summary>
		/// company_uid, company_name, domain, creation_date, company_type, [start_date], end_date, is_active
		/// </summary>
		public static IDataReader GetCompanyByKeyword(string keyword)
		{
			return DBHelper.RunSPReturnDataReader("ASP_GET_COMPANY_BY_KEYWORD",
				DBHelper.mp("@keyword", SqlDbType.NVarChar, 100, keyword));
		}

		public static IDataReader GetTrialRequestByKeyword(string keyword)
		{
			return DBHelper.RunSPReturnDataReader("ASP_GET_TRIAL_BY_KEYWORD",
				DBHelper.mp("@keyword", SqlDbType.NVarChar, 100, keyword));
		}

		public static IDataReader GetResellerByKeyword(string keyword)
		{
			return DBHelper.RunSPReturnDataReader("ASP_GET_RESSELLER_BY_KEYWORD",
				DBHelper.mp("@keyword", SqlDbType.NVarChar, 100, keyword));
		}
		#endregion


		#region CompanyCreate(...)
		internal static Guid CompanyCreate(
			string companyName
			, string domainName
			, string defaultLocale
			, bool isActive
			, int maxUsers
			, int maxExternalUsers
			, int maxDiskSpace
			, string contactName
			, string contactPhone
			, string contactEmail
			, string adminFirstName
			, string adminLastName
			, string adminLogin
			, string adminPassword
			, string adminEmail
			, bool isTrial
			, DateTime trialStartDate
			, DateTime trialEndDate
			, string applicationPool
			)
		{
			if (!isTrial)
			{
				trialStartDate = DateTime.MinValue;
				trialEndDate = DateTime.MaxValue;
			}

			AspSettings settings = AspSettings.Load();

			if (applicationPool == null)
			{
				if (isTrial)
					applicationPool = settings.DefaultTrialPool;
				else
					applicationPool = settings.DefaultBillablePool;
			}

			if (maxUsers <= 0)
				maxUsers = settings.MaxUsers;
			if (maxExternalUsers <= 0)
				maxExternalUsers = settings.MaxExternalUsers;
			if (maxDiskSpace <= 0)
				maxDiskSpace = settings.MaxHDD;

			if (string.IsNullOrEmpty(adminFirstName))
				adminFirstName = "System";
			if (string.IsNullOrEmpty(adminLastName))
				adminLastName = "User";

			Guid companyUid;
			bool newTran = DBHelper.BeginTransaction();
			try
			{
				IConfigurator config = Configurator.Create();
				string id = config.CreateCompany(
					companyName,
					domainName,
					defaultLocale,
					isActive,
					settings.IisIpAddress,
					settings.IisPort,
					applicationPool,
					adminLogin,
					adminPassword,
					adminFirstName,
					adminLastName,
					adminEmail);
				companyUid = new Guid(id);

				if (isTrial)
				{
					UpdateTrialCompany(companyUid, companyName, domainName, "http", "", 2, trialStartDate, trialEndDate, isActive, contactName, contactPhone, contactEmail, maxUsers, maxExternalUsers, maxDiskSpace, true, false);
				}
				else
				{
					UpdateCompany(companyUid, companyName, domainName, "http", "", 1, isActive, contactName, contactPhone, contactEmail, maxUsers, maxExternalUsers, maxDiskSpace, false, -1, 0m, 0, 0m, 0m, true);
				}

				TemplateVariables vars = CManage.CompanyGetVariables(companyUid);
				vars["Login"] = adminLogin;
				vars["Password"] = adminPassword;
				vars["TrialUsers"] = maxUsers.ToString();
				vars["TrialDiskSpace"] = maxDiskSpace.ToString();
				vars["TrialPeriod"] = settings.TrialPeriod.ToString();
				vars["Locale"] = defaultLocale;

				if (isTrial)
				{
					try
					{
						SendEmail(adminEmail, EmailType.UserActivated, vars);
					}
					catch (Exception ex)
					{
						Log.WriteError(ex.ToString());
					}
				}

				DBHelper.Commit(newTran);
			}
			catch (Exception)
			{
				DBHelper.Rollback(newTran);
				throw;
			}

			return companyUid;
		}
		#endregion

		#region internal GetDates(string range, out DateTime start, out DateTime finish, string fromCustom, string toCustom)
		internal static void GetDates(string range, out DateTime start, out DateTime finish, string fromCustom, string toCustom)
		{
			switch (range)
			{
				case "1": //Today
					start = DateTime.Today;
					finish = DateTime.Now;
					break;
				case "2": //Yesterday
					finish = DateTime.Today;
					start = finish.AddDays(-1);
					break;
				case "3": //ThisWeek
					start = DateTime.Today.AddDays(1 - (int)DateTime.Now.DayOfWeek);
					finish = DateTime.Now;
					break;
				case "4": //LastWeek
					finish = DateTime.Today.AddDays(1 - (int)DateTime.Now.DayOfWeek);
					start = finish.AddDays(-7);
					break;
				case "5": //ThisMonth
					start = DateTime.Today.AddDays(1 - DateTime.Now.Day);
					finish = DateTime.Now;
					break;
				case "6": //LastMonth
					finish = DateTime.Today.AddDays(1 - DateTime.Now.Day);
					start = finish.AddMonths(-1);
					break;
				case "7": //ThisYear
					start = DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear);
					finish = DateTime.Now;
					break;
				case "8": //LastYear
					finish = DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear);
					start = finish.AddYears(-1);
					break;
				case "9": //Custom
					if (!DateTime.TryParse(fromCustom, out start))
						start = DefaultStartDate;
					if (!DateTime.TryParse(toCustom, out finish))
						finish = DefaultEndDate;
					else
						finish = finish.AddDays(1);
					break;
				default:
					start = DefaultStartDate;
					finish = DefaultEndDate;
					break;
			}
		}
		#endregion
		#region internal DefaultStartDate
		internal static DateTime DefaultStartDate
		{
			get
			{
				return DateTime.Now.AddYears(-TimeInterval);
			}
		}
		#endregion
		#region internal DefaultEndDate
		internal static DateTime DefaultEndDate
		{
			get
			{
				return DateTime.Now.AddYears(TimeInterval);
			}
		}
		#endregion

		#region CreateUserTicket
		/// <summary>
		/// PrincipalId, Login, Email, Ticket
		/// </summary>
		public static IDataReader CreateUserTicket(Guid companyUid, int userId)
		{
			DBHelper.SwitchToPortalDatabase(GetDbNameByCompanyId(companyUid));
			try
			{
				return DBHelper.RunSPReturnDataReader("UserCreateTicket",
					DBHelper.mp("@UserId", SqlDbType.Int, userId));
			}
			finally
			{
				DBHelper.SwitchToAspDatabase();
			}
		}
		#endregion

		#region GetDbNameByCompanyId
		private static string GetDbNameByCompanyId(Guid companyUid)
		{
			return Configurator.Create().GetCompanyInfo(companyUid.ToString()).Database;
		}
		#endregion

		#region DeleteCompany(companyUid)
		public static void DeleteCompany(Guid companyUid)
		{
			bool newTran = DBHelper.BeginTransaction();
			try
			{
				DBHelper.RunSP("ASP_Company_Delete",
					DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, companyUid));

				Configurator.Create().DeleteCompany(companyUid.ToString(), true);

				DBHelper.Commit(newTran);
			}
			catch (Exception)
			{
				DBHelper.Rollback(newTran);
				throw;
			}
		}
		#endregion

		#region SettingsGet
		/// <summary>
		/// Reader returns fields:
		///		MaxHDD, MaxUsers, MaxExternalUsers, TrialPeriod, 
		/// 	EmailFrom, OperatorEmail, DnsParentDomain, IisIpAddress, IisPort,
		/// 	AutoDeactivateExpired, AutoDeleteOutdated, OutdatePeriod, SendSpam,
		/// 	SendSpamOneDayAfter, SendSpamOneWeekAfter, SendSpamOneWeekBefore, SendSpamOneDayBefore,
		/// 	OneDayAfterPeriod, OneWeekAfterPeriod, OneWeekBeforePeriod, OneDayBeforePeriod,
		/// 	UseTariffs, SendBillableSpam, SendBillableSpam7day, SendBillableSpam3day, 
		/// 	SendBillableSpam1day, SendBillableSpamNegativeBalance, AutoDeactivateUnpaid,
		/// 	SmtpServer, SmtpPort, SmtpSecureConnection, SmtpAuthenticate, SmtpUser, SmtpPassword,
		/// 	DefaultTrialPool, DefaultBillablePool, DefaultTariff
		/// </summary>
		public static IDataReader SettingsGet()
		{
			return DBHelper.RunSPReturnDataReader("ASP_SETTINGS_GET");
		}
		#endregion

		#region SettingsUpdate
		public static void SettingsUpdate(
			int MaxHDD, int MaxUsers, int MaxExternalUsers, int TrialPeriod,
			string EmailFrom, string OperatorEmail,
			string DnsParentDomain, string IisIpAddress, int IisPort,
			bool AutoDeactivateExpired, bool AutoDeleteOutdated, int OutdatePeriod,
			bool SendSpam,
			bool SendSpamOneDayAfter, bool SendSpamOneWeekAfter, bool SendSpamOneWeekBefore, bool SendSpamOneDayBefore,
			int OneDayAfterPeriod, int OneWeekAfterPeriod, int OneWeekBeforePeriod, int OneDayBeforePeriod,
			bool UseTariffs, bool SendBillableSpam, bool SendBillableSpam7day, bool SendBillableSpam3day,
			bool SendBillableSpam1day, bool SendBillableSpamNegativeBalance, bool AutoDeactivateUnpaid,
			string SmtpServer, int SmtpPort, int SmtpSecureConnection, bool SmtpAuthenticate, string SmtpUser, string SmtpPassword,
			string DefaultTrialPool, string DefaultBillablePool, int DefaultTariff)
		{
			DBHelper.RunSP("ASP_SETTINGS_UPDATE",
				DBHelper.mp("@MaxHDD", SqlDbType.Int, MaxHDD),
				DBHelper.mp("@MaxUsers", SqlDbType.Int, MaxUsers),
				DBHelper.mp("@MaxExternalUsers", SqlDbType.Int, MaxExternalUsers),
				DBHelper.mp("@TrialPeriod", SqlDbType.Int, TrialPeriod),
				DBHelper.mp("@EmailFrom", SqlDbType.NVarChar, 100, EmailFrom),
				DBHelper.mp("@OperatorEmail", SqlDbType.NVarChar, 100, OperatorEmail),
				DBHelper.mp("@DnsParentDomain", SqlDbType.NVarChar, 100, DnsParentDomain),
				DBHelper.mp("@IisIpAddress", SqlDbType.NVarChar, 15, IisIpAddress),
				DBHelper.mp("@IisPort", SqlDbType.Int, IisPort),
				DBHelper.mp("@AutoDeactivateExpired", SqlDbType.Bit, AutoDeactivateExpired),
				DBHelper.mp("@AutoDeleteOutdated", SqlDbType.Bit, AutoDeleteOutdated),
				DBHelper.mp("@OutdatePeriod", SqlDbType.Int, OutdatePeriod),
				DBHelper.mp("@SendSpam", SqlDbType.Bit, SendSpam),
				DBHelper.mp("@SendSpamOneDayAfter", SqlDbType.Bit, SendSpamOneDayAfter),
				DBHelper.mp("@SendSpamOneWeekAfter", SqlDbType.Bit, SendSpamOneWeekAfter),
				DBHelper.mp("@SendSpamOneWeekBefore", SqlDbType.Bit, SendSpamOneWeekBefore),
				DBHelper.mp("@SendSpamOneDayBefore", SqlDbType.Bit, SendSpamOneDayBefore),
				DBHelper.mp("@OneDayAfterPeriod", SqlDbType.Int, OneDayAfterPeriod),
				DBHelper.mp("@OneWeekAfterPeriod", SqlDbType.Int, OneWeekAfterPeriod),
				DBHelper.mp("@OneWeekBeforePeriod", SqlDbType.Int, OneWeekBeforePeriod),
				DBHelper.mp("@OneDayBeforePeriod", SqlDbType.Int, OneDayBeforePeriod),
				DBHelper.mp("@UseTariffs", SqlDbType.Bit, UseTariffs),
				DBHelper.mp("@SendBillableSpam", SqlDbType.Bit, SendBillableSpam),
				DBHelper.mp("@SendBillableSpam7day", SqlDbType.Bit, SendBillableSpam7day),
				DBHelper.mp("@SendBillableSpam3day", SqlDbType.Bit, SendBillableSpam3day),
				DBHelper.mp("@SendBillableSpam1day", SqlDbType.Bit, SendBillableSpam1day),
				DBHelper.mp("@SendBillableSpamNegativeBalance", SqlDbType.Bit, SendBillableSpamNegativeBalance),
				DBHelper.mp("@AutoDeactivateUnpaid", SqlDbType.Bit, AutoDeactivateUnpaid),
				DBHelper.mp("@SmtpServer", SqlDbType.NVarChar, 250, SmtpServer),
				DBHelper.mp("@SmtpPort", SqlDbType.Int, SmtpPort),
				DBHelper.mp("@SmtpSecureConnection", SqlDbType.Int, SmtpSecureConnection),
				DBHelper.mp("@SmtpAuthenticate", SqlDbType.Int, SmtpAuthenticate),
				DBHelper.mp("@SmtpUser", SqlDbType.NVarChar, 100, SmtpUser),
				DBHelper.mp("@SmtpPassword", SqlDbType.NVarChar, 50, SmtpPassword),
				DBHelper.mp("@DefaultTrialPool", SqlDbType.NVarChar, 100, DefaultTrialPool),
				DBHelper.mp("@DefaultBillablePool", SqlDbType.NVarChar, 100, DefaultBillablePool),
				DBHelper.mp("@DefaultTariff", SqlDbType.Int, DefaultTariff)
				);
		}
		#endregion

		#region public static Guid[] GetCompaniesByCategory(CompanyCategory category)
		public static Guid[] GetCompaniesByCategory(CompanyCategory category, int period, bool checkSpamFlag)
		{
			List<Guid> list = new List<Guid>();

			using (IDataReader r = DBHelper.RunSPReturnDataReader("ASP_COMPANY_GET_BY_CATEGORY",
					DBHelper.mp("@Category", SqlDbType.Int, (int)category),
					DBHelper.mp("@Now", SqlDbType.DateTime, DateTime.UtcNow),
					DBHelper.mp("@Period", SqlDbType.Int, period),
					DBHelper.mp("@CheckSpamFlag", SqlDbType.Bit, checkSpamFlag)))
			{
				while (r.Read())
					list.Add(r.GetGuid(0));
			}
			return list.ToArray();
		}
		#endregion

		#region TrialRequestSetIsDeleted
		public static void TrialRequestSetIsDeleted(Guid companyUid)
		{
			DBHelper.RunSP("ASP_TRIAL_REQUEST_SET_ISDELETED",
				DBHelper.mp("@Company_uid", SqlDbType.UniqueIdentifier, companyUid));
		}
		#endregion

		#region DeactivateCompany
		public static void DeactivateCompany(Guid companyUid)
		{
			IConfigurator config = Configurator.Create();
			ICompanyInfo company = config.GetCompanyInfo(companyUid.ToString());

			bool newTran = DBHelper.BeginTransaction();
			try
			{
				DBHelper.RunSP("ASP_COMPANY_DEACTIVATE",
					DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, companyUid));

				if (company.IsActive)
					config.ActivateCompany(companyUid.ToString(), false, false);

				DBHelper.Commit(newTran);
			}
			catch (Exception)
			{
				DBHelper.Rollback(newTran);
				throw;
			}
		}
		#endregion

		#region * Notifications *
		internal static bool NotificationGet(Guid company_uid, EmailType type)
		{
			int notifID = NotificationGetID(type);
			return 0 != DBHelper.RunSPReturnInteger("ASP_TRIAL_NOTIFICATION_GET",
				DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, company_uid),
				DBHelper.mp("@notif_id", SqlDbType.Int, notifID)
				);
		}

		internal static void NotificationUpdate(Guid company_uid, EmailType type, bool value)
		{
			int notifID = NotificationGetID(type);
			DBHelper.RunSP("ASP_TRIAL_NOTIFICATION_UPDATE",
				DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, company_uid),
				DBHelper.mp("@notif_id", SqlDbType.Int, notifID),
				DBHelper.mp("@value", SqlDbType.Bit, value)
				);
		}

		private static int NotificationGetID(EmailType type)
		{
			int notifID = 0;
			switch (type)
			{
				case EmailType.UserAfterOneWeek:
					notifID = 1;
					break;
				case EmailType.UserOneWeekBefore:
					notifID = 2;
					break;
				case EmailType.UserOneDayBefore:
					notifID = 3;
					break;
				case EmailType.UserAfterOneDayIM:
					notifID = 4;
					break;
				case EmailType.TrialOneDayBefore:
					notifID = 5;
					break;
				default:
					throw new Exception("Invalid notification type.");
			}
			return notifID;
		}

		internal static DateTime GetNotificationClientSpamDate(Guid company_uid)
		{
			DateTime retval = DateTime.Now.AddYears(-1);
			using (IDataReader reader = DBHelper.RunSPReturnDataReader("ASP_TRIAL_NOTIFICATION_GET_INFO",
				DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, company_uid)))
			{
				if (reader.Read())
				{
					if (reader["clientSpamDate"] != DBNull.Value)
						retval = (DateTime)reader["clientSpamDate"];
				}
			}
			return retval;
		}

		internal static DateTime GetNotificationOperatorSpamDate(Guid company_uid)
		{
			DateTime retval = DateTime.Now.AddYears(-1);
			using (IDataReader reader = DBHelper.RunSPReturnDataReader("ASP_TRIAL_NOTIFICATION_GET_INFO",
				DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, company_uid)))
			{
				if (reader.Read())
				{
					if (reader["operatorSpamDate"] != DBNull.Value)
						retval = (DateTime)reader["operatorSpamDate"];
				}
			}
			return retval;
		}

		internal static void UpdateNotificationClientSpamDate(Guid company_uid, DateTime clientSpamDate)
		{
			DBHelper.RunSP("ASP_TRIAL_NOTIFICATION_UPDATE_CLIENT_SPAM_DATE",
				DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, company_uid),
				DBHelper.mp("@clientSpamDate", SqlDbType.DateTime, clientSpamDate));
		}

		internal static void UpdateNotificationOperatorSpamDate(Guid company_uid, DateTime operatorSpamDate)
		{
			DBHelper.RunSP("ASP_TRIAL_NOTIFICATION_UPDATE_OPERATOR_SPAM_DATE",
				DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, company_uid),
				DBHelper.mp("@operatorSpamDate", SqlDbType.DateTime, operatorSpamDate));
		}
		#endregion

		#region CheckImSessions
		public static bool CheckImSessions(string dbName)
		{
			bool retval = false;
			DBHelper.SwitchToPortalDatabase(dbName);
			try
			{
				if (IsPortalInitialized())
				{
					using (IDataReader reader =
						DBHelper.RunSPReturnDataReader("ASP_REP_GET_IMSESSIONS_COUNT_GET",
							DBHelper.mp("@StartDate", SqlDbType.DateTime, DateTime.UtcNow.AddYears(-1)),
							DBHelper.mp("@EndDate", SqlDbType.DateTime, DateTime.UtcNow.AddYears(1)),
							DBHelper.mp("@TimeOffset", SqlDbType.Int, 0)))
					{
						if (reader.Read())
							retval = true;
					}
				}
			}
			finally
			{
				DBHelper.SwitchToAspDatabase();
			}

			return retval;
		}
		#endregion

		#region ChangeDefaultAddress()
		public static void ChangeDefaultAddress(Guid companyUid, string scheme, string host, string port)
		{
			bool newTran = DBHelper.BeginTransaction();
			try
			{
				DBHelper.RunSP("ASP_COMPANY_UPDATE_DOMAIN",
					DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, companyUid),
					DBHelper.mp("@domain", SqlDbType.NVarChar, 255, host));

				Configurator.Create().ChangeCompanyAddress(companyUid.ToString(), scheme, host, port, false);

				DBHelper.Commit(newTran);
			}
			catch (Exception)
			{
				DBHelper.Rollback(newTran);
				throw;
			}
		}
		#endregion

		#region ActivateCompany()
		public static void ActivateCompany(Guid companyUid, bool isActive)
		{
			bool newTran = DBHelper.BeginTransaction();
			try
			{
				DBHelper.RunSP("ASP_COMPANY_UPDATE_IS_ACTIVE",
					DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, companyUid),
					DBHelper.mp("@is_active", SqlDbType.Bit, isActive));

				Configurator.Create().ActivateCompany(companyUid.ToString(), isActive, false);

				DBHelper.Commit(newTran);
			}
			catch (Exception)
			{
				DBHelper.Rollback(newTran);
				throw;
			}
		}
		#endregion

		#region UpdateCompanyType()
		public static void UpdateCompanyType(Guid companyUid, byte companyType)
		{
			bool newTran = DBHelper.BeginTransaction();
			try
			{
				DBHelper.RunSP("ASP_COMPANY_UPDATE_TYPE",
					DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, companyUid),
					DBHelper.mp("@company_type", SqlDbType.TinyInt, companyType));

				AspSettings settings = AspSettings.Load();
				if (companyType == (int)CompanyType.Trial)
				{
					UpdateCompanyTrialPeriod(companyUid, DateTime.Now, DateTime.Now.AddDays(settings.TrialPeriod));
					Configurator.Create().ChangeCompanyApplicationPool(companyUid.ToString(), settings.DefaultTrialPool);
				}
				else
				{
					Configurator.Create().ChangeCompanyApplicationPool(companyUid.ToString(), settings.DefaultBillablePool);
				}

				DBHelper.Commit(newTran);
			}
			catch (Exception)
			{
				DBHelper.Rollback(newTran);
				throw;
			}
		}
		#endregion

		#region UpdateCompanyTrialPeriod()
		public static void UpdateCompanyTrialPeriod(Guid companyUid, DateTime? startDate, DateTime? endDate)
		{
			DBHelper.RunSP("ASP_COMPANY_UPDATE_TRIAL_PERIOD",
				DBHelper.mp("@company_uid", SqlDbType.UniqueIdentifier, companyUid),
				DBHelper.mp("@start_date", SqlDbType.DateTime, startDate),
				DBHelper.mp("@end_date", SqlDbType.DateTime, endDate));
		}
		#endregion

		#region DeleteReseller
		public static void DeleteReseller(int resellerId)
		{
			DBHelper.RunSP("ASP_TRIAL_RESELLER_DELETE",
				DBHelper.mp("@resellerId", SqlDbType.Int, resellerId));
		}
		#endregion

		#region IsPortalInitialized
		/// <summary>
		/// Supposed that current database is portal
		/// </summary>
		/// <returns></returns>
		private static bool IsPortalInitialized()
		{
			bool retval = false;
			using (IDataReader reader = DBHelper.RunSQLReturnDataReader("SELECT [State] FROM [DatabaseVersion] WITH (NOLOCK)"))
			{
				if (reader.Read() && reader["State"].ToString() == "6")
					retval = true;
			}
			return retval;
		}
		#endregion

		#region GetCompanyInfo
		internal static CompanyInfo GetCompanyInfo(Guid companyUid)
		{
			IConfigurator config = Configurator.Create();
			ICompanyInfo company = config.GetCompanyInfo(companyUid.ToString());

			CompanyInfo info = new CompanyInfo();
			info.CompanyId = companyUid.ToString();
			info.IsActive = company.IsActive;
			info.Scheme = company.Scheme;
			info.Host = company.Host;
			info.Port = company.Port;

			int port = (string.IsNullOrEmpty(company.Port) ? -1 : int.Parse(company.Port, CultureInfo.InvariantCulture));
			UriBuilder uriBuilder = new UriBuilder(company.Scheme, company.Host, port);
			info.PortalUrl = uriBuilder.ToString();

			info.DatabaseSize = company.DatabaseSize;
			info.UsersCount = company.InternalUsersCount;
			info.MaxDatabaseSize = int.Parse(config.GetCompanyPropertyValue(companyUid.ToString(), CManage.keyCompanyDatabaseSize));
			info.MaxUsersCount = int.Parse(config.GetCompanyPropertyValue(companyUid.ToString(), CManage.keyCompanyMaxUsers));
			info.MaxExternalUsersCount = int.Parse(config.GetCompanyPropertyValue(companyUid.ToString(), CManage.keyCompanyMaxExternalUsers));
			if (config.GetCompanyPropertyValue(companyUid.ToString(), CManage.keyCompanyType) == ((byte)CompanyType.Trial).ToString())
			{
				info.IsTrial = true;
				string endDateString = config.GetCompanyPropertyValue(companyUid.ToString(), CManage.keyCompanyEndDate);

				if (!String.IsNullOrEmpty(endDateString))
					info.TrialEndDate = DateTime.Parse(endDateString, CultureInfo.InvariantCulture);
			}
			else
			{
				info.IsTrial = false;
			}


			using (IDataReader reader = CManage.GetCompany(companyUid))
			{
				if (reader.Read())
				{
					info.CompanyName = (string)reader["company_name"];

					if (reader["start_date"] != DBNull.Value && info.IsTrial)
						info.TrialStartDate = (DateTime)reader["start_date"];

					info.ContactName = (string)reader["contact_name"];
					info.ContactPhone = (string)reader["contact_phone"];
					info.ContactEmail = (string)reader["contact_email"];
					info.SendNotifications = (bool)reader["send_spam"];

					if (reader["tariffId"] != DBNull.Value)
					{
						info.TariffId = (int)reader["tariffId"];
						info.TariffName = (string)reader["tariffName"];
						if (reader["tariffStartDate"] != DBNull.Value)
							info.TariffStartDate = (DateTime)reader["tariffStartDate"];
						info.TariffCurrencySymbol = (string)reader["symbol"];
					}

					info.Balance = (decimal)reader["balance"];
					info.Discount = (int)reader["discount"];
					info.CreditLimit = (decimal)reader["creditLimit"];
					info.AlertThreshold = (decimal)reader["alertThreshold"];
				}
			}

			if (info.IsActive && !info.IsTrial && info.TariffId > 0)
			{
				using (IDataReader reader = Tariff.GetTariff(info.TariffId, 0))
				{
					if (reader.Read())
					{
						info.TariffDescription = (string)reader["description"];
						info.TariffMonthlyCost = (decimal)reader["monthlyCost"];
					}
				}
			}

			return info;
		}
		#endregion
	}
}
