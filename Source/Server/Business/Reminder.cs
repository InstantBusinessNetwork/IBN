using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for Reminder.
	/// </summary>
	internal class Reminder
	{
		private Reminder()
		{
		}

		#region GetMessageTemplate()
		/// <summary>
		/// Gets the message template.
		/// </summary>
		/// <param name="dateType">Type of the date.</param>
		/// <param name="objecTypeId">The objec type id.</param>
		/// <param name="languageId">The language id.</param>
		/// <returns></returns>
		public static ReminderTemplate GetMessageTemplate(DateTypes dateType, int languageId)
		{
			ReminderTemplate retVal = null;

			retVal = new ReminderTemplate(dateType, languageId);
			retVal.Load();

			return retVal;
		}
		#endregion

		#region GetMessage
		public static Alerts2.Message GetMessage(ReminderTemplate template, int? objectId, Guid? objectUid, ObjectTypes objectType, int userId, VariableInfo[] vars)
		{
			StringBuilder sbSubject = new StringBuilder();
			StringBuilder sbBody = new StringBuilder();

			IFormatProvider prov = new CultureInfo(template.Locale, true);

			sbSubject.Append(template.Subject);
			sbBody.Append(template.Body);

			bool IsExternalUser = User.IsExternal(userId);

			string gateGuid = "";

			if (IsExternalUser && objectId.HasValue)
			{
				if (userId > 0)
					gateGuid = DBCommon.GetGateGuid((int)objectType, objectId.Value, userId);
				//				else
				//					gateGuid = DBCommon.GetGateGuid(eInfo.ObjectTypeId, eInfo.ObjectId, User);
			}

			// Replace variables with values
			foreach (VariableInfo var in vars)
			{
				string nameBegin = string.Format("[={0}=]", var.name);
				string nameEnd = string.Format("[=/{0}=]", var.name);

				string linkStartValue = string.Empty;
				string linkEndValue = string.Empty;

				if (var.IsLink) // Modify link
				{
					if (IsExternalUser && !var.External)
						var.value = string.Empty;

					if (var.value.Length == 0)
						linkStartValue = linkEndValue = string.Empty;
					else
					{
						if (IsExternalUser)
						{
							if (userId > 0)
								var.value = Alerts2.MakeExternalLink(var.value, User.GetUserLogin(userId), gateGuid);
							//							else
							//								var.value = Alert2.MakeClientLink(var.value, gateGuid);
						}

						// Add NoMenu parameter to the end of link
						if (!var.DisableNoMenu && !User.GetMenuInAlerts(userId) && var.name != "PortalLink" && var.name != "ServerLink")
						{
							if (var.value.IndexOf('?') != -1)
								var.value += '&';
							else
								var.value += '?';
							var.value += "nomenu=1";
						}

						linkStartValue = string.Format("<a href='{0}'>", var.value);
						linkEndValue = "</a>";
					}
				}

				if (var.type == VariableType.Date.ToString())
					var.value = Alerts2.DateReformat(var.value, prov);
				else if (var.type == VariableType.DateTime.ToString())
					var.value = Alerts2.DateTimeReformat(var.value, prov, User.GetUserTimeZoneId(userId));

				sbSubject.Replace(nameBegin, var.value);

				if (var.IsLink)
				{
					sbBody.Replace(nameBegin, linkStartValue);
					sbBody.Replace(nameEnd, linkEndValue);
				}
				else
				{
					var.value = HttpUtility.HtmlEncode(var.value).Replace("\r\n", "<BR>");
					sbBody.Replace(nameBegin, var.value);
				}
			}

			return new Alerts2.Message(sbSubject.ToString(), sbBody.ToString());
		}
		#endregion

		#region GetAddress
		public static string GetAddress(DeliveryType type, int userdId)
		{
			using(IDataReader reader = DBUser.GetUserInfo(userdId))
			{
				if(reader.Read())
				{
					switch(type)
					{
						case DeliveryType.Email:
							return Alerts2.BuldEmailAddress(reader["FirstName"].ToString(), reader["LastName"].ToString(), reader["Email"].ToString());
						case DeliveryType.IBN:
							return reader["OriginalId"].ToString();
					}
				}
			}

			return null;
		}
		#endregion

		#region -- GetVariables --
		internal static AlertVariable[] GetVariables(DateTypes dateType)
		{
			// TODO: назначить событиям переменные.
			ArrayList vars = new ArrayList();

			vars.Add(new AlertVariable(Variable.End));
			vars.Add(new AlertVariable(Variable.Start));
			vars.Add(new AlertVariable(Variable.Title));

			bool linkForExternalUser = false;
			switch (dateType)
			{
				case DateTypes.CalendarEntry_FinishDate:
				case DateTypes.CalendarEntry_StartDate:
				case DateTypes.Task_FinishDate:
				case DateTypes.Task_StartDate:
				case DateTypes.Todo_FinishDate:
				case DateTypes.Todo_StartDate:
					linkForExternalUser = true;
					break;
			}
			vars.Add(new AlertVariable(Variable.Link, true, linkForExternalUser, false)); 

			return vars.ToArray(typeof(AlertVariable)) as AlertVariable[];
		}
		#endregion
	}
}
