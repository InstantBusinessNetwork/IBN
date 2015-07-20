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

	/// <summary>
	///		Summary description for ActivitiesByManager.
	/// </summary>
	public partial class ActivitiesByManager : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ActivitiesByManager).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindDG();
			if (dgProjects.Items.Count==0)
			{
				lblNoItems.Text = LocRM.GetString("tTheAreNoItems");
				spanLbl.Visible = true;
				dgProjects.Visible = false;
			}
			else
			{
				spanLbl.Visible = false;
				dgProjects.Visible = true;
			}
		}

		private void BindDG()
		{
			dgProjects.Columns[0].HeaderText = LocRM.GetString("Title");
			dgProjects.Columns[1].HeaderText = LocRM.GetString("Status");
			dgProjects.Columns[2].HeaderText = LocRM.GetString("tOpenTasks");
			dgProjects.Columns[3].HeaderText = LocRM.GetString("tCompletedTasks");
			dgProjects.Columns[4].HeaderText = LocRM.GetString("tIssues");
			int PortfolioId = int.Parse(pc["ActivitiesTracking_PrjGrp"].ToString());
			int PhaseId = int.Parse(pc["ActivitiesTracking_PrjPhase"].ToString());
			int StatusId = int.Parse(pc["ActivitiesTracking_Status"].ToString());
			
			DataTable dt = Project.GetListProjectsGroupedByManager(PortfolioId, PhaseId, StatusId, 0);
			///  ManagerId, ManagerName, ProjectId, ProjectName, StatusName, 
			///  OpenTasks, CompletedTasks, Issues, IsHeader
			dgProjects.DataSource = dt.DefaultView;
			dgProjects.DataBind();
			if(!Configuration.HelpDeskEnabled)
				dgProjects.Columns[4].Visible = false;
		}

		protected string GetTitle(bool IsHeader, int PrjId, int ManagerId)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"<b>" + CommonHelper.GetUserStatus(ManagerId) + "</b>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:15px'>"+CommonHelper.GetProjectStatusWithId(PrjId)+"</span>";
			}
		}

		protected string GetStatus(bool IsHeader, string StatusName)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='boldtext'>"+
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:5px'>"+StatusName+"</span>";
			}
		}

		protected string GetOpenTasks(bool IsHeader, int OpenTasksCount)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:5px'>"+OpenTasksCount+"</span>";
			}
		}

		protected string GetCompletedTasks(bool IsHeader, int TCount)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='text'>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:5px'>"+TCount+"</span>";
			}
		}

		protected string GetIssues(bool IsHeader, int IssCount)
		{
			if (IsHeader) 
			{
				return "<table class='alt-tblstyle' style='width:100%'><tr><td class='boldtext'>" +
					"</td></tr></table>";
			}
			else
			{
				return  "<span class='text' style='padding-left:5px'>"+IssCount+"</span>";
			}
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
	}
}
