using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace Mediachase.Ibn.WebAsp.WebServices
{
	#region enum TrialResult
	public enum TrialResult
	{
		Success,
		RequestPending,
		WaitingForActivation,
		Failed,
		UserRegistered,
		DomainExists,
		InvalidRequest,
		AlreadyActivated
	}
	#endregion

	/// <summary>
	/// Summary description for Trial.
	/// </summary>
	[WebService(Namespace = "http://ibnportal.ru/")]
	public class Trial : System.Web.Services.WebService
	{
		public Trial()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code

		//Required by the Web Services Designer 
		private IContainer components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		#region [Web] public TrialResult CompanyRequestTrial(...)
		[WebMethod]
		[Obsolete("Use CompanyRequestTrial2.")]
		public TrialResult CompanyRequestTrial(
			string companyName,
			string sizeOfGroup,
			string description,
			string domain,
			string firstName,
			string lastName,
			string email,
			string phone,
			string country,
			string login,
			string password,
			string resellerGuid,
			string xml,
			string locale,
			out int requestId,
			out string requestGuid
			)
		{
			return RequestTrial(companyName, sizeOfGroup, description, domain, firstName, lastName
				, email, phone, country, login, password, resellerGuid, xml 
				, locale, null, out requestId, out requestGuid);
		}
		#endregion
		#region [Web] public TrialResult CompanyRequestTrial2(...)
		[WebMethod]
		public TrialResult CompanyRequestTrial2(
			string companyName
			, string sizeOfGroup
			, string description
			, string domain
			, string firstName
			, string lastName
			, string email
			, string phone
			, string country
			, string login
			, string password
			, string resellerGuid
			, string xml
			, string locale
			, string referrer
			, out int requestId
			, out string requestGuid
			)
		{
			return RequestTrial(companyName, sizeOfGroup, description, domain, firstName, lastName
				, email, phone, country, login, password, resellerGuid, xml
				, locale, referrer, out requestId, out requestGuid);
		}
		#endregion

		#region [Web] ActivateTrialCompany
		[WebMethod]
		public TrialResult ActivateTrialCompany(
			int requestId
			, string requestGuid
			, out string portalUrl
			, out string login
			, out string password
			)
		{
			string companyId;
			return ActivateTrialCompany2(requestId, requestGuid, out portalUrl, out login, out password, out companyId);
		}
		#endregion

		#region [Web] ActivateTrialCompany2
		[WebMethod]
		public TrialResult ActivateTrialCompany2(
			int requestId
			, string requestGuid
			, out string portalUrl
			, out string login
			, out string password
			, out string companyId
			)
		{
			portalUrl = "";
			login = "";
			password = "";
			companyId = "";

			TrialResult ret = TrialResult.InvalidRequest;
			Guid guid = new Guid(requestGuid);
			if (DBTrialRequest.CheckGuid(requestId, guid))
			{
				bool isActive = false;
				using (IDataReader reader = DBTrialRequest.Get(requestId, true, false))
				{
					if (reader.Read())
					{
						isActive = (bool)reader["IsActive"];
						if (reader["Company_uid"] != DBNull.Value)
							companyId = (string)reader["Company_uid"];
					}
				}

				if (isActive)
				{
					ret = TrialResult.AlreadyActivated;
				}
				else
				{
					Guid companyUid = CManage.ASPCreateTrialCompany(requestId);
					ret = TrialResult.Success;
					companyId = companyUid.ToString();

					AspSettings settings = AspSettings.Load();
					TemplateVariables vars = CManage.CompanyGetVariables(companyUid);
					TemplateVariables varsR = DBTrialRequest.GetVariables(requestId);
					vars["Login"] = varsR["Login"];
					vars["Password"] = varsR["Password"];
					vars["TrialUsers"] = settings.MaxUsers.ToString();
					vars["TrialDiskSpace"] = settings.MaxHDD.ToString();
					vars["TrialPeriod"] = settings.TrialPeriod.ToString();

					portalUrl = vars["PortalLink"];

					if (!string.IsNullOrEmpty(vars["ContactEmail"]))
						CManage.SendEmail(vars["ContactEmail"], EmailType.UserActivated, vars);
					if (!string.IsNullOrEmpty(settings.OperatorEmail))
						CManage.SendEmail(settings.OperatorEmail, EmailType.TrialActivated, vars);

					login = varsR["Login"];
					password = varsR["Password"];
				}
			}
			return ret;
		}
		#endregion

		#region [Web] DomainExists
		[WebMethod]
		public bool DomainExists(string domain)
		{
			return CManage.CompanyExists(domain);
		}
		#endregion

		#region [Web] GetTrialPeriod()
		[WebMethod]
		public int GetTrialPeriod()
		{
			AspSettings settings = AspSettings.Load();
			return settings.TrialPeriod;
		}
		#endregion
		#region [Web] GetParentDomain()
		[WebMethod]
		public string GetParentDomain()
		{
			AspSettings settings = AspSettings.Load();
			return settings.DnsParentDomain;
		}
		#endregion

		#region [Web] GetCompanyInfo()
		[WebMethod]
		public CompanyInfo GetCompanyInfo(string companyId)
		{
			return CManage.GetCompanyInfo(new Guid(companyId));
		}
		#endregion

		#region [Web] AddPayment()
		[WebMethod]
		public void AddPayment(string companyId, decimal amount, string orderNo)
		{
			Tariff.AddPayment(new Guid(companyId), amount, orderNo);
		}
		#endregion

		#region private TrialResult RequestTrial(...)
		private TrialResult RequestTrial(
			string companyName
			, string sizeOfGroup
			, string description
			, string domain
			, string firstName
			, string lastName
			, string email
			, string phone
			, string country
			, string login
			, string password
			, string resellerGuid
			, string xml
			, string locale
			, string referrer
			, out int requestId
			, out string requestGuid
			)
		{
			TrialResult retVal = TrialResult.Failed;
			requestId = -1;
			requestGuid = string.Empty;

			AspSettings settings = AspSettings.Load();
			domain += "." + settings.DnsParentDomain;
			try
			{
				//if (CManage.IsUserRegistered(settings, email))
				//    retVal = TrialResult.UserRegistered;
				//else
				if (CManage.CompanyExists(domain))
					retVal = TrialResult.DomainExists;
				else
				{
					requestId = DBTrialRequest.Create(
						companyName,
						sizeOfGroup,
						description,
						domain,
						firstName,
						lastName,
						email,
						phone,
						country,
						login,
						password,
						new Guid(resellerGuid),
						xml,
						locale
						, referrer
						);

					TemplateVariables vars = DBTrialRequest.GetVariables(requestId);
					requestGuid = vars["RequestGUID"];
					retVal = TrialResult.WaitingForActivation;

//					if (!string.IsNullOrEmpty(settings.OperatorEmail))
//						CManage.SendEmail(settings.OperatorEmail, EmailType.TrialNewRequest, vars);
				}
			}
			finally
			{
				if (retVal != TrialResult.Success && retVal != TrialResult.WaitingForActivation && retVal != TrialResult.RequestPending)
				{
					object obj = null;
					try
					{
						obj = new Guid(resellerGuid);
					}
					catch (ArgumentNullException)
					{
					}
					catch (FormatException)
					{
					}

					DBTrialRequestFailed.Create(
						companyName,
						sizeOfGroup,
						description,
						domain,
						firstName,
						lastName,
						email,
						phone,
						login,
						login,
						password,
						obj
						, locale
						, referrer
						, (int)retVal
						);
				}
			}
			return retVal;
		}
		#endregion

		#region [Web] GetDailyLog(string companyId, int days)
		/// <summary>
		/// Gets the daily log.
		/// </summary>
		/// <param name="companyId">The company id.</param>
		/// <param name="days">The days.</param>
		/// <returns>logId, dt, companyUid, company_name, tariffId, tariffName, amount, balance</returns>
		[WebMethod]
		public DataTable GetDailyLog(string companyId, int days)
		{
			return Tariff.GetDailyLogForPeriod(new Guid(companyId), days);
		}
		#endregion

		#region [Web] GetPaymentInfo(string companyId, int days)
		/// <summary>
		/// Gets the payment info.
		/// </summary>
		/// <param name="companyId">The company id.</param>
		/// <param name="days">The days.</param>
		/// <returns>paymentId, companyUid, company_name, dt, amount, bonus, orderNo</returns>
		[WebMethod]
		public DataTable GetPaymentInfo(string companyId, int days)
		{
			return Tariff.GetPaymentForPeriod(new Guid(companyId), days);
		}
		#endregion

		#region [Web] GetTariffs()
		/// <summary>
		/// Gets the tariffs.
		/// </summary>
		/// <returns>tariffId, tariffName, description, typeId, typeName, currencyId, currencyName, symbol, monthlyCost, dailyCost28, dailyCost29, dailyCost30, dailyCost31, maxHdd, maxUsers, maxExternalUsers</returns>
		[WebMethod]
		public DataTable GetTariffs()
		{
			return Tariff.GetTariffActive();
		}
		#endregion

		#region [Web] AddTariffRequest(string companyId, int tariffId, string description)
		/// <summary>
		/// Adds the tariff request.
		/// </summary>
		/// <param name="companyId">The company id.</param>
		/// <param name="tariffId">The tariff id.</param>
		/// <param name="description">The description.</param>
		[WebMethod]
		public void AddTariffRequest(string companyId, int tariffId, string description)
		{
			Tariff.AddTariffRequest(new Guid(companyId), tariffId, description);
		}
		#endregion
	}
}
