using System;
using System.Data;

using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Apps.Shell.Modules
{
	public partial class ActiveDocuments : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Grid.css");
			BindDataGrid(!IsPostBack);
		}

		void BindDataGrid(bool dataBind)
		{
			DataTable dt = Document.GetListActiveDocumentsByUserOnlyDataTable();

			DataView dv = dt.DefaultView;

			ctrlGrid.DataSource = dv;
			
			divNoObjects.Visible = false;
			if (dv.Count == 0)
			{
				ctrlGrid.Visible = false;
				lblNoObjects.Text = String.Format("{0} <a href='{1}'>{2}</a>",
					GetGlobalResourceObject("IbnFramework.Global", "NoDocuments").ToString(),
					this.Page.ResolveUrl("~/Documents/DocumentEdit.aspx"),
						GetGlobalResourceObject("IbnFramework.Global", "CreateDocument").ToString());
				divNoObjects.Visible = true;
			}
		}

		#region GetDocumentTitle
		public static string GetDocumentTitle(string Title, int DocumentId, int ReasonId)
		{
			string img = "document_active.gif";
			switch (ReasonId)
			{
				case 1:
				case 2:
					img = "document_completed.gif";
					break;
				case 3:
				case 4:
					img = "document_suspensed.gif";
					break;
			}
			
			if (!Security.CurrentUser.IsExternal)
				return String.Format(
					@"<a href='{4}{0}{3}'><img alt='' src='{2}'/> {1}</a>"
					, DocumentId, Title, CHelper.GetAbsolutePath("/Layouts/Images/icons/" + img), "", CHelper.GetAbsolutePath("/Documents/DocumentView.aspx?DocumentId="));
			else return String.Format(
					@"<img alt='' src='{1}'/>{0}", Title, CHelper.GetAbsolutePath("/Layouts/Images/icons/" + img));
		}
		#endregion
	}
}