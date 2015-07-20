namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Text;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.Ibn;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.Interfaces;
	using System.Reflection;

	/// <summary>
	///		Summary description for WebStubs.
	/// </summary>
	public partial class AdmWebStubs : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strWebStubs", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));


		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!PortalConfig.UseIM) throw new AccessDeniedException();

			if (!IsPostBack)
				BindDG();

			BindToolbar();
		}

		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tTitle");
			secHeader.AddLink("<img alt='' src='" + Page.ResolveUrl("~/Layouts/Images/icons/webstub_create.gif") + "'/> " + LocRM.GetString("Add"), Page.ResolveUrl("~/Admin/AddEditWebStub.aspx?Group=1"));
			secHeader.AddLink(string.Format("<img alt='' title='{0}' src='{1}'/>&nbsp;{0}",
				LocRM2.GetString("tAddTools"),
				Page.ResolveUrl("~/Layouts/Images/cancel.gif")),
				Page.ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin8"));
		}

		protected string GetIcon(int HasIcon, string shortText, int StubID)
		{
			if (HasIcon == 0)
				return shortText;
			else
				return "<img alt=\"Icon\" src=\"" + Page.ResolveUrl("~/Common/WebStub.aspx?StubId=" + StubID) + "\" />";
		}

		protected string GetGroups(int StubId)
		{
			StringBuilder sb = new StringBuilder();
			using (IDataReader rdr = WebStubs.GetListGroupsByStub(StubId))
			{

				while (rdr.Read())
				{
					sb.Append(CommonHelper.GetGroupLink((int)rdr["GroupId"], CommonHelper.GetResFileString((string)rdr["GroupName"])));
					sb.Append("<br />");
				}
			}
			return sb.ToString();
		}

		private void BindDG()
		{
			dgStubs.Columns[1].HeaderText = LocRM.GetString("Icon");
			dgStubs.Columns[2].HeaderText = LocRM.GetString("Title");
			dgStubs.Columns[3].HeaderText = LocRM.GetString("Url");
			dgStubs.Columns[4].HeaderText = LocRM.GetString("Groups");
			dgStubs.Columns[5].HeaderText = LocRM.GetString("Options");

			dgStubs.DataSource = WebStubs.GetListStubsForAdmin();
			dgStubs.DataBind();

			foreach (DataGridItem dgi in dgStubs.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");

				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgStubs.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgstubs_Delete);

		}
		#endregion

		private void dgstubs_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int StubId = int.Parse(e.Item.Cells[0].Text);
			WebStubs.Delete(StubId);
			BindDG();
		}

		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			return LocRM.GetString("tTitle");
		}
		#endregion
	}
}
