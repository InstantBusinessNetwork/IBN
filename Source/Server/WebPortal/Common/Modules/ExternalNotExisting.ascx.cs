namespace Mediachase.UI.Web.Common.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using System.Resources;
	using Mediachase.Ibn.Web.Interfaces;


	/// <summary>
	///		Summary description for ExternalNotExisting.
	/// </summary>
	public partial  class ExternalNotExisting : System.Web.UI.UserControl,IPageTemplateTitle
	{
    public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Common.Resources.strNotExistingId", typeof(ExternalNotExisting).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblText.Text = LocRM.GetString("Text");
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

		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			return LocRM.GetString("Closed");
		}
		#endregion
	}
}
