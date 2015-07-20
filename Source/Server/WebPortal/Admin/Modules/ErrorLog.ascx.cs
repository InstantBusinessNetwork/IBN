namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.IO;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.IBN.Business;
	using System.Reflection;

	/// <summary>
	///		Summary description for UserLog.
	/// </summary>
	public partial  class UserLog : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			ApplyLocalization();
			if (!IsPostBack)
				BinddgErrors();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			Header2.Title = LocRM.GetString("tErrorLog");
			if (Security.IsUserInGroup(InternalSecureGroups.Administrator))
			{
				Header2.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
				LocRM.GetString("tClearErrorLog"),
				this.Page.ResolveUrl("~/Layouts/Images/delete.gif")),
				"javascript:ClearLog()");
			}
			Header2.AddLink(String.Format("<img width='16' height='16' title='{0}' border='0' align='top' src='{1}'/>&nbsp;{0}",
				LocRM.GetString("tAddTools"),
				this.Page.ResolveUrl("~/Layouts/Images/cancel.gif")),
				ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin8"));
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			dgeErrors.Columns[0].HeaderText = LocRM.GetString("tErrorId");
			dgeErrors.Columns[1].HeaderText = LocRM.GetString("tCreationTime");
		}
		#endregion

		#region BinddgErrors
		private void BinddgErrors()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("ErrorID");
			dt.Columns.Add("ErrorLink");
			dt.Columns.Add("ErrorPortal");
			dt.Columns.Add("CreationTime",typeof(DateTime));

			string path = (Server.MapPath(ResolveClientUrl("~/Admin/Log/Error/")));
			DirectoryInfo dir = new DirectoryInfo(path);
			foreach (FileInfo fileinfo in dir.GetFiles())
			{
				string ErrorLink	= fileinfo.Name;
				if (!ErrorLink.EndsWith(".html"))
					continue;
				string PureName		= ErrorLink.Substring(0,ErrorLink.Length-5);
				string ErrorID		= ErrorLink.Substring(PureName.Length-6,6);
				string ErrorPortal	= ErrorLink.Substring(0,PureName.Length-7);

				if (ErrorPortal == Request.Url.Host.Replace(".","_"))
				{
					DataRow dr = dt.NewRow();
					dr["ErrorID"] = String.Format("<a href='{2}/{0}' target=_blank>{1}</a>", ErrorLink, ErrorID, ResolveClientUrl("~/admin/log/error"));
					dr["ErrorLink"]		= ErrorLink;
					dr["ErrorPortal"]	= ErrorPortal;
					dr["CreationTime"] = fileinfo.CreationTime;//.ToShortDateString() + "&nbsp;" + fileinfo.CreationTime.ToShortTimeString();
					dt.Rows.Add(dr);
				}
			}
			
			DataView dv = dt.DefaultView;
			dv.Sort = "CreationTime DESC";
						
			dgeErrors.DataSource = dv;
			if (pc["ErrorLog_PageSize"]!=null)
				dgeErrors.PageSize = int.Parse(pc["ErrorLog_PageSize"].ToString());

			if (pc["ErrorLog_PageIndex"] != null)
			{
				int iPageIndex = int.Parse(pc["ErrorLog_PageIndex"].ToString());
				int ppi = dv.Count / dgeErrors.PageSize;
				if (dv.Count % dgeErrors.PageSize == 0)
					ppi = ppi - 1;
				if (iPageIndex <= ppi)
					dgeErrors.CurrentPageIndex = iPageIndex;
				else dgeErrors.CurrentPageIndex = 0;
			}
			dgeErrors.DataBind();
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgeErrors.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(dgeErrors_PageSizeChanged);
			this.dgeErrors.PageIndexChanged += new DataGridPageChangedEventHandler(dgeErrors_PageIndexChanged);
		}

		void dgeErrors_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			pc["ErrorLog_PageIndex"] = e.NewPageIndex.ToString();
			BinddgErrors();
		}

		void dgeErrors_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["ErrorLog_PageSize"] = e.NewPageSize.ToString();
			BinddgErrors();
		}
		#endregion

		#region ClearLogButton_Click
		protected void ClearLogButton_Click(object sender, EventArgs e)
		{
			DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Admin/Log/Error/"));
			foreach (FileInfo fileinfo in dir.GetFiles())
			{
				if (fileinfo.Name.EndsWith(".html"))
					fileinfo.Delete();
			}

			pc["ErrorLog_PageIndex"] = "0";
			BinddgErrors();
		}
		#endregion

	}


}
