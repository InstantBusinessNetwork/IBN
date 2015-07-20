using System;
using System.Resources;
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
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for AddRelFromClip.
	/// </summary>
	public partial class AddRelFromClip : System.Web.UI.Page
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(AddRelFromClip).Assembly);
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

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
					return -1;
				}
			}
		}

		protected string BtnID
		{
			get
			{
				return Request["BtnID"] != null ? Request["BtnID"] : String.Empty;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");

			Response.Cache.SetNoStore();
			if (!Page.IsPostBack)
				BindDG();
		}

		private void BindDG()
		{
			int iCount = 10;
			if (pc["ClipboardItemsCount"] != null)
				iCount = int.Parse(pc["ClipboardItemsCount"].ToString());
			string sNewPrjClip = "";
			if (pc["ClipboardPrjs"] != null)
				sNewPrjClip = pc["ClipboardPrjs"].ToString();
			string sCheck = sNewPrjClip;
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("ProjectId", typeof(int)));
			DataRow dr;
			while (sCheck.Length > 0)
			{
				if (sCheck.IndexOf("|") >= 0)
				{
					dr = dt.NewRow();
					int PrjId = int.Parse(sCheck.Substring(0, sCheck.IndexOf("|")));
					dr["ProjectId"] = PrjId;
					dt.Rows.Add(dr);
					sCheck = sCheck.Substring(sCheck.IndexOf("|") + 1);
				}
			}
			if (dt.Rows.Count == 0)
			{
				dgPrjs.Visible = false;
				lblAlert.Visible = true;
				lblAlert.Text = LocRM.GetString("tNoItems");
			}
			else
			{
				dgPrjs.Visible = true;
				lblAlert.Visible = false;
				dgPrjs.DataSource = dt.DefaultView;
				dgPrjs.DataBind();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

		protected void Add_Click(object sender, System.EventArgs e)
		{
			int RelProjectId = int.Parse(hdnProjectId.Value);
			if (ProjectId > 0 && ProjectId != RelProjectId)
				Project2.AddRelation(ProjectId, RelProjectId);

			CommandParameters cp = new CommandParameters("MC_PM_Redirect");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
	}
}
