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
	///		Summary description for ProjectGeneralInfo.
	/// </summary>
	public partial class ProjectGeneralInfo : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ProjectGeneralInfo).Assembly);

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
			if(!Page.IsPostBack)
			{
				BindValues();
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

		#region BindValues
		private void BindValues()
		{
			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				if(reader.Read())
				{
					lblDescription.Text = CommonHelper.parsetext(reader["Description"].ToString(), false);
					lblScope.Text = CommonHelper.parsetext(reader["Scope"].ToString(), false);
					lblGoals.Text = CommonHelper.parsetext(reader["Goals"].ToString(), false);
					lblDeliverables.Text = CommonHelper.parsetext(reader["Deliverables"].ToString(), false);
				}
			}

			GoalsRow.Visible = PortalConfig.ProjectAllowViewGoalsField;
			DeliverablesRow.Visible = PortalConfig.ProjectAllowViewDeliverablesField;
			ScopeRow.Visible = PortalConfig.ProjectAllowViewScopeField;

			if (String.IsNullOrEmpty(lblDescription.Text) 
				&& (String.IsNullOrEmpty(lblScope.Text) || !ScopeRow.Visible) 
				&& (String.IsNullOrEmpty(lblGoals.Text) || !GoalsRow.Visible)
				&& (String.IsNullOrEmpty(lblDeliverables.Text) || !DeliverablesRow.Visible))
				this.Visible = false;
		}
		#endregion

		#region BindToolbar()
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("general_info"));

			if (Project.CanUpdate(ProjectId))
			{
				secHeader.AddRightLink(
					String.Format("<img src='../Layouts/Images/Edit.gif' width='16' height='16' border=0 align=absmiddle> {0}", LocRM.GetString("tbEdit")), 
					String.Format("javascript:ShowWizard('EditGeneralInfo.aspx?ProjectId={0}', 550, 450);", ProjectId));
			}
		}
		#endregion
	}
}
