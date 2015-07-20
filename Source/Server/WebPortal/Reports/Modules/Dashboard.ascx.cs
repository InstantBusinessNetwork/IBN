using System;
using System.Data;
using System.Resources;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;


namespace Mediachase.UI.Web.Reports.Modules
{
	/// <summary>
	///		Summary description for Dashboard.
	/// </summary>
	public partial class Dashboard : System.Web.UI.UserControl
	{
		//protected string grdMain_SortColumn = "TemplateName";

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strReports", typeof(Dashboard).Assembly);
		protected ResourceManager LocRM1 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strXMLReport", typeof(Dashboard).Assembly);
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Configuration.ProjectManagementEnabled && !Configuration.HelpDeskEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			ApplyLocalization();
			if (pc["CRep_Sorting"] == null)
				pc["CRep_Sorting"] = "TemplateName";
			if (!IsPostBack)
			{
				BindLists();
				BindValues();
				BindGraph();
				if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
				{
					trCustomReps.Visible = true;
					BindDG();
				}
				else
					trCustomReps.Visible = false;
			}
			BindToolBar();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			grdMain.Columns[2].HeaderText = LocRM1.GetString("s6ReportName");
			//grdMain.Columns[3].HeaderText = LocRM1.GetString("tType");
			grdMain.Columns[3].HeaderText = LocRM1.GetString("tCreator");
			grdMain.Columns[4].HeaderText = LocRM1.GetString("tCreateDate");
			grdMain.Columns[5].HeaderText = LocRM1.GetString("tModifier");
			grdMain.Columns[6].HeaderText = LocRM1.GetString("tModifiedDate");
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			secHeader.Title = LocRM.GetString("Dashboard");
			secPieChart.AddText(LocRM.GetString("tCharting"));
			secReports.AddText(LocRM.GetString("tAvailCustRep"));
		}
		#endregion

		#region BindLists
		private void BindLists()
		{
			if (Configuration.ProjectManagementEnabled)
				ddPerspect.Items.Add(new ListItem(LocRM.GetString("Projects"), "0"));
			else
				pc["Dashboard_Perspect"] = "1";

			if (Configuration.HelpDeskEnabled)
				ddPerspect.Items.Add(new ListItem(LocRM.GetString("Issues"), "1"));
			else
				pc["Dashboard_Perspect"] = "0";

			if (pc["Dashboard_Perspect"] != null)
				ddPerspect.SelectedValue = pc["Dashboard_Perspect"].ToString();

			rbChartType.Items.Add(new ListItem(LocRM.GetString("tPieChart"), "0"));
			rbChartType.Items.Add(new ListItem(LocRM.GetString("tBarGraph"), "1"));
			if (pc["Dashboard_GraphType"] != null)
				rbChartType.SelectedIndex = int.Parse(pc["Dashboard_GraphType"].ToString());
			else
				rbChartType.SelectedIndex = 0;

			BindChartView();
		}
		#endregion

		#region BindChartView
		private void BindChartView()
		{
			ddChartView.Items.Clear();
			switch (ddPerspect.SelectedValue)
			{
				case "0":	//Project
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Status"), "Dash_Prj_Stat"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Priority"), "Dash_Prj_Prior"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Type"), "Dash_Prj_Typ"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Manager"), "Dash_Prj_Man"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("GenCat"), "Dash_Prj_GenCat"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("PrjCat"), "Dash_Prj_PrjCat"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Portfolio"), "Dash_Prj_PrjGrp"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Phase"), "Dash_Prj_Phas"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Client"), "Dash_Prj_Clnt"));
					break;
				case "1":	//Issue
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Status"), "Dash_Iss_Stat"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Priority"), "Dash_Iss_Prior"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Type"), "Dash_Iss_Typ"));
					if (Configuration.ProjectManagementEnabled)
						ddChartView.Items.Add(new ListItem(LocRM.GetString("Project"), "Dash_Iss_Prj"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("tIncBox"), "Dash_Iss_Man"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("Severity"), "Dash_Iss_Sev"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("GenCat"), "Dash_Iss_GenCat"));
					ddChartView.Items.Add(new ListItem(LocRM.GetString("IssCat"), "Dash_Iss_IssCat"));
					break;
				default:
					break;
			}
			if (pc["Dashboard_GraphView"] != null && ddChartView.Items.FindByValue(pc["Dashboard_GraphView"].ToString()) != null)
				ddChartView.SelectedValue = pc["Dashboard_GraphView"].ToString();
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			IDataReader reader;
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("Count", typeof(string)));
			DataRow dr;
			switch (ddPerspect.SelectedValue)
			{
				case "0":
					using (reader = Project.GetProjectMetrics())
					{
						if (reader.Read())
						{
							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("tActPrj") + ":";
							dr["Count"] = reader["ActiveProjects"].ToString();
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("tOnHoldPrj") + ":";
							dr["Count"] = reader["OnHoldProjects"].ToString();
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("CompletedProjects") + ":";
							dr["Count"] = reader["CompletedProjects"].ToString();
							dt.Rows.Add(dr);

							if (Configuration.HelpDeskEnabled)
							{
								dr = dt.NewRow();
								dr["Title"] = "&nbsp;";
								dr["Count"] = "&nbsp;";
								dt.Rows.Add(dr);

								dr = dt.NewRow();
								dr["Title"] = LocRM.GetString("TotalProjectIssues") + ":";
								dr["Count"] = reader["ProjectIncidents"].ToString();
								dt.Rows.Add(dr);
							}
							/*
														dr = dt.NewRow();
														dr["Title"] = LocRM.GetString("TotalProjectFiles") +":";
														dr["Count"] = reader["ProjectFiles"].ToString();
														dt.Rows.Add(dr);
							*/
							dr = dt.NewRow();
							dr["Title"] = "&nbsp;";
							dr["Count"] = "&nbsp;";
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("TargetBudget") + ":";
							dr["Count"] = reader["TargetBudget"].ToString();
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("EstimatedBudget") + ":";
							dr["Count"] = reader["EstimatedBudget"].ToString();
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("ActualBudget") + ":";
							dr["Count"] = reader["ActualBudget"].ToString();
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = "&nbsp;";
							dr["Count"] = "&nbsp;";
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("ActiveTasks") + ":";
							dr["Count"] = reader["ActiveTasks"].ToString();
							dt.Rows.Add(dr);

							if (Configuration.HelpDeskEnabled)
							{
								dr = dt.NewRow();
								dr["Title"] = LocRM.GetString("OpenIssues") + ":";
								dr["Count"] = reader["OpenIssues"].ToString();
								dt.Rows.Add(dr);
							}
						}
					}
					break;
				case "1":
					using (reader = Incident.GetIncidentStatistic(0))
					{
						if (reader.Read())
						{
							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("TotalIssues") + ":";
							dr["Count"] = reader["IncidentCount"].ToString();
							dt.Rows.Add(dr);

							//							dr = dt.NewRow();
							//							dr["Title"] = LocRM.GetString("MailIssues") + ":";
							//							dr["Count"] = reader["Pop3IncidentCount"].ToString();
							//							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = "&nbsp;";
							dr["Count"] = "&nbsp;";
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("NewIssues") + ":";
							dr["Count"] = reader["NewIncidentCount"].ToString();
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("ActiveIssues") + ":";
							dr["Count"] = ((int)reader["ActiveIncidentCount"] + (int)reader["ReOpenIncidentCount"]).ToString();
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("SuspendedIssues") + ":";
							dr["Count"] = reader["SuspendedIncidentCount"].ToString();
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("OnCheckIssues") + ":";
							dr["Count"] = reader["OnCheckIncidentCount"].ToString();
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("ClosedIssues") + ":";
							dr["Count"] = reader["ClosedIncidentCount"].ToString();
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = "&nbsp;";
							dr["Count"] = "&nbsp;";
							dt.Rows.Add(dr);

							/*dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("AvgTimeInNewState") + ":";
							dr["Count"] = CommonHelper.GetStringInterval(new TimeSpan(0,0,(int)reader["AvgTimeInNewState"]));
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("AvgTimeInActiveState") + ":";
							dr["Count"] = CommonHelper.GetStringInterval(new TimeSpan(0,0,(int)reader["AvgTimeInActiveState"]));
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = "&nbsp;";
							dr["Count"] = "&nbsp;";
							dt.Rows.Add(dr);*/

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("AvgTimeToResolveClosed") + ":";
							dr["Count"] = CommonHelper.GetStringInterval(new TimeSpan(0, 0, (int)reader["AvgTimeForResolveClosed"]));
							dt.Rows.Add(dr);

							dr = dt.NewRow();
							dr["Title"] = LocRM.GetString("AvgTimeToResolveAll") + ":";
							dr["Count"] = CommonHelper.GetStringInterval(new TimeSpan(0, 0, (int)reader["AvgTimeForResolveAll"]));
							dt.Rows.Add(dr);
						}
					}
					break;
				default:
					break;
			}
			repMetrics.DataSource = dt.DefaultView;
			repMetrics.DataBind();
		}
		#endregion

		#region BindGraph
		private void BindGraph()
		{
			string sQ_Str = "?Type=";
			if (rbChartType.SelectedValue == "0")
				sQ_Str += "Pie&";
			else
				sQ_Str += "Gist&";
			DateTime dt = DateTime.Now;
			sQ_Str += ddChartView.SelectedValue + "=1&id=" + dt.Hour + dt.Minute + dt.Second;
			imgGraph.ImageUrl = "~/Modules/ChartImage.aspx" + sQ_Str;
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			DateTime StartDate = Report.DefaultStartDate;
			DateTime EndDate = Report.DefaultEndDate;

			DataTable dt = Report.GetReportTemplatesByFilterDataTable(0, StartDate, EndDate, 0);
			if (dt != null)
				dt.Columns.Add("ObjectType", typeof(string));
			foreach (DataRow dr in dt.Rows)
			{
				// O.R. [2009-06-05]: If report name contains "&" then we got the exception
				//string s = HttpUtility.HtmlDecode(dr["TemplateXML"].ToString());
				string s = dr["TemplateXML"].ToString();
				XmlDocument temp = new XmlDocument();
				temp.InnerXml = s;

				bool deleteRow = false;
				int currentType = int.Parse(ddPerspect.SelectedValue);
				switch (temp.SelectSingleNode("IBNReportTemplate/ObjectName").InnerText)
				{
					case "Incident":
						if (!(currentType == 1))
							deleteRow = true;
						s = LocRM1.GetString("tIncidents");
						break;
					case "Project":
						if (!(currentType == 0))
							deleteRow = true;
						s = LocRM1.GetString("tProjects");
						break;
					default:
						deleteRow = true;
						s = "other";
						break;
				}
				dr["ObjectType"] = s;
				if (deleteRow)
					dr.Delete();
			}
			DataView dv = dt.DefaultView;
			dv.Sort = pc["CRep_Sorting"].ToString();
			dv.RowFilter = "";
			if (pc["CRep_PageSize"] != null)
				grdMain.PageSize = int.Parse(pc["CRep_PageSize"]);


			int pageindex = grdMain.CurrentPageIndex;
			int ppi = dv.Count / grdMain.PageSize;
			if (dv.Count % grdMain.PageSize == 0)
				ppi = ppi - 1;

			if (pageindex > ppi)
				grdMain.CurrentPageIndex = 0;

			grdMain.DataSource = dv;
			grdMain.DataBind();
			foreach (DataGridItem dgi in grdMain.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibView");
				if (ib != null)
					ib.ToolTip = LocRM1.GetString("tViewReport");
				ImageButton ib1 = (ImageButton)dgi.FindControl("ibDelete");
				if (ib1 != null)
				{
					ib1.ToolTip = LocRM1.GetString("tDelete");
					ib1.Attributes.Add("onclick", "return confirm('" + LocRM1.GetString("tWarning1") + "')");
				}
			}
		}
		#endregion

		protected string GetType(bool IsGlobal)
		{
			if (IsGlobal)
				return "<img src='../layouts/images/earth.gif' title='" + LocRM1.GetString("tIsGlobal") + "' align='middle' border='0' width='16' height='16'>";
			else
				return "<img src='../layouts/images/icon-key.gif' title='" + LocRM1.GetString("tOnlyForMe") + "' align='middle' border='0' width='16' height='16'>";
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
			this.ddPerspect.SelectedIndexChanged += new EventHandler(ddPerspect_SelectedIndexChanged);
			this.grdMain.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageIndexChanged);
			this.grdMain.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.grdMain_Sort);
			this.grdMain.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChanged);
			this.grdMain.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_view);
			this.grdMain.DeleteCommand += new DataGridCommandEventHandler(grdMain_DeleteCommand);
		}
		#endregion

		private void ddPerspect_SelectedIndexChanged(object sender, EventArgs e)
		{
			pc["Dashboard_Perspect"] = ddPerspect.SelectedValue;
			pc["Dashboard_GraphView"] = null;
			BindChartView();
			BindValues();
			BindGraph();
			if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
			{
				trCustomReps.Visible = true;
				BindDG();
			}
			else
				trCustomReps.Visible = false;
		}

		#region DataGrid Events
		private void grdMain_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (pc["CRep_Sorting"].ToString() == (string)e.SortExpression)
				pc["CRep_Sorting"] = (string)e.SortExpression + " DESC";
			else
				pc["CRep_Sorting"] = (string)e.SortExpression;

			BindDG();
		}

		private void dg_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["CRep_PageSize"] = e.NewPageSize.ToString();
			BindDG();
		}

		private void dg_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			grdMain.CurrentPageIndex = e.NewPageIndex;
			BindDG();
		}

		private void dg_view(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Edit")
			{
				int ReportTemplateId = int.Parse(e.Item.Cells[0].Text);
				Response.Redirect("../Reports/TemplateEdit.aspx?TemplateId=" + ReportTemplateId.ToString());
			}
			if (e.CommandName == "View")
			{
				int ReportTemplateId = int.Parse(e.Item.Cells[0].Text);
				Response.Redirect("../Reports/TemplateEdit.aspx?Generate=1&TemplateId=" + ReportTemplateId.ToString());
			}
		}

		private void grdMain_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ReportTemplateId = int.Parse(e.Item.Cells[0].Text);
			Report.DeleteReportTemplate(ReportTemplateId);
			Response.Redirect("../Reports/default.aspx?Tab=Dashboard");
		}
		#endregion

	}
}
