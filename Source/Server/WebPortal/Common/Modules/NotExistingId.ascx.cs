namespace Mediachase.UI.Web.Common.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for NotExistingId.
	/// </summary>
	public partial class NotExistingId : System.Web.UI.UserControl
	{
		public ResourceManager LocRM;

		#region UserId
		private int UserId
		{
			get
			{
				try
				{
					return Request["UserId"] != null ? int.Parse(Request["UserId"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region SGroupId
		private int SGroupId
		{
			get
			{
				try
				{
					return Request["SGroupId"] != null ? int.Parse(Request["SGroupId"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region ProjID
		private int ProjID
		{
			get
			{
				try
				{
					return Request["ProjectID"] != null ? int.Parse(Request["ProjectID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region TaskID
		private int TaskID
		{
			get
			{
				try
				{
					return Request["TaskID"] != null ? int.Parse(Request["TaskID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region ToDoID
		private int ToDoID
		{
			get
			{
				try
				{
					return Request["ToDoID"] != null ? int.Parse(Request["ToDoID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region EventID
		private int EventID
		{
			get
			{
				try
				{
					return Request["EventID"] != null ? int.Parse(Request["EventID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region IncidentID
		private int IncidentID
		{
			get
			{
				try
				{
					return Request["IncidentID"] != null ? int.Parse(Request["IncidentID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region DocumentID
		private int DocumentID
		{
			get
			{
				try
				{
					return Request["DocumentID"] != null ? int.Parse(Request["DocumentID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region FolderID
		private int FolderID
		{
			get
			{
				try
				{
					return Request["FolderID"] != null ? int.Parse(Request["FolderID"]) : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region MaxDiskSpace
		private int MaxDiskSpace
		{
			get
			{
				try
				{
					return Request["MaxDiskSpace"] != null ? 1 : -1;
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region sType
		private string sType
		{
			get
			{
				if (UserId >= 0) return "User";
				if (ProjID >= 0) return "Project";
				if (TaskID >= 0) return "Task";
				if (ToDoID >= 0) return "ToDo";
				if (EventID >= 0) return "Event";
				if (IncidentID >= 0) return "Incident";
				if (FolderID >= 0) return "Folder";
				if (DocumentID >= 0) return "Document";
				if (MaxDiskSpace >= 0) return "MaxDiskSpace";
				if (Request["ClassName"] != null)
				{
					_className = Request["ClassName"];
					return "MetaClassObject";
				}
				if (!String.IsNullOrEmpty(Request["AssignmentId"]))
					return "Assignment";
				return "";
			}
		}
		#endregion

		private string _className = String.Empty;
		#region sURL
		private string sURL
		{
			get
			{
				switch (sType)
				{
					case "User":
						return "../Directory/Directory.aspx";
					case "Project":
						return this.Page.ResolveClientUrl("~/Apps/ProjectManagement/Pages/ProjectList.aspx");
					case "Task":
						return "../Projects/WorksForResource.aspx";
					case "ToDo":
						return "../Projects/WorksForResource.aspx";
					case "Event":
						return "../Projects/WorksForResource.aspx";
					case "Incident":
						return this.Page.ResolveClientUrl("~/Apps/HelpDeskManagement/Pages/IncidentListNew.aspx");
					case "Document":
						return "../Documents/default.aspx";
					case "MetaClassObject":
						if (_className.ToLower() == Mediachase.Ibn.Events.CalendarEventEntity.ClassName.ToLower())
							return this.Page.ResolveClientUrl("~/Apps/Calendar/Pages/Calendar.aspx");
						return this.Page.ResolveClientUrl("~/Apps/MetaUIEntity/Pages/EntityList.aspx?ClassName=" + _className);
					case "MaxDiskSpace":
					default:
						return "../Workspace/default.aspx";
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strNotExistingId", typeof(NotExistingId).Assembly);
			if (!Page.IsPostBack)
			{
				btnSMTP.Visible = false;
				if (Request["SMTP"] != null)
				{
					lblMessage.Text = LocRM.GetString("SMTPSettingsIncorrect") + "<br/>" +
					  LocRM.GetString("tWereErrorsSMTP");
					if (Request["SMTP"] == "Socket")
						lblMessage.Text += "<br/><br/><font style='color:#444'><i>A SocketException exception is thrown by the Socket and Dns classes when an error occurs with the network.</i></font>";
					else if (Request["SMTP"] == "Client" && Request["mess"] != null)
						lblMessage.Text += String.Format("<br/><br/>{0}: <font style='color:#444'><i>&quot;{1}&quot;</i></font>", LocRM.GetString("tServerSMTPError"), HttpUtility.UrlDecode(Request["mess"]));
					if (!Security.IsUserInGroup(InternalSecureGroups.Administrator))
						lblMessage.Text += "<br/><br/>" + LocRM.GetString("tContactSMTPError");
					else
					{
						btnSMTP.Visible = true;
						btnSMTP.Text = LocRM.GetString("tSetupSMTPServer");
					}
				}
				else if (Request["AD"] == null)
				{
					if (sType == "Assignment")
						lblMessage.Text = LocRM.GetString("Unfortunately") + LocRM.GetString(sType) + LocRM.GetString("NotExist");
					else if (sType != "MetaClassObject")
						lblMessage.Text = LocRM.GetString("Unfortunately") + LocRM.GetString(sType) + LocRM.GetString("Report") + "<a href=\"" + sURL + "\">" + LocRM.GetString(sType + "List") + "</a>.";
					else
						lblMessage.Text = LocRM.GetString("Unfortunately") + CommonHelper.GetResFileString(Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(_className).FriendlyName).ToLower() + LocRM.GetString("Report") + "<a href=\"" + sURL + "\">" + LocRM.GetString("MetaObjectList") + "</a>.";
				}
				else if (sType == "MaxDiskSpace")
					lblMessage.Text = LocRM.GetString("MaxDiskSpace");
				else
					lblMessage.Text = LocRM.GetString("Text1");
			}
		}

		protected void btnSMTP_Click(object sender, EventArgs e)
		{
			Response.Redirect("~/Admin/SMTPSettings.aspx");
		}

	}
}
