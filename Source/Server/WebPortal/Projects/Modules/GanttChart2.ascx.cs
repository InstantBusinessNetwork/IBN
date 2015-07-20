using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Mediachase.IBN.Business;
using System.Resources;
using System.Reflection;
using Mediachase.UI.Web.Modules;
using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Projects.Modules
{
	public partial class GanttChart2 : System.Web.UI.UserControl
	{

		private IFormatProvider _culture = CultureInfo.InvariantCulture;
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strGanttChart", Assembly.GetExecutingAssembly());
		private string _iFrameHeight = "0";
		private int _itemPos = 0;
		protected bool isMSProject = false;

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

		#region BasePlanSlotId
		public int BasePlanSlotId
		{
			get
			{
				if (Request["BasePlanSlotId"] != null)
				{
					return int.Parse(Request["BasePlanSlotId"].ToString());
				}
				else
					return -1;
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

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Gantt.css");
			UtilHelper.RegisterScript(Page, "~/Scripts/ganttScript2.js");

			isMSProject = Project.GetIsMSProject(ProjectId);

			object startDate = Task.GetMinStartDate(ProjectId);

			ibtnApply.Text = LocRM.GetString("Apply");
			ibtnApply.CustomImage = this.Page.ResolveUrl("~/layouts/images/accept.gif");
			ltCompareWith.Text = LocRM.GetString("CompareWith");

			lbStartPrj.Text = LocRM.GetString("tPrjStart");
			lbEndPrj.Text = LocRM.GetString("tPrjEnd");
			lbToday.Text = LocRM.GetString("tToday");

			grdTasks.Columns[5].HeaderText = LocRM.GetString("tTitle");
			grdTasks.Columns[11].HeaderText = LocRM.GetString("StartDate");
			grdTasks.Columns[12].HeaderText = LocRM.GetString("FinishDate");


			if (Request.Browser.Browser.IndexOf("IE") >= 0)
			{
				grdTasks.HeaderStyle.Height = Unit.Pixel(GanttManager.HeaderHeight - 1);
				grdTasks.ItemStyle.Height = Unit.Pixel(GanttManager.ItemHeight - 1);
			}
			else
			{
				grdTasks.HeaderStyle.Height = Unit.Pixel(GanttManager.HeaderHeight);
				grdTasks.ItemStyle.Height = Unit.Pixel(GanttManager.ItemHeight);
			}
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack || CHelper.NeedToBindGrid())
			{
				BindTasks();
				BindBasePlans();
			}

			grdTasks.Columns[11].Visible = (_pc["GanttView"] == "1");
			grdTasks.Columns[12].Visible = (_pc["GanttView"] == "1");

			int lx = -1;
			if (_pc["GantChart_Lx_New_" + ProjectId.ToString()] != null)
				lx = int.Parse(_pc["GantChart_Lx_New_" + ProjectId.ToString()]);
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "f_onload(" + lx + ", " + GanttManager.PortionWidth + ");", true);
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.grdTasks.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdTasks_Delete);
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
			this.ibtnApply.ServerClick += new EventHandler(ibtnApply_ServerClick);
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

			if (pageIndex < ppi || pageIndex == ppi && fh == 0)
				fh = grdTasks.PageSize;

			fh = fh * GanttManager.ItemHeight + GanttManager.HeaderHeight;

			_iFrameHeight = fh.ToString();

			grdTasks.DataSource = dt.DefaultView;
			grdTasks.DataBind();
			if (!CanEdit)
				grdTasks.Columns[grdTasks.Columns.Count - 2].Visible = false;

			divImg.Style.Add("HEIGHT", _iFrameHeight + "px");
			hdnLinkToGantt.Value = String.Format("../Projects/GanttChartImage2.aspx?ProjectID={0}&PageItems={1}&PageNumber={2}&Days=366&RenderPortion=1&BasePlanSlotId={3}", ProjectId, grdTasks.PageSize, grdTasks.CurrentPageIndex, BasePlanSlotId);
		}
		#endregion

		#region BindBasePlans
		protected void BindBasePlans()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("BasePlanSlotId", typeof(int)));
			Hashtable ht = ProjectSpreadSheet.GetFilledSlotHash(int.Parse(Request["ProjectId"]));
			if (ht.Keys.Count <= 0)
			{
				trBasePlanFilter.Visible = false;
			}

			foreach (BasePlanSlot bps in BasePlanSlot.List())
			{
				DataRow row = dt.NewRow();

				row["BasePlanSlotId"] = bps.BasePlanSlotId;
				DateTime d_time = DateTime.MinValue;
				if (ht.Contains(bps.BasePlanSlotId))
				{

					foreach (BasePlan bp in BasePlan.List(int.Parse(Request["ProjectId"])))
					{
						if (bp.BasePlanSlotId == bps.BasePlanSlotId)
						{
							d_time = bp.Created;
							break;
						}
					}
				}
				if (d_time != DateTime.MinValue)
					row["Name"] = String.Format("{0} ({1}: {2})", bps.Name, LocRM.GetString("LastSaved"), d_time);
				else
					continue;
				dt.Rows.Add(row);
			}

			ddlBasePlans.DataSource = dt;
			ddlBasePlans.DataTextField = "Name";
			ddlBasePlans.DataValueField = "BasePlanSlotId";
			ddlBasePlans.DataBind();
			ddlBasePlans.Items.Insert(0, new ListItem(LocRM.GetString("NotSet"), "-1"));
			ddlBasePlans.SelectedValue = BasePlanSlotId.ToString();

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
					cvUnableMove.ErrorMessage = @"<table border=0 cellpadding=5 class='ibn-alerttext'><tr><td><img align='absmiddle' border=0 src='" + ResolveUrl("~/layouts/images/warning.gif") + "' width='16' height='16'>&nbsp;" + LocRM.GetString("TaskUnmovable") + "</td></tr></table>";
					cvUnableMove.IsValid = false;
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

				// «начки +/-
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

				// —двиг влево
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

				// —двиг вправо
				bool isFirstChild = false;
				if (outlineNumber == "1" || (outlineNumber.Length > 2 && outlineNumber.Substring(outlineNumber.Length - 2) == ".1"))
					isFirstChild = true;

				if (!isFirstChild && CanEdit && !isMSProject)	// если это первый дочерний, то вправо сдвигать нельз€
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
					"{0}{1}<a href='../Tasks/TaskView.aspx?TaskId={2}' title='{3}'{4}>{5}</a>{6}{7}",
					divPrefix, prefix, taskId, title, addStyle, tit, postfix, divPostfix);

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

		#region grdTasks_Delete
		private void grdTasks_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
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
			Response.Redirect("../Projects/ProjectView.aspx?ProjectId=" + ProjectId);
		}

		private void lbToday_Click(object sender, EventArgs e)
		{
			_pc["GantChart_Lx_New_" + ProjectId.ToString()] = (GanttManager.GetPortionX(ProjectId, UserDateTime.UserToday) - 1).ToString();
			Response.Redirect("../Projects/ProjectView.aspx?ProjectId=" + ProjectId);
		}

		private void lbEndPrj_Click(object sender, EventArgs e)
		{
			_pc["GantChart_Lx_New_" + ProjectId.ToString()] = (GanttManager.GetPortionX(ProjectId, Task.GetMaxFinishDate(ProjectId)) - 1).ToString();
			Response.Redirect("../Projects/ProjectView.aspx?ProjectId=" + ProjectId);
		}
		#endregion

		#region ibtnApply_ServerClick
		private void ibtnApply_ServerClick(object sender, EventArgs e)
		{
			if (PortalConfig.ProjectViewControl == String.Empty || PortalConfig.ProjectViewControl == PortalConfig.ProjectViewControlDefaultValue)
				Response.Redirect(String.Format("ProjectView.aspx?ProjectId={0}&Tab=6&SubTab=GanttChart2&BasePlanSlotId={1}", ProjectId, ddlBasePlans.SelectedValue), true);
			else
				Response.Redirect(String.Format("ProjectView.aspx?ProjectId={0}&Tab=tabGant&BasePlanSlotId={1}", ProjectId, ddlBasePlans.SelectedValue), true);
		}
		#endregion
	}
}