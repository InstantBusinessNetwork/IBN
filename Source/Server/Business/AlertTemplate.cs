using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Threading;
using System.Xml;

using Mediachase.Ibn;
using Mediachase.IBN.Database;
using System.Text;


namespace Mediachase.IBN.Business
{
	public enum AlertTemplateTypes
	{
		Notification,
		Reminder,
		Special
	}

	public class AlertTemplate
	{
		internal const string sAlertTemplates = "Mediachase.IBN.Business.Resources.SystemEventTemplates";
		internal const string sReminderTemplates = "Mediachase.IBN.Business.Resources.ReminderTemplates";
		internal const string sSpecialTemplates = "Mediachase.IBN.Business.Resources.SpecialMessageTemplates";

		private static ResourceManager rmTemplates = new ResourceManager(sAlertTemplates, typeof(AlertTemplate).Assembly);

		#region * Fields *
		public string Key;

		internal int LanguageId;
		internal int EventTypeId;
		internal int MessageTypeId;
		
		public string Subject;
		public string Body;

		public string Locale;
		#endregion

		#region * Constructors *
		public AlertTemplate(string locale, string key, int languageId, int eventTypeId, int messageTypeId)
		{
			if(string.IsNullOrEmpty(key))
				Key = MakeKey(ref locale, languageId, eventTypeId, messageTypeId);
			else
				Key = key;

			Locale = locale;
			LanguageId = languageId;
			EventTypeId = eventTypeId;
			MessageTypeId = messageTypeId;
		}
		#endregion

		#region MakeKey()
		public static string MakeKey(ref string locale, int languageId, int eventTypeId, int messageTypeId)
		{
			if(locale == null || locale.Length == 0)
			{
				using(IDataReader reader = Common.GetListLanguages())
				{
					while(reader.Read())
					{
						if((int)reader["LanguageId"] == languageId)
						{
							locale = reader["Locale"].ToString();
							break;
						}
					}
				}
			}

			return string.Format("{0}|{1}|{2}"
				,locale
				,Enum.Parse(typeof(SystemEventTypes), eventTypeId.ToString())
				,Enum.Parse(typeof(MessageTypes), messageTypeId.ToString())
				);
		}
		#endregion

		#region Load()
		internal void Load()
		{
			if(!GetFromDatabase(Key, out Subject, out Body))
				GetFromResources(Key, out Subject, out Body);
		}
		#endregion

		#region GetFromDatabase()
		/// <summary>
		/// Gets from database.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="body">The body.</param>
		/// <returns></returns>
		private static bool GetFromDatabase(string key, out string subject, out string body)
		{
			bool ret = false;
			subject = string.Empty;
			body = string.Empty;

			using(IDataReader reader = DbAlert2.TemplateGet(key))
			{
				if(reader.Read())
				{
					subject = reader["Subject"].ToString();
					body = reader["Body"].ToString();

					// OZ: 2009-02-02 Process Global Subject Template
					subject = ApplyGlobalSubjectTemplate(subject);

					ret = true;
				}
			}

			return ret;
		}

		/// <summary>
		/// Applies the global subject template.
		/// </summary>
		/// <param name="subject">The subject.</param>
		/// <returns></returns>
		private static string ApplyGlobalSubjectTemplate(string subject)
		{
			string result = subject;

			if (!string.IsNullOrEmpty(subject) && !SkipApplyGlobalSubjectTemplate.IsActive)
			{
				const string prefix = IbnConst.ProductFamilyShort + " " + IbnConst.VersionMajorDotMinor + " - "; // "IBN 4.7 - "
				if (subject.StartsWith(prefix))
					subject = subject.Substring(prefix.Length);

				result = PortalConfig.AlertSubjectFormat.Replace("[=EventSubject=]", subject);
			}

			return result;
		}
		#endregion

		private static string ReplaceGlobalVariables(string value)
		{
			string result = value;

			if (!string.IsNullOrEmpty(value))
			{
				StringBuilder builder = new StringBuilder(value);

				builder.Replace("[=ProductFamily=]", IbnConst.ProductFamily);
				builder.Replace("[=ProductFamilyShort=]", IbnConst.ProductFamilyShort);
				builder.Replace("[=ProductWebSite=]", GlobalResourceManager.Strings["CompanyWebUrl"]);

				result = builder.ToString();
			}

			return result;
		}

		#region GetFromResources()
		/// <summary>
		/// Gets from resources.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="body">The body.</param>
		private static void GetFromResources(string key, out string subject, out string body)
		{
			string[] parts = key.Split('|'); // Locale|EventType|MessageType
			GetFromResources(rmTemplates, parts[0], string.Format("{0}|{1}", parts[1], parts[2]), false, out subject, out body);
		}

		/// <summary>
		/// Gets from resources.
		/// </summary>
		/// <param name="rm">The rm.</param>
		/// <param name="locale">The locale.</param>
		/// <param name="name">The name.</param>
		/// <param name="allowEmpty">if set to <c>true</c> [allow empty].</param>
		/// <param name="subject">The subject.</param>
		/// <param name="body">The body.</param>
		private static void GetFromResources(ResourceManager rm, string locale, string name, bool allowEmpty, out string subject, out string body)
		{
			bool ok = false;
			body = null;
			subject = null;

			// Load from XML
			try
			{
				string fileName = null;
				switch(rm.BaseName)
				{
					case sAlertTemplates:
						fileName = "TemplatesAlert";
						break;
					case sReminderTemplates:
						fileName = "TemplatesReminder";
						break;
					case sSpecialTemplates:
						fileName = "TemplatesSpecial";
						break;
				}

				if(fileName != null)
				{
					fileName = string.Format("{0}{1}_{2}.xml", Configuration.GetToolsDir(), fileName, locale);
					if(File.Exists(fileName))
					{
						XmlDocument doc = new XmlDocument();
						doc.Load(fileName);
						subject = GetFromXml(doc, name + "|Subject");
						body = GetFromXml(doc, name + "|Body");
						ok = (subject != null && body != null);
					}
				}
			}
			catch(Exception ex)
			{
				Log.WriteError(ex.ToString());
			}

			if(!ok)
			{
				CultureInfo culture = new CultureInfo(locale);
				subject = GetFromResources(rm, culture, name + "|Subject", allowEmpty);
				body = GetFromResources(rm, culture, name + "|Body", allowEmpty);
			}

			subject = ReplaceGlobalVariables(subject);
			body = ReplaceGlobalVariables(body);

			// OZ: 2009-02-02 Process Global Subject Template
			subject = ApplyGlobalSubjectTemplate(subject);
			//
		}

		/// <summary>
		/// Gets from resources.
		/// </summary>
		/// <param name="rm">The rm.</param>
		/// <param name="culture">The culture.</param>
		/// <param name="name">The name.</param>
		/// <param name="allowEmpty">if set to <c>true</c> [allow empty].</param>
		/// <returns></returns>
		private static string GetFromResources(ResourceManager rm, CultureInfo culture, string name, bool allowEmpty)
		{
			string ret = rm.GetString(name, culture);
			if(!allowEmpty && string.IsNullOrEmpty(ret))
				ret = string.Format("{0}|{1}", culture.Name, name);
			return ret;
		}
		#endregion

		#region GetFromXml()
		private static string GetFromXml(XmlDocument doc, string name)
		{
			string ret = null;
			
			XmlNode val = doc.SelectSingleNode(string.Format("/root/data[@name='{0}']/value", name));
			if(val != null)
				ret = val.InnerText;

			return ret;
		}
		#endregion

		// For template editor:
		internal static ResourceManager TemplatesRM{ get{ return rmTemplates; } }

		#region GetLanguages()
		public static DataTable GetLanguages()
		{
			DataTable ret = new DataTable();
			ret.Columns.Add("Key", typeof(string));
			ret.Columns.Add("Name", typeof(string));

			using(IDataReader reader = Common.GetListLanguages())
			{
				DataRow row;
				while(reader.Read())
				{
					row = ret.NewRow();
					row["Key"] = reader["Locale"].ToString();
					row["Name"] = reader["FriendlyName"].ToString();
					ret.Rows.Add(row);
				}
			}
			return ret;
		}
		#endregion
		#region UpdateTemplate()
		public static void UpdateTemplate(string locale, string name, string subject, string body)
		{
			// TODO: Check if user is admin.
			DbAlert2.TemplateUpdate(string.Format("{0}|{1}", locale, name), subject, body);
		}
		#endregion
		#region ResetTemplate()
		public static void ResetTemplate(string locale, string name)
		{
			// TODO: Check if user is admin.
			DbAlert2.TemplateDelete(string.Format("{0}|{1}", locale, name));
		}
		#endregion

		#region GetVariablesForTemplateEditor()
		public static DataTable GetVariablesForTemplateEditor(AlertTemplateTypes type, string templateName)
		{
			DataTable table = new DataTable();
			table.Columns.Add(new DataColumn("Name", typeof(string)));
			table.Columns.Add(new DataColumn("Description", typeof(string)));
			table.Columns.Add(new DataColumn("Value", typeof(string)));

			AlertVariable[] vars = null;
			switch(type)
			{
				case AlertTemplateTypes.Notification:
					string[] parts = templateName.Split('|');
					vars = AlertVariable.GetVariables((SystemEventTypes)Enum.Parse(typeof(SystemEventTypes), parts[0]));
					break;
				case AlertTemplateTypes.Reminder:
					vars = Reminder.GetVariables((DateTypes)Enum.Parse(typeof(DateTypes), templateName));
					break;
				case AlertTemplateTypes.Special:
					vars = SpecialMessage.GetVariables((SpecialMessageType)Enum.Parse(typeof(SpecialMessageType), templateName));
					break;
			}

			if(vars != null)
			{
				foreach(AlertVariable var in vars)
				{
					DataRow row = table.NewRow();

					row["Name"] = var.Name;
					row["Description"] = string.Empty;

					string value = "[=" + var.Name + "=]";
					if (var.isLink)
						value += "[=/" + var.Name + "=]";
					row["Value"] = value;

					table.Rows.Add(row);
				}
			}

			return table;
		}
		#endregion
		#region GetNames()
		public static DataTable GetNames(AlertTemplateTypes type)
		{
			DataTable ret = new DataTable();
			ret.Columns.Add("Key", typeof(string));
			ret.Columns.Add("Name", typeof(string));

			ResourceManager rm = null;
			switch(type)
			{
				case AlertTemplateTypes.Notification:
					rm = AlertTemplate.TemplatesRM;
					break;
				case AlertTemplateTypes.Reminder:
					rm = ReminderTemplate.TemplatesRM;
					break;
				case AlertTemplateTypes.Special:
					rm = SpecialMessage.TemplatesRM;
					break;
			}

			if(rm != null)
			{
				bool pm = License.ProjectManagement;
				bool hd = License.HelpDesk;
				bool wf = License.WorkflowModule;

				ResourceSet rs = rm.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
				IDictionaryEnumerator en = rs.GetEnumerator();
				DataRow row;
				while(en.MoveNext()) 
				{
					string key = en.Key.ToString();
					int i = key.LastIndexOf("|");
					string field = key.Substring(i+1);
					if(0 == string.Compare(field, "subject", true))
					{
						string name = key.Substring(0, i);

						if(
							!pm && (name.StartsWith("Project_") || name.StartsWith("Task_"))
							|| 
							!hd && (name.StartsWith("Issue_") || name.StartsWith("IssueRequest_") || name.StartsWith("Project_Updated_IssueList"))
							||
							!wf && name.StartsWith("Assignment_")
							)
						{
							// skip
						}
						else
						{
							row = ret.NewRow();
							row["Key"] = name;
							row["Name"] = GetDisplayName(type, name);
							ret.Rows.Add(row);
						}
					}
				}
			}
			return ret;
		}
		#endregion
		#region GetDisplayName()
		public static string GetDisplayName(AlertTemplateTypes type, string key)
		{
			string ret = null;
			switch(type)
			{
				case AlertTemplateTypes.Notification:
					string[] parts = key.Split('|'); // EventType|MessageType
					ret = string.Format("{0} [{1}]", SystemEvents.GetSystemEventName(parts[0]), SystemEvents.GetMessageTypeName(parts[1]));
					break;
				case AlertTemplateTypes.Reminder:
					ret = ReminderTemplate.TypesRM.GetString(key);
					break;
				case AlertTemplateTypes.Special:
					ret = SpecialMessage.TypesRM.GetString(key);
					break;
			}
			return ret;
		}
		#endregion
		#region GetTemplate()
		public static bool GetTemplate(AlertTemplateTypes type, string locale, string name, bool getFromDb, out string subject, out string body)
		{
			return GetTemplate(type, locale, name, getFromDb, false, out subject, out body);
		}

		public static bool GetTemplate(AlertTemplateTypes type, string locale, string name, bool getFromDb, bool allowEmpty, out string subject, out string body)
		{
			bool ret = false;
			subject = body = null;
			if(getFromDb)
				ret = GetFromDatabase(string.Format("{0}|{1}", locale, name), out subject, out body);
			if(!ret)
			{
				ResourceManager rm = null;
				switch(type)
				{
					case AlertTemplateTypes.Notification:
						rm = AlertTemplate.TemplatesRM;
						break;
					case AlertTemplateTypes.Reminder:
						rm = ReminderTemplate.TemplatesRM;
						break;
					case AlertTemplateTypes.Special:
						rm = SpecialMessage.TemplatesRM;
						break;
				}
				GetFromResources(rm, locale, name, allowEmpty, out subject, out body);
			}
			return ret;
		}
		#endregion
	}
}
