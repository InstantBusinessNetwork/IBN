namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for MetaDataView.
	/// </summary>
	public partial class MetaDataView : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlTable tblCustomFields;

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserView", typeof(MetaDataView).Assembly);
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

	}
}