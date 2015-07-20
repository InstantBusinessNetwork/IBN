namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.Interfaces;

	public partial class DocumentTypes : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDocTypes", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected UserLightPropertyCollection _pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (_pc["DocTypes_SortColumn"] == null)
				_pc["DocTypes_SortColumn"] = "Extension";
			BindToolbar();
			if (!Page.IsPostBack)
				BindDataGrid();
		}

		private void BindToolbar()
		{
			secHeader.AddLink(LocRM.GetString("AddCT"), this.Page.ResolveUrl("~/Admin/DocumentTypeEdit.aspx"));
			secHeader.AddLink(String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' />&nbsp;{1}",
				this.Page.ResolveUrl("~/Layouts/Images/cancel.gif"), LocRM2.GetString("tFilesForms")),
				this.Page.ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin6"));
			secHeader.Title = LocRM.GetString("tDocumentTypesList");
		}

		private void BindDataGrid()
		{
			int i = 2;
			dgDocType.Columns[i++].HeaderText = LocRM.GetString("Extension");
			dgDocType.Columns[i++].HeaderText = LocRM.GetString("Title");
			dgDocType.Columns[i++].HeaderText = "WebDAV";
			dgDocType.Columns[i++].HeaderText = LocRM.GetString("InNewWindow");
			dgDocType.Columns[i++].HeaderText = LocRM.GetString("AllowForceDownload");
			dgDocType.Columns[i++].HeaderText = LocRM.GetString("FriendlyName");

			foreach (DataGridColumn dgc in dgDocType.Columns)
			{
				if (dgc.SortExpression == _pc["DocTypes_SortColumn"])
					dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}' />", this.Page.ResolveUrl("~/layouts/images/upbtnF.jpg"));
				else if (dgc.SortExpression + " DESC" == _pc["DocTypes_SortColumn"])
					dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}' />", this.Page.ResolveUrl("~/layouts/images/downbtnF.jpg"));
			}

			DataView dv = Mediachase.IBN.Business.ContentType.GetListContentTypesDataTable().DefaultView;

			dv.Sort = _pc["DocTypes_SortColumn"];

			dgDocType.DataSource = dv;
			dgDocType.DataBind();

			foreach (DataGridItem dgi in dgDocType.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
			}
		}

		protected string GetAllowUrl(bool IsAllow)
		{
			if (IsAllow)
				return this.Page.ResolveUrl("~/layouts/images/accept_1.gif");
			else
				return this.Page.ResolveUrl("~/layouts/images/deny_1.gif");
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
			this.dgDocType.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgDocType_Delete);
			this.dgDocType.SortCommand += new DataGridSortCommandEventHandler(dgDocType_SortCommand);
		}
		#endregion

		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			return LocRM.GetString("DocumentTypes");
		}
		#endregion

		private void dgDocType_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int cid = int.Parse(e.CommandArgument.ToString());
			ContentType.Delete(cid);
			BindDataGrid();
		}

		private void dgDocType_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if ((_pc["DocTypes_SortColumn"] != null) && (_pc["DocTypes_SortColumn"].ToString() == (string)e.SortExpression))
				_pc["DocTypes_SortColumn"] = (string)e.SortExpression + " DESC";
			else
				_pc["DocTypes_SortColumn"] = (string)e.SortExpression;
			BindDataGrid();
		}
	}
}
