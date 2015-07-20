using System;
using System.Globalization;
using System.Resources;

using Mediachase.Ibn;
using Mediachase.Ibn.Web.Interfaces;

namespace Mediachase.UI.Web.Public.Modules
{
	public partial class Error : System.Web.UI.UserControl, IPageTemplateTitle
	{
		private ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.strContactUs", typeof(Error).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblTitle.Text = LocRM.GetString("Error");

			string errorId = Request["ErrorId"];

			string prefix = Request.Url.Host.Replace(".", "_");
			string reportLink = Page.ResolveUrl(string.Format(CultureInfo.InvariantCulture, "~/Admin/Log/Error/{0}_{1}.html", prefix, errorId));
			string errorDetails = string.Format("<a target='_blank' href='{0}'>{1}</a>", reportLink, LocRM.GetString("ErrorDetails"));

			string addMessageLink = Page.ResolveUrl("~/Incidents/AddEMailMessage.aspx?ErrorId=" + errorId);
			string composeImageLink = Page.ResolveUrl("~/Layouts/Images/compose.gif");
			string support = string.Format("<a href='javascript:ShowResizableWizard(&quot;{0}&quot;,800,600);'><img alt='' src='{1}' style='vertical-align:middle; border:0'/> {2}</a>", addMessageLink, composeImageLink, GlobalResourceManager.Strings["SupportEmail"]);

			lblText.Text = string.Format(LocRM.GetString("ErrorText"), errorDetails, support);
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
			return LocRM.GetString("Error");
		}
		#endregion
	}
}
