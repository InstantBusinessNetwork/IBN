using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using System.Reflection;


namespace Mediachase.UI.Web.Admin.Modules
{
	/// <summary>
	/// Summary description for EmailBoxes.
	/// </summary>
	public partial class CustomSQLReport : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				ApplyLocalization();
			}
			btnProcess.CustomImage = this.Page.ResolveUrl("~/layouts/images/icons/status_active.gif");
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			secHeader.Title = LocRM.GetString("tCustomSQLReport");
			secHeader.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
				LocRM.GetString("tAddTools"),
				this.Page.ResolveUrl("~/Layouts/Images/cancel.gif")),
				ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin8"));
			btnProcess.Text = LocRM.GetString("tProcess");
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
			this.btnProcess.ServerClick += new EventHandler(btnProcess_ServerClick);
		}
		#endregion

		private void btnProcess_ServerClick(object sender, EventArgs e)
		{
			Response.Clear();
			Response.ContentType = "text/html";
			Response.ContentEncoding = System.Text.Encoding.UTF8;
			Response.AddHeader("content-disposition", String.Format("attachment; filename=\"CustomSqlReport_{0}.html\"", DateTime.Now.ToString("yyyy-MM-dd")));
			Response.Write(SqlReport.ExecuteScript(txtSQLRequest.Text));
			Response.End();
		}
	}
}
