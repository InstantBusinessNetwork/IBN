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

	/// <summary>
	///		Summary description for ProjectStateInfo.
	/// </summary>
	public partial class ProjectStateInfo : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ProjectStateInfo).Assembly);

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
			if (!Page.IsPostBack)
				BindValues();

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
				if (reader.Read())
				{
					lblStatus.Text = reader["StatusName"].ToString();
					lblPhase.Text = reader["PhaseName"].ToString();
					lblRiskLevel.Text = reader["RiskLevelName"].ToString();
					lblPercent.Text = reader["PercentCompleted"].ToString();
					lblPriority.Text = reader["PriorityName"].ToString();
				}
			}

			trPriority.Visible = PortalConfig.CommonProjectAllowViewPriorityField;
		}
		#endregion

		#region BindToolbar()
		private void BindToolbar()
		{
			secHeader.AddText(LocRM.GetString("state_info"));
			if (Project.CanUpdate(ProjectId))
			{
				secHeader.AddRightLink(
					String.Format("<img src='../Layouts/Images/Edit.gif' width='16' height='16' border=0 align=absmiddle> {0}", LocRM.GetString("tbEdit")),
					String.Format("javascript:ShowWizard('EditStateInfo.aspx?ProjectId={0}', 350, 265);", ProjectId));
			}
		}
		#endregion
	}
}
