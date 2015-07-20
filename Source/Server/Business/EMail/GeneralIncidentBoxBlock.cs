using System;
using System.Data;
using System.Collections;
using System.Text;
using Mediachase.IBN.Database;

namespace Mediachase.IBN.Business.EMail
{
	public enum ControllerAssignType
	{
		CustomUser = 0,
		Creator    = 1,
		Manager    = 2,
	}

	public enum ResponsibleAssignType
	{
		CustomUser = 0,
		Pool = 1,
		Manual = 2,
	}

	/// <summary>
	/// Summary description for GeneralIncidentBoxBlock.
	/// </summary>
	public class GeneralIncidentBoxBlock: BaseIncidentBoxBlock
	{
		public GeneralIncidentBoxBlock()
		{
		}

		/// <summary>
		/// Gets or sets the manager.
		/// </summary>
		/// <value>The manager.</value>
		public int Manager
		{
			set 
			{
				base.Params["Manager"] = value;
			}
			get 
			{
				if(base.Params.Contains("Manager"))
					return (int)base.Params["Manager"];

				return -1;
			}
		}


		#region Control

		/// <summary>
		/// Gets or sets a value indicating whether [allow control].
		/// </summary>
		/// <value><c>true</c> if [allow control]; otherwise, <c>false</c>.</value>
		public bool AllowControl
		{
			set 
			{
				base.Params["AllowController"] = value;
			}
			get 
			{
				if(base.Params.Contains("AllowController"))
					return (bool)base.Params["AllowController"];

				return false;
			}
		}

		/// <summary>
		/// Gets or sets the type of the controller assign.
		/// </summary>
		/// <value>The type of the controller assign.</value>
		public ControllerAssignType ControllerAssignType
		{
			set 
			{
				base.Params["ControllerAssignType"] = value;
			}
			get 
			{
				if(base.Params.Contains("ControllerAssignType"))
					return (ControllerAssignType)base.Params["ControllerAssignType"];

				return ControllerAssignType.CustomUser;
			}
		}

		/// <summary>
		/// Gets or sets the controller.
		/// </summary>
		/// <value>The controller.</value>
		public int Controller
		{
			set 
			{
				base.Params["Controller"] = value;
			}
			get 
			{
				if(base.Params.Contains("Controller"))
					return (int)base.Params["Controller"];

				return -1;
			}
		}
		#endregion

		#region Responsible
		/// <summary>
		/// Gets or sets the type of the responsible assign.
		/// </summary>
		/// <value>The type of the responsible assign.</value>
		public ResponsibleAssignType ResponsibleAssignType
		{
			set 
			{
				base.Params["ResponsibleAssignType"] = value;
			}
			get 
			{
				if(base.Params.Contains("ResponsibleAssignType"))
					return (ResponsibleAssignType)base.Params["ResponsibleAssignType"];

				return ResponsibleAssignType.CustomUser;
			}
		}

		/// <summary>
		/// Gets the responsible pool.
		/// </summary>
		/// <value>The responsible pool.</value>
		public ArrayList ResponsiblePool
		{
			get 
			{
				if(!base.Params.Contains("ResponsiblePool"))
					base.Params["ResponsiblePool"] = new ArrayList();

				return (ArrayList)base.Params["ResponsiblePool"];
			}
			set 
			{
				base.Params["ResponsiblePool"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the responsible.
		/// </summary>
		/// <value>The responsible.</value>
		public int Responsible
		{
			set 
			{
				base.Params["Responsible"] = value;
			}
			get 
			{
				if(base.Params.Contains("Responsible"))
					return (int)base.Params["Responsible"];

				return -1;
			}
		}

		public bool AllowUserToComeResponsible
		{
			set 
			{
				base.Params["AllowUserToComeResponsible"] = value;
			}
			get 
			{
				if(base.Params.Contains("AllowUserToComeResponsible"))
					return (bool)base.Params["AllowUserToComeResponsible"];

				return false;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [allow to reassign responsibility].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [allow to reassign responsibility]; otherwise, <c>false</c>.
		/// </value>
		public bool AllowToReassignResponsibility
		{
			set 
			{
				base.Params["AllowToReassignResponsibility"] = value;
			}
			get 
			{
				if(base.Params.Contains("AllowToReassignResponsibility"))
					return (bool)base.Params["AllowToReassignResponsibility"];

				return true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [allow to decline responsibility].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [allow to decline responsibility]; otherwise, <c>false</c>.
		/// </value>
		public bool AllowToDeclineResponsibility
		{
			set 
			{
				base.Params["AllowToDeclineResponsibility"] = value;
			}
			get 
			{
				if(base.Params.Contains("AllowToDeclineResponsibility"))
					return (bool)base.Params["AllowToDeclineResponsibility"];

				return true;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [reassign responsibile on re open].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [reassign responsibile on re open]; otherwise, <c>false</c>.
		/// </value>
		public bool ReassignResponsibileOnReOpen
		{
			set 
			{
				base.Params["ReassignResponsibileOnReOpen"] = value;
			}
			get 
			{
				if(base.Params.Contains("ReassignResponsibileOnReOpen"))
					return (bool)base.Params["ReassignResponsibileOnReOpen"];

				return false;
			}
		}
		#endregion

		#region Child Objects
		public bool AllowAddToDo
		{
			set 
			{
				base.Params["AllowAddToDo"] = value;
			}
			get 
			{
				if(base.Params.Contains("AllowAddToDo"))
					return (bool)base.Params["AllowAddToDo"];

				return true;
			}
		}

		public bool AllowAddResources
		{
			set 
			{
				base.Params["AllowAddResources"] = value;
			}
			get 
			{
				if(base.Params.Contains("AllowAddResources"))
					return (bool)base.Params["AllowAddResources"];

				return false;
			}
		}
		#endregion

		#region Time Parameters
		/// <summary>
		/// Gets or sets the expected duration.
		/// </summary>
		/// <value>The expected duration.</value>
		public int CalendarId
		{
			set 
			{
				base.Params["CalendarId"] = value;
			}
			get 
			{
				if(base.Params.Contains("CalendarId"))
					return (int)base.Params["CalendarId"];

				using(IDataReader reader = DBCalendar.GetListCalendars())
				{
					if(reader.Read())
					{
						return (int)reader["CalendarId"];
					}
				}

				return -1;
			}
		}

		/// <summary>
		/// Gets or sets the expected duration.
		/// </summary>
		/// <value>The expected duration.</value>
		public int ExpectedDuration
		{
			set 
			{
				base.Params["ExpectedDuration"] = value;
			}
			get 
			{
				if(base.Params.Contains("ExpectedDuration"))
					return (int)base.Params["ExpectedDuration"];

				return 60*24*7;
			}
		}

		/// <summary>
		/// Gets or sets the expected response time.
		/// </summary>
		/// <value>The expected response time.</value>
		public int ExpectedResponseTime
		{
			set 
			{
				base.Params["ExpectedResponseTime"] = value;
			}
			get 
			{
				if(base.Params.Contains("ExpectedResponseTime"))
					return (int)base.Params["ExpectedResponseTime"];

				return 60*24*1;
			}
		}

		/// <summary>
		/// Gets or sets the expected assign time.
		/// </summary>
		/// <value>The expected assign time.</value>
		public int ExpectedAssignTime
		{
			set
			{
				base.Params["ExpectedAssignTime"] = value;
			}
			get
			{
				if (base.Params.Contains("ExpectedAssignTime"))
					return (int)base.Params["ExpectedAssignTime"];

				return 60 * 8 * 1;
			}
		}

		/// <summary>
		/// Gets or sets the task time.
		/// </summary>
		/// <value>The task time.</value>
		public int TaskTime
		{
			set
			{
				base.Params["TaskTime"] = value;
			}
			get
			{
				if (base.Params.Contains("TaskTime"))
					return (int)base.Params["TaskTime"];

				return 60 * 3;
			}
		}
		#endregion

		#region Autoreply
		/// <summary>
		/// Resets the auto reply settings.
		/// </summary>
		public void ResetAutoReplySettings()
		{
			base.Params.Remove("AutoReplyEMailSubjectTemplate");
			base.Params.Remove("AutoReplyEMailBodyTemplate");
		}

		/// <summary>
		/// Gets or sets the auto reply E mail subject template.
		/// </summary>
		/// <value>The auto reply E mail subject template.</value>
		public string AutoReplyEMailSubjectTemplate
		{
			set 
			{
				base.Params["AutoReplyEMailSubjectTemplate"] = value;
			}
			get 
			{
				if(base.Params.Contains("AutoReplyEMailSubjectTemplate"))
					return (string)base.Params["AutoReplyEMailSubjectTemplate"];

				string subject, body;

				AlertTemplate.GetTemplate(AlertTemplateTypes.Notification, 
					System.Globalization.CultureInfo.CurrentCulture.ToString(), 
					"Issue_Created|Resource", 
					true, 
					out subject, out body);
				return subject;

			}
		}

		public bool IsAutoReplyEMailCustom
		{
			get
			{
				return base.Params.Contains("AutoReplyEMailSubjectTemplate");
			}
		}

		/// <summary>
		/// Gets or sets the auto reply E mail body template.
		/// </summary>
		/// <value>The auto reply E mail body template.</value>
		public string AutoReplyEMailBodyTemplate
		{
			set 
			{
				base.Params["AutoReplyEMailBodyTemplate"] = value;
			}
			get 
			{
				if(base.Params.Contains("AutoReplyEMailBodyTemplate"))
					return (string)base.Params["AutoReplyEMailBodyTemplate"];

				string subject, body;

				AlertTemplate.GetTemplate(AlertTemplateTypes.Notification, 
					System.Globalization.CultureInfo.CurrentCulture.ToString(), 
					"Issue_Created|Resource", 
					true, 
					out subject, out body);
				return body;
			}
		}
		
		#endregion

		#region CloseAutoreply
		/// <summary>
		/// Resets the auto reply settings.
		/// </summary>
		public void ResetOnCloseAutoReplySettings()
		{
			base.Params.Remove("OnCloseAutoReplyEMailSubjectTemplate");
			base.Params.Remove("OnCloseAutoReplyEMailBodyTemplate");
		}

		/// <summary>
		/// Gets or sets the auto reply E mail subject template.
		/// </summary>
		/// <value>The auto reply E mail subject template.</value>
		public string OnCloseAutoReplyEMailSubjectTemplate
		{
			set
			{
				base.Params["OnCloseAutoReplyEMailSubjectTemplate"] = value;
			}
			get
			{
				if (base.Params.Contains("OnCloseAutoReplyEMailSubjectTemplate"))
					return (string)base.Params["OnCloseAutoReplyEMailSubjectTemplate"];

				string subject, body;

				AlertTemplate.GetTemplate(AlertTemplateTypes.Notification,
					System.Globalization.CultureInfo.CurrentCulture.ToString(),
					"Issue_Closed|Resource",
					true,
					out subject, out body);
				return subject;

			}
		}

		public bool OnCloseIsAutoReplyEMailCustom
		{
			get
			{
				return base.Params.Contains("OnCloseAutoReplyEMailSubjectTemplate");
			}
		}

		/// <summary>
		/// Gets or sets the auto reply E mail body template.
		/// </summary>
		/// <value>The auto reply E mail body template.</value>
		public string OnCloseAutoReplyEMailBodyTemplate
		{
			set
			{
				base.Params["OnCloseAutoReplyEMailBodyTemplate"] = value;
			}
			get
			{
				if (base.Params.Contains("OnCloseAutoReplyEMailBodyTemplate"))
					return (string)base.Params["OnCloseAutoReplyEMailBodyTemplate"];

				string subject, body;

				AlertTemplate.GetTemplate(AlertTemplateTypes.Notification,
					System.Globalization.CultureInfo.CurrentCulture.ToString(),
					"Issue_Closed|Resource",
					true,
					out subject, out body);
				return body;
			}
		}

		#endregion

		#region OutgoingEmailFormat
		public bool AllowOutgoingEmailFormat
		{
			set 
			{
				base.Params["AllowOutgoingEmailFormat"] = value;
			}
			get 
			{
				if(base.Params.Contains("AllowOutgoingEmailFormat"))
					return (bool)base.Params["AllowOutgoingEmailFormat"];

				return true;
			}
		}

		public string OutgoingEmailFormatSubject
		{
			set 
			{
				base.Params["OutgoingEmailFormatSubject"] = value;
			}
			get 
			{
				if(base.Params.Contains("OutgoingEmailFormatSubject"))
					return (string)base.Params["OutgoingEmailFormatSubject"];

				string subject, body;

				AlertTemplate.GetTemplate(AlertTemplateTypes.Special, 
					System.Globalization.CultureInfo.CurrentCulture.ToString(), 
					SpecialMessageType.Issue_ResponseSignature.ToString(), 
					true, 
					out subject, out body);

				return subject;
			}
		}

		public string OutgoingEmailFormatBody
		{
			set 
			{
				base.Params["OutgoingEmailFormatBody"] = value;
			}
			get 
			{
				if(base.Params.Contains("OutgoingEmailFormatBody"))
					return (string)base.Params["OutgoingEmailFormatBody"];

				string subject, body;

				AlertTemplate.GetTemplate(AlertTemplateTypes.Special, 
					System.Globalization.CultureInfo.CurrentCulture.ToString(), 
					SpecialMessageType.Issue_ResponseSignature.ToString(), 
					true, 
					out subject, out body);

				return body;
			}
		}

		public bool IsCustomOutgoingEmailFormat
		{
			get
			{
				return base.Params.Contains("OutgoingEmailFormatSubject");
			}
		}

		/// <summary>
		/// Resets the auto signature.
		/// </summary>
		public void ResetOutgoingEmailFormat()
		{
			base.Params.Remove("OutgoingEmailFormatSubject");
			base.Params.Remove("OutgoingEmailFormatBody");
		}
		#endregion
	}
}
