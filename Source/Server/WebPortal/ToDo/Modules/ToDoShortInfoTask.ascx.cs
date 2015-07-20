namespace Mediachase.UI.Web.ToDo.Modules
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
	///		Summary description for ToDoShortInfoTask.
	/// </summary>
	public partial class ToDoShortInfoTask : System.Web.UI.UserControl
	{

    public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoGeneral", typeof(ToDoShortInfoTask).Assembly);

		#region ToDoID
		private int ToDoID
		{
			get
			{
				try
				{
					return int.Parse(Request["ToDoId"]);
				}
				catch
				{
					throw new Exception("Invalid Incident Id!!!");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				BindValues();
			}
			BindToolBar();			
		}

		#region BindToolbar
		private void BindToolBar()
		{
			tbView.AddText(LocRM.GetString("tbView"));
			
			if (ViewState["TaskID"]!=null && !Security.CurrentUser.IsExternal)
				tbView.AddRightLink("<img alt='' src='../Layouts/Images/icon-search.gif'/> " + LocRM.GetString("View"),"../Tasks/TaskView.aspx?TaskID=" + ViewState["TaskID"].ToString());
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			using(IDataReader reader = ToDo.GetToDo(ToDoID))
			{
				if (reader.Read())
				{
					if (reader["TaskID"]!=DBNull.Value)
						ViewState["TaskID"] = (int)reader["TaskID"];
				}
			}

			if( ViewState["TaskID"] != null)
			{
				int TaskID  = (int)ViewState["TaskID"];
				using(IDataReader reader = Task.GetTask(TaskID))
				{
					if (reader.Read())
					{
						///  TaskId, TaskNum, ProjectId, ProjectTitle, ManagerId, CreatorId, CompletedBy, Title, 
						///  Description,	CreationDate, StartDate, FinishDate, 	Duration, 
						///  ActualFinishDate, PriorityId, PriorityName, PercentCompleted, OutlineNumber, 
						///  OutlineLevel, 	IsSummary, IsMilestone, ConstraintTypeId, ConstraintTypeName, 
						///  ConstraintDate, CompletionTypeId, CompletionTypeName, IsCompleted, 
						///  MustBeConfirmed, ReasonId
					
						bool IsExternal = Security.CurrentUser.IsExternal;
						if(!IsExternal)
							lblTitle.Text = "<a href='../Tasks/TaskView.aspx?TaskID="+ ((int)reader["TaskID"]).ToString() + "'>" + reader["Title"].ToString() + "</a>";
						else
							lblTitle.Text = reader["Title"].ToString();

						lblTimeline.Text = String.Format("{0} {1} - {2} {3}", 
							((DateTime)reader["StartDate"]).ToShortDateString(), ((DateTime)reader["StartDate"]).ToShortTimeString(),
							((DateTime)reader["FinishDate"]).ToShortDateString(), ((DateTime)reader["FinishDate"]).ToShortTimeString());

						lblState.ForeColor = Util.CommonHelper.GetStateColor((int)reader["StateId"]);
						lblState.Text = reader["StateName"].ToString();
						if ((int)reader["StateId"] == (int)ObjectStates.Active || (int)reader["StateId"] == (int)ObjectStates.Overdue)
							lblState.Text += String.Format(" ({0} %)", reader["PercentCompleted"].ToString());

						lblPriority.Text = reader["PriorityName"].ToString() + " " + LocRM.GetString("Priority").ToLower();
						lblPriority.ForeColor = Util.CommonHelper.GetPriorityColor((int)reader["PriorityId"]);
						lblPriority.Visible = PortalConfig.CommonTaskAllowViewPriorityField;

						if(reader["Description"] != DBNull.Value)
						{
							string txt = CommonHelper.parsetext(reader["Description"].ToString(), false);
							if (PortalConfig.ShortInfoDescriptionLength > 0 && txt.Length > PortalConfig.ShortInfoDescriptionLength)
								txt = txt.Substring(0, PortalConfig.ShortInfoDescriptionLength) + "...";
							lblDescription.Text = txt;
						}
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
	}
}
