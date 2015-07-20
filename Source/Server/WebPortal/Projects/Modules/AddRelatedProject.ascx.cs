namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Data.Common;
	using System.Collections;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using System.Globalization;
	using System.Resources;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Data;

	/// <summary>
	///		Summary description for AddRelatedProject.
	/// </summary>
	public partial class AddRelatedProject : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(AddRelatedProject).Assembly);
		private string _sortColumn="Title";
		private UserLightPropertyCollection pc;

		private int ProjectId
		{
			get
			{
				try 
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		private int PGID
		{
			get
			{
				try 
				{
					return int.Parse(Request["PrjGroupId"]);
				}
				catch
				{
					return -1;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			pc =  Security.CurrentUser.Properties;
			if (!Page.IsPostBack)
			{
				if (pc["RelPrjSort"]==null)
					pc["RelPrjSort"] = _sortColumn;
				if (pc["RelPrjPageSize"]==null)
					pc["RelPrjPageSize"] = "10";
				if (pc["RelPrjPage"]==null)
					pc["RelPrjPage"] = "1";
			}
			BindDataGrid();
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			if(ProjectId>0)
			{
				secHeader.Title = LocRM.GetString("tAddRelProject") + "'"+Task.GetProjectTitle(ProjectId)+"'";
				secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tCancel"),"../Projects/ProjectView.aspx?ProjectId=" + ProjectId);
			}
			else
			{
				string sName = "";
				using(IDataReader reader = ProjectGroup.GetProjectGroups(PGID))
				{
					if(reader.Read())
					{
						sName = reader["Title"].ToString();
					}
				}
				secHeader.Title = LocRM.GetString("tAddToPortfolio") + " '"+sName+"'";
				secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tCancel"),"../Projects/ProjectGroupView.aspx?ProjectGroupId=" + PGID);
			}
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			DataTable dt = Project.GetListProjectsByFilterDataTable("", 0, 0, 0, -1, PrimaryKeyId.Empty, PrimaryKeyId.Empty, 0, DateTime.Now, 0, DateTime.Now, 0, 0, 0, 0, 0, 0, false);
			DataView dv = dt.DefaultView;
			DataTable dtAlready = new DataTable();
			if (ProjectId>0)
				dtAlready = Project.GetListProjectRelationsDataTable(ProjectId);
			else
				dtAlready = Project.GetListProjectsByFilterDataTable("", 0, 0, 0, -1, PrimaryKeyId.Empty, PrimaryKeyId.Empty, 0, DateTime.Now, 0, DateTime.Now, 0, 0, 0, 0, -PGID, 0, false);
			ArrayList alPrjs = new ArrayList();
			foreach(DataRow dr in dtAlready.Rows)
			{
				alPrjs.Add((int)dr["ProjectId"]);
			}
			string sfilter = "(ProjectId <> "+ProjectId+")";
			foreach(int prj_id in alPrjs)
				sfilter += "AND(ProjectId <> "+prj_id+")";
			dv.RowFilter = sfilter;
			dv.Sort = pc["RelPrjSort"];

			dgProjects.DataSource = dt.DefaultView;
			if (pc["RelPrjPageSize"]!=null)
				dgProjects.PageSize = int.Parse(pc["RelPrjPageSize"]);
			if (pc["RelPrjPage"]!=null)
			{
				int pageindex = int.Parse(pc["RelPrjPage"]);
				int ppi = dv.Count / dgProjects.PageSize;
				if  (dv.Count % dgProjects.PageSize == 0)
					ppi = ppi - 1;

				if (pageindex <= ppi)
					dgProjects.CurrentPageIndex = pageindex;
				else dgProjects.CurrentPageIndex = 0;
			}
			dgProjects.DataBind();
			foreach (DataGridItem dgi in dgProjects.Items)
			{
				HyperLink hl=(HyperLink)dgi.FindControl("ibAdd");
				if (hl!=null)
				{
					hl.ToolTip = LocRM.GetString("tAdd");
					if (PGID>0)
					{
						hl.ImageUrl = "../../layouts/images/newitem.GIF";
					}
				}
			}
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
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgProjects.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgProjects_PageChange);
			this.dgProjects.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgProjects_Sort);
			this.dgProjects.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgProjects_PageSizeChange);			
		}
		#endregion

		protected void Add_Click(object sender, System.EventArgs e)
		{
			int RelProjectId = int.Parse(hdnProjectId.Value);
			if (ProjectId>0)
			{
				Project2.AddRelation(ProjectId, RelProjectId);
				Response.Redirect("../Projects/ProjectView.aspx?Tab=1&ProjectId="+ProjectId);
			}
			else
			{
				ProjectGroup.AssignProjectGroup(RelProjectId, PGID);
				Response.Redirect("../Projects/ProjectGroupView.aspx?ProjectGroupId=" + PGID);
			}
		}

		#region DataGrid Events
		private void dgProjects_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["RelPrjPage"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void dgProjects_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["RelPrjPageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}

		private void dgProjects_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if(pc["RelPrjSort"] !=null && pc["RelPrjSort"].ToString() == (string)e.SortExpression)
				_sortColumn = (string)e.SortExpression + " DESC";
			else
				_sortColumn = (string)e.SortExpression;

			pc["RelPrjSort"] = _sortColumn;
			BindDataGrid();
		}
		#endregion

	}
}
