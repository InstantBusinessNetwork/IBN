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
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Incidents
{
	/// <summary>
	/// Summary description for AddRelFromClip.
	/// </summary>
	public partial class AddRelFromClip : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(AddRelFromClip).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(AddRelFromClip).Assembly);
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region IncidentId
		protected int IncidentId
		{
			get
			{
				try
				{
					return int.Parse(Request["IncidentId"]);
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

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");

			Response.Cache.SetNoStore();
			if (!Page.IsPostBack)
				BindDG();
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
			this.lbAddIssue.Click += new EventHandler(lbAddIssue_Click);
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			int iCount = 10;
			if (pc["ClipboardItemsCount"] != null)
				iCount = int.Parse(pc["ClipboardItemsCount"].ToString());
			string sNewClip = "";
			if (pc["ClipboardIssues"] != null)
				sNewClip = pc["ClipboardIssues"].ToString();
			string sCheck = sNewClip;
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("IncidentId", typeof(int)));
			DataRow dr;
			while (sCheck.Length > 0)
			{
				if (sCheck.IndexOf("|") >= 0)
				{
					dr = dt.NewRow();
					int incId = int.Parse(sCheck.Substring(0, sCheck.IndexOf("|")));
					dr["IncidentId"] = incId;
					dt.Rows.Add(dr);
					sCheck = sCheck.Substring(sCheck.IndexOf("|") + 1);
				}
			}
			if (dt.Rows.Count == 0)
			{
				dgIssues.Visible = false;
				lblAlert.Visible = true;
				lblAlert.Text = LocRM.GetString("tNoItems");
			}
			else
			{
				dgIssues.Visible = true;
				lblAlert.Visible = false;
				dgIssues.DataSource = dt.DefaultView;
				dgIssues.DataBind();
			}
		}
		#endregion

		#region lbAddIssue_Click
		private void lbAddIssue_Click(object sender, EventArgs e)
		{
			int RelIssueId = int.Parse(hdnIssueId.Value);
			if (IncidentId > 0 && IncidentId != RelIssueId)
				Issue2.AddRelation(IncidentId, RelIssueId);

			CommandParameters cp = new CommandParameters("MC_HDM_Redirect");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion
	}
}
