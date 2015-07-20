using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Resources;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for ProjectsByBusinessScoresPopUp.
	/// </summary>
	public partial class ProjectsByBusinessScoresPopUp : System.Web.UI.Page
	{

		protected UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ProjectsByBusinessScoresPopUp).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");

			// Put user code to initialize the page here
			if (!Page.IsPostBack)
			{
				BindDataGrid();
			}

			BindTitles();

		}

		private void BindTitles()
		{
			secHeader.Title = LocRM.GetString("tSelectProjects");
			secHeader.AddLink("<img alt='' src='../Layouts/Images/saveitem.gif'/> " + LocRM.GetString("Apply"), "javascript:" + Page.ClientScript.GetPostBackEventReference(lbApply, ""));
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tCancel"), "javascript:window.close()");
			cbAllProjects.Text = LocRM.GetString("tSelectAll");
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
			this.lbApply.Click += new EventHandler(lbApply_Click);

		}
		#endregion

		private void BindDataGrid()
		{
			dgProjects.DataSource = Project.GetListProjectsSimple();
			dgProjects.DataBind();
		}

		private void lbApply_Click(object sender, EventArgs e)
		{
			string s = string.Empty;
			foreach (DataGridItem dgi in dgProjects.Items)
			{
				CheckBox cb = (CheckBox)dgi.FindControl("cbProject");
				if (cb != null && cb.Checked)
					s += dgProjects.DataKeys[dgi.ItemIndex].ToString() + ";";
			}
			if (s.Length > 0)
			{
				pc["Report_ProjectListType"] = "Custom";
				pc["Report_ProjectListData"] = s.Remove(s.Length - 1, 1);
			}

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "try {window.opener.location.href=window.opener.location.href;}" +
					  "catch (e){} window.close();", true);
		}
	}
}
