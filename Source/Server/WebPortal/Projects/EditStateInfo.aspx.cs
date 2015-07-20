using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using System.Resources;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for EditStateInfo.
	/// </summary>
	public partial class EditStateInfo : System.Web.UI.Page
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(EditStateInfo).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(EditStateInfo).Assembly);

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
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (!IsPostBack)
				BindValues();

			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");

			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnCancel.Attributes.Add("onclick", "window.close()");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
		}
		#endregion

		#region BindValues()
		private void BindValues()
		{
			ddlStatus.DataSource = Project.GetListProjectStatus();
			ddlStatus.DataTextField = "StatusName";
			ddlStatus.DataValueField = "StatusId";
			ddlStatus.DataBind();

			ddlPhase.DataSource = Project.GetListProjectPhases();
			ddlPhase.DataTextField = "PhaseName";
			ddlPhase.DataValueField = "PhaseId";
			ddlPhase.DataBind();

			ddlRiskLevel.DataTextField = "RiskLevelName";
			ddlRiskLevel.DataValueField = "RiskLevelId";
			ddlRiskLevel.DataSource = Project.GetListRiskLevels();
			ddlRiskLevel.DataBind();

			ddlPriority.DataTextField = "PriorityName";
			ddlPriority.DataValueField = "PriorityId";
			ddlPriority.DataSource = Project.GetListPriorities();
			ddlPriority.DataBind();

			ddlOverallStatus.Items.Clear();
			for (int i = 0; i <= 100; i++)
				ddlOverallStatus.Items.Add(new ListItem(i.ToString() + " %", i.ToString()));

			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				if (reader.Read())
				{
					CommonHelper.SafeSelect(ddlRiskLevel, reader["RiskLevelId"].ToString());
					CommonHelper.SafeSelect(ddlOverallStatus, reader["PercentCompleted"].ToString());
					CommonHelper.SafeSelect(ddlStatus, reader["StatusId"].ToString());
					CommonHelper.SafeSelect(ddlPhase, reader["PhaseId"].ToString());
					CommonHelper.SafeSelect(ddlPriority, reader["PriorityId"].ToString());
				}
			}

			trPriority.Visible = PortalConfig.GeneralAllowPriorityField;
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			Project2.UpdateStateInfo(ProjectId, int.Parse(ddlStatus.SelectedValue), int.Parse(ddlPhase.SelectedValue), int.Parse(ddlRiskLevel.SelectedValue), int.Parse(ddlOverallStatus.SelectedValue), int.Parse(ddlPriority.SelectedValue));

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "try {window.opener.top.frames['right'].location.href='../Projects/ProjectView.aspx?ProjectId=" + ProjectId + "';}" +
					  "catch (e){} window.close();", true);
		}
		#endregion
	}
}
