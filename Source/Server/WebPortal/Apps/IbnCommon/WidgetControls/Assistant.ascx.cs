using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using System.Globalization;
using Mediachase.Ibn;


namespace Mediachase.UI.Web.Workspace.Modules
{
	/// <summary>
	///		Summary description for Assistant.
	/// </summary>
	public partial class Assistant : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", typeof(Assistant).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			BindToolbar();
			BindMenu();
			BindValues();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tAssistantTitle");
			secHeader.AddLink(
				String.Format(CultureInfo.InvariantCulture, "<img alt='' src='{0}'/>", ResolveClientUrl("~/Layouts/Images/b4.gif")),
				String.Format(CultureInfo.InvariantCulture, "javascript:{0}", Page.ClientScript.GetPostBackEventReference(lbHide, "")));
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lHelpDocs.Text = String.Format(CultureInfo.InvariantCulture,
				"<img alt='' src='{0}' border='0' width='20px' height='20px' align='absmiddle'/>&nbsp;{1}<br>",
				ResolveClientUrl("~/Layouts/images/help.gif"),
				LocRM.GetString("tHelpDocs"));
			lDashboardView.Text = String.Format(CultureInfo.InvariantCulture,
				"<br><img alt='' src='{0}' border='0' align='absmiddle'/>&nbsp;{1}<br>",
				ResolveClientUrl("~/Layouts/images/piechart.gif"),
				LocRM.GetString("tViewDashboard"));
			lChangeProfile.Text = String.Format(CultureInfo.InvariantCulture,
				"<br><img alt='' src='{0}' border='0' width='16px' height='16px' align='absmiddle'/>&nbsp;{1}<br>",
				ResolveClientUrl("~/Layouts/images/icons/user_edit.gif"),
				LocRM.GetString("tChangeProfile"));

			DataTable dt = null;
			if (PortalConfig.UseIM && !Security.CurrentUser.IsExternal)
			{
				string sClientStr = String.Empty;
				sClientStr = String.Format(CultureInfo.InvariantCulture,
									"<br><img alt='' src='{0}' width='16px' height='16px' align='absmiddle'/>&nbsp;<font color='#ff0000'>{1}</font>&nbsp;{2}",
									ResolveClientUrl("~/Layouts/images/status/status_online.gif"),
									Report.GetOnlineUsersCount(),
									String.Format(LocRM.GetString("tPeopleOnline"), IbnConst.ProductFamilyShort));
				int iLoginsClient = Report.GetUserLoginsClient(Security.CurrentUser.UserID);
				if (iLoginsClient == 0)
				{
					sClientStr += String.Format(CultureInfo.InvariantCulture,
						"&nbsp;&nbsp;<a href='{0}'>{1}</a><br>",
						ResolveClientUrl("~/Home/Download.aspx"),
						LocRM.GetString("tDownloadClient"));
				}
				else
				{
					sClientStr += "<br>";
				}
				lblClient.Text = sClientStr;
			}
			if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
			{
				lblCustReports.Visible = true;
				dt = null;
				dt = Report.GetReportTemplatesByFilterDataTable(0, Report.DefaultStartDate, Report.DefaultEndDate, 0);
				int iCustReps = 0;
				if (dt != null)
					iCustReps = dt.Rows.Count;
				string sCustRepStr = String.Format(CultureInfo.InvariantCulture,
					"<br><img alt='' src='{0}' width='16px' height='16px' align='absmiddle'/>&nbsp;<font color='#ff0000'>{1}</font>&nbsp;{2}&nbsp;&nbsp;<a href='{3}'>{4}</a><br>",
					ResolveClientUrl("~/Layouts/images/report.gif"),
					iCustReps,
					LocRM.GetString("tCustomReps"),
					ResolveClientUrl("~/Apps/ReportManagement/Pages/UserReport.aspx"),
					LocRM.GetString("tView"));
				lblCustReports.Text = sCustRepStr;
			}
			else
				lblCustReports.Visible = false;
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			if ((!Configuration.ProjectManagementEnabled && !Configuration.HelpDeskEnabled) ||
				!(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)))
			{
				lDashboardView.Visible = false;
			}

			lDashboardView.NavigateUrl = "~/Reports/default.aspx?Tab=Dashboard";
			lChangeProfile.NavigateUrl = "~/Directory/UserView.aspx?UserId=" + Security.CurrentUser.UserID;
			lHelpDocs.NavigateUrl = "~/Home/Help.aspx";

			string con_str = "";
			using (IDataReader reader = Mediachase.IBN.Business.Common.GetBroadCastMessages(true))
			{
				while (reader.Read())
				{
					con_str += String.Format("<div style='padding-bottom:10px'><font color='#ff0000'>[ {0} {1} ]</font><br>{2}</div>", ((DateTime)reader["CreationDate"]).ToShortDateString(), ((DateTime)reader["CreationDate"]).ToShortTimeString(), reader["Text"].ToString());
				}
			}
			if (con_str == "")
				con_str += "<font color='#ff0000'>[ " + DateTime.Today.ToShortDateString() + " ]&nbsp;-&nbsp;" + LocRM.GetString("tNoAlerts") + "</font>";

			lblLastAlert.Text = con_str;
		}
		#endregion

		#region BindMenu
		private void BindMenu()
		{
			ddMenu.Items.Clear();
			ddMenu.Items.Add(new ListItem("----" + LocRM.GetString("tSelectAction") + "----", "-1"));
			if (Configuration.ProjectManagementEnabled && Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.InternalSecureGroups.ProjectManager))
			{
				ddMenu.Items.Add(new ListItem(LocRM.GetString("NewProjectWizard"), "NewProjectWizard"));
			}
			//			ddMenu.Items.Add(new ListItem(LocRM.GetString("NewIssueWizard"),"NewIssueWizard"));
			ddMenu.Items.Add(new ListItem(LocRM.GetString("NewEventWizard"), "NewEventWizard"));
			ddMenu.Items.Add(new ListItem(LocRM.GetString("NewToDoWizard"), "NewToDoWizard"));
			ddMenu.Items.Add(new ListItem(LocRM.GetString("NewUserWizard"), "NewUserWizard"));
			if (Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.InternalSecureGroups.PowerProjectManager))
			{
				ddMenu.Items.Add(new ListItem(LocRM.GetString("IBNReport"), "IBNReport"));
			}
			if (Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.InternalSecureGroups.Administrator))
			{
				ddMenu.Items.Add(new ListItem(LocRM.GetString("tUsersImport"), "ADConvertWizard"));
			}
			if (Configuration.HelpDeskEnabled)
				ddMenu.Items.Add(new ListItem(LocRM.GetString("tOtherImport"), "ImportWizard"));

			//ddMenu.Items.Add(new ListItem(LocRM.GetString("tOtherImport1"), "ImportWizard2"));
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			lbHide.Click += new EventHandler(lbHide_Click);
		}
		#endregion

		private void lbHide_Click(object sender, EventArgs e)
		{
			//Util.CommonHelper.HideWorkspaceControl("11"); 
			Response.Redirect("~/Workspace/default.aspx?BTab=Workspace");
		}
	}
}
