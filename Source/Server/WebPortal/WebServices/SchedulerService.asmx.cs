using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using Mediachase.IBN.Business.EMail;
using System.ComponentModel;
using Mediachase.IBN.Business;
using Mediachase.Ibn;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Business.Messages;
using System.Collections.Specialized;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;

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

		private const string AlertSystemName = "AlertSystem";
		private const int AlertSystemInterval = 10; // seconds

		private const string GlobalProcessSystemName = "GlobalProcessSystem";
		private const int GlobalProcessSystemInterval = 60; // seconds

		private const string EMailRouterSystemName = "EMailRouterSystem";
		private const int EMailRouterSystemInterval = 300; // seconds

		private const string Pop3SystemName = "Pop3System";
		private const int Pop3SystemInterval = 300; // seconds

		private const string BusinessProcessSystemName = "BusinessProcess";
		private const int BusinessProcessSystemInterval = 60; // seconds

		/// <summary>
		/// Invokes this instance.
		/// </summary>
		[WebMethod]
		public void Push()
		{
			lock (typeof(SchedulerService))
			{
				//Trace.WriteLine("Push");
				//Thread.Sleep(5 * 60 * 1000);
				//return;

				#region Init
				// Init in Global.ASAX 
				#endregion

				if (CanInvoke(GlobalProcessSystemName, GlobalProcessSystemInterval))
				{
					InvokeGlobalProcessSystem();
				}

				if (CanInvoke(Pop3SystemName, Pop3SystemInterval))
				{
					InvokePop3System();
				}

				if (CanInvoke(EMailRouterSystemName, EMailRouterSystemInterval))
				{
					InvokeEMailRouterSystem();
				}

				if (CanInvoke(AlertSystemName, AlertSystemInterval))
				{
					InvokeAlertSystem();
				}

				// TODO: Remove DEBUG If Publish Business Process System
//#if DEBUG
				if (CanInvoke(BusinessProcessSystemName, BusinessProcessSystemInterval))
				{
					InvokeBusinessProcessSystem();
				}
//#endif
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

		#region InvokeAlertSystem
		/// <summary>
		/// Invokes the alert system.
		/// </summary>
		private void InvokeAlertSystem()
		{
			// Remove Expired Messages


			// Invoke Ibn Client
			IbnClientMessageDeliveryProvider ibnClientProvider = new IbnClientMessageDeliveryProvider();

			NameValueCollection ibnClientProviderConfig = new NameValueCollection();

			ibnClientProvider.Initialize("IbnClient", ibnClientProviderConfig);
			ibnClientProvider.Invoke();

			// Invoke Email
			EmailDeliveryProvider emailProvider = new EmailDeliveryProvider();

			NameValueCollection emailProviderConfig = new NameValueCollection();

			emailProvider.Initialize("Email", emailProviderConfig);
			emailProvider.Invoke();
		}
		#endregion

		#region InvokePop3System
		/// <summary>
		/// Invokes the pop3 system.
		/// </summary>
		private void InvokePop3System()
		{
			Pop3ScheduleClient scheduleClient = new Pop3ScheduleClient();

			try
			{
				scheduleClient.InternalInvoke(IbnConst.ProductName);
			}
			catch (Exception ex)
			{
				// Create Exception Dump
				Mediachase.IBN.Business.Common.WriteExceptionToEventLog(ex);
			}
		}
		#endregion

		#region InvokeEMailRouterSystem
		/// <summary>
		/// Invokes the e-mail router system.
		/// </summary>
		private void InvokeEMailRouterSystem()
		{
			EMailRouterScheduleClient scheduleClient = new EMailRouterScheduleClient();

			try
			{
				scheduleClient.SingleCompanyInvoke();
			}
			catch (Exception ex)
			{
				// Create Exception Dump
				Mediachase.IBN.Business.Common.WriteExceptionToEventLog(ex);
			}
		}
		#endregion

		#region InvokeGlobalProcessSystem
		/// <summary>
		/// Invokes the global process system.
		/// </summary>
		private void InvokeGlobalProcessSystem()
		{
			ProcessHandlerScheduleClient scheduleClient = new ProcessHandlerScheduleClient();

			try
			{
				scheduleClient.InternalInvoke(IbnConst.ProductName);
			}
			catch (Exception ex)
			{
				// Create Exception Dump
				Mediachase.IBN.Business.Common.WriteExceptionToEventLog(ex);
			}

		}
		#endregion

		#region InvokeBusinessProcessSystem
		private void InvokeBusinessProcessSystem()
		{
			try
			{
				BusinessProcessSheduler.Process();
			}
			catch (Exception ex)
			{
				// Create Exception Dump
				Mediachase.IBN.Business.Common.WriteExceptionToEventLog(ex);
			}

		}
		#endregion
	}
}
