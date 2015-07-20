namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using System.Reflection;

	/// <summary>
	///		Summary description for LdapLogs.
	/// </summary>
	public partial class LdapLogs : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(pc["ad_Logs_Sort"]==null)
				pc["ad_Logs_Sort"] = "Dt DESC";
			if(!Page.IsPostBack)
				BindDG();
			BindToolbar();
		}

		#region BindDG
		private void BindDG()
		{
			int i = 2;
			dgLogs.Columns[i++].HeaderText = LocRM.GetString("tSynchDate");
			dgLogs.Columns[i++].HeaderText = LocRM.GetString("tTitle");
			dgLogs.Columns[i++].HeaderText = LocRM.GetString("tUserCount");
			foreach(DataGridColumn dgc in dgLogs.Columns)
			{
				if(dgc.SortExpression == pc["ad_Logs_Sort"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='" + ResolveUrl("~/layouts/images/upbtnF.jpg") + "'/>";
				else if(dgc.SortExpression + " DESC" == pc["ad_Logs_Sort"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='" + ResolveUrl("~/layouts/images/downbtnF.jpg") + "'/>";
			}

			DataView dv = Mediachase.IBN.Business.Ldap.SyncLogGetList(0).DefaultView;
			if(pc["ad_Logs_Sort"]!=null)
				dv.Sort = pc["ad_Logs_Sort"].ToString();

			dgLogs.DataSource = dv;
			
			if (pc["ad_Logs_PageSize"]!=null)
				dgLogs.PageSize = int.Parse(pc["ad_Logs_PageSize"].ToString());

			if (pc["ad_Logs_Page"]!=null)
			{
				int iPageIndex = int.Parse(pc["ad_Logs_Page"].ToString());
				int ppi = dv.Count / dgLogs.PageSize;
				if  (dv.Count % dgLogs.PageSize == 0)
					ppi = ppi - 1;
				if (iPageIndex <= ppi)
					dgLogs.CurrentPageIndex = iPageIndex;
				else dgLogs.CurrentPageIndex = 0;
			}
			dgLogs.DataBind();

			foreach (DataGridItem dgi in dgLogs.Items)
			{
				ImageButton ib=(ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
					ib.Attributes.Add("onclick","return confirm('"+LocRM.GetString("tWarningDelLog")+"')");
			}
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tLDAPLogs");
			secHeader.AddLink("<img width='16' height='16' title='"+LocRM.GetString("tDeleteChecked")+"' border='0' align='top' src='" +
				ResolveUrl("~/Layouts/Images/delete.gif") + "'/>&nbsp;" + LocRM.GetString("tDeleteChecked"), "javascript:ActionDelete()");
			secHeader.AddLink("<img width='16' height='16' title='"+LocRM.GetString("tDeleteEmpty")+"' border='0' align='top' src='" +
				ResolveUrl("~/Layouts/Images/delete.gif") + "'/>&nbsp;" + LocRM.GetString("tDeleteEmpty"), "javascript:ActionDelete2()");
		}
		#endregion

		protected string GetDate(object date)
		{
			string retVal = "";
			if(date!=DBNull.Value && ((DateTime)date).Year>1)
				retVal = ((DateTime)date).ToString("g");
			return retVal;
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
			this.dgLogs.DeleteCommand += new DataGridCommandEventHandler(dg_DeleteCommand);
			this.dgLogs.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageIndexChanged);
			this.dgLogs.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChange);
			this.dgLogs.SortCommand += new DataGridSortCommandEventHandler(dg_SortCommand);
			this.lbDeleteChecked.Click += new EventHandler(lbDeleteChecked_Click);
			this.lbDeleteEmpty.Click += new EventHandler(lbDeleteEmpty_Click);
		}
		#endregion

		#region DG Events
		private void dg_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if((pc["ad_Logs_Sort"] != null) && (pc["ad_Logs_Sort"].ToString() == (string)e.SortExpression))
				pc["ad_Logs_Sort"] = (string)e.SortExpression + " DESC";
			else
				pc["ad_Logs_Sort"] = (string)e.SortExpression;
			BindDG();
		}

		private void dg_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int sid = int.Parse(e.Item.Cells[0].Text);
			Ldap.SyncLogDelete(sid);
			Response.Redirect("~/Admin/LdapLogs.aspx");
		}

		private void dg_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["ad_Logs_PageSize"] = e.NewPageSize.ToString();
			BindDG();
		}

		private void dg_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["ad_Logs_Page"] = e.NewPageIndex.ToString();
			BindDG();
		}
		#endregion

		#region Delete_Checked
		private void lbDeleteChecked_Click(object sender, EventArgs e)
		{
			string sIds = hidForDelete.Value;
			ArrayList alIds = new ArrayList();
			while(sIds.Length>0)
			{
				alIds.Add(int.Parse(sIds.Substring(0,sIds.IndexOf(","))));
				sIds = sIds.Remove(0,sIds.IndexOf(",")+1);
			}
			Ldap.SyncLogDelete(alIds);
			Response.Redirect("~/Admin/LdapLogs.aspx");
		}
		#endregion

		#region Delete Empty
		private void lbDeleteEmpty_Click(object sender, EventArgs e)
		{
			Ldap.SyncLogDeleteWithoutChanges();
			Response.Redirect("~/Admin/LdapLogs.aspx");
		}
		#endregion
	}
}
