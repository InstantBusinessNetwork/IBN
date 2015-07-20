namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for ProjectInfo2.
	/// </summary>
	public partial class ProjectInfo2 : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ProjectInfo2).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(ProjectInfo2).Assembly);

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
			{
				BindValues();
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

		#region BindValues
		private void BindValues()
		{
			lnkMSProject.Visible = false;
			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				if (reader.Read())
				{
					lblTitle.Text = String.Concat(reader["Title"].ToString(), Mediachase.Ibn.Web.UI.CHelper.GetProjectNumPostfix(ProjectId, (string)reader["ProjectCode"]));

					lblPriority.Text = reader["PriorityName"].ToString() + " " + LocRM.GetString("Priority").ToLower();
					lblPriority.ForeColor = Util.CommonHelper.GetPriorityColor((int)reader["PriorityId"]);
					lblPriority.Visible = PortalConfig.GeneralAllowPriorityField;

					lblState.Text = reader["StatusName"].ToString();

					lblManager.Text = CommonHelper.GetUserStatus((int)reader["ManagerId"]);

					if (reader["Description"] != DBNull.Value)
					{
						string txt = CommonHelper.parsetext(reader["Description"].ToString(), false);
						if (PortalConfig.ShortInfoDescriptionLength > 0 && txt.Length > PortalConfig.ShortInfoDescriptionLength)
							txt = txt.Substring(0, PortalConfig.ShortInfoDescriptionLength) + "...";
						lblDescription.Text = txt;
					}

					lblTimeline.Text = ((DateTime)reader["TargetStartDate"]).ToShortDateString() + " - " + ((DateTime)reader["TargetFinishDate"]).ToShortDateString();

					if ((bool)reader["IsMSProject"])
					{
						lnkMSProject.Visible = true;
						lnkMSProject.NavigateUrl = String.Format(CultureInfo.InvariantCulture,
							"javascript:OpenWindow('../Projects/ProjectExportImportNew.aspx?ProjectId={0}',600,400);", ProjectId);
						lnkMSProject.Text = String.Format(CultureInfo.InvariantCulture,
							"<img src='../Layouts/Images/icons/export.gif' width='16px' height='16px' border='0' title='{0}' align='absmiddle' style='padding-left:10px;'/>", LocRM2.GetString("MSProjectExchange"));

						SynchronizationInfoDiv.Visible = true;
					}
				}
			}

			lblPriority.Visible = PortalConfig.CommonProjectAllowViewPriorityField;
		}
		#endregion
	}
}
