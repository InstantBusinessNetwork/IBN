namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Text;
	using System.Drawing;
	using System.Collections;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business; 

	/// <summary>
	///		Summary description for UsersReports.
	/// </summary>
	public partial  class UsersReports : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlTableCell tdAssigned;
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strDirectory", typeof(UsersReports).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
