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
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Modules
{
	/// <summary>
	/// Summary description for ActiveToDo.
	/// </summary>
	public partial class ActiveToDo : System.Web.UI.Page
	{
		protected Mediachase.UI.Web.Modules.BlockHeader secHeader;
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (!IsPostBack)
				BindGrid();
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

		#region BindGrid
		private void BindGrid()
		{
			dgActivities.DataSource = Mediachase.IBN.Business.ToDo.GetListActiveToDoAndTasksWithoutUpcoming(true, false, false).DefaultView;
			dgActivities.DataBind();
		}
		#endregion

		#region GetTarget()
		protected string GetTarget()
		{
			if(pc["HideTopFrame"]==null || pc["HideTopFrame"].ToString() == "1")
				return "_parent";
			else
				return "right";
		}
		#endregion
	}
}
