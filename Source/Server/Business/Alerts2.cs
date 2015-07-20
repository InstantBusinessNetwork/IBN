using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

using Mediachase.Alert.Service;
using Mediachase.Ibn;
using Mediachase.Ibn.Business.Messages;
using Mediachase.IBN.Database;
using Mediachase.Net.Mail;
using Mediachase.Ibn.Assignments;


namespace Mediachase.IBN.Business
{
	#region * Enums *
	enum RecipientsType
	{
		Batch,
		Transaction
	}
	#endregion

	internal class Alerts2
	{
		// Private
		#region * Constants *
		private const string DocumentLink = "/Documents/DocumentView.aspx?DocumentId=";
		private const string EventLink = "/Events/EventView.aspx?EventId=";
		private const string IssueLink = "/Incidents/IncidentView.aspx?IncidentId=";
		private const string IssueBoxLink = "/Admin/EMailIssueBoxView.aspx?IssBoxId=";
		private const string IssueRequestLink = "/Incidents/MailRequestView.aspx?RequestId=";
		private const string ListDataLink = "/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName=List_";
		private const string ListInfoLink = "/Apps/ListApp/Pages/ListInfoView.aspx?class=List_";
		private const string MailBoxLink = "/Incidents/default.aspx?BTab=MailIncidents&MailBoxId=";
		private const string ProjectLink = "/Projects/ProjectView.aspx?ProjectID=";
		private const string TaskLink = "/Tasks/TaskView.aspx?TaskId=";
		private const string TodoLink = "/ToDo/ToDoView.aspx?ToDoId=";
		private const string UserLink = "/Directory/UserView.aspx?UserId=";

		private const string DocumentLinkExt = "/External/ExternalDocument.aspx?DocumentId=";
		private const string EventLinkExt = "/External/ExternalEvent.aspx?EventId=";
		private const string IssueLinkExt = "/External/ExternalIncident.aspx?IncidentId=";
		private const string TaskLinkExt = "/External/ExternalTask.aspx?TaskId=";
		private const string TodoLinkExt = "/External/ExternalToDo.aspx?ToDoId=";

		private const string IssueLinkClient = "/External/ClientIssue.aspx?IssueId=";

		private const string ExternalLogin = "&ExternalLogin=";
		private const string ExternalGuid = "&Guid=";
		private const string MailIncidentLink = "/Incidents/MailIncidentView.aspx?IncidentId=";
		private const string PortfolioLink = "/Projects/ProjectGroupView.aspx?ProjectGroupId=";
		#endregion

		#region * Configuration *
		private const string ConstEnableAlerts = "EnableAlerts";

		private static bool AlertsEnabled { get { return string.Compare(ConfigurationManager.AppSettings[ConstEnableAlerts], "true", StringComparison.OrdinalIgnoreCase) == 0 && PortalConfig.EnableAlerts; } }

		private string _alertSenderFirstName;
		private string _alertSenderLastName;
		private string _alertSenderEmail;
		private string _alertSenderIM;
		private bool _sendToCurrentUser;

		private static IFormatProvider provDefault;
		private static IFormatProvider provEn;
		private static IFormatProvider provRu;

		internal static string AlertSenderFirstName { get { return Current._alertSenderFirstName; } }
		internal static string AlertSenderLastName { get { return Current._alertSenderLastName; } }
		internal static string AlertSenderEmail { get { return Current._alertSenderEmail; } }
		internal static string AlertSenderIM { get { return Current._alertSenderIM; } }
		#endregion

		[ThreadStatic]
		private static Alerts2 _current;
		private static string _typeName = typeof(Alerts2).FullName;
		#region Current
		public static Alerts2 Current
		{
			get
			{
				Alerts2 ret;

				if (HttpContext.Current != null)
					ret = HttpContext.Current.Items[_typeName] as Alerts2;
				else
					ret = _current;

				if (ret == null)
				{
					ret = new Alerts2();
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

		internal static bool SendToCurrentUser
		{
			get
			{
				return Current._sendToCurrentUser;
			}
			set
			{
				Current._sendToCurrentUser = value;
			}
		}

		#region * Helper classes *
		#region class Attachment
		private class Attachment
		{
			internal string FileName;
			internal Stream Data;

			internal Attachment(string fileName, Stream data)
			{
				FileName = fileName;
				Data = data;
			}
		}
		#endregion

		#region class Message
		internal class Message
		{
			internal string Subject;
			internal string Body;
			internal ArrayList Attachments = new ArrayList();

			internal Message(string subject, string body)
			{
				Subject = subject;
				Body = body;
			}
		}
		#endregion

		#region class RecipientInfo
		private class RecipientInfo
		{
			internal int RecipientId;
			internal int SystemEventId;
			internal int MessageTypeId;
			internal int UserId;
			internal string Email;
			internal string EmailFrom;

			internal void Load(IDataReader reader)
			{
				RecipientId = (int)reader["RecipientId"];
				SystemEventId = (int)reader["SystemEventId"];
				MessageTypeId = (int)reader["MessageTypeId"];
				UserId = (int)reader["UserId"];
				Email = reader["Email"].ToString();
				EmailFrom = reader["EmailFrom"].ToString();
			}
		}
		#endregion

		#region class EventInfo
		private class EventInfo
		{
			internal int EventId;
			internal int EventTypeId;
			internal string EventType;
			internal int? ObjectId;
			internal Guid? ObjectUid;
			internal int ObjectTypeId;
			internal string ObjectTitle;
			internal int RelObjectId;
			internal int RelObjectTypeId;
			internal string RelObjectTitle;
			internal DateTime EventDate;
			internal int UserId;
			internal string XmlData;

			internal ArrayList Recipients = new ArrayList();
			internal ArrayList Users = new ArrayList();

			internal void Load(IDataReader reader)
			{
				EventId = (int)reader["SystemEventId"];
				EventTypeId = (int)reader["EventTypeId"];
				EventType = reader["SystemEventTitle"].ToString();

				if (reader["ObjectId"] != DBNull.Value)
					ObjectId = (int)reader["ObjectId"];
				else
					ObjectId = null;

				if (reader["ObjectUid"] != DBNull.Value)
					ObjectUid = (Guid)reader["ObjectUid"];
				else
					ObjectUid = null;

				ObjectTypeId = (int)reader["ObjectTypeId"];
				ObjectTitle = reader["ObjectTitle"].ToString();
				RelObjectId = (int)reader["RelObjectId"];
				RelObjectTypeId = (int)reader["RelObjectTypeId"];
				RelObjectTitle = reader["RelObjectTitle"].ToString();
				EventDate = (DateTime)reader["Dt"];
				UserId = (int)DBCommon.NullToObject(reader["UserId"], -1);
				XmlData = reader["XmlData"].ToString();
				if (XmlData.Length == 0)
					XmlData = "<variables/>";
			}

			internal void AddRecipient(object id)
			{
				if (!Recipients.Contains(id))
					Recipients.Add(id);
			}

			internal void AddUser(object id)
			{
				if (!Users.Contains(id))
					Users.Add(id);
			}

			internal XmlNodeList GetVariables()
			{
				return GetVariables(0);
			}

			internal XmlNodeList GetVariables(int lang_id)
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(XmlData);
				string xpath = "/variables/var[not(@lang)";
				if (lang_id > 0)
					xpath += string.Format(provDefault, " or @lang='{0}'", lang_id);
				xpath += "]";
				return doc.SelectNodes(xpath);
			}

			internal ArrayList GetVariablesArrayList(int langId)
			{
				ArrayList vars = new ArrayList();
				foreach (XmlNode varNode in GetVariables(langId))
				{
					VariableInfo var = new VariableInfo(varNode);
					vars.Add(var);
				}
				return vars;
			}


			internal string GetVariablesXML(int lang_id)
			{
				StringBuilder sb = new StringBuilder();
				XmlTextWriter w = new XmlTextWriter(new StringWriter(sb));

				w.WriteStartElement("variables");
				foreach (XmlNode var in GetVariables(lang_id))
				{
					w.WriteStartElement("var");
					w.WriteAttributeString("name", var.Attributes["name"].Value);
					w.WriteAttributeString("value", var.Attributes["value"].Value);
					w.WriteEndElement();
				}
				w.WriteEndElement();

				return sb.ToString();
			}
		}
		#endregion

		#region class UserInfo
		private class UserInfo
		{
			internal int UserId;
			internal string Email;
			internal int ImId;
			internal bool IsNotifiedByEmail;
			internal bool IsNotifiedByIBN;
			internal string FirstName;
			internal string LastName;
			internal int LanguageId;
			internal string Locale;
			internal int TimeZoneId;
			internal bool IsExternal;
			internal string Login;
			internal bool MenuInAlerts;
			internal DateTime BatchLastSent;
			internal DateTime BatchNextSend;
			internal int From;
			internal int Till;
			internal int Period;
			internal string EmailFrom;

			internal ArrayList Events = new ArrayList();
			internal ArrayList Recipients = new ArrayList();

			internal void Init(UserLight user, string email, string emailFrom)
			{
				Email = email;
				IsExternal = true;
				IsNotifiedByEmail = true;
				LanguageId = user.LanguageId;
				Locale = user.Culture;
				TimeZoneId = user.TimeZoneId;
				EmailFrom = emailFrom;
			}

			internal void Load(int userId)
			{
				using (IDataReader reader = DbAlert2.GetUser(userId))
				{
					reader.Read();
					Load(reader);
				}
			}

			private void Load(IDataReader reader)
			{
				UserId = (int)reader["PrincipalId"];
				Email = reader["Email"].ToString();
				ImId = DBCommon.NullToInt32(reader["ImId"]);
				IsNotifiedByEmail = (bool)reader["IsNotifiedByEmail"];
				IsNotifiedByIBN = ImId > 0 && (bool)reader["IsNotifiedByIbn"];
				FirstName = reader["FirstName"].ToString();
				LastName = reader["LastName"].ToString();
				LanguageId = (int)reader["LanguageId"];
				TimeZoneId = (int)reader["TimeZoneId"];
				Locale = reader["Locale"].ToString();
				IsExternal = (bool)reader["IsExternal"];
				Login = reader["Login"].ToString();
				MenuInAlerts = (bool)reader["MenuInAlerts"];
				BatchLastSent = DBCommon.NullToDateTime(reader["BatchLastSent"]);
				BatchNextSend = DBCommon.NullToDateTime(reader["BatchNextSend"]);
				From = DBCommon.NullToInt32(reader["From"]);
				Till = DBCommon.NullToInt32(reader["Till"]);
				Period = DBCommon.NullToInt32(reader["Period"]);
			}

			internal string GetAddress(DeliveryType type)
			{
				switch (type)
				{
					case DeliveryType.Email:
						return Alerts2.BuldEmailAddress(FirstName, LastName, Email);
					case DeliveryType.IBN:
						return ImId.ToString(CultureInfo.InvariantCulture);
				}
				return null;
			}

			internal void AddEvent(EventInfo ei)
			{
				if (!Events.Contains(ei))
					Events.Add(ei);
			}

			internal void AddRecipient(RecipientInfo ri)
			{
				if (!Recipients.Contains(ri))
					Recipients.Add(ri);
			}
		}
		#endregion
		#endregion

		#region CollectEventsAndRecipients()
		private static void CollectEventsAndRecipients(RecipientsType type, ArrayList recipients, Hashtable events, Hashtable users, string param1, DateTime param2)
		{
			EventInfo eInfo;
			UserInfo uInfo;

			// Load recipients info
			using (IDataReader reader = GetRecipientsReader(type, param1, param2))
			{
				RecipientInfo rInfo;
				while (reader.Read())
				{
					rInfo = new RecipientInfo();
					rInfo.Load(reader);
					recipients.Add(rInfo);
				}
			}

			foreach (RecipientInfo rInfo in recipients)
			{
				eInfo = (EventInfo)events[rInfo.SystemEventId];
				if (eInfo == null)
				{
					// Load events info
					using (IDataReader reader = DBSystemEvents.GetEvent(rInfo.SystemEventId))
					{
						while (reader.Read())
						{
							eInfo = new EventInfo();
							eInfo.Load(reader);
							events[rInfo.SystemEventId] = eInfo;
						}
					}
				}

				if (rInfo.UserId > 0)
				{
					eInfo.AddRecipient(rInfo.RecipientId);
					eInfo.AddUser(rInfo.UserId);

					uInfo = (UserInfo)users[rInfo.UserId];
					if (uInfo == null)
					{
						uInfo = new UserInfo();
						uInfo.Load(rInfo.UserId);
						users[rInfo.UserId] = uInfo;
					}
					uInfo.AddEvent(eInfo);
					uInfo.AddRecipient(rInfo);
				}
			}
		}
		#endregion
		#region GetRecipientsReader()
		static IDataReader GetRecipientsReader(RecipientsType type, string param1, DateTime param2)
		{
			switch (type)
			{
				case RecipientsType.Batch:
					return DBSystemEvents.RecipientsGetBatch(int.Parse(param1, provDefault), param2);
				case RecipientsType.Transaction:
					return DBSystemEvents.RecipientsGetTran(param1);
				default:
					return null;
			}
		}
		#endregion

		#region SendMessage()
		internal static void SendMessage(
			DeliveryType type
			, string address
			, string body
			, string subject
			)
		{
			SendMessage(type, null, address, body, subject);
		}

		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="from">From.</param>
		/// <param name="address">The address.</param>
		/// <param name="body">The body.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="attachments">The attachments.</param>
		/// <remarks>
		/// Primary Method
		/// </remarks>
		private static void SendMessage(
			DeliveryType type
			, string from
			, string address
			, string body
			, string subject
			//, Attachment[] attachments
			)
		{
			IAlertService5 alertService = GetAlertService();
			if (alertService != null)
			{
				switch (type)
				{
					case DeliveryType.Email:
						string sender = null;

						if (!string.IsNullOrEmpty(from))
							sender = BuldEmailAddress(null, null, from);
						else
							sender = BuldEmailAddress(AlertSenderFirstName, AlertSenderLastName, AlertSenderEmail);

						if (!string.IsNullOrEmpty(sender))
						{
							IEmailAlertMessage6 msg = CreateEmailAlertMessage(alertService);
							msg.Sender = sender;
							msg.Recipient = address;
							msg.Subject = subject;
							msg.Body = body;

							//if (attachments != null)
							//{
							//    byte[] buf = new byte[4096];
							//    IEmailAlertAttachment iAtt;
							//    int read;
							//    foreach (Attachment att in attachments)
							//    {
							//        iAtt = msg.CreateAttachment(att.FileName);
							//        if (iAtt != null)
							//        {
							//            do
							//            {
							//                read = att.Data.Read(buf, 0, buf.Length);
							//                iAtt.Write(buf, read);
							//            } while (read == buf.Length);
							//            iAtt.Complete();
							//        }
							//    }
							//}
							msg.BeginSend();
						}
						break;
					case DeliveryType.IBN:
						if (PortalConfig.UseIM && !string.IsNullOrEmpty(AlertSenderIM))
						{
							IIbnAlertMessage msgIbn = alertService.CreateIbnAlert();
							msgIbn.Sender = AlertSenderIM;
							msgIbn.Recipient = address;
							msgIbn.Body = body;
							msgIbn.BeginSend();
						}
						break;
				}
			}
		}
		#endregion

		#region GetMessageTemplate()
		private static AlertTemplate GetMessageTemplate(Hashtable templates, Hashtable locales, int eventTypeId, int messageTypeId, int languageId)
		{
			AlertTemplate ret;

			string locale = (string)locales[languageId];
			string key = AlertTemplate.MakeKey(ref locale, languageId, eventTypeId, messageTypeId);
			locales[languageId] = locale;
			ret = (AlertTemplate)templates[key];
			if (ret == null)
			{
				ret = new AlertTemplate(locale, key, languageId, eventTypeId, messageTypeId);
				ret.Load();
				templates[key] = ret;
			}
			return ret;
		}
		#endregion

		#region MakeObjectLink()
		private static string MakeObjectLink(string relativeLink, object id)
		{
			return string.Format("{0}{1}{2}", Configuration.PortalLink, relativeLink, id);
		}
		#endregion

		#region MakeClientLink()
		internal static string MakeClientLink(string value, string guid)
		{
			if (-1 != value.IndexOf(IssueLink))
				value = value.Replace(IssueLink, IssueLinkClient);

			if (value != "")
				value += ExternalGuid + guid;

			return value;
		}
		#endregion

		#region MakeExternalLink()
		internal static string MakeExternalLink(string value, string login, string guid)
		{
			string ret = "";

			if (-1 != value.IndexOf(EventLink))
				ret = value.Replace(EventLink, EventLinkExt);
			else if (-1 != value.IndexOf(IssueLink))
				ret = value.Replace(IssueLink, IssueLinkExt);
			else if (-1 != value.IndexOf(TaskLink))
				ret = value.Replace(TaskLink, TaskLinkExt);
			else if (-1 != value.IndexOf(TodoLink))
				ret = value.Replace(TodoLink, TodoLinkExt);
			else if (-1 != value.IndexOf(DocumentLink))
				ret = value.Replace(DocumentLink, DocumentLinkExt);

			if (ret != "")
				ret += ExternalLogin + login + ExternalGuid + guid;
			else
				ret = value;

			return ret;
		}
		#endregion

		internal static string MakeIssueLink(object id)
		{
			return MakeObjectLink(IssueLink, id);
		}

		#region GetMessage()
		private static Message GetMessage(
			Hashtable messages,
			AlertTemplate tmpl,
			RecipientInfo rInfo,
			EventInfo eInfo,
			UserInfo uInfo)
		{
			Message ret = null;

			string key = string.Format("{0}|{1}|{2}", tmpl.Key, eInfo.EventId, uInfo.TimeZoneId);
			ret = (Message)messages[key];
			if (ret == null || uInfo.IsExternal)
			{
				ArrayList variables = eInfo.GetVariablesArrayList(uInfo.LanguageId);

				ret = GetMessage(tmpl.Subject, tmpl.Body, uInfo, variables, eInfo.ObjectTypeId, eInfo.ObjectId, eInfo.ObjectUid);
				if (!uInfo.IsExternal)
					messages[key] = ret;
			}

			return ret;
		}

		internal static Message GetMessage(
			string subjectTemplate
			, string bodyTemplate
			, SystemEventTypes eventType
			, int? objectId
			, Guid? objectUid
			, int relObjectId
			, int userId
			)
		{
			ObjectTypes objectType, relObjectType;
			ArrayList vars = GetEventVariables(eventType, objectId, objectUid, relObjectId, out objectType, out relObjectType);

			UserInfo ui = new UserInfo();
			ui.Load(userId);

			return GetMessage(subjectTemplate, bodyTemplate, ui, vars, (int)objectType, objectId, objectUid);
		}

		private static Message GetMessage(
			string subjectTemplate
			, string bodyTemplate
			, UserInfo uInfo
			, ArrayList vars
			, int objectTypeId
			, int? objectId
			, Guid? objectUid
			)
		{
			StringBuilder sbSubject = new StringBuilder();
			StringBuilder sbBody = new StringBuilder();
			string name2, name3;

			IFormatProvider prov = new CultureInfo(uInfo.Locale, true);

			sbSubject.Append(subjectTemplate);
			sbBody.Append(bodyTemplate);

			// Replace variables with values
			Hashtable vars2 = new Hashtable();

			string guid = "";
			if (uInfo.IsExternal && objectId.HasValue)
			{
				if (uInfo.UserId > 0)
					guid = DBCommon.GetGateGuid(objectTypeId, objectId.Value, uInfo.UserId);
				else
					guid = DBCommon.GetGateGuid(objectTypeId, objectId.Value, uInfo.Email);
			}

			string linkStartValue = string.Empty, linkEndValue = string.Empty;
			foreach (VariableInfo var in vars)
			{
				//VariableInfo var = new VariableInfo(vNode);

				name2 = string.Format("[={0}=]", var.name);
				name3 = string.Format("[=/{0}=]", var.name);

				if (var.IsLink) // Modify link
				{
					if (uInfo.IsExternal && !var.External)
						var.value = string.Empty;

					if (string.IsNullOrEmpty(var.value))
						linkStartValue = linkEndValue = string.Empty;
					else
					{
						if (uInfo.IsExternal)
						{
							if (uInfo.UserId > 0)
								var.value = MakeExternalLink(var.value, uInfo.Login, guid);
							else
								var.value = MakeClientLink(var.value, guid);
						}

						// Add NoMenu parameter to the end of link
						if (!var.DisableNoMenu && !uInfo.MenuInAlerts && var.name != "PortalLink" && var.name != "ServerLink")
						{
							if (var.value.IndexOf('?') != -1)
								var.value += "&";
							else
								var.value += '?';
							var.value += "nomenu=1";
						}

						linkStartValue = string.Format(CultureInfo.InvariantCulture, "<a href=\"{0}\">", HttpUtility.HtmlAttributeEncode(var.value));
						linkEndValue = "</a>";
					}
				}

				if (var.type == VariableType.Date.ToString())
					var.value = Alerts2.DateReformat(var.value, prov);
				else if (var.type == VariableType.DateTime.ToString())
					var.value = Alerts2.DateTimeReformat(var.value, prov, uInfo.TimeZoneId);

				vars2[var.name] = var.value;
				sbSubject.Replace(name2, var.value);

				if (var.IsLink)
				{
					sbBody.Replace(name2, linkStartValue);
					sbBody.Replace(name3, linkEndValue);
				}
				else
				{
					if (!var.IsHtml)
					{
						var.value = HttpUtility.HtmlEncode(var.value);
						var.value = var.value.Replace("\r\n", "<br>");
					}
					sbBody.Replace(name2, var.value);
				}
			}
			return new Message(sbSubject.ToString(), sbBody.ToString());

			/*				// Attachments
							Stream data = null;
							string fileName = string.Empty;
							if(eInfo.EventType == AlertEventType.Asset_Object.ToString())
							{
								data = Asset.GetVersionContent(eInfo.ObjectId);
								fileName = (string)vars2["FileName"];
							}
							if(data != null)
								ret.Attachments.Add(new Attachment(fileName, data));
			*/
		}
		#endregion

		// Internal
		#region Init
		public static void Init()
		{
			Current.Init2();
		}

		public void Init2()
		{
			_alertSenderFirstName = "";
			_alertSenderLastName = "";
			_alertSenderEmail = "";
			_alertSenderIM = "";
			provDefault = new CultureInfo("");
			provEn = new CultureInfo("en-US");
			provRu = new CultureInfo("ru-RU");

			// Load information about "alert" user (alert sender).
			using (IDataReader reader = DBUser.GetUserInfoByLogin("alert"))
			{
				if (reader.Read())
				{
					_alertSenderFirstName = reader["FirstName"].ToString();
					_alertSenderLastName = reader["LastName"].ToString();
					_alertSenderEmail = reader["Email"].ToString();
					_alertSenderIM = reader["OriginalId"].ToString();
				}
			}
		}
		#endregion

		#region Send()
		internal static void Send(string tranId)
		{
			if (!AlertsEnabled)
				return;

			ArrayList recipients = new ArrayList();
			Hashtable events = new Hashtable();
			Hashtable users = new Hashtable();
			Hashtable templates = new Hashtable();
			Hashtable messages = new Hashtable();
			Hashtable locales = new Hashtable();
			EventInfo eInfo;
			UserInfo uInfo, currentUser = null;
			AlertTemplate tmpl;
			Message msg;

			CollectEventsAndRecipients(RecipientsType.Transaction, recipients, events, users, tranId, DateTime.MinValue);
			foreach (RecipientInfo rInfo in recipients)
			{
				eInfo = (EventInfo)events[rInfo.SystemEventId];
				if (rInfo.UserId > 0)
					uInfo = (UserInfo)users[rInfo.UserId];
				else
				{
					if (currentUser == null)
					{
						currentUser = new UserInfo();
						currentUser.Init(Security.CurrentUser, rInfo.Email, rInfo.EmailFrom);
					}
					uInfo = currentUser;
				}

				tmpl = GetMessageTemplate(templates, locales, eInfo.EventTypeId, rInfo.MessageTypeId, uInfo.LanguageId);
				msg = GetMessage(messages, tmpl, rInfo, eInfo, uInfo);
				DeliveryType deliveryType;

				using (DbTransaction tran = DbTransaction.Begin())
				{
					int LogId = DbAlert2.MessageLogAdd(msg.Subject, msg.Body);
					DBSystemEvents.RecipientUpdateSend(rInfo.RecipientId, uInfo.IsNotifiedByEmail, PortalConfig.UseIM && uInfo.IsNotifiedByIBN, LogId);

					tran.Commit();
				}

				// Send New Alert
				/*
				foreach (MessageDeliveryProvider provider in new MessageDeliveryProvider[] { })
				{
					// Check That Provider Enable
					if (provider.Enable &&
						PortalConfig_CheckDeliveryProviderEnable(provider) &&
						UserConfig_CheckDeliveryProviderEnable(provider))
					{
						// Create Message
						string from = uInfo.EmailFrom;
						string to = uInfo.GetAddress(deliveryType);

						EntityObject message = provider.CreateMessage(from, to, msg.Subject, msg.Body);

						CreateRequest createMessageRequest = new CreateRequest(message);

						createMessageRequest.Parameters.Add(OutgoingMessageQueuePlugin.AddToQueue, true);
						createMessageRequest.Parameters.Add(OutgoingMessageQueuePlugin.SourceName, "AlertService");

						CreateResponse response = (CreateResponse)BusinessManager.Execute(createMessageRequest);

						// TODO: Save Primary Key Id.
						// response.PrimaryKeyId;
					}
				}
				*/

				// Send alert to e-mail
				try
				{
					if (uInfo.IsNotifiedByEmail)
					{
						deliveryType = DeliveryType.Email;
						string body
							= "<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN' >"
							+ "<html>"
							+ "<head>"
							+ "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' >"
							+ "<title>"
							+ HttpUtility.HtmlEncode(msg.Subject)
							+ "</title>"
							+ "<style type='text/css'>"
							+ "body { font: 10pt Arial,Helvetica,Sans-Serif; color: #000000; }"
							+ ".subject { color: #999999; font-weight: bold; font-size: 120%; }"
							+ "</style>"
							+ "</head>"
							+ "<body>"
							+ msg.Body
							+ "</body>"
							+ "</html>";

						using (DbTransaction tran = DbTransaction.Begin())
						{
							SendMessage(deliveryType, uInfo.EmailFrom, uInfo.GetAddress(deliveryType), body, msg.Subject /*, (Attachment[])msg.Attachments.ToArray(typeof(Attachment))*/);
							DBSystemEvents.RecipientUpdateSent(rInfo.RecipientId, (int)deliveryType, true);

							tran.Commit();
						}
					}
				}
				catch (Exception ex)
				{
					Log.WriteError(ex.ToString());
				}

				// Send alert to IM
				try
				{
					if (uInfo != null && uInfo.IsNotifiedByIBN)
					{
						deliveryType = DeliveryType.IBN;
						using (DbTransaction tran = DbTransaction.Begin())
						{
							SendMessage(deliveryType, uInfo.GetAddress(deliveryType), msg.Body, msg.Subject);
							DBSystemEvents.RecipientUpdateSent(rInfo.RecipientId, (int)deliveryType, true);

							tran.Commit();
						}
					}
				}
				catch (Exception ex)
				{
					Log.WriteError(ex.ToString());
				}
			}
		}

		private static bool UserConfig_CheckDeliveryProviderEnable(MessageDeliveryProvider provider)
		{
			// TODO: Not Implemented yet

			return true;
		}

		private static bool PortalConfig_CheckDeliveryProviderEnable(MessageDeliveryProvider provider)
		{
			// TODO: Not Implemented yet

			return true;
		}
		#endregion

		#region AddVariableInitiatedBy()
		private static void AddVariableInitiatedBy(ArrayList vars)
		{
			string cuName = "";
			UserLight cu = Security.CurrentUser;
			if (cu != null)
				cuName = cu.DisplayName;
			vars.Add(new VariableInfo(new AlertVariable(Variable.InitiatedBy), cuName));
		}
		#endregion

		#region GetXmlData()
		internal static string GetXmlData(SystemEventTypes eventType, int? objectId, Guid? objectUid, int relObjectId, out ObjectTypes objectType, out ObjectTypes relObjectType, ref string objectTitle, ref string relObjectTitle, Dictionary<string, string> additionalValues)
		{
			ArrayList vars = GetEventVariables(eventType, objectId, objectUid, relObjectId, out objectType, out relObjectType);

			StringBuilder sb = new StringBuilder();
			XmlTextWriter w = new XmlTextWriter(new StringWriter(sb));

			w.WriteStartElement("variables");
			foreach (VariableInfo var in vars)
			{
				// O.R. [2008-12-08]: Override variables from additionalValues
				if (additionalValues != null && additionalValues.ContainsKey(var.name))
				{
					var.value = additionalValues[var.name];
				}

				var.Save(w);

				if (var.name == "Title")
					objectTitle = var.value;
				else if (var.name == "RelTitle")
					relObjectTitle = var.value;
			}
			w.WriteEndElement();

			return sb.ToString();
		}
		#endregion
		#region GetEventVariables()
		private static ArrayList GetEventVariables(
			SystemEventTypes eventType
			, int? objectId
			, Guid? objectUid
			, int relObjectId
			, out ObjectTypes objectType
			, out ObjectTypes relObjectType
			)
		{
			SystemEvents.GetSystemEventObjectTypes(eventType, out objectType, out relObjectType);

			AlertVariable[] names = AlertVariable.GetVariables(eventType);

			// Add common variables
			ArrayList vars = new ArrayList();

			foreach (AlertVariable var in names)
			{
				if (!var.isRelObject)
				{
					switch (var.name)
					{
						case Variable.InitiatedBy:
							AddVariableInitiatedBy(vars);
							break;
						case Variable.ServerLink:
							vars.Add(new VariableInfo(var, Configuration.ServerLink));
							break;
						case Variable.PortalLink:
							vars.Add(new VariableInfo(var, Configuration.PortalLink));
							break;
						case Variable.UnsubscribeLink:
							if (objectId.HasValue)
								vars.Add(new VariableInfo(var, string.Format("{0}/Directory/Unsubscribe.aspx?EventTypeId={1}&ObjectId={2}", Configuration.PortalLink, (int)eventType, objectId.Value)));
							else if (objectUid.HasValue)
								vars.Add(new VariableInfo(var, string.Format("{0}/Directory/Unsubscribe.aspx?EventTypeId={1}&ObjectUid={2}", Configuration.PortalLink, (int)eventType, objectUid.Value)));
							break;
					}
				}
			}

			GetObjectVariables(objectType, objectId, objectUid, false, names, vars);
			if (relObjectType != ObjectTypes.UNDEFINED)
			{
				if (relObjectId > 0 || relObjectType == ObjectTypes.User)
					GetObjectVariables(relObjectType, relObjectId, Guid.Empty, true, names, vars);
				else
				{
					foreach (AlertVariable var in names)
					{
						if (!var.isRelObject)
							vars.Add(new VariableInfo(var, string.Empty));
					}
				}
			}
			return vars;
		}
		#endregion
		#region GetObjectVariables*()
		#region GetObjectVariables()
		internal static void GetObjectVariables(ObjectTypes objectType, int? objectId, Guid? objectUid, bool isRelObject, AlertVariable[] names, ArrayList vars)
		{
			Hashtable multilangVars = new Hashtable();
			switch (objectType)
			{
				case ObjectTypes.File_FileStorage:
					GetObjectVariablesFile(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.CalendarEntry:
					GetObjectVariablesCalendarEntry(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.Comment:
					GetObjectVariablesComment(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.Document:
					GetObjectVariablesDocument(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.Issue:
					GetObjectVariablesIssue(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.IssueBox:
					GetObjectVariablesIssueBox(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.IssueRequest:
					GetObjectVariablesIssueRequest(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.List:
					GetObjectVariablesList(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.Project:
					GetObjectVariablesProject(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.Task:
					GetObjectVariablesTask(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.ToDo:
					GetObjectVariablesTodo(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.User:
					GetObjectVariablesUser(isRelObject, objectId.Value, names, vars, multilangVars);
					break;
				case ObjectTypes.Assignment:
					GetObjectVariablesAssignment(isRelObject, objectUid.Value, names, vars, multilangVars);
					break;
			}

			// Load languages
			Dictionary<int, string> languages = new Dictionary<int, string>();
			using (IDataReader reader = Common.GetListLanguages())
			{
				while (reader.Read())
				{
					int languageId = (int)reader["LanguageId"];
					string locale = reader["Locale"].ToString();
					languages.Add(languageId, locale);
				}
			}

			// Get multilanguage variables(multilangVars, vars);
			foreach (AlertVariable var in multilangVars.Keys)
			{
				string valueTemplate = multilangVars[var] as string;
				if (valueTemplate != null)
				{
					foreach (int languageId in languages.Keys)
					{
						string value = Common.GetWebResourceString(valueTemplate, CultureInfo.GetCultureInfo(languages[languageId]));
						vars.Add(new VariableInfo(var, value, languageId.ToString(CultureInfo.InvariantCulture)));
					}
				}
				else
				{
					using (IDataReader reader = GetMultilangVarsReader(var, (int)multilangVars[var]))
					{
						while (reader.Read())
							vars.Add(new VariableInfo(var, reader["Name"].ToString(), reader["LanguageId"].ToString()));
					}
				}
			}
		}
		#endregion

		#region GetObjectVariablesCalendarEntry
		private static void GetObjectVariablesCalendarEntry(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			string id;

			using (IDataReader reader = DbAlert2.GetVariablesCalendarEntry(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.CreatedBy:
								case Variable.Manager:
								case Variable.UpdatedBy:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Title:
								case Variable.Description:
								case Variable.ProjectTitle:
									vars.Add(new VariableInfo(var, HttpUtility.HtmlDecode(reader[var.name.ToString()].ToString())));
									break;
								case Variable.End:
								case Variable.Start:
									vars.Add(new VariableInfo(var, DateTimeToString(reader[var.name.ToString()]), VariableType.DateTime));
									break;
								case Variable.State:
									multilangVars[var] = reader["StateId"];
									break;
								case Variable.EventType:
									multilangVars[var] = reader["TypeId"];
									break;
								case Variable.Link:
									vars.Add(new VariableInfo(var, MakeObjectLink(EventLink, objectId)));
									break;
								case Variable.Priority:
									multilangVars[var] = reader["PriorityId"];
									break;
								case Variable.ProjectLink:
									id = reader["ProjectId"].ToString();
									vars.Add(new VariableInfo(var, id.Length == 0 ? id : MakeObjectLink(ProjectLink, id)));
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesComment()
		private static void GetObjectVariablesComment(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			using (IDataReader reader = DbAlert2.GetVariablesComment(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.Title:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString(), true));
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesDocument()
		private static void GetObjectVariablesDocument(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			string id;

			using (IDataReader reader = DbAlert2.GetVariablesDocument(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.CreatedBy:
								case Variable.DocumentStatus:
								case Variable.Manager:
								case Variable.UpdatedBy:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Title:
								case Variable.Description:
								case Variable.ProjectTitle:
									vars.Add(new VariableInfo(var, HttpUtility.HtmlDecode(reader[var.name.ToString()].ToString())));
									break;
								case Variable.Link:
									vars.Add(new VariableInfo(var, MakeObjectLink(DocumentLink, objectId)));
									break;
								case Variable.Priority:
									multilangVars[var] = reader["PriorityId"];
									break;
								case Variable.ProjectLink:
									id = reader["ProjectId"].ToString();
									vars.Add(new VariableInfo(var, id.Length == 0 ? id : MakeObjectLink(ProjectLink, id)));
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesFile()
		private static void GetObjectVariablesFile(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			string title = null;
			AlertVariable linkVariable = null;
			using (IDataReader reader = DbAlert2.GetVariablesFile(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.FileSize:
								case Variable.Title:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Link:
									title = reader[Variable.Title.ToString()].ToString();
									linkVariable = var;
									break;
							}
						}
					}
				}
			}

			if (title != null && linkVariable != null)
				vars.Add(new VariableInfo(linkVariable, WebDAV.Common.WebDavUrlBuilder.GetFileStorageWebDavUrlForAlerts(objectId, title, false)));
		}
		#endregion
		#region GetObjectVariablesIssue()
		private static void GetObjectVariablesIssue(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			string id;
			EMail.IncidentBox box = EMail.IncidentBox.Load(Incident.GetIncidentBox(objectId));

			using (IDataReader reader = DbAlert2.GetVariablesIssue(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.CreatedBy:
								case Variable.Manager:
								case Variable.UpdatedBy:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Title:
								case Variable.Description:
								case Variable.ProjectTitle:
									vars.Add(new VariableInfo(var, HttpUtility.HtmlDecode(reader[var.name.ToString()].ToString())));
									break;
								case Variable.IssueState:
									multilangVars[var] = reader["StateId"];
									break;
								case Variable.Link:
									vars.Add(new VariableInfo(var, MakeObjectLink(IssueLink, objectId)));
									break;
								case Variable.Priority:
									multilangVars[var] = reader["PriorityId"];
									break;
								case Variable.ProjectLink:
									id = reader["ProjectId"].ToString();
									vars.Add(new VariableInfo(var, id.Length == 0 ? id : MakeObjectLink(ProjectLink, id)));
									break;
								case Variable.Ticket:
									vars.Add(new VariableInfo(var, EMail.TicketUidUtil.Create(box.IdentifierMask, objectId)));
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesIssueBox()
		private static void GetObjectVariablesIssueBox(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			using (IDataReader reader = DbAlert2.GetVariablesIssueBox(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.Manager:
								case Variable.Title:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Link:
									vars.Add(new VariableInfo(var, MakeObjectLink(IssueBoxLink, objectId)));
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesIssueRequest()
		private static void GetObjectVariablesIssueRequest(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			using (IDataReader reader = DbAlert2.GetVariablesIssueRequest(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.CreatedBy:
								case Variable.Email:
								case Variable.MailBoxTitle:
								case Variable.Title:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Link:
									vars.Add(new VariableInfo(var, MakeObjectLink(IssueRequestLink, objectId)));
									break;
								case Variable.MailBoxLink:
									string id = reader["MailBoxId"].ToString();
									vars.Add(new VariableInfo(var, id.Length == 0 ? id : MakeObjectLink(MailBoxLink, id)));
									break;
								case Variable.Priority:
									multilangVars[var] = reader["Priority"];
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesList()
		private static void GetObjectVariablesList(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			using (IDataReader reader = DbAlert2.GetVariablesList(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.CreatedBy:
								case Variable.Description:
								case Variable.Title:
								case Variable.UpdatedBy:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Link:
									string linkTemplate = var.isDataLink ? ListDataLink : ListInfoLink;
									vars.Add(new VariableInfo(var, MakeObjectLink(linkTemplate, objectId)));
									break;
								case Variable.ListStatus:
								case Variable.ListType:
									multilangVars[var] = reader[var.name.ToString()].ToString();
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesProject()
		private static void GetObjectVariablesProject(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			using (IDataReader reader = DbAlert2.GetVariablesProject(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.CreatedBy:
								case Variable.Manager:
								case Variable.PercentCompleted:
								case Variable.UpdatedBy:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Title:
								case Variable.Description:
									vars.Add(new VariableInfo(var, HttpUtility.HtmlDecode(reader[var.name.ToString()].ToString())));
									break;
								case Variable.End:
								case Variable.Start:
									vars.Add(new VariableInfo(var, DateTimeToString(reader[var.name.ToString()]), VariableType.DateTime));
									break;
								case Variable.Link:
									vars.Add(new VariableInfo(var, MakeObjectLink(ProjectLink, objectId)));
									break;
								case Variable.Priority:
									multilangVars[var] = reader["PriorityId"];
									break;
								case Variable.ProjectStatus:
									multilangVars[var] = reader["StatusId"];
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesTask()
		private static void GetObjectVariablesTask(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			string id;

			using (IDataReader reader = DbAlert2.GetVariablesTask(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.CreatedBy:
								case Variable.Manager:
								case Variable.PercentCompleted:
								case Variable.UpdatedBy:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Title:
								case Variable.Description:
								case Variable.ProjectTitle:
									vars.Add(new VariableInfo(var, HttpUtility.HtmlDecode(reader[var.name.ToString()].ToString())));
									break;
								case Variable.End:
								case Variable.Start:
									vars.Add(new VariableInfo(var, DateTimeToString(reader[var.name.ToString()]), VariableType.DateTime));
									break;
								case Variable.State:
									multilangVars[var] = reader["StateId"];
									break;
								case Variable.Link:
									vars.Add(new VariableInfo(var, MakeObjectLink(TaskLink, objectId)));
									break;
								case Variable.Priority:
									multilangVars[var] = reader["PriorityId"];
									break;
								case Variable.ProjectLink:
									id = reader["ProjectId"].ToString();
									vars.Add(new VariableInfo(var, id.Length == 0 ? id : MakeObjectLink(ProjectLink, id)));
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesTodo()
		private static void GetObjectVariablesTodo(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			string id;

			using (IDataReader reader = DbAlert2.GetVariablesTodo(objectId))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							switch (var.name)
							{
								case Variable.CreatedBy:
								case Variable.Manager:
								case Variable.PercentCompleted:
								case Variable.UpdatedBy:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Title:
								case Variable.Description:
								case Variable.ProjectTitle:
									vars.Add(new VariableInfo(var, HttpUtility.HtmlDecode(reader[var.name.ToString()].ToString())));
									break;
								case Variable.End:
								case Variable.Start:
									vars.Add(new VariableInfo(var, DateTimeToString(reader[var.name.ToString()]), VariableType.DateTime));
									break;
								case Variable.State:
									multilangVars[var] = reader["StateId"];
									break;
								case Variable.Link:
									vars.Add(new VariableInfo(var, MakeObjectLink(TodoLink, objectId)));
									break;
								case Variable.Priority:
									multilangVars[var] = reader["PriorityId"];
									break;
								case Variable.ProjectLink:
									id = reader["ProjectId"].ToString();
									vars.Add(new VariableInfo(var, id.Length == 0 ? id : MakeObjectLink(ProjectLink, id)));
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesUser()
		private static void GetObjectVariablesUser(bool isRelObject, int objectId, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			if (isRelObject && objectId == -1) // Group responsibility for issues
			{
				foreach (AlertVariable var in names)
				{
					if (var.isRelObject == isRelObject)
					{
						switch (var.name)
						{
							case Variable.Title:
								multilangVars[var] = "{IbnFramework.Incident:GroupResp}";
								break;
							default:
								vars.Add(new VariableInfo(var, string.Empty));
								break;
						}
					}
				}
			}
			else // User
			{
				using (IDataReader reader = DbAlert2.GetVariablesUser(objectId))
				{
					if (reader.Read())
					{
						foreach (AlertVariable var in names)
						{
							if (var.isRelObject == isRelObject)
							{
								switch (var.name)
								{
									case Variable.Email:
									case Variable.Login:
									case Variable.Password: // TODO: remove
									case Variable.Title:
										vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
										break;
									case Variable.Link:
										vars.Add(new VariableInfo(var, MakeObjectLink(UserLink, objectId)));
										break;
								}
							}
						}
					}
				}
			}
		}
		#endregion
		#region GetObjectVariablesAssignment
		private static void GetObjectVariablesAssignment(bool isRelObject, Guid objectUid, AlertVariable[] names, ArrayList vars, Hashtable multilangVars)
		{
			using (IDataReader reader = DbAlert2.GetVariablesAssignment(objectUid))
			{
				if (reader.Read())
				{
					foreach (AlertVariable var in names)
					{
						if (var.isRelObject == isRelObject)
						{
							string link;
							switch (var.name)
							{
								case Variable.CreatedBy:
								case Variable.Participant:
									vars.Add(new VariableInfo(var, reader[var.name.ToString()].ToString()));
									break;
								case Variable.Title:
								case Variable.OwnerTitle:
									vars.Add(new VariableInfo(var, HttpUtility.HtmlDecode(reader[var.name.ToString()].ToString())));
									break;
								case Variable.End:
									vars.Add(new VariableInfo(var, DateTimeToString(reader[var.name.ToString()]), VariableType.DateTime));
									break;
								case Variable.State:
									int state = (int)reader["State"];
									if (state == (int)AssignmentState.Active)
										multilangVars[var] = "{IbnFramework.BusinessProcess:AssignmentStateActive}";
									else if (state == (int)AssignmentState.Closed)
										multilangVars[var] = "{IbnFramework.BusinessProcess:AssignmentStateClosed}";
									else if (state == (int)AssignmentState.Pending)
										multilangVars[var] = "{IbnFramework.BusinessProcess:AssignmentStatePending}";
									else if (state == (int)AssignmentState.Suspended)
										multilangVars[var] = "{IbnFramework.BusinessProcess:AssignmentStateSuspended}";
									break;
								case Variable.Link:
									link = string.Empty;
									if (reader["OwnerDocumentId"] != DBNull.Value)
									{
										link = string.Format(CultureInfo.InvariantCulture,
											"{0}/Documents/DocumentView.aspx?DocumentId={1}&assignmentId={2}&Tab=General",
											Configuration.PortalLink, 
											reader["OwnerDocumentId"].ToString(),
											objectUid);
									}
									vars.Add(new VariableInfo(var, link));
									break;
								case Variable.Priority:
									int priorityId = (int)reader["PriorityId"];
									switch (priorityId)
									{
										case (int)AssignmentPriority.Low:
											multilangVars[var] = (int)Priority.Low;
											break;
										case (int)AssignmentPriority.Normal:
											multilangVars[var] = (int)Priority.Normal;
											break;
										case (int)AssignmentPriority.High:
											multilangVars[var] = (int)Priority.High;
											break;
										case (int)AssignmentPriority.VeryHigh:
											multilangVars[var] = (int)Priority.VeryHigh;
											break;
										case (int)AssignmentPriority.Urgent:
											multilangVars[var] = (int)Priority.Urgent;
											break;
									}
									break;
								case Variable.OwnerLink:
									link = string.Empty;
									if (reader["OwnerDocumentId"] != DBNull.Value)
									{
										link = string.Format(CultureInfo.InvariantCulture,
											"{0}/Documents/DocumentView.aspx?DocumentId={1}",
											Configuration.PortalLink,
											reader["OwnerDocumentId"].ToString());
									}

									vars.Add(new VariableInfo(var, link));
									break;
							}
						}
					}
				}
			}
		}
		#endregion
		#endregion
		#region GetMultilangVarsReader
		private static IDataReader GetMultilangVarsReader(AlertVariable var, int itemId)
		{
			switch (var.name)
			{
				case Variable.EventType:
					return DbAlert2.GetEventTypeNames(itemId);
				case Variable.IssueState:
					return DbAlert2.GetIssueStateNames(itemId);
				case Variable.Priority:
					return DbAlert2.GetPriorityNames(itemId);
				case Variable.ProjectStatus:
					return DbAlert2.GetProjectStatusNames(itemId);
				case Variable.State:
					return DbAlert2.GetObjectStateNames(itemId);
				default:
					throw new Exception("Unknown multilanguage variable.");
			}
		}
		#endregion

		#region Message_GetByUserIdAndTimeFilter
		public static DataView Message_GetByUserIdAndTimeFilter(int userId, DateTime startDate, DateTime endDate)
		{
			if (startDate == DateTime.MinValue)
				startDate = Report.DefaultStartDate;
			if (endDate == DateTime.MinValue)
				endDate = Report.DefaultEndDate;

			DataTable dt = null;
			using (IDataReader reader = DbAlert2.Message_GetByUserIdAndTimeFilter(userId, startDate, endDate))
			{
				dt = new DataTable();
				dt.Columns.Add(new DataColumn("send_time", System.Type.GetType("System.DateTime")));
				dt.Columns.Add(new DataColumn("mess_text", System.Type.GetType("System.String")));
				while (reader.Read())
				{
					DataRow dr = dt.NewRow();
					dr["send_time"] = (DateTime)reader["Sent"];
					dr["mess_text"] = reader["Body"].ToString();
					dt.Rows.Add(dr);
				}
			}
			return dt.DefaultView;
		}
		#endregion

		#region DateTimeToString()
		internal static string DateTimeToString(object val)
		{
			string ret = string.Empty;
			if (val != null && val != DBNull.Value)
				ret = ((DateTime)val).ToString("s", provDefault);
			return ret;
		}
		#endregion
		#region DateTimeParse()
		internal static DateTime DateTimeParse(string val)
		{
			DateTime ret;
			try
			{
				ret = DateTime.Parse(val, provDefault);
			}
			catch
			{
				try
				{
					ret = DateTime.Parse(val, provEn);
				}
				catch
				{
					ret = DateTime.Parse(val, provRu);
				}
			}
			return ret;
		}
		#endregion
		#region DateReformat()
		internal static string DateReformat(string val, IFormatProvider prov)
		{
			string ret = val;
			if (val != null && val.Length > 0)
			{
				try
				{
					ret = DateTimeParse(val).Date.ToString("d", prov);
				}
				catch (Exception ex)
				{
					Log.WriteError(ex.ToString());
				}
			}
			return ret;
		}
		#endregion
		#region DateTimeReformat()
		internal static string DateTimeReformat(string val, IFormatProvider prov, int timeZoneId)
		{
			string ret = val;
			if (val != null && val.Length > 0)
			{
				try
				{
					ret = User.GetLocalDate(timeZoneId, DateTimeParse(val)).ToString("g", prov);
				}
				catch (Exception ex)
				{
					Log.WriteError(ex.ToString());
				}
			}
			return ret;
		}
		#endregion

		#region SendBatch()
		internal static void SendBatch(int userId, DateTime dt)
		{
			if (!AlertsEnabled)
				return;

			ArrayList recipients = new ArrayList();
			Hashtable events = new Hashtable();
			Hashtable users = new Hashtable();

			EventInfo ei;

			CollectEventsAndRecipients(RecipientsType.Batch, recipients, events, users, userId.ToString(provDefault), dt);
			UserInfo ui = (UserInfo)users[userId];
			if (ui == null)
			{
				ui = new UserInfo();
				ui.Load(userId);
				users[userId] = ui;
			}
			else
			{
				string msgSubject, msgBody, itemSubject, itemBody, itemSubjectDel, itemBodyDel;
				AlertTemplate.GetTemplate(AlertTemplateTypes.Special, ui.Locale, SpecialMessageType.BatchAlert.ToString(), true, out msgSubject, out msgBody);
				AlertTemplate.GetTemplate(AlertTemplateTypes.Special, ui.Locale, SpecialMessageType.BatchItem.ToString(), true, out itemSubject, out itemBody);
				AlertTemplate.GetTemplate(AlertTemplateTypes.Special, ui.Locale, SpecialMessageType.BatchItemDeleted.ToString(), true, out itemSubjectDel, out itemBodyDel);

				// Build events list
				ArrayList vars;
				Message msg;
				StringBuilder sbText = new StringBuilder();

				foreach (RecipientInfo ri in ui.Recipients)
				{
					ei = (EventInfo)events[ri.SystemEventId];

					vars = ei.GetVariablesArrayList(ui.LanguageId);

					string subject, body;

					switch ((SystemEventTypes)ei.EventTypeId)
					{
						case SystemEventTypes.CalendarEntry_Deleted:
						case SystemEventTypes.Document_Deleted:
						case SystemEventTypes.Issue_Deleted:
						case SystemEventTypes.IssueRequest_Approved:
						case SystemEventTypes.IssueRequest_Deleted:
						case SystemEventTypes.List_Deleted:
						case SystemEventTypes.Project_Deleted:
						case SystemEventTypes.Task_Deleted:
						case SystemEventTypes.Todo_Deleted:
							subject = itemSubjectDel;
							body = itemBodyDel;
							break;
						default:
							subject = itemSubject;
							body = itemBody;
							break;
					}

					vars.Add(new VariableInfo(new AlertVariable(Variable.Link, true, false, true), string.Empty)); // RelLink
					vars.Add(new VariableInfo(new AlertVariable(Variable.Title, false, true, true), string.Empty)); // RelTitle
					vars.Add(new VariableInfo(new AlertVariable(Variable.EventDate), DateTimeToString(ei.EventDate), VariableType.DateTime));
					vars.Add(new VariableInfo(new AlertVariable(Variable.EventName), SystemEvents.GetSystemEventName(ei.EventType, ui.Locale)));

					msg = GetMessage(subject, body, ui, vars, ei.ObjectTypeId, ei.ObjectId, ei.ObjectUid);
					vars.Clear();
					sbText.Append(msg.Body);
				}

				vars = new ArrayList();
				vars.Add(new VariableInfo(new AlertVariable(Variable.End), DateTimeToString(dt), VariableType.DateTime));
				vars.Add(new VariableInfo(new AlertVariable(Variable.Start), DateTimeToString(ui.BatchLastSent), VariableType.DateTime));
				vars.Add(new VariableInfo(new AlertVariable(Variable.Text), sbText.ToString(), true));

				msg = GetMessage(msgSubject, msgBody, ui, vars, -1, -1, null);

				// Add message to log.
				using (DbTransaction tran = DbTransaction.Begin())
				{
					int logId = DbAlert2.MessageLogAdd(msg.Subject, msg.Body);
					foreach (RecipientInfo ri in ui.Recipients)
						DBSystemEvents.RecipientUpdateSend(ri.RecipientId, ui.IsNotifiedByEmail, PortalConfig.UseIM && ui.IsNotifiedByIBN, logId);
					User.UpdateBatchLastSent(ui.UserId, dt);

					tran.Commit();
				}

				// Send alert to e-mail
				if (ui.IsNotifiedByEmail)
					SendBatch(ui, DeliveryType.Email, msg);

				// Send alert to IM
				if (ui.IsNotifiedByIBN)
					SendBatch(ui, DeliveryType.IBN, msg);
			}

			// Calculate next batch time
			DateTime lastBatch = User.GetLocalDate(ui.TimeZoneId, dt);
			DateTime nextBatch = lastBatch.AddMinutes(ui.Period);
			if (ui.From != ui.Till && nextBatch.Hour >= ui.Till || nextBatch.Hour < ui.From)
				nextBatch = lastBatch.Date.AddDays(1).AddHours(ui.From);

			nextBatch = User.GetUTCDate(ui.TimeZoneId, nextBatch);
			User.UpdateBatchNextSend(ui.UserId, nextBatch);

			// Add next batch time to schedule
			Schedule.AddDateTypeValue(DateTypes.BatchAlert, ui.UserId, nextBatch);
		}

		private static void SendBatch(UserInfo ui, DeliveryType deliveryType, Message msg)
		{
			try
			{
				//Attachment[] attachments = null;
				//if (deliveryType == DeliveryType.Email)
				//    attachments = (Attachment[])msg.Attachments.ToArray(typeof(Attachment));

				using (DbTransaction tran = DbTransaction.Begin())
				{
					SendMessage(deliveryType, ui.EmailFrom, ui.GetAddress(deliveryType), msg.Body, msg.Subject/*, attachments*/);
					foreach (RecipientInfo ri in ui.Recipients)
					{
						DBSystemEvents.RecipientUpdateSent(ri.RecipientId, (int)deliveryType, true);
					}
					tran.Commit();
				}
			}
			catch (Exception ex)
			{
				Log.WriteError(ex.ToString());
			}
		}
		#endregion
		#region SendBroadcastMessage()
		internal static void SendBroadcastMessage(int messageId, string text)
		{
			if (!AlertsEnabled)
				return;

			ArrayList users = new ArrayList();
			using (IDataReader reader = DbAlert2.GetBroadcastRecipients(messageId))
			{
				while (reader.Read())
				{
					UserInfo ui = new UserInfo();

					ui.ImId = (int)reader["ImId"];
					ui.Locale = reader["Locale"].ToString();

					users.Add(ui);
				}
			}

			ArrayList vars = new ArrayList();
			AddVariableInitiatedBy(vars);
			vars.Add(new VariableInfo(new AlertVariable(Variable.Text), text, true));

			Hashtable messages = new Hashtable();
			foreach (UserInfo ui in users)
			{
				string message = (string)messages[ui.Locale];
				if (message == null)
				{
					string subject, body;
					AlertTemplate.GetTemplate(AlertTemplateTypes.Special, ui.Locale, SpecialMessageType.BroadcastMessage.ToString(), true, out subject, out body);
					Message msg = GetMessage(subject, body, ui, vars, -1, -1, null);
					message = msg.Body;
					messages[ui.Locale] = message;
				}

				try
				{
					SendMessage(DeliveryType.IBN, ui.GetAddress(DeliveryType.IBN), message, null);
				}
				catch (Exception ex)
				{
					Log.WriteError(ex.ToString());
				}
			}
		}
		#endregion
		#region internal static void SendForgottenPassword(int userId, string logonLink)
		internal static void SendForgottenPassword(int userId, string logonLink)
		{
			if (!AlertsEnabled)
				return;

			UserInfo ui = new UserInfo();
			ui.Load(userId);

			ArrayList vars = new ArrayList();
			vars.Add(new VariableInfo(new AlertVariable(Variable.LogonLink, true, false), logonLink));

			string subject, body;
			AlertTemplate.GetTemplate(AlertTemplateTypes.Special, ui.Locale, SpecialMessageType.ForgottenPassword.ToString(), true, out subject, out body);
			Message msg = GetMessage(subject, body, ui, vars, -1, -1, null);

			try
			{
				SendMessage(DeliveryType.Email, ui.GetAddress(DeliveryType.Email), msg.Body, msg.Subject);
			}
			catch (Exception ex)
			{
				Log.WriteError(ex.ToString());
				throw;
			}
		}
		#endregion
		#region SendFiles()
		//internal static void SendFiles(string containerKey, ArrayList files, ArrayList users)
		//{
		//    if (!AlertsEnabled || AlertServiceURL.Length == 0 || AlertSenderEmail.Length == 0)
		//        return;

		//    StringBuilder recipients = new StringBuilder();
		//    ArrayList emails = new ArrayList();
		//    foreach (object o in users)
		//    {
		//        string emailAddress = o.ToString();
		//        if (o is int)
		//            emailAddress = User.GetUserEmail((int)o);

		//        if (!emails.Contains(emailAddress))
		//        {
		//            emails.Add(emailAddress);
		//            recipients.AppendFormat("{0};", emailAddress);
		//        }
		//    }

		//    UserInfo ui = new UserInfo();
		//    ui.Load(Security.CurrentUser.UserID);

		//    string subject, body;
		//    AlertTemplate.GetTemplate(AlertTemplateTypes.Special, ui.Locale, SpecialMessageType.FileDelivery.ToString(), true, out subject, out body);

		//    ArrayList vars = new ArrayList();
		//    AddVariableInitiatedBy(vars);

		//    Message msg = GetMessage(subject, body, ui, vars, -1, -1);

		//    IEmailAlertMessage6 email = CreateEmailAlertMessage();
		//    if (email != null)
		//    {
		//        email.Bcc = recipients.ToString();
		//        email.Sender = BuldEmailAddress(AlertSenderFirstName, AlertSenderLastName, AlertSenderEmail);
		//        email.Subject = msg.Subject;
		//        email.Body = msg.Body;

		//        byte[] buf = new byte[4096];
		//        IEmailAlertAttachment iAtt;
		//        int read;
		//        foreach (int fileId in files)
		//        {
		//            CS.FileInfo fi = CS.FileStorage.GetFile(Security.CurrentUser.UserID, "Read", fileId);

		//            iAtt = email.CreateAttachment(fi.Name);
		//            if (iAtt != null)
		//            {
		//                CS.BaseIbnContainer bic = CS.BaseIbnContainer.Create("FileLibrary", containerKey);
		//                CS.FileStorage fs = (CS.FileStorage)bic.LoadControl("FileStorage");

		//                // Transaction is required for FileOpenRead()
		//                // because it doesn't know the connection string.
		//                using (DbTransaction tran = DbTransaction.Begin())
		//                {
		//                    using (Stream data = fs.FileOpenRead(fi))
		//                    {
		//                        do
		//                        {
		//                            read = data.Read(buf, 0, buf.Length);
		//                            iAtt.Write(buf, read);
		//                        } while (read == buf.Length);
		//                    }
		//                    // Don't commit the tran - we haven't changed anything.

		//                    // Artyom: we must commit. Otherwise the transaction will roll back;
		//                    tran.Commit();
		//                }
		//                iAtt.Complete();
		//            }
		//        }

		//        email.BeginSend();
		//    }
		//}
		#endregion

		#region GetAlertService()
		private static IAlertService5 GetAlertService()
		{
			IAlertService5 retVal = null;

			//string serviceUrl = AlertServiceURL;
			//if (!string.IsNullOrEmpty(serviceUrl))
			//    retVal = Activator.GetObject(typeof(IAlertService5), serviceUrl) as IAlertService5;
			retVal = new InnerAlertService();

			return retVal;
		}
		#endregion

		#region CreateEmailAlertMessage()
		private static IEmailAlertMessage6 CreateEmailAlertMessage()
		{
			IAlertService5 alertService = GetAlertService();
			return CreateEmailAlertMessage(alertService);
		}

		private static IEmailAlertMessage6 CreateEmailAlertMessage(IAlertService5 alertService)
		{
			IEmailAlertMessage6 email = null;

			if (alertService != null)
			{
				//SmtpSettings smtp = PortalConfig.SmtpSettings;

				email = alertService.CreateEmailAlert();

				//email.SmtpServer = smtp.Server;
				//email.SmtpPort = smtp.Port;
				//email.SmtpSecureConnection = smtp.SecureConnection.ToString();
				//email.SmtpAuthenticate = smtp.Authenticate;
				//email.SmtpUser = smtp.User;
				//email.SmtpPassword = smtp.Password;
			}

			return email;
		}
		#endregion

		#region internal static string BuldEmailAddress(string firstName, string lastName, string email)
		internal static string BuldEmailAddress(string firstName, string lastName, string email)
		{
			string retVal = null;

			if (!string.IsNullOrEmpty(email))
			{
				string name = null;

				if (!string.IsNullOrEmpty(lastName))
					name = lastName;

				if (!string.IsNullOrEmpty(firstName))
				{
					if (!string.IsNullOrEmpty(name))
						name += " ";
					name += firstName;
				}

				if (!string.IsNullOrEmpty(name))
				{
					retVal = Rfc822HeaderCollection.Encode2AsciiString(name, true) + " ";
				}
				else
					retVal = string.Empty;

				retVal = string.Concat(retVal, "<", email, ">");
			}

			return retVal;
		}
		#endregion
	}
}
