namespace Mediachase.UI.Web.ToDo.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.UI.Web.Util;
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.UI;
	using System.Globalization;

	/// <summary>
	///		Summary description for ToDoInfo.
	/// </summary>
	public partial class ToDoInfo : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoGeneral", typeof(ToDoInfo).Assembly);

		#region ToDoID
		private int ToDoID
		{
			get
			{
				try
				{
					return int.Parse(Request["ToDoID"]);
				}
				catch
				{
					throw new Exception("Invalid ToDo ID");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			using (IDataReader rdr = ToDo.GetToDo(ToDoID))
			{
				///  ToDoId, ProjectId, ProjectTitle, IncidentId, IncidentTitle, StatusId,
				///  DocumentId, DocumentTitle, CompleteDocument, CreatorId, ManagerId, CompletedBy,
				///  Title, Description, CreationDate, StartDate, FinishDate, 
				///  ActualFinishDate, PriorityId, PriorityName, PercentCompleted, IsActual, 
				///  CompletionTypeId, IsCompleted, CompletionTypeName, MustBeConfirmed,
				///  ReasonId, TaskId, CompleteTask, TaskTitle, ProjectCode
				if (rdr.Read())
				{
					lblTitle.Text = "";
					string timeline = "";
					if (rdr["StartDate"] != DBNull.Value)
						timeline += ((DateTime)rdr["StartDate"]).ToShortDateString() + " " + ((DateTime)rdr["StartDate"]).ToShortTimeString();
					else
						timeline += LocRM.GetString("NotSet");
					timeline += " - ";
					if (rdr["FinishDate"] != DBNull.Value)
						timeline += ((DateTime)rdr["FinishDate"]).ToShortDateString() + " " + ((DateTime)rdr["FinishDate"]).ToShortTimeString();
					else
						timeline += LocRM.GetString("NotSet");
					lblTimeline.Text = timeline;

					if (Configuration.ProjectManagementEnabled && rdr["ProjectId"] != DBNull.Value)
					{
						string projectPostfix = CHelper.GetProjectNumPostfix((int)rdr["ProjectId"], (string)rdr["ProjectCode"]);
						if (Project.CanRead((int)rdr["ProjectId"]) && !Security.CurrentUser.IsExternal)
							lblTitle.Text = String.Format(CultureInfo.InvariantCulture,
								"<a href='../Projects/ProjectView.aspx?ProjectId={0}' title='{1}'>{2}{3}</a> \\ ", 
								rdr["ProjectId"].ToString(), 
								LocRM.GetString("Project"),
								rdr["ProjectTitle"].ToString(),
								projectPostfix);
						else
							lblTitle.Text = String.Format(CultureInfo.InvariantCulture,
								"<span title='{0}'>{1}{2}<span> \\ ",
								LocRM.GetString("Project"), 
								rdr["ProjectTitle"].ToString(),
								projectPostfix);
					}

					if (rdr["IncidentId"] != DBNull.Value)
					{
						if (Incident.CanRead((int)rdr["IncidentId"]) && !Security.CurrentUser.IsExternal)
							lblTitle.Text += String.Format("<a href='../Incidents/IncidentView.aspx?IncidentId={0}' title='{2}'>{1} (#{0})</a> \\ ", rdr["IncidentId"].ToString(), rdr["IncidentTitle"].ToString(), LocRM.GetString("Issue"));
						else
							lblTitle.Text += String.Format("<span title='{1}'>{0} (#{2})<span> \\ ", rdr["IncidentTitle"].ToString(), LocRM.GetString("Issue"), rdr["IncidentId"].ToString());
					}
					else if (rdr["TaskId"] != DBNull.Value)
					{
						if (Task.CanRead((int)rdr["TaskId"]) && !Security.CurrentUser.IsExternal)
							lblTitle.Text += String.Format("<a href='../Tasks/TaskView.aspx?TaskId={0}' title='{2}'>{1} (#{0})</a> \\ ", rdr["TaskId"].ToString(), rdr["TaskTitle"].ToString(), LocRM.GetString("Task"));
						else
							lblTitle.Text += String.Format("<span title='{1}'>{0} (#{2})</span> \\ ", rdr["TaskTitle"].ToString(), LocRM.GetString("Task"), rdr["TaskId"].ToString());
					}
					else if (rdr["DocumentId"] != DBNull.Value)
					{
						if (Document.CanRead((int)rdr["DocumentId"]) && !Security.CurrentUser.IsExternal)
							lblTitle.Text += String.Format("<a href='../Documents/DocumentView.aspx?DocumentId={0}' title='{2}'>{1}</a> \\ ", rdr["DocumentId"].ToString(), rdr["DocumentTitle"].ToString(), LocRM.GetString("Document"));
						else
							lblTitle.Text += String.Format("<span title='{1}'>{0}</span> \\ ", rdr["DocumentTitle"].ToString(), LocRM.GetString("Document"));
					}
					lblTitle.Text += String.Format("{0} (#{1})", rdr["Title"].ToString(), ToDoID);

					lblState.ForeColor = Util.CommonHelper.GetStateColor((int)rdr["StateId"]);
					lblState.Text = rdr["StateName"].ToString();
					if ((int)rdr["StateId"] == (int)ObjectStates.Active || (int)rdr["StateId"] == (int)ObjectStates.Overdue)
						lblState.Text += String.Format(" ({0} %)", rdr["PercentCompleted"].ToString());

					lblPriority.Text = rdr["PriorityName"].ToString() + " " + LocRM.GetString("Priority").ToLower();
					lblPriority.ForeColor = Util.CommonHelper.GetPriorityColor((int)rdr["PriorityId"]);
					lblPriority.Visible = PortalConfig.CommonToDoAllowViewPriorityField;

					if (rdr["Description"] != DBNull.Value)
					{
						string txt = CommonHelper.parsetext(rdr["Description"].ToString(), false);
						if (PortalConfig.ShortInfoDescriptionLength > 0 && txt.Length > PortalConfig.ShortInfoDescriptionLength)
							txt = txt.Substring(0, PortalConfig.ShortInfoDescriptionLength) + "...";
						lblDescription.Text = txt;
					}
				}
			}
		}
		#endregion

		#region GetCompletionType
		private string GetCompletionType(int type)
		{
			CompletionReason rsn = (CompletionReason)type;
			switch (rsn)
			{
				case CompletionReason.SuspendedManually:
				case CompletionReason.SuspendedAutomatically:
					return LocRM.GetString("Suspended");
				case CompletionReason.CompletedManually:
					return LocRM.GetString("CompletedByManager");
				case CompletionReason.CompletedAutomatically:
					return LocRM.GetString("CompletedByResource");
				case CompletionReason.NotCompleted:
					return LocRM.GetString("NotCompleted");
			}
			return "";
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			BindValues();
		}
		#endregion
	}
}
