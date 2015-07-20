namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for MailIncidentsList.
	/// </summary>
	public partial  class MailIncidentsList : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strMailIncidentsList", typeof(MailIncidentsList).Assembly);
		protected UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region MailBoxId
		private int MailBoxId
		{
			get
			{
				if (Request["MailBoxId"] != null)
					return int.Parse(Request["MailBoxId"]);
				else
					return 0;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(pc["MailInc_SortColumn"] == null)
				pc["MailInc_SortColumn"] = "Received DESC";

			if (Request["Export"] == "1")
				ExportGrid();
			else 
				if (Request["Export"] == "2")
				ExportXMLTable();
			else
				BindDataGrid();

			BindToolbar();
		}

		private void BindToolbar()
		{
			if(MailBoxId > 0 && this.Parent.Parent is IPageViewMenu)
			{
				PageViewMenu secHeader = (PageViewMenu)((IPageViewMenu)this.Parent.Parent).GetToolBar();
				if(secHeader!=null)
				{
					using (IDataReader reader = IssueRequest.GetMailBoxById(MailBoxId))
					{
						if (reader.Read())
							secHeader.Title =  LocRM.GetString("Box") + " - " + reader["Name"].ToString();
					}
				}
			}
		}

		#region ExportGrid
		private void ExportGrid()
		{
			BindDataGrid(false);
			dgIncidents.AllowSorting = false;
			dgIncidents.ShowFooter = false;
			dgIncidents.AllowPaging = false;

			dgIncidents.Columns[8].Visible = false;
			dgIncidents.Columns[1].Visible = false;
			dgIncidents.Columns[3].Visible = false;

			dgIncidents.Columns[2].Visible = true;
			dgIncidents.Columns[4].Visible = true;

			dgIncidents.Columns[6].ItemStyle.Width = 150;
			dgIncidents.Columns[7].ItemStyle.Width = 150;

			foreach (DataGridColumn dgc in dgIncidents.Columns)
			{
				dgc.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
				dgc.HeaderStyle.Font.Bold = true;
			}

			dgIncidents.DataBind();

			foreach (DataGridItem dgi in dgIncidents.Items)
			{
				dgi.Cells[7].Text = DateTime.Parse(dgi.Cells[7].Text).ToString("yyyy-MM-dd");
			}

			CommonHelper.ExportExcel(dgIncidents,"MailIssues.xls",null);

		}
		#endregion

		#region ExportXMLTable
		private void ExportXMLTable()
		{
			CommonHelper.SaveXML(BindDataGrid(false),"MailIssues.xml");
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			BindDataGrid(true);
		}

		private DataTable BindDataGrid(bool performbind)
		{
			dgIncidents.Columns[1].HeaderText = LocRM.GetString("Title");
			dgIncidents.Columns[2].HeaderText = LocRM.GetString("Title");
			dgIncidents.Columns[3].HeaderText = LocRM.GetString("Sender");
			dgIncidents.Columns[4].HeaderText = LocRM.GetString("Sender");
			dgIncidents.Columns[5].HeaderText = LocRM.GetString("Type");
			dgIncidents.Columns[6].HeaderText = LocRM.GetString("Priority");
			dgIncidents.Columns[7].HeaderText = LocRM.GetString("Received");
			foreach(DataGridColumn dgc in dgIncidents.Columns)
			{
				if(dgc.SortExpression == pc["MailInc_SortColumn"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='../layouts/images/upbtnF.jpg'/>";
				else if(dgc.SortExpression + " DESC" == pc["MailInc_SortColumn"].ToString())
					dgc.HeaderText += "&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='../layouts/images/downbtnF.jpg'/>";
			}

			DataTable dt =  IssueRequest.GetByMailBoxDataTable(MailBoxId);

			DataView dv = new DataView(dt);
			
			try
			{
				dv.Sort = pc["MailInc_SortColumn"];
			}
			catch
			{
				pc["MailInc_SortColumn"] = "Received DESC";
				dv.Sort = pc["MailInc_SortColumn"];
			}

			if (pc["MailInc_PageSize"]!=null)
				dgIncidents.PageSize = int.Parse(pc["MailInc_PageSize"]);

			if (pc["MailInc_Page"]!=null)
			{
				int iPageIndex = int.Parse(pc["MailInc_Page"]);
				int ppi = dt.Rows.Count / dgIncidents.PageSize;
				if  (dt.Rows.Count % dgIncidents.PageSize == 0)
					ppi = ppi - 1;
				if (iPageIndex <= ppi)
					dgIncidents.CurrentPageIndex = iPageIndex;
				else dgIncidents.CurrentPageIndex = 0;
			}

			dgIncidents.DataSource = dv;

			if (performbind)
				dgIncidents.DataBind();

			foreach (DataGridItem dgi in dgIncidents.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib!=null)
					ib.Attributes.Add("onclick","return confirm('"+ LocRM.GetString("Warning") +"')");
			}
			return dt;
		}
		#endregion

		#region GetSenderType
		protected string GetSenderType(int type)
		{
			switch(type) {
			case 0:
				return LocRM.GetString("Unknown");
			case 1:
				return LocRM.GetString("External");
			default:
				return LocRM.GetString("General");
			}
		}
		#endregion

		#region GetSender
		protected string GetSender(string email)
		{
			int UserId = User.GetUserByEmail(email);
			if (UserId==-1)
			{
				return String.Format("<a href='mailto:{0}'>{0}</a>",email);
			}
			else
			{
				return CommonHelper.GetUserStatus(UserId);
			}
		}
		#endregion

		#region DataGrid events
		private void dgProjects_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["MailInc_PageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}

		private void dgIncidents_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["MailInc_Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void dgIncidents_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if((pc["MailInc_SortColumn"] != null) && (pc["MailInc_SortColumn"].ToString() == (string)e.SortExpression))
				pc["MailInc_SortColumn"] = (string)e.SortExpression + " DESC";
			else
				pc["MailInc_SortColumn"] = (string)e.SortExpression;
			BindDataGrid();		
		}

		private void dgIncidents_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int pop3MailRequestId = int.Parse(e.CommandArgument.ToString());
			IssueRequest.Delete(pop3MailRequestId);
			BindDataGrid();
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
			this.dgIncidents.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgIncidents_DeleteCommand);
			this.dgIncidents.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgIncidents_PageIndexChanged);
			this.dgIncidents.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgIncidents_SortCommand);
			this.dgIncidents.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgProjects_PageSizeChange);
		}
		#endregion
	}
}
