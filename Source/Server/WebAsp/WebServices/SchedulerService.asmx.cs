using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

using Mediachase.Ibn;
using Mediachase.Ibn.Configuration;
using Mediachase.Ibn.WebAsp;

namespace Mediachase.UI.Web.WebServices
{
	/// <summary>
	/// Summary description for SchedulerService
	/// </summary>
	[WebService(Namespace = "http://mediachase.com/webservices/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class SchedulerService : System.Web.Services.WebService
	{
		private static Dictionary<string, DateTime> lastInvokeHash = new Dictionary<string, DateTime>();

		private const string AspSystemName = "AspSystem";
		private const int AspSystemInterval = 60*60; // 1 hour

		/// <summary>
		/// Invokes this instance.
		/// </summary>
		[WebMethod]
		public void Push()
		{
			#region Init
			// Init in Global.ASAX 
			#endregion

			if (CanInvoke(AspSystemName, AspSystemInterval))
			{
				InvokeAspSystem();
			}
		}

		#region CanInvoke
		/// <summary>
		/// Determines whether this instance can invoke the specified system name.
		/// </summary>
		/// <param name="systemName">Name of the system.</param>
		/// <param name="systemInterval">The system interval.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can invoke the specified system name; otherwise, <c>false</c>.
		/// </returns>
		private static bool CanInvoke(string systemName, int systemInterval)
		{
			DateTime nowTime = DateTime.Now;

			lock (lastInvokeHash)
			{
				if (!lastInvokeHash.ContainsKey(systemName))
				{
					lastInvokeHash.Add(systemName, nowTime);
					return true;
				}
				else
				{
					DateTime lastTime = lastInvokeHash[systemName];

					if ((nowTime - lastTime).TotalSeconds > systemInterval)
					{
						lastInvokeHash[systemName] = nowTime;
						return true;
					}
				}
			}

			return false;
		}
		
		#endregion

		#region InvokeAspSystem
		private void InvokeAspSystem()
		{
			RecalculateBalance();
			DeactivateExpiredCompanies();
			DeleteOutdatedCompanies();
			SendUserEmails();
			SendOperatorEmails();
		} 
		#endregion

		#region void RecalculateBalance()
		private void RecalculateBalance()
		{
			AspSettings settings = AspSettings.Load();
			if (settings.UseTariffs)
			{
				Tariff.RecalculateBalance();
			}
		}
		#endregion

		#region void DeactivateExpiredCompanies()
		private void DeactivateExpiredCompanies()
		{
			AspSettings settings = AspSettings.Load();
			if (settings.AutoDeactivateExpired)
			{
				foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.Expired, 0, false))
				{
					try
					{
						CManage.DeactivateCompany(cid);

						// Send e-mail to operator
						if (!string.IsNullOrEmpty(settings.OperatorEmail))
						{
							TemplateVariables vars = CManage.CompanyGetVariables(cid);
							CManage.SendEmail(settings.OperatorEmail, EmailType.TrialDeactivated, vars);
						}
					}
					catch (Exception ex)
					{
						Log.WriteError(ex.ToString());
					}
				}
			}

			if (settings.AutoDeactivateUnpaid && settings.UseTariffs)
			{
				foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.BillableForDeactivate, 0, false))
				{
					try
					{
						CManage.DeactivateCompany(cid);

						// Send e-mail to operator
						if (!string.IsNullOrEmpty(settings.OperatorEmail))
						{
							TemplateVariables vars = CManage.CompanyGetVariables(cid);
							CManage.SendEmail(settings.OperatorEmail, EmailType.OperatorCompanyDeactivated, vars);
						}
					}
					catch (Exception ex)
					{
						Log.WriteError(ex.ToString());
					}
				}
			}
		}
		#endregion

		#region void DeleteOutdatedCompanies()
		private void DeleteOutdatedCompanies()
		{
			AspSettings settings = AspSettings.Load();
			if (settings.AutoDeleteOutdated)
			{
				foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.Outdated, settings.OutdatePeriod, false))
				{
					try
					{
						CManage.DeleteCompany(cid);
					}
					catch (Exception ex)
					{
						Log.WriteError(ex.ToString());
					}
				}
			}
		}
		#endregion

		#region void SendUserEmails()
		private void SendUserEmails()
		{
			AspSettings settings = AspSettings.Load();
			IConfigurator config = Configurator.Create();

			#region trial spam
			if (settings.SendSpam)
			{
				// One day after start
				if (settings.SendSpamOneDayAfter)
				{
					foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.OneDayNoIM, settings.OneDayAfterPeriod, true))
					{
						try
						{
							string dbName = config.GetCompanyInfo(cid.ToString()).Database;
							if (!CManage.NotificationGet(cid, EmailType.UserAfterOneDayIM))
							{
								CManage.NotificationUpdate(cid, EmailType.UserAfterOneDayIM, true);

								if (!CManage.CheckImSessions(dbName))
								{
									// Send e-mails to every active user
									TemplateVariables vars = CManage.CompanyGetVariables(cid);
									foreach (UserInfo ui in CManage.GetPortalUsers(dbName))
									{
										vars["Login"] = ui.Login;
										vars["Name"] = ui.Name;
										try
										{
											CManage.SendEmail(ui.Email, EmailType.UserAfterOneDayIM, vars);
										}
										catch { }
									}
								}
							}
						}
						catch (Exception ex)
						{
							Log.WriteError(ex.ToString());
						}
					}
				}

				// One week after start
				if (settings.SendSpamOneWeekAfter)
				{
					foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.OneWeekAfterStart, settings.OneWeekAfterPeriod, true))
					{
						try
						{
							if (!CManage.NotificationGet(cid, EmailType.UserAfterOneWeek))
							{
								CManage.NotificationUpdate(cid, EmailType.UserAfterOneWeek, true);

								// Send e-mail
								TemplateVariables vars = CManage.CompanyGetVariables(cid);
								if (!string.IsNullOrEmpty(vars["ContactEmail"]))
								{
									CManage.SendEmail(vars["ContactEmail"], EmailType.UserAfterOneWeek, vars);
								}
							}
						}
						catch (Exception ex)
						{
							Log.WriteError(ex.ToString());
						}
					}
				}

				// One week before end
				if (settings.SendSpamOneWeekBefore)
				{
					foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.OneWeekBeforeEnd, settings.OneWeekBeforePeriod, true))
					{
						try
						{
							if (!CManage.NotificationGet(cid, EmailType.UserOneWeekBefore))
							{
								CManage.NotificationUpdate(cid, EmailType.UserOneWeekBefore, true);

								// Send e-mail
								TemplateVariables vars = CManage.CompanyGetVariables(cid);
								if (!string.IsNullOrEmpty(vars["ContactEmail"]))
								{
									CManage.SendEmail(vars["ContactEmail"], EmailType.UserOneWeekBefore, vars);
								}
							}
						}
						catch (Exception ex)
						{
							Log.WriteError(ex.ToString());
						}
					}
				}

				// One day before end
				if (settings.SendSpamOneDayBefore)
				{
					foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.OneDayBeforeEnd, settings.OneDayBeforePeriod, true))
					{
						try
						{
							if (!CManage.NotificationGet(cid, EmailType.UserOneDayBefore))
							{
								CManage.NotificationUpdate(cid, EmailType.UserOneDayBefore, true);

								// Send e-mail
								TemplateVariables vars = CManage.CompanyGetVariables(cid);
								if (!string.IsNullOrEmpty(vars["ContactEmail"]))
								{
									CManage.SendEmail(vars["ContactEmail"], EmailType.UserOneDayBefore, vars);
								}
							}
						}
						catch (Exception ex)
						{
							Log.WriteError(ex.ToString());
						}
					}
				}
			}
			#endregion

			#region billable spam
			if (settings.SendBillableSpam && settings.UseTariffs)
			{
				// 7 days before
				if (settings.SendBillableSpam7day)
				{
					foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.BillableNdaysBefore, 7, true))
					{
						// ensure that we send emails only once for a day
						DateTime lastSpamDate = CManage.GetNotificationClientSpamDate(cid);
						if (lastSpamDate > DateTime.Now.AddDays(-1))
							continue;

						// Send e-mail
						try
						{
							TemplateVariables vars = CManage.CompanyGetVariables(cid);
							if (!string.IsNullOrEmpty(vars["ContactEmail"]))
							{
								CManage.SendEmail(vars["ContactEmail"], EmailType.Client7DayZeroBalance, vars);
								CManage.UpdateNotificationClientSpamDate(cid, DateTime.Now);
							}
						}
						catch (Exception ex)
						{
							Log.WriteError(ex.ToString());
						}
					}
				}

				// 3 days before
				if (settings.SendBillableSpam3day)
				{
					foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.BillableNdaysBefore, 3, true))
					{
						// ensure that we send emails only once for a day
						DateTime lastSpamDate = CManage.GetNotificationClientSpamDate(cid);
						if (lastSpamDate > DateTime.Now.AddDays(-1))
							continue;

						// Send e-mail
						try
						{
							TemplateVariables vars = CManage.CompanyGetVariables(cid);
							if (!string.IsNullOrEmpty(vars["ContactEmail"]))
							{
								CManage.SendEmail(vars["ContactEmail"], EmailType.Client3DayZeroBalance, vars);
								CManage.UpdateNotificationClientSpamDate(cid, DateTime.Now);
							}
						}
						catch (Exception ex)
						{
							Log.WriteError(ex.ToString());
						}
					}
				}

				// 1 day before
				if (settings.SendBillableSpam1day)
				{
					foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.BillableNdaysBefore, 1, true))
					{
						// ensure that we send emails only once for a day
						DateTime lastSpamDate = CManage.GetNotificationClientSpamDate(cid);
						if (lastSpamDate > DateTime.Now.AddDays(-1))
							continue;

						// Send e-mail
						try
						{
							TemplateVariables vars = CManage.CompanyGetVariables(cid);
							if (!string.IsNullOrEmpty(vars["ContactEmail"]))
							{
								CManage.SendEmail(vars["ContactEmail"], EmailType.Client1DayZeroBalance, vars);
								CManage.UpdateNotificationClientSpamDate(cid, DateTime.Now);
							}
						}
						catch (Exception ex)
						{
							Log.WriteError(ex.ToString());
						}
					}
				}

				// Negative balance
				if (settings.SendBillableSpamNegativeBalance)
				{
					foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.BillableNegativeBalance, 0, true))
					{
						// ensure that we send emails only once for a day
						DateTime lastSpamDate = CManage.GetNotificationClientSpamDate(cid);
						if (lastSpamDate > DateTime.Now.AddDays(-1))
							continue;

						// Send e-mail
						try
						{
							TemplateVariables vars = CManage.CompanyGetVariables(cid);
							if (!string.IsNullOrEmpty(vars["ContactEmail"]))
							{
								CManage.SendEmail(vars["ContactEmail"], EmailType.ClientZeroBalance, vars);
								CManage.UpdateNotificationClientSpamDate(cid, DateTime.Now);
							}
						}
						catch (Exception ex)
						{
							Log.WriteError(ex.ToString());
						}
					}
				}
			}
			#endregion
		}
		#endregion

		#region void SendOperatorEmails()
		private void SendOperatorEmails()
		{
			AspSettings settings = AspSettings.Load();
			IConfigurator config = Configurator.Create();

			// Trial - One day before end
			foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.OneDayBeforeEnd, settings.OneDayBeforePeriod, true))
			{
				try
				{
					if (!CManage.NotificationGet(cid, EmailType.TrialOneDayBefore))
					{
						if (string.IsNullOrEmpty(settings.OperatorEmail))
						{
							CManage.NotificationUpdate(cid, EmailType.TrialOneDayBefore, true);

							TemplateVariables vars = CManage.CompanyGetVariables(cid);
							CManage.SendEmail(settings.OperatorEmail, EmailType.TrialOneDayBefore, vars);
						}
					}
				}
				catch (Exception ex)
				{
					Log.WriteError(ex.ToString());
				}
			}

			if (settings.UseTariffs)
			{
				// Billable - One day before end
				foreach (Guid cid in CManage.GetCompaniesByCategory(CompanyCategory.BillableNdaysBefore, 1, false))
				{
					// ensure that we send emails only once for a day
					DateTime lastSpamDate = CManage.GetNotificationOperatorSpamDate(cid);
					if (lastSpamDate > DateTime.Now.AddDays(-1))
						continue;

					try
					{
						if (string.IsNullOrEmpty(settings.OperatorEmail))
						{
							TemplateVariables vars = CManage.CompanyGetVariables(cid);
							CManage.SendEmail(settings.OperatorEmail, EmailType.OperatorCompanyDeactivatedDayBefore, vars);
							CManage.UpdateNotificationOperatorSpamDate(cid, DateTime.Now);
						}
					}
					catch (Exception ex)
					{
						Log.WriteError(ex.ToString());
					}
				}
			}
		}
		#endregion
	}
}
