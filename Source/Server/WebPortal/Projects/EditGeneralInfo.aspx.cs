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
	/// Summary description for EditGeneralInfo.
	/// </summary>
	public partial class EditGeneralInfo : System.Web.UI.Page
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(EditGeneralInfo).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(EditGeneralInfo).Assembly);

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
			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				if (reader.Read())
				{
					txtTitle.Text = HttpUtility.HtmlDecode(reader["Title"].ToString());
					txtDescription.Text = HttpUtility.HtmlDecode(reader["Description"].ToString());
					txtDeliverables.Text = HttpUtility.HtmlDecode(reader["Deliverables"].ToString());
					txtScope.Text = HttpUtility.HtmlDecode(reader["Scope"].ToString());
					txtGoals.Text = HttpUtility.HtmlDecode(reader["Goals"].ToString());
				}
			}

			GoalsRow.Visible = PortalConfig.ProjectAllowEditGoalsField;
			ScopeRow.Visible = PortalConfig.ProjectAllowEditScopeField;
			DeliverablesRow.Visible = PortalConfig.ProjectAllowEditDeliverablesField;
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			txtTitle.Text = HttpUtility.HtmlEncode(txtTitle.Text);
			txtGoals.Text = HttpUtility.HtmlEncode(txtGoals.Text);
			txtScope.Text = HttpUtility.HtmlEncode(txtScope.Text);
			txtDescription.Text = HttpUtility.HtmlEncode(txtDescription.Text);
			txtDeliverables.Text = HttpUtility.HtmlEncode(txtDeliverables.Text);

			Project2.UpdateGeneralInfo(ProjectId, txtTitle.Text, txtDescription.Text, txtGoals.Text, txtScope.Text, txtDeliverables.Text);

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"try {window.opener.top.frames['right'].location.href='../Projects/ProjectView.aspx?ProjectId=" + ProjectId + "';}" +
				"catch (e){} window.close();", true);
		}
		#endregion
	}
}
