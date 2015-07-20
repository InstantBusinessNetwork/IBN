using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects.Modules
{
	public partial class ActivitiesByMilestones2 : System.Web.UI.UserControl
	{
		public string iframeheight = "0";
		IFormatProvider culture = CultureInfo.InvariantCulture;
		private int _userId = Security.CurrentUser.UserID;
		private int _topProjectId = 0;

		#region Properties
		public string ProjectListData
		{
			get
			{
				if (pc["Report_ProjectListData"] != null)
					return pc["Report_ProjectListData"];
				else
					return "";
			}
			set
			{
				pc["Report_ProjectListData"] = value;
			}
		}

		public string ProjectListType
		{
			get
			{
				if (pc["Report_ProjectListType"] != null)
					return pc["Report_ProjectListType"];
				else
					return "All";
			}
			set
			{
				pc["Report_ProjectListType"] = value;
			}
		}

		private int BasePlan1
		{
			get
			{
				if (pc["ProjectsByBS_BasePlan1"] != null && pc["ProjectsByBS_BasePlan1"] != "-1")
					return int.Parse(pc["ProjectsByBS_BasePlan1"]);
				else
					return 0;
			}
			set
			{
				pc["ProjectsByBS_BasePlan1"] = value.ToString();
			}
		}

		private int BasePlan2
		{
			get
			{
				if (pc["ProjectsByBS_BasePlan2"] != null && pc["ProjectsByBS_BasePlan2"] != "-1" && pc["ProjectsByBS_BasePlan2"] != "0")
					return int.Parse(pc["ProjectsByBS_BasePlan2"]);
				else
					return -2;
			}
			set
			{
				pc["ProjectsByBS_BasePlan2"] = value.ToString();
			}
		}
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", System.Reflection.Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Calendar.Resources.strGanttView", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Gantt.css");
			UtilHelper.RegisterScript(Page, "~/Scripts/ganttScript2.js");

			if (!Page.IsPostBack)
			{
				BindInitialValues();
				BindDataGrid();
			}
			BindTitles();
		}

		#region BindLegendTable
		private void BindLegendTable()
		{
			tblLegend.Rows.Clear();

			TableRow tr0 = new TableRow();
			TableCell tc01 = new TableCell();
			TableCell tc02 = new TableCell();
			TableCell tc03 = new TableCell();

			tc01.HorizontalAlign = HorizontalAlign.Center;
			tc02.HorizontalAlign = HorizontalAlign.Center;
			tc01.Text = LocRM2.GetString("tActive");
			tc02.Text = LocRM2.GetString("tCompleted");
			tr0.Cells.Add(tc01);
			tr0.Cells.Add(tc02);
			tr0.Cells.Add(tc03);
			tblLegend.Rows.Add(tr0);

			bool isBasePlanSlot = (BasePlan2 > 0);
			foreach (GanttItem gItem in GanttManager.GetLegendItems(true, isBasePlanSlot))
			{
				string item = gItem.ToString();
				TableRow tr = new TableRow();
				TableCell tc1 = new TableCell();
				TableCell tc2 = new TableCell();
				TableCell tc3 = new TableCell();
				tc1.VerticalAlign = VerticalAlign.Top;
				tc2.VerticalAlign = VerticalAlign.Top;
				tc3.VerticalAlign = VerticalAlign.Top;
				Image img = new Image();
				img.ImageUrl = String.Format("{0}?ganttItem={1}&Completed={2}",
					ResolveUrl("~/Common/GanttLegendPortion.aspx"), item, "0");
				tc1.Controls.Add(img);
				Image imgC = new Image();
				imgC.ImageUrl = String.Format("{0}?ganttItem={1}&Completed={2}",
					ResolveUrl("~/Common/GanttLegendPortion.aspx"), item, "1");
				tc2.Controls.Add(imgC);
				Label lbl = new Label();
				lbl.Text = LocRM2.GetString(item);
				tc3.Controls.Add(lbl);
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				tr.Cells.Add(tc3);
				tblLegend.Rows.Add(tr);
			}
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, System.EventArgs e)
		{
			BindLegendTable();

			int lx = -1;
			if (pc["GantChartMilestones_Lx_New"] != null)
				lx = int.Parse(pc["GantChartMilestones_Lx_New"]);
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "f_onload(" + lx + ", " + GanttManager.PortionWidth + ");", true);
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
			this.btnApplyFilter.ServerClick += new EventHandler(btnApplyFilter_ServerClick);
			this.grdTasks.PageIndexChanged += new DataGridPageChangedEventHandler(grdTasks_PageIndexChanged);
			this.grdTasks.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdTasks_PageSizeChanged);
			this.grdTasks.ItemCommand += new DataGridCommandEventHandler(grdTasks_ItemCommand);
			this.lbToday.Click += new EventHandler(lbToday_Click);

		}
		#endregion

		#region BindInitialValues
		private void BindInitialValues()
		{
			ddPrjGroup.DataSource = ProjectGroup.GetProjectGroups();
			ddPrjGroup.DataTextField = "Title";
			ddPrjGroup.DataValueField = "ProjectGroupId";
			ddPrjGroup.DataBind();
			ddPrjGroup.Items.Insert(0, new ListItem(LocRM.GetString("AllFem"), "0"));
			ddPrjGroup.Items.Add(new ListItem(LocRM.GetString("tCustom"), "-1"));
			switch (ProjectListType)
			{
				case "All":
					if (ddPrjGroup.Items.FindByValue("0") != null)
						ddPrjGroup.SelectedValue = "0";
					break;
				case "Custom":
					if (ddPrjGroup.Items.FindByValue("-1") != null)
						ddPrjGroup.SelectedValue = "-1";
					break;
				case "Portfolio":
					if (ddPrjGroup.Items.FindByValue(ProjectListData) != null)
						ddPrjGroup.SelectedValue = ProjectListData;
					break;
			}

			DataTable dt1 = new DataTable();
			dt1.Columns.Add(new DataColumn("Id", typeof(int)));
			dt1.Columns.Add(new DataColumn("Name", typeof(string)));
			foreach (BasePlanSlot bps in BasePlanSlot.List())
			{
				DataRow dr = dt1.NewRow();
				dr["Id"] = bps.BasePlanSlotId;
				dr["Name"] = bps.Name;
				dt1.Rows.Add(dr);
			}
			dt1.Rows.Add(new object[] { "-2", LocRM.GetString("tNotSelected") });
			DataView dv = dt1.DefaultView;
			dv.Sort = "Id ASC";
			ddBasePlan.DataSource = dv;
			ddBasePlan.DataTextField = "Name";
			ddBasePlan.DataValueField = "Id";
			ddBasePlan.DataBind();
			ddBasePlan.SelectedValue = BasePlan2.ToString();

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			foreach (BasePlanSlot bps in BasePlanSlot.List())
			{
				DataRow dr = dt.NewRow();
				dr["Id"] = bps.BasePlanSlotId;
				dr["Name"] = bps.Name;
				dt.Rows.Add(dr);
			}
			dt.Rows.Add(new object[] { "0", LocRM.GetString("tCurrent") });
			dv = dt.DefaultView;
			dv.Sort = "Id ASC";
			ddOrBasePlan.DataSource = dv;
			ddOrBasePlan.DataTextField = "Name";
			ddOrBasePlan.DataValueField = "Id";
			ddOrBasePlan.DataBind();
			ddOrBasePlan.SelectedValue = BasePlan1.ToString();
		}
		#endregion

		#region BindTitles
		private void BindTitles()
		{
			secHeader.Title = LocRM.GetString("tPrjsByMilestones");
			btnApplyFilter.Text = LocRM.GetString("Apply");
			btnApplyFilter.CustomImage = this.Page.ResolveUrl("~/Layouts/Images/accept.gif");
			lbToday.Text = LocRM.GetString("tToday");
			secHeader.AddLink("<img alt='' id='_imgLegendId' src='../Layouts/Images/pin_off.gif'/> " + LocRM2.GetString("tLegend"), "javascript:ShowLegend('_imgLegendId');");
		}
		#endregion

		#region btnApplyFilter_ServerClick
		private void btnApplyFilter_ServerClick(object sender, EventArgs e)
		{
			switch (ddPrjGroup.SelectedValue)
			{
				case "0":
					ProjectListType = "All";
					break;
				case "-1":
					ProjectListType = "Custom";
					break;
				default:
					ProjectListType = "Portfolio";
					ProjectListData = ddPrjGroup.SelectedValue;
					break;
			}
			BasePlan2 = int.Parse(ddBasePlan.SelectedValue);
			BasePlan1 = int.Parse(ddOrBasePlan.SelectedValue);
			BindDataGrid();
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			DataTable dt = GanttManager.GetAnalysisDataTable(BasePlan1, BasePlan2);
			if (dt != null && dt.Rows.Count > 0)
			{
				_topProjectId = (int)dt.Rows[0]["ProjectId"];

				if (pc["ProjectsByMS_PageSize"] == null)
					pc["ProjectsByMS_PageSize"] = "10";

				grdTasks.PageSize = int.Parse(pc["ProjectsByMS_PageSize"]);


				int pageindex = 0;
				int ppi = dt.Rows.Count / grdTasks.PageSize;
				if (pc["ProjectsByMS_Page"] != null)
				{
					pageindex = int.Parse(pc["ProjectsByMS_Page"]);

					if (dt.Rows.Count % grdTasks.PageSize == 0)
						ppi = ppi - 1;

					if (pageindex <= ppi)
						grdTasks.CurrentPageIndex = pageindex;
					else grdTasks.CurrentPageIndex = 0;
				}

				int fh = dt.Rows.Count % grdTasks.PageSize;
				if (pageindex == ppi && fh == 0)
					fh = grdTasks.PageSize * 19 + 29;
				else if
					(pageindex < ppi)
					fh = grdTasks.PageSize * 19 + 29;
				else
					fh = fh * 19 + 29;

				iframeheight = fh.ToString();

				grdTasks.DataSource = dt.DefaultView;
				grdTasks.Columns[3].HeaderText = LocRM.GetString("tProjectMilestone");
				grdTasks.DataBind();
				foreach (DataGridItem dgi in grdTasks.Items)
				{
					ImageButton ib = (ImageButton)dgi.FindControl("ibProjectStart");
					if (ib != null)
					{
						ib.Attributes.Add("title", LocRM.GetString("tProjectStartDate"));
					}
					ib = (ImageButton)dgi.FindControl("ibProjectEnd");
					if (ib != null)
					{
						ib.Attributes.Add("title", LocRM.GetString("tProjectEndDate"));
					}
				}

				divImg.Style.Add("HEIGHT", iframeheight + "px");
				hdnLinkToGantt.Value = String.Format("../Projects/GanttReportImage2.aspx?PageItems={0}&PageNumber={1}&Days=366&RenderPortion=1&BasePlanSlotId={2}&OriginalPlanSlotId={3}", grdTasks.PageSize, grdTasks.CurrentPageIndex, ddBasePlan.SelectedValue, ddOrBasePlan.SelectedValue);
			}
		}
		#endregion

		#region Events
		private void grdTasks_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			pc["ProjectsByMS_Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void grdTasks_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["ProjectsByMS_PageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}

		private void grdTasks_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Collapse")
			{
				string s = pc["Report_CollapsedProjectsList"];
				if (s != null && s.Length > 0)
				{
					if (s[s.Length - 1] == ';')
						s += e.CommandArgument.ToString();
					else
						s += ";" + e.CommandArgument.ToString();
					pc["Report_CollapsedProjectsList"] = s;
				}
				else
					pc["Report_CollapsedProjectsList"] = e.CommandArgument.ToString();
				BindDataGrid();
			}
			if (e.CommandName == "Expand")
			{
				string s = pc["Report_CollapsedProjectsList"];
				if (s == e.CommandArgument.ToString())
				{
					s = string.Empty;
				}
				else
				{
					int index = s.IndexOf(e.CommandArgument.ToString() + ";");
					if (index >= 0)
					{
						s = s.Remove(index, e.CommandArgument.ToString().Length + 1);
					}
					else
					{
						index = s.IndexOf(";" + e.CommandArgument.ToString());
						if (index >= 0)
							s = s.Remove(index, e.CommandArgument.ToString().Length + 1);
					}
				}
				pc["Report_CollapsedProjectsList"] = s;
				BindDataGrid();
			}
			if (e.CommandName == "ProjectStart")
			{
				DataTable dt = GanttManager.GetAnalysisDataTable(BasePlan1, BasePlan2);
				if (dt.Rows.Count > 0)
				{
					_topProjectId = (int)dt.Rows[0]["ProjectId"];
				}
				int projectId = int.Parse(e.CommandArgument.ToString());
				DateTime zeroPoint = DateTime.Now;
				using (IDataReader reader = Project.GetProject(projectId))
				{
					if (reader.Read())
					{
						if (reader["TargetStartDate"] != DBNull.Value)
							zeroPoint = (DateTime)reader["TargetStartDate"];
					}
				}
				pc["GantChartMilestones_Lx_New"] = (GanttManager.GetPortionX(_topProjectId, zeroPoint)).ToString();
				Response.Redirect("../Projects/ProjectsByMilestones.aspx", true);
			}
			if (e.CommandName == "ProjectEnd")
			{
				DataTable dt = GanttManager.GetAnalysisDataTable(BasePlan1, BasePlan2);
				if (dt.Rows.Count > 0)
				{
					_topProjectId = (int)dt.Rows[0]["ProjectId"];
				}
				int projectId = int.Parse(e.CommandArgument.ToString());
				DateTime zeroPoint = DateTime.Now;
				using (IDataReader reader = Project.GetProject(projectId))
				{
					if (reader.Read())
					{
						if (reader["TargetFinishDate"] != DBNull.Value)
							zeroPoint = (DateTime)reader["TargetFinishDate"];
					}
				}
				pc["GantChartMilestones_Lx_New"] = (GanttManager.GetPortionX(_topProjectId, zeroPoint) - 1).ToString();
				Response.Redirect("../Projects/ProjectsByMilestones.aspx", true);
			}
		}

		private void lbToday_Click(object sender, EventArgs e)
		{
			DataTable dt = GanttManager.GetAnalysisDataTable(BasePlan1, BasePlan2);
			if (dt.Rows.Count > 0)
			{
				_topProjectId = (int)dt.Rows[0]["ProjectId"];
			}
			pc["GantChartMilestones_Lx_New"] = (GanttManager.GetPortionX(_topProjectId, UserDateTime.UserToday)).ToString();
			Response.Redirect("../Projects/ProjectsByMilestones.aspx", true);
		}
		#endregion
	}
}
