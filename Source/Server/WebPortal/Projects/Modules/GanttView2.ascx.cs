using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Modules;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Projects.Modules
{
	public partial class GanttView2 : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Calendar.Resources.strGanttView", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strGanttChart", Assembly.GetExecutingAssembly());
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;
		private string _viewValue = "ViewItem";

		#region _projectId
		private string _projectId
		{
			get
			{
				return Request["ProjectID"];
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Project.IsWebGanttChartEnabled())
				throw new AccessDeniedException();

			if (!IsPostBack)
			{
				BindDefaultValues();
				BindSavedData();
				if (_pc["GanttView"] == null)
					_pc["GanttView"] = "0";
			}

			if (ddProject.SelectedItem != null)
				ctrlGanttChart.ProjectId = int.Parse(ddProject.SelectedItem.Value);

			if (!IsPostBack)
				BindDataGrid();

			BindToolBar();
			BindLegendTable();
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			
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
			tc01.Text = LocRM.GetString("tActive");
			tc02.Text = LocRM.GetString("tCompleted");
			tr0.Cells.Add(tc01);
			tr0.Cells.Add(tc02);
			tr0.Cells.Add(tc03);
			tblLegend.Rows.Add(tr0);

			bool isBasePlanSlot = (Request["BasePlanSlotId"] != null && int.Parse(Request["BasePlanSlotId"]) > 0);
			foreach (GanttItem gItem in GanttManager.GetLegendItems(false, isBasePlanSlot))
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
				lbl.Text = LocRM.GetString(item);
				tc3.Controls.Add(lbl);
				tr.Cells.Add(tc1);
				tr.Cells.Add(tc2);
				tr.Cells.Add(tc3);
				tblLegend.Rows.Add(tr);
			}
		} 
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			using (IDataReader rdr = Project.GetListProjects())
			{
				ddProject.Items.Clear();
				while (rdr.Read())
					ddProject.Items.Add(new ListItem((string)rdr["Title"], rdr["ProjectId"].ToString()));
			}

			if (_projectId != null)
			{
				tdProject1.Visible = false;
				tdProject2.Visible = false;
			}
		}
		#endregion

		#region BindSavedData
		private void BindSavedData()
		{
			if (_pc["gv_Project"] != null)
				CommonHelper.SafeSelect(ddProject, _pc["gv_Project"]);
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			ctrlGanttChart.BindTasks();
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			if (ddProject.SelectedItem != null)
				_pc["gv_Project"] = ddProject.SelectedItem.Value;
		}
		#endregion

		#region BindToolBar
		private void BindToolBar()
		{
			if (this.Parent.Parent is IToolbarLight)
			{
				BlockHeaderLightWithMenu secHeader = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();

				ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = LocRM2.GetString("FullScreenView");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/task1.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(16);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.ClientSideCommand = String.Format("javascript:{{OpenWindow('{0}', 800, 600, false);}}",
					this.Page.ResolveUrl("~/Projects/GanttChart.aspx?ProjectId=") + _projectId);
				topMenuItem.LookId = "TopItemLook";
				secHeader.ActionsMenu.Items.Add(topMenuItem);

				topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = LocRM2.GetString("tView");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.Value = _viewValue;
				topMenuItem.LookId = "TopItemLook";

				ComponentArt.Web.UI.MenuItem subItem;

				subItem = new ComponentArt.Web.UI.MenuItem();
				if (_pc["GanttView"] == "1")
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeView, "1");
				subItem.Text = LocRM2.GetString("DateView");
				topMenuItem.Items.Add(subItem);

				subItem = new ComponentArt.Web.UI.MenuItem();
				if (_pc["GanttView"] == "0")
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeView, "0");
				subItem.Text = LocRM2.GetString("WithoutDateView");
				topMenuItem.Items.Add(subItem);

				secHeader.ActionsMenu.Items.Add(topMenuItem);

				if (_projectId != null && Task.CanCreate(int.Parse(_projectId)))
				{
					topMenuItem = new ComponentArt.Web.UI.MenuItem();
					topMenuItem.Text = LocRM.GetString("AddTask");
					topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/icons/task1_create.gif");
					topMenuItem.Look.LeftIconHeight = Unit.Pixel(16);
					topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
					topMenuItem.LookId = "TopItemLook";
					topMenuItem.NavigateUrl = "~/Tasks/TaskEdit.aspx?ProjectID=" + _projectId + "&Back=Gantt";
					secHeader.ActionsMenu.Items.Add(topMenuItem);

					//secHeader.AddRightLink("<img alt='' src='../Layouts/Images/icons/task1_create.gif'/> " + LocRM.GetString("AddTask"), "../Tasks/TaskEdit.aspx?ProjectID=" + _projectId + "&Back=Gantt");
				}
				topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = LocRM.GetString("tLegend");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/pin_off.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(16);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.LookId = "TopItemLook";
				topMenuItem.ClientSideCommand = String.Format("javascript:ShowLegend('{0}');", secHeader.RightCornerClientId);
				secHeader.ActionsMenu.Items.Add(topMenuItem);
			}
		}
		#endregion

		#region ddProjects_IndexChange
		protected void ddProjects_IndexChange(object sender, System.EventArgs e)
		{
			SaveValues();
			if (ddProject.SelectedItem != null)
				ctrlGanttChart.ProjectId = int.Parse(ddProject.SelectedItem.Value);
			BindDataGrid();
		}
		#endregion

		#region lbChangeView_Click
		protected void lbChangeView_Click(object sender, EventArgs e)
		{
			string arg = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(arg))
			{
				BlockHeaderLightWithMenu secHeader = null;
				if (this.Parent.Parent is IToolbarLight)
					secHeader = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();
				ComponentArt.Web.UI.MenuItem subItem;
				ComponentArt.Web.UI.MenuItem topItem = null;
				if(secHeader.ActionsMenu.Items.Count > 0)
					foreach(ComponentArt.Web.UI.MenuItem mi in secHeader.ActionsMenu.Items)
						if (mi.Value == _viewValue)
						{
							topItem = mi;
							break;
						}
				if (topItem == null)
					return;
				if (arg == "0")
				{
					_pc["GanttView"] = "0";
					if (secHeader != null)
					{
						subItem = topItem.Items[0];
						subItem.Look.LeftIconUrl = String.Empty;
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						subItem = topItem.Items[1];
						subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
					}
				}
				else if (arg == "1")
				{
					_pc["GanttView"] = "1";
					if (secHeader != null)
					{
						subItem = topItem.Items[1];
						subItem.Look.LeftIconUrl = String.Empty;
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
						subItem = topItem.Items[0];
						subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
						subItem.Look.LeftIconWidth = Unit.Pixel(16);
						subItem.Look.LeftIconHeight = Unit.Pixel(16);
					}
				}
			}
		} 
		#endregion
	}
}