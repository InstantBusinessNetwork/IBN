using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using System.Resources;
using System.Xml;

namespace Mediachase.UI.Web.Apps.ReportManagement.Modules
{
	public partial class UserReportPersonal : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", typeof(UserReportPersonal).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		Mediachase.IBN.Business.ControlSystem.ReportStorage rs;
		BaseIbnContainer bic;

		protected void Page_Load(object sender, EventArgs e)
		{
			bic = BaseIbnContainer.Create("Reports", "GlobalReports");
			rs = (ReportStorage)bic.LoadControl("ReportStorage");
			BindToolBar();
			if (!IsPostBack)
			{
				BindRepeaters();
				BindDGs(true, true);
			}

			this.dgRepPers.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgRepPers_PageIndexChanged);
			this.dgRepPers.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgRepPers_Sort);
			this.dgRepPers.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgRepPers_PageSizeChanged);
			this.dgRepPers.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgRepPers_view);
			this.dgRepPers.DeleteCommand += new DataGridCommandEventHandler(this.dgRepPers_DeleteCommand);
		}

		#region BindToolBar
		/// <summary>
		/// Binds the tool bar.
		/// </summary>
		private void BindToolBar()
		{
			tbLightPers.ActionsMenu.Items.Clear();
			tbLightPers.ClearRightItems();
			tbLightPers.AddText(LocRM.GetString("Reports"));

		}
		#endregion

		#region BindRepeaters
		/// <summary>
		/// Binds the repeaters.
		/// </summary>
		private void BindRepeaters()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("sortName", typeof(string)));

			DataTable dt1 = new DataTable();
			dt1.Columns.Add(new DataColumn("Name", typeof(string)));
			dt1.Columns.Add(new DataColumn("sortName", typeof(string)));

			DataRow dr;

			ReportInfo[] _ri = rs.GetReports();
			foreach (ReportInfo ri in _ri)
			{
				if (!rs.CanUserRead(Security.CurrentUser.UserID, ri.Id))
					continue;
				if (ri.Name == "ProjectTime" && !Configuration.ProjectManagementEnabled)
					continue;
				if (ri.Type == Mediachase.IBN.Business.UserReport.UserReportType.Personal)
				{
					dr = dt.NewRow();
					dr["Name"] = String.Format("<a href='{0}'>{1}</a>", ResolveUrl("~" + ri.Url), ri.ShowName);
					dr["sortName"] = ri.ShowName;
					dt.Rows.Add(dr);
				}
				else if (ri.Type == Mediachase.IBN.Business.UserReport.UserReportType.Global)
				{
					dr = dt1.NewRow();
					dr["Name"] = String.Format("<a href='{0}'>{1}</a>", ResolveUrl("~" + ri.Url), ri.ShowName);
					dr["sortName"] = ri.ShowName;
					dt1.Rows.Add(dr);
				}
			}

			DataView dv = dt.DefaultView;
			dv.Sort = "sortName";
			repPerc.DataSource = dv;
			repPerc.DataBind();

			dv = dt1.DefaultView;
			dv.Sort = "sortName";
		}
		#endregion

		#region BindDGs
		private void BindDGs(bool left, bool right)
		{
			if (Security.IsUserInGroup(InternalSecureGroups.ProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
			{
				dgRepPers.Columns[2].HeaderText = LocRM.GetString("tName");
				dgRepPers.Columns[3].HeaderText = LocRM.GetString("tType");

				DataTable dt = Report.GetReportTemplatesByFilterDataTable(0, Report.DefaultStartDate, Report.DefaultEndDate, 0);

				if (dt != null)
					dt.Columns.Add("ObjectType", typeof(string));
				foreach (DataRow dr in dt.Rows)
				{
					// O.R. [2009-06-05]: If report name contains "&" then we got the exception
					//string s = HttpUtility.HtmlDecode(dr["TemplateXML"].ToString());
					string s = dr["TemplateXML"].ToString();
					XmlDocument temp = new XmlDocument();
					temp.InnerXml = s;

					switch (temp.SelectSingleNode("IBNReportTemplate/ObjectName").InnerText)
					{
						case "Incident":
							s = LocRM.GetString("tIncidents");
							break;
						case "Project":
							s = LocRM.GetString("tProjects");
							break;
						case "ToDo":
							s = LocRM.GetString("tToDo");
							break;
						case "Event":
							s = LocRM.GetString("tCalEntries");
							break;
						case "Document":
							s = LocRM.GetString("tDocuments");
							break;
						case "Library":
							s = LocRM.GetString("tAssets");
							break;
						case "Directory":
							s = LocRM.GetString("tUsers");
							break;
						case "Task":
							s = LocRM.GetString("tTasks");
							break;
						case "Portfolio":
							s = LocRM.GetString("tPrjGrps");
							break;
					}
					dr["ObjectType"] = s;
				}

				if (left)
				{
					DataView dv = dt.DefaultView;
					dv.RowFilter = "IsGlobal=0";

					if (pc["rep_Global_Sorting"] == null)
						pc["rep_Global_Sorting"] = "TemplateName";
					dv.Sort = pc["rep_Global_Sorting"].ToString();

					if (pc["rep_Global_PageSize"] == null)
						pc["rep_Global_PageSize"] = "10";
					dgRepPers.PageSize = int.Parse(pc["rep_Global_PageSize"].ToString());

					if (pc["rep_Global_Page"] == null)
						pc["rep_Global_Page"] = "0";
					int PageIndex = int.Parse(pc["rep_Global_Page"].ToString());
					int ppi = dv.Count / dgRepPers.PageSize;
					if (dv.Count % dgRepPers.PageSize == 0)
						ppi = ppi - 1;
					if (PageIndex <= ppi)
					{
						dgRepPers.CurrentPageIndex = PageIndex;
					}
					else
					{
						dgRepPers.CurrentPageIndex = 0;
						pc["rep_Global_Page"] = "0";
					}

					dgRepPers.DataSource = dv;
					dgRepPers.DataBind();

					foreach (DataGridItem dgi in dgRepPers.Items)
					{
						//					LinkButton ib=(LinkButton)dgi.FindControl("ibView");
						//					if (ib!=null)
						//						ib.ToolTip = LocRM.GetString("tViewReport");
						ImageButton ib2 = (ImageButton)dgi.FindControl("ibEdit");
						if (ib2 != null)
							ib2.ToolTip = LocRM.GetString("tEdit");
						ImageButton ib1 = (ImageButton)dgi.FindControl("ibDelete");
						if (ib1 != null)
						{
							ib1.ToolTip = LocRM.GetString("tDelete");
							ib1.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning1") + "')");
						}
					}
				}
			}
		}
		#endregion

		#region DataGrid Protected Strings
		protected string GetType(bool IsGlobal)
		{
			if (IsGlobal)
				return string.Format("<img src='{0}' title='", this.ResolveClientUrl("~/layouts/images/earth.gif")) + LocRM.GetString("tIsGlobal") + "' align='middle' border='0' width='16' height='16'>";
			else
				return string.Format("<img src='{0}' title='", this.ResolveClientUrl("~/layouts/images/icon-key.gif")) + LocRM.GetString("tOnlyForMe") + "' align='middle' border='0' width='16' height='16'>";
		}
		#endregion

		#region dgRepPers_Events
		private void dgRepPers_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (pc["rep_Global_Sorting"].ToString() == (string)e.SortExpression)
				pc["rep_Global_Sorting"] = (string)e.SortExpression + " DESC";
			else
				pc["rep_Global_Sorting"] = (string)e.SortExpression;
			BindDGs(true, false);
		}

		private void dgRepPers_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["rep_Global_PageSize"] = e.NewPageSize.ToString();
			BindDGs(true, false);
		}

		private void dgRepPers_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["rep_Global_Page"] = e.NewPageIndex.ToString();
			BindDGs(true, false);
		}

		private void dgRepPers_view(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Edit")
			{
				int ReportTemplateId = int.Parse(e.Item.Cells[0].Text);
				Response.Redirect("~/Reports/TemplateEdit.aspx?TemplateId=" + ReportTemplateId.ToString());
			}
			if (e.CommandName == "View")
			{
				int ReportTemplateId = int.Parse(e.Item.Cells[0].Text);
				Response.Redirect("~/Reports/TemplateEdit.aspx?Generate=1&TemplateId=" + ReportTemplateId.ToString());
			}
		}

		private void dgRepPers_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ReportTemplateId = int.Parse(e.Item.Cells[0].Text);
			Report.DeleteReportTemplate(ReportTemplateId);
			Response.Redirect("~/Apps/ReportManagement/Pages/UserReport.aspx");
		}
		#endregion
	}
}