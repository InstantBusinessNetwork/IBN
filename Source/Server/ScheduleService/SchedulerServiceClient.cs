using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

using Mediachase.Ibn;
using Mediachase.Ibn.Configuration;
using Mediachase.Schedule.Service.IbnPortalSchedulerService;
using Mediachase.Schedule.Service.Configuration;
using System.Net;
using System.Data.SqlClient;
using System.Data;

namespace Mediachase.Schedule.Service
{
	/// <summary>
	/// Represents SchedulerServiceClient.
	/// </summary>
	public class SchedulerServiceClient
	{
		private object _lockObject = new object();

		#region .Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="SchedulerServiceClient"/> class.
		/// </summary>
		public SchedulerServiceClient()
		{
			this.WaitingStack = new Stack<SchedulerService>();
			this.ActiveList = new List<SchedulerService>();

			this.MaxActiveConnections = 10;
			this.CommandTimeout = 60 * 60; // 1 Hour
		}

		#endregion

		#region Properties

		public int MaxActiveConnections { get; set; }
		public int CommandTimeout { get; set; }

		public List<UrlInfo> WebServiceUrlList { get; private set; }

		protected Stack<SchedulerService> WaitingStack { get; private set; }
		protected List<SchedulerService> ActiveList { get; private set; }


		/// <summary>
		/// Gets the INSTALLDIR.
		/// </summary>
		/// <value>The INSTALLDIR.</value>
		public static string InstallDir { get { return RegistrySettings.ReadString("INSTALLDIR"); } }

		#endregion

		#region Methods
		#region GetWebServiceUrlList
		/// <summary>
		/// Gets the web service URL list.
		/// </summary>
		/// <returns></returns>
		protected UrlInfo[] GetWebServiceUrlList()
		{
			LoadWebServiceUrlList();

			return this.WebServiceUrlList.ToArray();
		}

		protected int GetWebServiceUrlListCount()
		{
			LoadWebServiceUrlList();

			return this.WebServiceUrlList.Count;
		}

		private void LoadWebServiceUrlList()
		{
			if (this.WebServiceUrlList == null)
			{
				lock (_lockObject)
				{
					if (this.WebServiceUrlList == null)
					{
						List<UrlInfo> items = new List<UrlInfo>();

						IConfigurator configurator = Configurator.Create();
						ICompanyInfo[] companies = configurator.ListCompanies(false);

						string webServicePath = ConfigurationManager.AppSettings["WebServicePath"];
						if (string.IsNullOrEmpty(webServicePath))
							webServicePath = "WebServices/SchedulerService.asmx";

						foreach (ICompanyInfo company in companies)
						{
							if (company.IsActive && company.IsScheduleServiceEnabled)
							{
								int port = (string.IsNullOrEmpty(company.Port) ? -1 : int.Parse(company.Port, CultureInfo.InvariantCulture));
								string url = new UriBuilder(company.Scheme, company.Host, port, webServicePath).ToString();
								string connectionString = GetCompanyConnectionString(url, company);

								items.Add(new UrlInfo(url, connectionString));
							}
						}

						// Load Url from config file
						ScheduleServiceSection section = (ScheduleServiceSection)ConfigurationManager.GetSection("scheduleService");

						if (section != null)
						{
							foreach (WebServiceElement element in section.WebServices)
							{
								if (!string.IsNullOrEmpty(element.Url))
								{
									items.Add(new UrlInfo(element.Url, element.Credential));
								}
							}
						}


						this.WebServiceUrlList = items;
					}
				}
			}
		}

		/// <summary>
		/// Gets the company connection string.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="company">The company.</param>
		/// <returns></returns>
		private string GetCompanyConnectionString(string url, ICompanyInfo company)
		{
			// Open Web Config
			try
			{
				string webConfigPath = Path.Combine(Path.Combine(InstallDir, company.CodePath), @"Web\Portal\Web.config");

				XmlDocument webConfig = new XmlDocument();
				webConfig.Load(webConfigPath);

				XmlNode connectionStringNode = webConfig.SelectSingleNode("configuration/appSettings/add[@key='ConnectionString']/@value");

				if (connectionStringNode == null)
					throw new ScheduleServiceException("Cannot find database connection string in portal web.config.");

				return connectionStringNode.Value;

			}
			catch (Exception ex)
			{
				// Save Error
				string msg = string.Format("SchedulerService.CheckCompanyRequiredPush() failed.\r\nURL: {0}\r\nException: {1}",
					url, ex);
				System.Diagnostics.Trace.WriteLine(msg);
				Log.WriteEntry(msg, EventLogEntryType.Error);
			}

			return string.Empty;
		}

		private bool CheckCompanyRequiredPush(string url, string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
				return true;

			// Step 1. Check DatabaseVersion
			int majorVersion = 0;
			int minorVersion = 0;
			int buildVersion = 0;

			try
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					SqlCommand command = connection.CreateCommand();
					command.CommandType = System.Data.CommandType.Text;

					command.CommandText = @"SELECT * FROM [DatabaseVersion] WITH (NOLOCK) WHERE State = 6";

					using (IDataReader reader = command.ExecuteReader())
					{
						if (!reader.Read())
							return false;

						majorVersion = (int)reader["major"];
						minorVersion = (int)reader["minor"];
						buildVersion = (int)reader["build"];
					}
				}
			}
			catch (Exception ex)
			{
				// Save Error
				string msg = string.Format("SchedulerService.CheckCompanyRequiredPush() failed.\r\nURL: {0}\r\nException: {1}",
					url, ex);
				System.Diagnostics.Trace.WriteLine(msg);
				Log.WriteEntry(msg, EventLogEntryType.Error);
				return false;
			}

			// Step 2. Check the need for a call 
			try
			{

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					SqlCommand command = connection.CreateCommand();
					command.CommandType = System.Data.CommandType.Text;

					// OZ: Don't copy SQL Script to SP, becuase IBN Database can be uninitialized
					StringBuilder sbCommandText = new StringBuilder(2500);

					sbCommandText.AppendFormat(@"IF EXISTS(SELECT [State] FROM [DatabaseVersion] WITH (NOLOCK) WHERE State = 6){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"BEGIN{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"SELECT SUM(Items) FROM{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"({0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	SELECT COUNT(*) AS Items{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	FROM DATE_TYPE_VALUES V WITH (NOLOCK){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	  JOIN DATE_TYPE_HOOKS H ON (V.DateTypeId = H.DateTypeId){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	  JOIN DATE_TYPES D ON (V.DateTypeId = D.DateTypeId){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	 WHERE {0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	  V.ObjectId IS NOT NULL {0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	  AND{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	  (V.ObjectId = H.ObjectId OR H.ObjectId IS NULL) {0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	  AND {0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	  (DATEADD(mi, -H.Lag, V.DateValue) <= (getutcdate()) ){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	  AND NOT EXISTS (SELECT * FROM DATE_TYPE_PROCESSED P WITH (NOLOCK) WHERE V.ValueId = P.ValueId {0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"AND H.HookId = P.HookId AND V.DateValue = P.DateValue AND H.Lag = P.Lag){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"{0}", Environment.NewLine);

					// OZ: 4.7.55 Build Addon
					if (majorVersion >= 4 && minorVersion >= 7 && buildVersion >= 55)
					{
						sbCommandText.AppendFormat(@"UNION ALL{0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	SELECT COUNT(*) AS Items{0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	FROM DATE_TYPE_VALUES V WITH (NOLOCK){0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	  JOIN DATE_TYPE_HOOKS H ON (V.DateTypeId = H.DateTypeId){0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	  JOIN DATE_TYPES D ON (V.DateTypeId = D.DateTypeId){0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	 WHERE {0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	  V.ObjectUid IS NOT NULL {0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	  AND{0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	  (V.ObjectUid = H.ObjectUid OR H.ObjectUid IS NULL) {0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	  AND {0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	  (DATEADD(mi, -H.Lag, V.DateValue) <= (getutcdate()) ){0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"	  AND NOT EXISTS (SELECT * FROM DATE_TYPE_PROCESSED P WITH (NOLOCK) WHERE V.ValueId = P.ValueId {0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"AND H.HookId = P.HookId AND V.DateValue = P.DateValue AND H.Lag = P.Lag){0}", Environment.NewLine);
						sbCommandText.AppendFormat(@"{0}", Environment.NewLine);
					}

					sbCommandText.AppendFormat(@"UNION ALL{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"--- Outgoin Emails{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"SELECT COUNT(*) AS Items FROM cls_OutgoingMessageQueue WITH (NOLOCK) WHERE IsDelivered = 0{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"UNION ALL{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"-- Incoiming Email Box{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"SELECT COUNT(*) AS Items FROM EMailRouterPop3Box EB  WITH (NOLOCK){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"JOIN EMailRouterPop3BoxActivity EA ON EA.EMailRouterPop3BoxId = EB.EMailRouterPop3BoxId{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"WHERE EA.IsActive = 1 AND (LastRequest IS NULL OR getutcdate() > DATEADD(mi, 5, LastRequest)){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"UNION ALL{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"-- Old Pop3Boxes{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"SELECT COUNT(*) AS Items FROM Pop3Boxes  WITH (NOLOCK) WHERE Active = 1{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"AND (getutcdate() > DATEADD(mi, Interval, LastRequest)){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	-- Business BusinessProcess{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	UNION ALL{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	SELECT COUNT(*) FROM cls_WorkflowInstance WHERE State = 2 AND TimeStatus IS NULL AND ");
					sbCommandText.AppendFormat(@"PlanFinishDate <= GETUTCDATE(){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	UNION ALL{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	SELECT COUNT(*) FROM cls_WorkflowInstance WHERE State = 2 AND TimeStatus = 2 AND PlanFinishDate > ");
					sbCommandText.AppendFormat(@"GETUTCDATE(){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	UNION ALL{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	SELECT COUNT(*) FROM cls_Assignment WHERE State = 2 AND TimeStatus IS NULL AND PlanFinishDate <= ");
					sbCommandText.AppendFormat(@"GETUTCDATE(){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	UNION ALL{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"	SELECT COUNT(*) FROM cls_Assignment WHERE State = 2 AND TimeStatus = 3 AND PlanFinishDate > ");
					sbCommandText.AppendFormat(@"GETUTCDATE(){0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@") AS A{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"END{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"ELSE{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"BEGIN{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"SELECT 0{0}", Environment.NewLine);
					sbCommandText.AppendFormat(@"END{0}", Environment.NewLine);

					command.CommandText = sbCommandText.ToString();

					int retVal = (int)command.ExecuteScalar();
					return retVal > 0;
				}
			}
			catch (Exception ex)
			{
				// Save Error
				string msg = string.Format("SchedulerService.CheckCompanyRequiredPush() failed.\r\nURL: {0}\r\nException: {1}",
					url, ex);
				System.Diagnostics.Trace.WriteLine(msg);
				Log.WriteEntry(msg, EventLogEntryType.Error);
			}

			return false;
		}

		/// <summary>
		/// Resets the web service list.
		/// </summary>
		public void RefreshWebServiceList()
		{
			// Force a reload of the changed section.
			ConfigurationManager.RefreshSection("scheduleService");
			ConfigurationManager.RefreshSection("appSettings");

			lock (_lockObject)
			{
				this.WebServiceUrlList = null;
			}

		}
		#endregion

		#region Invoke Methods
		/// <summary>
		/// Pushes this instance.
		/// </summary>
		public void Push()
		{
			lock (_lockObject)
			{
				// Step 0. Remove Completed Element from Active List
				RemoveCompletedElementFromActiveList();

				// Step 1. Check Or Create Waiting Stack
				CheckWaitingStack();

				// Step 2. Move Element From Waiting Stack To Active List
				MoveElementFromWaitingStackToActiveList();

				// Step 3. Check Command Timeout
				CheckCommandTimeout();
			}
		}

		/// <summary>
		/// Removes the completed element from active list.
		/// </summary>
		private void RemoveCompletedElementFromActiveList()
		{
			for (int index = this.ActiveList.Count - 1; index >= 0; index--)
			{
				SchedulerService service = this.ActiveList[index];

				if (service.Completed.HasValue)
				{
					if (service.Result != null)
					{
						if (service.Result.Error != null)
						{
							// Save Error
							string msg = string.Format("SchedulerService.Push() failed.\r\nURL: {0}\r\nException: {1}",
								service.Url, service.Result.Error);
							System.Diagnostics.Trace.WriteLine(msg);
							Log.WriteEntry(msg, EventLogEntryType.Error);
						}
						else if (service.Result.Cancelled)
						{
							// Save Error
							string msg = string.Format("SchedulerService.Push() canceled.\r\nURL: {0}",
								service.Url);
							System.Diagnostics.Trace.WriteLine(msg);
							Log.WriteEntry(msg, EventLogEntryType.Error);
						}
					}

					// Remove Subcription to event
					service.PushCompleted -= new PushCompletedEventHandler(SchedulerService_PushCompleted);

					this.ActiveList.RemoveAt(index);
				}
			}
		}

		/// <summary>
		/// Checks the waiting stack.
		/// </summary>
		private void CheckWaitingStack()
		{
			if (this.WaitingStack.Count == 0)
			{
				foreach (UrlInfo url in GetWebServiceUrlList())
				{
					// Check we should push web-service
					if (string.IsNullOrEmpty(url.ConnectionString) ||
						CheckCompanyRequiredPush(url.Url, url.ConnectionString))
					{
						SchedulerService service = CreateSchedulerService(url);
						this.WaitingStack.Push(service);
					}
				}
			}
		}

		/// <summary>
		/// Moves the element from waiting stack to active list.
		/// </summary>
		private void MoveElementFromWaitingStackToActiveList()
		{
			// Max Active List is minum (value from Config and Url List)
			int realMaxElement = Math.Min(this.MaxActiveConnections, GetWebServiceUrlListCount());

			// Move Element From Waiting Stack to Active List And Activate

			while (this.ActiveList.Count < realMaxElement)
			{
				// Skip if waiting stack is empty
				if (this.WaitingStack.Count == 0)
					break;

				SchedulerService service = this.WaitingStack.Pop();

				service.Activated = DateTime.Now;

				this.ActiveList.Add(service);

				try
				{
					if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["WriteDebugInfo"]))
					{
						string msg = string.Format("SchedulerService.Push() invoke.\r\nURL: {0}",
							service.Url);
						Log.WriteEntry(msg, EventLogEntryType.Information);
					}


					service.PushAsync(service);
				}
				catch (Exception ex)
				{
					service.Result = new AsyncCompletedEventArgs(ex, true, service);
					service.Completed = DateTime.Now;
				}
			}
		}

		/// <summary>
		/// Checks the command timeout.
		/// </summary>
		private void CheckCommandTimeout()
		{
			DateTime nowDateTime = DateTime.Now;

			foreach (SchedulerService service in this.ActiveList)
			{
				if (service.Activated.Value.AddSeconds(this.CommandTimeout) < nowDateTime)
				{
					try
					{
						service.CancelAsync(service);
					}
					catch (Exception ex)
					{
						service.Result = new AsyncCompletedEventArgs(ex, true, service);
						service.Completed = nowDateTime;
					}
				}
			}
		}

		#endregion

		#region Push Completed Methods

		/// <summary>
		/// Handles the PushCompleted event of the SchedulerService control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.AsyncCompletedEventArgs"/> instance containing the event data.</param>
		void SchedulerService_PushCompleted(object sender, AsyncCompletedEventArgs e)
		{
			lock (_lockObject)
			{
				OnPushCompleted(sender, e);
			}
		}

		/// <summary>
		/// Called when [push completed].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.ComponentModel.AsyncCompletedEventArgs"/> instance containing the event data.</param>
		protected virtual void OnPushCompleted(object sender, AsyncCompletedEventArgs e)
		{
			SchedulerService service = e.UserState as SchedulerService;

			if (service != null)
			{
				service.Result = e;
				service.Completed = DateTime.Now;
			}
		}

		#endregion

		#region CreateSchedulerService
		/// <summary>
		/// Creates the scheduler service.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		protected SchedulerService CreateSchedulerService(UrlInfo url)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			SchedulerService retVal = new SchedulerService();

			if (url.Credential != null)
				retVal.Credentials = url.Credential;

			retVal.Url = url.Url;
			retVal.PushCompleted += new PushCompletedEventHandler(SchedulerService_PushCompleted);

			return retVal;
		}
		#endregion

		#endregion
	}
}
