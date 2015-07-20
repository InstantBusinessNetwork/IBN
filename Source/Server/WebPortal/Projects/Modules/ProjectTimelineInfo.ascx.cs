namespace Mediachase.UI.Web.Projects.Modules
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
	using ComponentArt.Web.UI;
	using System.Reflection;

	/// <summary>
	///		Summary description for ProjectTimelineInfo.
	/// </summary>
	public partial class ProjectTimelineInfo : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ProjectTimelineInfo).Assembly);
    protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(ProjectTimelineInfo).Assembly);
	protected ResourceManager LocRM3 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMessageTemplates", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		private DateTime TargetStartDate = DateTime.UtcNow;
		private DateTime TargetFinishDate = DateTime.UtcNow;
		private bool IsMember = false;

		#region ProjectId
		protected int ProjectId
		{
			get 
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.IsPostBack)
			{
				BindValues();
				BindReminders();
			}

			BindToolbar();
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

		#region BindReminders
		private void BindReminders()
		{
			Project.ProjectSecurity ps = Project.GetSecurity(ProjectId);
			IsMember = ps.IsManager || ps.IsManager || ps.IsTeamMember || ps.IsSponsor || ps.IsStakeHolder;

			if (IsMember && TargetStartDate > DateTime.UtcNow)
			{
				int Lag = -1;
				bool IsActive = false;

				using (IDataReader reader = Schedule.GetReminderSubscriptionPersonalForObject(DateTypes.Project_StartDate, ProjectId))
				{
					if (reader.Read())
					{
						Lag = (int)reader["Lag"];
						IsActive = (bool)reader["IsActive"];
					}
				}	

				if (IsActive)
				{
					imgReminderStart.ToolTip = String.Format("{0}: {1}", LocRM3.GetString("Start"), CommonHelper.GetIntervalString(Lag));
				}
				else
				{
					imgReminderStart.ToolTip = CommonHelper.GetIntervalString(-1);
					if (imgReminderStart.ToolTip.StartsWith("["))
						imgReminderStart.ToolTip = imgReminderStart.ToolTip.Substring(1);
					if (imgReminderStart.ToolTip.EndsWith("]"))
						imgReminderStart.ToolTip = imgReminderStart.ToolTip.Substring(0, imgReminderStart.ToolTip.Length - 1);
					imgReminderStart.ImageUrl = "~/Layouts/Images/reminder2.gif";
				}
			}
			else
			{
				imgReminderStart.Visible = false;
			}


			if (IsMember && TargetFinishDate > DateTime.UtcNow)
			{
				int Lag = -1;
				bool IsActive = false;

				using (IDataReader reader = Schedule.GetReminderSubscriptionPersonalForObject(DateTypes.Project_FinishDate, ProjectId))
				{
					if (reader.Read())
					{
						Lag = (int)reader["Lag"];
						IsActive = (bool)reader["IsActive"];
					}
				}	

				if (IsActive)
				{
					imgReminderFinish.ToolTip = String.Format("{0}: {1}", LocRM3.GetString("Finish"), CommonHelper.GetIntervalString(Lag));
				}
				else
				{
					imgReminderFinish.ToolTip = CommonHelper.GetIntervalString(-1);
					if (imgReminderFinish.ToolTip.StartsWith("["))
						imgReminderFinish.ToolTip = imgReminderFinish.ToolTip.Substring(1);
					if (imgReminderFinish.ToolTip.EndsWith("]"))
						imgReminderFinish.ToolTip = imgReminderFinish.ToolTip.Substring(0, imgReminderFinish.ToolTip.Length - 1);
					imgReminderFinish.ImageUrl = "~/Layouts/Images/reminder2.gif";
				}
			}
			else
			{
				imgReminderFinish.Visible = false;
			}
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				///		ProjectId, TypeId, TypeName, CalendarId, CalendarName, CreatorId, 
				///		ManagerId, ExecutiveManagerId, Title, Description, CreationDate, 
				///		StartDate, FinishDate, TargetStartDate, TargetFinishDate, ActualStartDate, ActualFinishDate, FixedHours, 
				///		FixedCost, Goals, Scope, Deliverables, StatusId, StatusName, 
				///		ClientId, ClientName, XMLFileId, CurrencyId, CurrencySymbol,
				///		PriorityId, PriorityName, PercentCompleted, PhaseId, PhaseName,
				///		RiskLevelId, RiskLevelName, RiskLevelWeight
				if(reader.Read())
				{
					TargetStartDate = (DateTime)reader["TargetStartDate"];
					TargetFinishDate = (DateTime)reader["TargetFinishDate"];

					lblTargetStartDate.Text = TargetStartDate.ToShortDateString();
					lblTargetFinishDate.Text = TargetFinishDate.ToShortDateString();

					if (reader["ActualStartDate"] != DBNull.Value)
						lblActualStartDate.Text = ((DateTime)reader["ActualStartDate"]).ToShortDateString();
					else
						lblActualStartDate.Text = LocRM.GetString("n_a");

					if (reader["ActualFinishDate"] != DBNull.Value)
						lblActualFinishDate.Text = ((DateTime)reader["ActualFinishDate"]).ToShortDateString();
					else
						lblActualFinishDate.Text = LocRM.GetString("n_a");

					if (reader["StartDate"] != DBNull.Value)
						lblCalculatedStartDate.Text = ((DateTime)reader["StartDate"]).ToShortDateString();
					else
						lblCalculatedStartDate.Text = LocRM.GetString("n_a");

					if (reader["FinishDate"] != DBNull.Value)
						lblCalculatedFinishDate.Text = ((DateTime)reader["FinishDate"]).ToShortDateString();
					else
						lblCalculatedFinishDate.Text = LocRM.GetString("n_a");
				}
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("timeline_info"));

			if (Project.CanUpdate(ProjectId) || imgReminderStart.Visible)
			{
				ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("Actions");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.LookId = "TopItemLook";

				ComponentArt.Web.UI.MenuItem subItem; 

				if (Project.CanUpdate(ProjectId))
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.ClientSideCommand = String.Format("javascript:ShowWizard('EditTargetTimeline.aspx?ProjectId={0}', 400, 250)", ProjectId);
					subItem.Text = LocRM2.GetString("EditTargetTimeline");
					topMenuItem.Items.Add(subItem);

					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.ClientSideCommand = String.Format("javascript:ShowWizard('EditActualTimeline.aspx?ProjectId={0}', 400, 250)", ProjectId);
					subItem.Text = LocRM2.GetString("EditActualTimeline");
					topMenuItem.Items.Add(subItem);
				}

				if (IsMember)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.Look.LeftIconUrl = "~/Layouts/Images/reminder.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.ClientSideCommand = String.Format("javascript:ShowWizard('../Directory/SystemRemindersForObject.aspx?ObjectTypeId={0}&ObjectId={1}', 420, 150)", (int)ObjectTypes.Project, ProjectId);
					subItem.Text = LocRM2.GetString("EditReminder");
					topMenuItem.Items.Add(subItem);
				}

				secHeader.ActionsMenu.Items.Add(topMenuItem);
			}
		}
		#endregion
	}
}
