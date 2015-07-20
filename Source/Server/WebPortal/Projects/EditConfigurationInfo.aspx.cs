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
using System.Resources;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for EditConfigurationInfo.
	/// </summary>
	public partial class EditConfigurationInfo : System.Web.UI.Page
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(EditConfigurationInfo).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(EditConfigurationInfo).Assembly);

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

			if (!Page.IsPostBack)
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

		#region BindValues
		private void BindValues()
		{
			ddlCalendar.DataTextField = "CalendarName";
			ddlCalendar.DataValueField = "CalendarId";
			ddlCalendar.DataSource = Project.GetListCalendars(ProjectId);
			ddlCalendar.DataBind();

			ddlCurrency.DataTextField = "CurrencySymbol";
			ddlCurrency.DataValueField = "CurrencyId";
			ddlCurrency.DataSource = Project.GetListCurrency();
			ddlCurrency.DataBind();

			ddlType.DataValueField = "TypeId";
			ddlType.DataTextField = "TypeName";
			ddlType.DataSource = Project.GetListProjectTypes();
			ddlType.DataBind();

			ddInitialPhase.DataSource = Project.GetListProjectPhases();
			ddInitialPhase.DataTextField = "PhaseName";
			ddInitialPhase.DataValueField = "PhaseId";
			ddInitialPhase.DataBind();

			ddlBlockType.DataTextField = "Title";
			ddlBlockType.DataValueField = "primaryKeyId";
			ddlBlockType.DataSource = Mediachase.IbnNext.TimeTracking.TimeTrackingBlockType.List(Mediachase.Ibn.Data.FilterElement.EqualElement("IsProject", "1"));
			ddlBlockType.DataBind();

			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				if (reader.Read())
				{
					CommonHelper.SafeSelect(ddlCalendar, reader["CalendarId"].ToString());
					CommonHelper.SafeSelect(ddlCurrency, reader["CurrencyId"].ToString());
					CommonHelper.SafeSelect(ddlType, reader["TypeId"].ToString());
					CommonHelper.SafeSelect(ddInitialPhase, reader["InitialPhaseId"].ToString());
					if (reader["BlockTypeId"] != DBNull.Value)
						CommonHelper.SafeSelect(ddlBlockType, reader["BlockTypeId"].ToString());
				}
			}
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			Project2.UpdateConfigurationInfo(ProjectId, int.Parse(ddlCalendar.SelectedValue), int.Parse(ddlCurrency.SelectedValue), int.Parse(ddlType.SelectedValue), int.Parse(ddInitialPhase.SelectedValue), int.Parse(ddlBlockType.SelectedValue));

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"try {window.opener.top.frames['right'].location.href='../Projects/ProjectView.aspx?ProjectId=" + ProjectId + "';}" +
				"catch (e){} window.close();", true);
		}
		#endregion
	}
}
