using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Mediachase.IBN.Business;
using System.Resources;
using System.Reflection;
using System.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects.Modules
{
	public partial class GanttChartPage : System.Web.UI.UserControl
	{
		protected bool isMSProject = false;
		private IFormatProvider _culture = CultureInfo.InvariantCulture;
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strGanttChart", Assembly.GetExecutingAssembly());
		private int _itemPos = 0;
		private int _itemHeight = GanttManager.ItemHeight;
		private string _iFrameHeight = "0";
		private string _viewValue = "ViewItem";

		#region GanttTitleLength
		private int iGanttTitleLength = -1;
		protected int GanttTitleLength
		{
			get
			{
				if (iGanttTitleLength < 0)
				{
					iGanttTitleLength = 25;
					string sGanttTitleLength = System.Configuration.ConfigurationManager.AppSettings["GanttTitleLength"];
					if (sGanttTitleLength != null)
					{
						try
						{
							iGanttTitleLength = int.Parse(sGanttTitleLength);
						}
						catch { }
					}
				}
				return iGanttTitleLength;
			}
		}
		#endregion

		#region ProjectId
		private int _projectId = 0;
		public int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return _projectId;
				}
			}
			set
			{
				_projectId = value;
			}
		}
		#endregion

		#region CanEdit
		private bool? _canEdit = null;
		private bool CanEdit
		{
			get
			{
				if (!_canEdit.HasValue)
				{
					_canEdit = ((Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)) ||
						(Security.CurrentUser.UserID == Project.GetProjectManager(ProjectId)));
				}
				return _canEdit.Value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Grid.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Gantt.css");
			UtilHelper.RegisterScript(Page, "~/Scripts/ganttScriptFullScreen.js");

			isMSProject = Project.GetIsMSProject(ProjectId);

			BindHandlers();
			BindLabels();

			if (!Page.IsPostBack)
			{
				if (_pc["GanttView"] == null)
					_pc["GanttView"] = "0";

				BindTasks();
			}
			if (Request.Browser.Browser.IndexOf("IE") >= 0)
			{
				//grdTasks.HeaderStyle.Height = Unit.Pixel(_itemHeight + 1);
				grdTasks.ItemStyle.Height = Unit.Pixel(_itemHeight - 1);
			}
			else
			{
				//grdTasks.HeaderStyle.Height = Unit.Pixel(_itemHeight + 10);
				grdTasks.ItemStyle.Height = Unit.Pixel(_itemHeight);
			}
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();
			grdTasks.Columns[11].Visible = (_pc["GanttView"] == "1");
			grdTasks.Columns[12].Visible = (_pc["GanttView"] == "1");

			int lx = -1;
			if (_pc["GantChart_Lx_New_" + ProjectId.ToString()] != null)
				lx = int.Parse(_pc["GantChart_Lx_New_" + ProjectId.ToString()]);
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "f_onload(" + lx + ", " + GanttManager.PortionWidth + ");", true);
		}
		#endregion

		#region BindLabels
		private void BindLabels()
		{
			lblHeaderTitle.Text = LocRM.GetString("tTitle");
			lblHeaderStart.Text = LocRM.GetString("StartDate");
			lblHeaderFinish.Text = LocRM.GetString("FinishDate");
			lbStartPrj.Text = LocRM.GetString("tPrjStart");
			lbEndPrj.Text = LocRM.GetString("tPrjEnd");
			lbToday.Text = LocRM.GetString("tToday");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			tbTasks.Title = LocRM.GetString("GanttChartTitle");
			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = LocRM.GetString("tView");
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
			subItem.Text = LocRM.GetString("DateView");
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (_pc["GanttView"] == "0")
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept_1.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeView, "0");
			subItem.Text = LocRM.GetString("WithoutDateView");
			topMenuItem.Items.Add(subItem);

			tbTasks.ActionsMenu.Items.Add(topMenuItem);
		}
		#endregion

		#region BindHandlers
		private void BindHandlers()
		{
			this.grdTasks.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.grdTasks_PageChange);
			this.grdTasks.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdTasks_Update);
			this.grdTasks.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdTasks_Cancel);
			this.grdTasks.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.grdTasks_ItemDataBound);
			this.grdTasks.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdTasks_Edit);
			this.grdTasks.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.grdTasks_PageSizeChange);
			this.grdTasks.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdTasks_ItemCommand);

			this.lbStartPrj.Click += new EventHandler(lbStartPrj_Click);
			this.lbEndPrj.Click += new EventHandler(lbEndPrj_Click);
			this.lbToday.Click += new EventHandler(lbToday_Click);
		}
		#endregion

		#region grdTasks_PageSizeChange
		private void grdTasks_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			_pc["GanttChart_PageSize"] = e.NewPageSize.ToString();
			BindTasks();
		}
		#endregion

		#region grdTasks_PageChange
		private void grdTasks_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			grdTasks.CurrentPageIndex = e.NewPageIndex;
			BindTasks();
		}
		#endregion

		#region grdTasks_Edit
		private void grdTasks_Edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int taskId = int.Parse(e.CommandArgument.ToString());
			using (IDataReader reader = Task.GetListTasksForMove(taskId))
			{
				if (reader.Read())
					grdTasks.EditItemIndex = e.Item.ItemIndex;
			}
			BindTasks();
		}
		#endregion

		#region grdTasks_Cancel
		private void grdTasks_Cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			grdTasks.EditItemIndex = -1;
			BindTasks();
		}
		#endregion

		#region grdTasks_Update
		private void grdTasks_Update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddl");
			int sourcetask = int.Parse(e.Item.Cells[0].Text);
			int desttask = int.Parse(ddl.SelectedItem.Value);
			Task.MoveTo(sourcetask, desttask);
			grdTasks.EditItemIndex = -1;
			BindTasks();
		}
		#endregion

		#region BindTasks
		public void BindTasks()
		{
			DataTable dt = Task.GetListTasksByProjectCollapsedDataTable(ProjectId);
			dt.AcceptChanges();

			if (_pc["GanttChart_PageSize"] != null)
				grdTasks.PageSize = int.Parse(_pc["GanttChart_PageSize"]);


			int pageIndex = grdTasks.CurrentPageIndex;
			int ppi = dt.Rows.Count / grdTasks.PageSize;
			if (dt.Rows.Count % grdTasks.PageSize == 0)
				ppi = ppi - 1;

			if (pageIndex > ppi)
				pageIndex = 0;
			grdTasks.CurrentPageIndex = pageIndex;

			//if (pageIndex <= ppi)
			//    grdTasks.CurrentPageIndex = pageIndex;
			//else
			//    grdTasks.CurrentPageIndex = 0;

			int fh = dt.Rows.Count % grdTasks.PageSize;
			if (pageIndex == ppi && fh == 0)
				fh = grdTasks.PageSize * _itemHeight;
			else if
				(pageIndex < ppi)
				fh = grdTasks.PageSize * _itemHeight;
			else
				fh = fh * _itemHeight;

			_iFrameHeight = fh.ToString();

			grdTasks.DataSource = dt.DefaultView;
			grdTasks.DataBind();
			if (!CanEdit)
				grdTasks.Columns[grdTasks.Columns.Count - 2].Visible = false;

			divImg.Style.Add("HEIGHT", _iFrameHeight + "px");
			hdnLinkToGantt.Value = String.Format("../Projects/GanttChartImage2.aspx?ProjectID={0}&PageItems={1}&PageNumber={2}&Days=366&RenderPortion=1&BasePlanSlotId=-1", ProjectId, grdTasks.PageSize, grdTasks.CurrentPageIndex);
		}
		#endregion

		#region grdTasks_ItemDataBound
		private void grdTasks_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			_itemPos++;

			int taskIdPos = 0;
			int isSummaryPos = 1;
			int taskNumPos = 2;
			int outlineNumberPos = 3;
			int outlineLevelPos = 4;
			int titlePos = 5;
			int isCollapsedPos = 6;

			//int isCompetedPos = 12;
			int isCompetedPos = 14;
			int statePos = 15;
			int isMilestonePos = 16;

			int indent = 14;
			int plusMinusWidth = 9;
			int plusMinusHeight = 9;
			int leftRightWidth = 16;
			int leftRightHeight = 16;
			int spacing = 3;

			string moveLeftText = LocRM.GetString("MoveLeft");
			string moveRightText = LocRM.GetString("MoveRight");
			string collapseText = LocRM.GetString("Collapse");
			string expandText = LocRM.GetString("Expand");

			string divPrefix = "<div class='d'>";
			string divPostfix = "</div>";

			string prefix = "";
			string postfix = "";

			DataGridItem item = e.Item;
			if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.EditItem)
			{
				int outlineLevel = int.Parse(item.Cells[outlineLevelPos].Text);
				bool isSummary = bool.Parse(item.Cells[isSummaryPos].Text);
				string taskId = item.Cells[taskIdPos].Text;
				bool isCollapsed = (item.Cells[isCollapsedPos].Text == "1");
				string outlineNumber = item.Cells[outlineNumberPos].Text;
				bool isCompeted = bool.Parse(item.Cells[isCompetedPos].Text);
				int stateId = int.Parse(item.Cells[statePos].Text);
				bool isMilestone = bool.Parse(item.Cells[isMilestonePos].Text);

				int spacerLength = (outlineLevel - 1) * indent;
				if (spacerLength > 0)
					prefix = String.Format("<img src='../layouts/images/spacer.gif' width={0} height=1 border=0 align=absmiddle>", spacerLength);

				// Значки +/-
				int halfIndent = (indent - plusMinusWidth) / 2 + 1;
				prefix += String.Format("<img src='../layouts/images/spacer.gif' width={0} height=1 border=0 align=absmiddle>", halfIndent);
				if (isSummary)
				{
					prefix += "<b>";
					if (isCollapsed)
					{
						Button btnExpand = (Button)item.FindControl("btnExpand");
						string postBackString = Page.ClientScript.GetPostBackEventReference(btnExpand, taskId);
						prefix += String.Format("<a href=javascript:{3}><img src='../layouts/images/plus.gif' width={0} height={1} border=0 title='{2}' align=absmiddle></a>", plusMinusWidth, plusMinusHeight, expandText, postBackString);
					}
					else	// Expanded
					{
						Button btnCollapse = (Button)item.FindControl("btnCollapse");
						string postBackString = Page.ClientScript.GetPostBackEventReference(btnCollapse, taskId);
						prefix += String.Format("<a href=javascript:{3}><img src='../layouts/images/minus.gif' width={0} height={1} border=0 title='{2}' align=absmiddle></a>", plusMinusWidth, plusMinusHeight, collapseText, postBackString);
					}
					prefix += String.Format("<img src='../layouts/images/spacer.gif' width={0} height=1 border=0 align=absmiddle>", indent - plusMinusWidth - halfIndent);

					postfix += "</b>";
				}
				else
				{
					prefix += String.Format("<img src='../layouts/images/spacer.gif' width={0} height=1 border=0 align=absmiddle>", plusMinusWidth);
					prefix += String.Format("<img src='../layouts/images/spacer.gif' width={0} height=1 border=0 align=absmiddle>", indent - plusMinusWidth - halfIndent);
				}

				// Сдвиг влево
				if ((outlineLevel > 1) && CanEdit && !isMSProject)
				{
					Button btnLeft = (Button)item.FindControl("btnLeft");
					string postBackString = Page.ClientScript.GetPostBackEventReference(btnLeft, taskId);
					prefix += String.Format("<a href=\"javascript:if(confirm('{0}?')){{{1}}}\"><img src='../layouts/images/left.gif' width={2} height={3} border=0 title='{4}' align=absmiddle></a>",
						LocRM.GetString("MoveLeft"),
						postBackString,
						leftRightWidth,
						leftRightHeight,
						moveLeftText);
					prefix += String.Format("<img src='../layouts/images/spacer.gif' width={0} height=1 border=0 align=absmiddle>", spacing);
				}
				else	// сдвиг влево на первом уровне невозможен - резервируем место
				{
					prefix += String.Format("<img src='../layouts/images/spacer.gif' width={0} height=1 border=0 align=absmiddle>", spacing);
				}

				// Сдвиг вправо
				bool isFirstChild = false;
				if (outlineNumber == "1" || (outlineNumber.Length > 2 && outlineNumber.Substring(outlineNumber.Length - 2) == ".1"))
					isFirstChild = true;

				if (!isFirstChild && CanEdit && !isMSProject)	// если это первый дочерний, то вправо сдвигать нельзя
				{
					Button btnRight = (Button)item.FindControl("btnRight");
					string postBackString = Page.ClientScript.GetPostBackEventReference(btnRight, taskId);
					postfix += String.Format("<img src='../layouts/images/spacer.gif' width={0} height=1 border=0 >", spacing);
					postfix += String.Format("<a href=\"javascript:if(confirm('{0}?')){{{1}}}\"><img src='../layouts/images/right.gif' width={2} height={3} border=0 title='{4}' align=absmiddle></a>",
						LocRM.GetString("MoveRight"),
						postBackString,
						leftRightWidth,
						leftRightHeight,
						moveRightText);
				}

				string tit = HttpUtility.HtmlDecode(item.Cells[titlePos].Text);
				if (tit.Length > GanttTitleLength)
					tit = tit.Substring(0, GanttTitleLength);
				tit = HttpUtility.HtmlEncode(tit);

				string title = item.Cells[titlePos].Text;
				// Resources
				string resources = String.Empty;
				using (IDataReader reader = Task.GetListResources(int.Parse(taskId)))
				{
					while (reader.Read())
					{
						if (!String.IsNullOrEmpty(resources))
							resources += ", ";
						resources += reader["ResourceName"].ToString();
					}
				}
				if (!String.IsNullOrEmpty(resources))
					title = String.Format(CultureInfo.InvariantCulture, "{0}\r\n{1}: {2}",
						title, GetGlobalResourceObject("IbnFramework.Global", "_mc_Resources").ToString(), resources);
				title = title.Replace("'", "&#039;");

				string addStyle = String.Empty;
				if (isCompeted)
					addStyle = " style='color:Gray;'";
				else if (stateId == (int)ObjectStates.Overdue)
					addStyle = " style='color:Red;'";

				item.Cells[titlePos].Text = String.Format(
					"<div name='divGridTitle'{0} title='{1}'>{2}{3}{4}{5}{6}</div>",
					addStyle, title, divPrefix, prefix, tit, postfix, divPostfix);

				item.Cells[taskNumPos].Text = String.Format(CultureInfo.InvariantCulture,
					"{0}{1}{2}{3}",
					divPrefix,
					item.Cells[taskNumPos].Text,
					isMilestone ? "*" : "",
					divPostfix);

				// Edit
				if (item.ItemType == ListItemType.EditItem)
				{
					// Retrieve the drop-down list control to set up
					DropDownList ddl;
					ddl = (DropDownList)e.Item.FindControl("ddl");
					ddl.ClearSelection();
					using (IDataReader rdr = Task.GetListTasksForMove(int.Parse(taskId)))
					{
						while (rdr.Read())
						{
							ddl.Items.Add(new ListItem(rdr["TaskNum"].ToString()));
						}
					}
				}

				ImageButton completeButton = (ImageButton)e.Item.FindControl("ibComplete");
				if (completeButton != null)
				{
					completeButton.OnClientClick = String.Format(CultureInfo.InvariantCulture,
						"if (!confirm('{0}')) return false;", LocRM.GetString("CompleteConfirm"));
				}

				ImageButton activateButton = (ImageButton)e.Item.FindControl("ibActivate");
				if (activateButton != null)
				{
					activateButton.OnClientClick = String.Format(CultureInfo.InvariantCulture,
						"if (!confirm('{0}')) return false;", LocRM.GetString("ActivateConfirm"));
				}

				ImageButton deactivateButton = (ImageButton)e.Item.FindControl("ibDeactivate");
				if (deactivateButton != null)
				{
					deactivateButton.OnClientClick = String.Format(CultureInfo.InvariantCulture,
						"if (!confirm('{0}')) return false;", LocRM.GetString("DeactivateConfirm"));
				}
			}
		}
		#endregion

		#region grdTasks_ItemCommand
		private void grdTasks_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandSource is Button)
			{
				Button btn = (Button)e.CommandSource;
				int taskId = int.Parse(btn.CommandArgument);

				try
				{
					switch (btn.CommandName)
					{
						case "MoveLeft":
							Task.MoveLeft(taskId);
							break;
						case "MoveRight":
							Task.MoveRight(taskId);
							break;
						case "Collapse":
							Task.Collapse(taskId);
							break;
						case "Expand":
							Task.Expand(taskId);
							break;
					}
				}
				catch (UnableMoveTask)
				{
					Page.ClientScript.RegisterStartupScript(this.Page.GetType(),
						Guid.NewGuid().ToString("N"), String.Format("alert('{0}');", LocRM.GetString("TaskUnmovable")), true);
				}
				BindTasks();
			}
			else if (e.CommandSource is ImageButton)
			{
				ImageButton btn = (ImageButton)e.CommandSource;
				int taskId = int.Parse(btn.CommandArgument);

				switch (btn.CommandName)
				{
					case "Complete":
						Task.CompleteTask(taskId, true);
						break;
					case "Activate":
						Task.ActivateTask(taskId);
						break;
					case "Deactivate":
						Task.DeactivateTask(taskId);
						break;
				}
				BindTasks();
			}
		}
		#endregion

		#region lbChangeView_Click
		protected void lbChangeView_Click(object sender, EventArgs e)
		{
			string arg = Request["__EVENTARGUMENT"];
			if (!String.IsNullOrEmpty(arg))
			{
				if (arg == "0" || arg == "1")
					_pc["GanttView"] = arg;
			}
		}
		#endregion

		#region Delete
		protected void lblDeleteTaskAll_Click(object sender, System.EventArgs e)
		{
			int taskId = int.Parse(hdnTaskId.Value);
			Task.Delete(taskId);
			BindTasks();
		}
		#endregion

		#region Start-End Project
		private void lbStartPrj_Click(object sender, EventArgs e)
		{
			_pc["GantChart_Lx_New_" + ProjectId.ToString()] = "-1";
			Response.Redirect("~/Projects/GanttChart.aspx?ProjectId=" + ProjectId);
		}

		private void lbToday_Click(object sender, EventArgs e)
		{
			_pc["GantChart_Lx_New_" + ProjectId.ToString()] = (GanttManager.GetPortionX(ProjectId, UserDateTime.UserToday) - 1).ToString();
			Response.Redirect("~/Projects/GanttChart.aspx?ProjectId=" + ProjectId);
		}

		private void lbEndPrj_Click(object sender, EventArgs e)
		{
			_pc["GantChart_Lx_New_" + ProjectId.ToString()] = (GanttManager.GetPortionX(ProjectId, Task.GetMaxFinishDate(ProjectId)) - 1).ToString();
			Response.Redirect("~/Projects/GanttChart.aspx?ProjectId=" + ProjectId);
		}
		#endregion
	}
}
