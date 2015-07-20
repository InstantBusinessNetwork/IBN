namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using System.Resources;

	/// <summary>
	///		Summary description for ProjectHistory.
	/// </summary>
	public partial class ProjectHistory : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

    ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectReports", typeof(ProjectHistory).Assembly);
		private int ProjectId
		{
			get 
			{
				try
				{
					return int.Parse(Request["ProjectID"]);
				}
				catch
				{
					throw new Exception("Invalid Project ID");
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if(!Page.IsPostBack)
			{
				pc["History_Page"] = "0";
				if(pc["ShowHistoryTable"]==null)
					pc["ShowHistoryTable"] = "Hide";
				if(pc["HistoryPageSize"]==null)
					pc["HistoryPageSize"] = "10";
			}
		}

		protected void Page_PreRender(object sender,System.EventArgs e)
		{
			BindToolBar();
			if(pc["ShowHistoryTable"].ToString() != "Hide")
				trHistory.Style.Add("display","block");
			else
				trHistory.Style.Add("display","none");
			if(pc["ShowHistoryTable"].ToString() == "3Items")
			{
				pc["HistoryPageSize"] = "3";
				pc["History_Page"] = "0";
			}
			else
			{
				pc["HistoryPageSize"] = "10";
			}
			BindData();
		}

		#region BindToolBar
		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("tProjectSnapshots");
			if (pc["ShowHistoryTable"].ToString()!="Hide")
				secHeader.AddLink("<img src='../Layouts/Images/b1.gif' border='0' width='16' height='16' align='absmiddle'>",Page.ClientScript.GetPostBackClientHyperlink(lbHideTable,String.Empty));
			if (pc["ShowHistoryTable"].ToString()!="3Items")
				secHeader.AddLink("<img src='../Layouts/Images/b3.gif' border='0' width='16' height='16' align='absmiddle'>",Page.ClientScript.GetPostBackClientHyperlink(lb3Items,String.Empty));
			if (pc["ShowHistoryTable"].ToString()!="Show")
				secHeader.AddLink("<img src='../Layouts/Images/b2.gif' border='0' width='16' height='16' align='absmiddle'>",Page.ClientScript.GetPostBackClientHyperlink(lbShowTable,String.Empty));
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			dgHistory.Columns[1].HeaderText = LocRM.GetString("tCreated");
			dgHistory.Columns[2].HeaderText = LocRM.GetString("tCreatedBy");
			dgHistory.Columns[3].HeaderText = LocRM.GetString("tStatus");
			dgHistory.Columns[4].HeaderText = LocRM.GetString("tPrjPhase");
			dgHistory.Columns[5].HeaderText = LocRM.GetString("tTargetBudget");
			dgHistory.Columns[6].HeaderText = LocRM.GetString("tEstimatedBudget");
			dgHistory.Columns[7].HeaderText = LocRM.GetString("tActualBudget");
			dgHistory.Columns[8].HeaderText = LocRM.GetString("tStartDate");
			dgHistory.Columns[9].HeaderText = LocRM.GetString("tFinishDate");
			dgHistory.Columns[10].HeaderText = LocRM.GetString("tTargetFinishDate");
		}
		#endregion

		private void BindData()
		{
			DataTable dt = Project.GetListProjectSnapshots(ProjectId);
			DataView dv = dt.DefaultView;
			dv.Sort = "CreationDate Desc";
			dgHistory.PageSize = int.Parse(pc["HistoryPageSize"].ToString());
			if(dgHistory.PageSize < 10)
				dgHistory.PagerStyle.Visible = false;
			else
				dgHistory.PagerStyle.Visible = true;
			dgHistory.DataSource  =  dv;
			if (pc["History_Page"]!=null)
			{
				int pageindex = int.Parse(pc["History_Page"].ToString());
				int ppi = dt.Rows.Count / dgHistory.PageSize;
				if  (dt.Rows.Count % dgHistory.PageSize == 0)
					ppi = ppi - 1;

				if (pageindex <= ppi)
					dgHistory.CurrentPageIndex = pageindex;
				else dgHistory.CurrentPageIndex = 0;
			}
			dgHistory.DataBind();
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
			this.dgHistory.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageIndexChanged);			
		}
		#endregion

		private void dg_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["History_Page"] = e.NewPageIndex.ToString();
		}

		protected void lbHideTable_Click(object sender, System.EventArgs e)
		{
			pc["ShowHistoryTable"] = "Hide";
		}

		protected void lbShowTable_Click(object sender, System.EventArgs e)
		{
			pc["ShowHistoryTable"] = "Show";
		}

		protected void lb3Items_Click(object sender, System.EventArgs e)
		{
			pc["ShowHistoryTable"] = "3Items";
		}
	}
}
