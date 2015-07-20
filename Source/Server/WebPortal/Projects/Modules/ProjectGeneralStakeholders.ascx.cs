namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Globalization;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for ProjectGeneralStakeholders.
	/// </summary>
	public partial class ProjectGeneralStakeholders : System.Web.UI.UserControl
	{
		ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ProjectGeneralStakeholders).Assembly);

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
					throw new Exception("ProjectID is Reqired");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BinddgMembers();
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		#region BinddgMembers
		private void BinddgMembers()
		{
			dgMembers.DataSource = Project.GetListStakeholdersDataTable(ProjectId);
			dgMembers.DataBind();

			this.Visible = dgMembers.Items.Count > 0;
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("ProjectStakeholders"));
			if (Project.CanUpdate(ProjectId))
			{
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_PM_StakesEdit");
				string cmd = cm.AddCommand("Project", "", "ProjectView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/stakeholders.gif'/> " + LocRM.GetString("Modify"), "javascript:" + cmd);
				//secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/stakeholders.gif'/> " + LocRM.GetString("Modify"), "javascript:ShowWizard('StakeholdersEditor.aspx?ProjectId=" + ProjectId + "', 650, 350);");
			}
		}
		#endregion
	}
}
