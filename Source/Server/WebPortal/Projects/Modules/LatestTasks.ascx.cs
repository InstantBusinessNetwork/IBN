namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using System.Resources;

	/// <summary>
	///		Summary description for LatestTasks.
	/// </summary>
	public partial  class LatestTasks : System.Web.UI.UserControl
	{
		protected UserLightPropertyCollection pcCurrentUser;

		private int ProjID
		{
			get 
			{
				try
				{
					return int.Parse(Request["ProjectID"]);
				}
				catch
				{
					throw new Exception("Invalid Project ID");
				}
			}
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			pcCurrentUser = Security.CurrentUser.Properties;
			BindToolbars();
			if(!Page.IsPostBack)
			{
				ApplyLocalization();
				BindValues();
			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectSummary", typeof(LatestTasks).Assembly);
			dgTasks.Columns[0].HeaderText = LocRM.GetString("title");
			dgTasks.Columns[1].HeaderText = LocRM.GetString("Completed");
			dgTasks.Columns[2].HeaderText = LocRM.GetString("Priority");
			dgTasks.Columns[3].HeaderText = LocRM.GetString("FinishDate");

		}
		#endregion

		#region BindToolbars
		private void BindToolbars()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectSummary", typeof(LatestTasks).Assembly);
			tbTasks.Title = LocRM.GetString("tbTask");
			tbTasks.AddLink("<img alt='' src='../Layouts/Images/icon-search.gif'/> " + LocRM.GetString("tbView"),Page.ClientScript.GetPostBackClientHyperlink(lbViewAll,""));
			if (Project.CanUpdate(ProjID) && !Project.GetIsMSProject(ProjID))
				tbTasks.AddLink("<img alt='' src='../Layouts/Images/newitem.gif'/> " + LocRM.GetString("tbAdd"),"../Tasks/TaskEdit.aspx?ProjectId=" + ProjID);	
		}
		#endregion

		#region BindValues

		private void BindValues()
		{
			if(ProjID != 0)
			{
				DataTable dt = Task.GetListTasksForUserByProject(ProjID);
				DataView dv = dt.DefaultView;
				dgTasks.DataSource = dv;
				dgTasks.DataBind();
			}
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void lbViewAll_Click(object sender, System.EventArgs e)
		{
			UserLightPropertyCollection pc= Security.CurrentUser.Properties;
			int mask = (int)CalendarView.CalendarFilter.Task;
			pc["ListView_Type"] = mask.ToString();
			Response.Redirect(String.Format("../Projects/ProjectView.aspx?ProjectID={0}&Tab=6&ABTab=ListView",ProjID.ToString()));		
		}

		protected string GetTaskStatus(DateTime FinishDate,bool IsCompleted,int TaskId, string Title)
		{
			if (FinishDate<UserDateTime.UserNow && !IsCompleted)
			{
				return 	@"<a href='../Tasks/TaskView.aspx?TaskId="+ TaskId +"' style='color:red'>"+Title+"</a>";
			}
			else
				return @"<a href='../Tasks/TaskView.aspx?TaskId="+ TaskId +"'>"+Title+"</a>";
		}
	}
}
