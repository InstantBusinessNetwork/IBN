using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.IBN.Business;
using System.Data;
using System.Xml;
using Mediachase.IBN.Business.ControlSystem;
using System.Resources;

namespace Mediachase.UI.Web.Apps.ReportManagement.Modules
{
	public partial class UserReportCustom : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", typeof(UserReportCustom).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		BaseIbnContainer bic;
		Mediachase.IBN.Business.ControlSystem.ReportStorage rs;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			bic = BaseIbnContainer.Create("Reports", "GlobalReports");
			rs = (ReportStorage)bic.LoadControl("ReportStorage");
			if (!Page.IsPostBack)
			{
				BindDGs(true, true);
			}
			BindToolBar();

			this.dgRepGeneral.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgRepGeneral_PageIndexChanged);
			this.dgRepGeneral.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgRepGeneral_Sort);
			this.dgRepGeneral.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dgRepGeneral_PageSizeChanged);
			this.dgRepGeneral.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgRepGeneral_view);
			this.dgRepGeneral.DeleteCommand += new DataGridCommandEventHandler(this.dgRepGeneral_DeleteCommand);

		}

		#region BindToolBar
		private void BindToolBar()
		{
			tbLightGeneral.ActionsMenu.Items.Clear();
			tbLightGeneral.ClearRightItems();
			tbLightGeneral.AddText(LocRM.GetString("GeneralReports"));
			if (Security.IsUserInGroup(InternalSecureGroups.ProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager))
				tbLightGeneral.AddRightLink(string.Format("<img alt='' src='{0}'/> ", this.ResolveClientUrl("~/Layouts/Images/icons/newtemplate.gif")) + LocRM.GetString("tCreateTemplate"), string.Format("javascript:OpenWindow('{0}?Mode=Rep',750,466,true);", this.ResolveUrl("~/Reports/XMLReport.aspx")));
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
				trHide3.Visible = true;
				trHide4.Visible = true;

				dgRepGeneral.Columns[2].HeaderText = LocRM.GetString("tName");
				dgRepGeneral.Columns[3].HeaderText = LocRM.GetString("tType");

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

				if (right)
				{
					DataView dv1 = dt.DefaultView;
					dv1.RowFilter = "IsGlobal=1";

					if (pc["rep_Global_Sorting1"] == null)
						pc["rep_Global_Sorting1"] = "TemplateName";
					dv1.Sort = pc["rep_Global_Sorting1"].ToString();

					if (pc["rep_Global_PageSize1"] == null)
						pc["rep_Global_PageSize1"] = "10";
					dgRepGeneral.PageSize = int.Parse(pc["rep_Global_PageSize1"].ToString());

					if (pc["rep_Global_Page1"] == null)
						pc["rep_Global_Page1"] = "0";
					int PageIndex1 = int.Parse(pc["rep_Global_Page1"].ToString());
					int ppi1 = dv1.Count / dgRepGeneral.PageSize;
					if (dv1.Count % dgRepGeneral.PageSize == 0)
						ppi1 = ppi1 - 1;
					if (PageIndex1 <= ppi1)
					{
						dgRepGeneral.CurrentPageIndex = PageIndex1;
					}
					else
					{
						dgRepGeneral.CurrentPageIndex = 0;
						pc["rep_Global_Page1"] = "0";
					}

					dgRepGeneral.DataSource = dv1;
					dgRepGeneral.DataBind();

					foreach (DataGridItem dgi in dgRepGeneral.Items)
					{
						//					ImageButton ib=(ImageButton)dgi.FindControl("ibView1");
						//					if (ib!=null)
						//						ib.ToolTip = LocRM.GetString("tViewReport");
						ImageButton ib2 = (ImageButton)dgi.FindControl("ibEdit1");
						if (ib2 != null)
							ib2.ToolTip = LocRM.GetString("tEdit");
						ImageButton ib1 = (ImageButton)dgi.FindControl("ibDelete1");
						if (ib1 != null)
						{
							ib1.ToolTip = LocRM.GetString("tDelete");
							ib1.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning1") + "')");
						}
					}
				}
			}
			else
			{
				trHide3.Visible = false;
				trHide4.Visible = false;
			}
		}
		#endregion

		#region DataGrid Protected Strings
		protected string GetType(bool IsGlobal)
		{
			if (IsGlobal)
				return string.Format("<img alt='' src='{0}' title='", this.ResolveClientUrl("~/layouts/images/earth.gif")) + LocRM.GetString("tIsGlobal") + "' align='middle' border='0' width='16' height='16'>";
			else
				return string.Format("<img alt='' src='{0}' title='", this.ResolveClientUrl("~/layouts/images/icon-key.gif")) + LocRM.GetString("tOnlyForMe") + "' align='middle' border='0' width='16' height='16'>";
		}
		#endregion

		#region dgRepGeneral_Events
		private void dgRepGeneral_Sort(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			if (pc["rep_Global_Sorting1"].ToString() == (string)e.SortExpression)
				pc["rep_Global_Sorting1"] = (string)e.SortExpression + " DESC";
			else
				pc["rep_Global_Sorting1"] = (string)e.SortExpression;
			BindDGs(false, true);
		}

		private void dgRepGeneral_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["rep_Global_PageSize1"] = e.NewPageSize.ToString();
			BindDGs(false, true);
		}

		private void dgRepGeneral_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["rep_Global_Page1"] = e.NewPageIndex.ToString();
			BindDGs(false, true);
		}

		private void dgRepGeneral_view(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
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

		private void dgRepGeneral_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ReportTemplateId = int.Parse(e.Item.Cells[0].Text);
			Report.DeleteReportTemplate(ReportTemplateId);
			Response.Redirect("~/Apps/ReportManagement/Pages/UserReport.aspx");
		}
		#endregion

	}
}