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
	using ComponentArt.Web.UI;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for ProjectManagers.
	/// </summary>
	public partial class ProjectManagers : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ProjectManagers).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(ProjectManagers).Assembly);

		#region ProjectId
		private int ProjectId
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
			//if (!Page.IsPostBack)
			//{
				BindValues();
			//}
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

		#region BindValues
		private void BindValues()
		{
			///		ProjectId, TypeId, TypeName, CalendarId, CalendarName, CreatorId, 
			///		ManagerId, ExecutiveManagerId, Title, Description, CreationDate, 
			///		StartDate, FinishDate, TargetStartDate, TargetFinishDate, ActualStartDate, ActualFinishDate, FixedHours, 
			///		FixedCost, Goals, Scope, Deliverables, StatusId, StatusName, 
			///		ClientId, ClientName, XMLFileId, CurrencyId, CurrencySymbol,
			///		PriorityId, PriorityName, PercentCompleted, PhaseId, PhaseName,
			///		RiskLevelId, RiskLevelName, RiskLevelWeight
			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				if (reader.Read())
				{
					lblManager.Text = CommonHelper.GetUserStatus((int)reader["ManagerId"]);

					if (reader["ExecutiveManagerId"] != DBNull.Value)
					{
						lblExecManager.Text = CommonHelper.GetUserStatus((int)reader["ExecutiveManagerId"]);
						ExecutiveManagerRow.Visible = true;
					}
					else
					{
						ExecutiveManagerRow.Visible = false;
					}
				}
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("managers"));
			if (Project.CanUpdate(ProjectId))
			{
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_PM_Managers");
				string cmd = cm.AddCommand("Project", "", "ProjectView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				secHeader.AddRightLink("<img alt='' src='../Layouts/Images/Edit.gif'/> " + LocRM.GetString("tbEdit"), "javascript:" + cmd);
				//secHeader.AddRightLink(
				//    String.Format("<img alt='' src='../Layouts/Images/Edit.gif' width='16' height='16' border=0 align=absmiddle> {0}", LocRM.GetString("tbEdit")), 
				//    String.Format("javascript:ShowWizard('EditManagers.aspx?ProjectId={0}', 370, 160);", ProjectId));
			}
		}
		#endregion
	}
}
