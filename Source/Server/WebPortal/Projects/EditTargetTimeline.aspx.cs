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
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for EditTargetTimeline.
	/// </summary>
	public partial class EditTargetTimeline : System.Web.UI.Page
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(EditTargetTimeline).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(EditTargetTimeline).Assembly);

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
			this.cvDates.ServerValidate += new ServerValidateEventHandler(cvDates_ServerValidate);
		}
		#endregion

		#region BindValues()
		private void BindValues()
		{
			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				if (reader.Read())
				{
					dtcTargetStartDate.SelectedDate = (DateTime)reader["TargetStartDate"];
					dtcTargetEndDate.SelectedDate = (DateTime)reader["TargetFinishDate"];
				}
			}
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			Project2.UpdateTargetDates(ProjectId, dtcTargetStartDate.SelectedDate, dtcTargetEndDate.SelectedDate);

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"<script language=javascript>" +
				"try {window.opener.location.href=window.opener.location.href;}" +
				"catch (e){} window.close();</script>");
		}
		#endregion

		#region cvDates_ServerValidate
		private void cvDates_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if ((dtcTargetEndDate.SelectedDate != DateTime.MinValue) && (dtcTargetEndDate.SelectedDate < dtcTargetStartDate.SelectedDate))
			{
				cvDates.ErrorMessage = LocRM.GetString("EndDateError") + " (" + dtcTargetStartDate.SelectedDate.ToShortDateString() + ")";
				args.IsValid = false;
			}
			else
				args.IsValid = true;
		}
		#endregion
	}
}
