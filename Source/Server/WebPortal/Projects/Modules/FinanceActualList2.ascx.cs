namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.SpreadSheet;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.UI;

	/// <summary>
	///		Summary description for FincanceActualList2.
	/// </summary>
	public partial class FinanceActualList2 : System.Web.UI.UserControl
	{
		protected Mediachase.UI.Web.Modules.BlockHeader secHeader;
		IFormatProvider culture = CultureInfo.InvariantCulture;

		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(FinanceActualList2).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(FinanceActualList2).Assembly);

		#region IDs
		private int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int TaskId
		{
			get
			{
				try
				{
					return int.Parse(Request["TaskId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int IncidentId
		{
			get
			{
				try
				{
					return int.Parse(Request["IncidentId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int DocumentId
		{
			get
			{
				try
				{
					return int.Parse(Request["DocumentId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int EventId
		{
			get
			{
				try
				{
					return int.Parse(Request["EventId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int ToDoId
		{
			get
			{
				try
				{
					return int.Parse(Request["ToDoId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		private int ParentProjectId = -1;
		#endregion

		#region prop: ObjectId
		private int ObjectId
		{
			get
			{
				if (ProjectId > -1) return ProjectId;
				else if (TaskId > -1) return TaskId;
				else if (IncidentId > -1) return IncidentId;
				else if (DocumentId > -1) return DocumentId;
				else if (EventId > -1) return EventId;
				else if (ToDoId > -1) return ToDoId;
				else return -1;
			}
		}
		#endregion

		#region prop: ObjectTypeId
		private int ObjectTypeId
		{
			get
			{
				if (ProjectId > -1) return (int)ObjectTypes.Project;
				else if (TaskId > -1) return (int)ObjectTypes.Task;
				else if (IncidentId > -1) return (int)ObjectTypes.Issue;
				else if (DocumentId > -1) return (int)ObjectTypes.Document;
				else if (EventId > -1) return (int)ObjectTypes.CalendarEntry;
				else if (ToDoId > -1) return (int)ObjectTypes.ToDo;
				else return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (pc["FinAct_Sort"] == null)
				pc["FinAct_Sort"] = "ActualDate";

			DefineParentProjectId();
			ApplyLocalization();

			int ProjectId = Util.CommonHelper.GetProjectIdByObjectIdObjectType(this.ObjectId, this.ObjectTypeId);

			if (!IsPostBack)
				BindValues();

			if (!Page.IsPostBack && ProjectSpreadSheet.IsActive(ProjectId))
				BindDG(dgAccounts);
			BindToolbar();
		}

		#region DefineParentProjectId
		private void DefineParentProjectId()
		{
			if (ProjectId > 0)
				ParentProjectId = ProjectId;
			else if (TaskId > 0)
				ParentProjectId = Task.GetProject(TaskId);
			else if (IncidentId > 0)
				using (IDataReader reader = Incident.GetIncident(IncidentId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
			else if (DocumentId > 0)
				using (IDataReader reader = Document.GetDocument(DocumentId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
			else if (EventId > 0)
				using (IDataReader reader = CalendarEntry.GetEvent(EventId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
			else if (ToDoId > 0)
				using (IDataReader reader = ToDo.GetToDo(ToDoId))
				{
					if (reader.Read() && reader["ProjectId"] != DBNull.Value)
						ParentProjectId = (int)reader["ProjectId"];
				}
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			// Dates
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_Any}"), "[DateTimeThisAny]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ThisWeek}"), "[DateTimeThisWeek]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_LastWeek}"), "[DateTimeLastWeek]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ThisMonth}"), "[DateTimeThisMonth]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_LastMonth}"), "[DateTimeLastMonth]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ThisYear}"), "[DateTimeThisYear]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_LastYear}"), "[DateTimeLastYear]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_CustomWeek}"), "0"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_CustomPeriod}"), "-1"));

			Dtc0.SelectedDate = CHelper.GetRealWeekStartByDate(DateTime.Today);
			Dtc1.SelectedDate = CHelper.GetRealWeekStartByDate(DateTime.Today);
			Dtc2.SelectedDate = CHelper.GetRealWeekEndByDate(DateTime.Today);

			Dtc0.Visible = false;
			Dtc1.Visible = false;
			Dtc2.Visible = false;
			DashLabel.Visible = false;
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			dgAccounts.Columns[2].HeaderText = LocRM.GetString("Date");
			dgAccounts.Columns[3].HeaderText = LocRM.GetString("tAccount");
			dgAccounts.Columns[4].HeaderText = String.Format(CultureInfo.InvariantCulture,
				"{0} / {1}", 
				LocRM.GetString("tObjTitle"),
				LocRM.GetString("Description"));
			dgAccounts.Columns[5].HeaderText = LocRM2.GetString("tWorkResourcesWorkTime");
			dgAccounts.Columns[6].HeaderText = LocRM.GetString("Actual");
			dgAccounts.Columns[7].HeaderText = LocRM2.GetString("tResource");
			dgAccounts.Columns[8].HeaderText = LocRM.GetString("tModifiedBy");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			//DV: Esli netu userskih row v spreadshhete proekta to dobavit actual finansy nevozmozhno
			if (ProjectSpreadSheet.GetFactAvailableRows(Util.CommonHelper.GetProjectIdByObjectIdObjectType(ObjectId, ObjectTypeId)).Length == 0)
				return;

			if (this.Parent.Parent is IToolbarLight)
			{
				BlockHeaderLightWithMenu secHeaderLight = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();
				secHeaderLight.ActionsMenu.Items.Clear();
				secHeaderLight.ClearRightItems();

				if (ProjectId < 0)
					secHeaderLight.AddText(LocRM.GetString("tActFinances"));

				ComponentArt.Web.UI.MenuItem subItem;

				#region New Item
				string command = String.Empty;

				if (ProjectId > 0 && Project.CanEditFinances(ProjectId))
					command = String.Format(CultureInfo.InvariantCulture, "OpenWindow(\"{0}?ObjectId={1}&ObjectTypeId={2}&btn={3}\",520,220,false);",
						ResolveClientUrl("~/projects/AddFinanceActual.aspx"), ProjectId, (int)ObjectTypes.Project, Page.ClientScript.GetPostBackEventReference(RefreshButton, ""));
				if (TaskId > 0 && Task.CanViewFinances(TaskId))
					command = String.Format(CultureInfo.InvariantCulture, "OpenWindow(\"{0}?ObjectId={1}&ObjectTypeId={2}&btn={3}\",520,220,false);",
						ResolveClientUrl("~/projects/AddFinanceActual.aspx"), TaskId, (int)ObjectTypes.Task, Page.ClientScript.GetPostBackEventReference(RefreshButton, ""));
				if (IncidentId > 0 && Incident.CanViewFinances(IncidentId))
					command = String.Format(CultureInfo.InvariantCulture, "OpenWindow(\"{0}?ObjectId={1}&ObjectTypeId={2}&btn={3}\",520,220,false);",
						ResolveClientUrl("~/projects/AddFinanceActual.aspx"), IncidentId, (int)ObjectTypes.Issue, Page.ClientScript.GetPostBackEventReference(RefreshButton, ""));
				if (DocumentId > 0 && Document.CanViewFinances(DocumentId))
					command = String.Format(CultureInfo.InvariantCulture, "OpenWindow(\"{0}?ObjectId={1}&ObjectTypeId={2}&btn={3}\",520,220,false);",
						ResolveClientUrl("~/projects/AddFinanceActual.aspx"), DocumentId, (int)ObjectTypes.Document, Page.ClientScript.GetPostBackEventReference(RefreshButton, ""));
				if (EventId > 0 && CalendarEntry.CanViewFinances(EventId))
					command = String.Format(CultureInfo.InvariantCulture, "OpenWindow(\"{0}?ObjectId={1}&ObjectTypeId={2}&btn={3}\",520,220,false);",
						ResolveClientUrl("~/projects/AddFinanceActual.aspx"), EventId, (int)ObjectTypes.CalendarEntry, Page.ClientScript.GetPostBackEventReference(RefreshButton, ""));
				if (ToDoId > 0 && Mediachase.IBN.Business.ToDo.CanViewFinances(ToDoId))
					command = String.Format(CultureInfo.InvariantCulture, "OpenWindow(\"{0}?ObjectId={1}&ObjectTypeId={2}&btn={3}\",520,220,false);",
						ResolveClientUrl("~/projects/AddFinanceActual.aspx"), ToDoId, (int)ObjectTypes.ToDo, Page.ClientScript.GetPostBackEventReference(RefreshButton, ""));

				if (command != String.Empty)
				{
					subItem = new ComponentArt.Web.UI.MenuItem();
					subItem.LookId = "TopItemLook";
					subItem.Look.LeftIconUrl = "~/Layouts/Images/newitem.gif";
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.ClientSideCommand = command;
					subItem.Text = LocRM.GetString("tbAdd");
					secHeaderLight.ActionsMenu.Items.Add(subItem);
				}
				#endregion

				#region Export
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.LookId = "TopItemLook";
				subItem.Look.LeftIconUrl = "~/Layouts/Images/Icons/xlsexport.gif";
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.ClientSideCommand = Page.ClientScript.GetPostBackEventReference(ExportButton, "");
				subItem.Text = LocRM2.GetString("ExcelExport");
				secHeaderLight.ActionsMenu.Items.Add(subItem);
				#endregion
			}
			else if (this.Parent is IToolbarLight)
			{
				BlockHeaderLightWithMenu secHeaderLight = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent).GetToolBar();
				secHeaderLight.AddText(LocRM.GetString("tActFinances"));

				if (Project.CanEditFinances(ProjectId) && ProjectId > 0)
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>&nbsp;" + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../projects/AddFinanceActual.aspx?ObjectId=" + ProjectId.ToString() + "&ObjectTypeId=" + (int)ObjectTypes.Project + "',520,270,false);");
				if (TaskId > 0 && Task.CanViewFinances(TaskId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddFinanceActual.aspx?ObjectId=" + TaskId + "&ObjectTypeId=" + (int)ObjectTypes.Task + "',520,270,false);");
				if (IncidentId > 0 && Incident.CanViewFinances(IncidentId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddFinanceActual.aspx?ObjectId=" + IncidentId + "&ObjectTypeId=" + (int)ObjectTypes.Issue + "',520,270,false);");
				if (DocumentId > 0 && Document.CanViewFinances(DocumentId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddFinanceActual.aspx?ObjectId=" + DocumentId + "&ObjectTypeId=" + (int)ObjectTypes.Document + "',520,270,false);");
				if (EventId > 0 && CalendarEntry.CanViewFinances(EventId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddFinanceActual.aspx?ObjectId=" + EventId + "&ObjectTypeId=" + (int)ObjectTypes.CalendarEntry + "',520,270,false);");
				if (ToDoId > 0 && Mediachase.IBN.Business.ToDo.CanViewFinances(ToDoId))
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/newitem.gif'/>  " + LocRM.GetString("tbAdd"), "javascript:OpenWindow('../Projects/AddFinanceActual.aspx?ObjectId=" + ToDoId + "&ObjectTypeId=" + (int)ObjectTypes.ToDo + "',520,270,false);");
			}
		}
		#endregion

		#region BindDG
		private void BindDG(DataGrid dg)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("ActualId", typeof(int)));
			dt.Columns.Add(new DataColumn("OutlineLevel", typeof(int)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));
			dt.Columns.Add(new DataColumn("ActualDate", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("AValue", typeof(double)));
			dt.Columns.Add(new DataColumn("LastEditorId", typeof(int)));
			dt.Columns.Add(new DataColumn("LastEditorName", typeof(string)));
			dt.Columns.Add(new DataColumn("ObjectTypeId", typeof(int)));
			dt.Columns.Add(new DataColumn("ObjectId", typeof(int)));
			dt.Columns.Add(new DataColumn("RowId", typeof(string)));
			dt.Columns.Add(new DataColumn("BlockId", typeof(int)));
			dt.Columns.Add(new DataColumn("TotalApproved", typeof(string)));
			dt.Columns.Add(new DataColumn("OwnerDisplayName", typeof(string)));
			dt.Columns.Add(new DataColumn("OwnerName", typeof(string)));

			if (ObjectId > 0)
			{
				int projectId = Util.CommonHelper.GetProjectIdByObjectIdObjectType(ObjectId, ObjectTypeId);
				bool projectSpreadSheetIsActive = ProjectSpreadSheet.IsActive(projectId);
				Hashtable rowNameHashtable = ProjectSpreadSheet.GetRowNameByIdHash(projectId);

				#region dates
				DateTime? dt1 = null;
				DateTime? dt2 = null;
				switch (PeriodList.SelectedValue)
				{
					case "[DateTimeThisAny]":
						break;
					case "[DateTimeThisWeek]":
						dt1 = CHelper.GetRealWeekStartByDate(DateTime.Today);
						dt2 = CHelper.GetRealWeekEndByDate(DateTime.Today);
						break;
					case "[DateTimeLastWeek]":
						dt1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(-7));
						dt2 = CHelper.GetRealWeekEndByDate(DateTime.Today.AddDays(-7));
						break;
					case "[DateTimeThisMonth]":
						dt1 = DateTime.Today.AddDays(1 - DateTime.Today.Day);
						dt2 = DateTime.Today;
						break;
					case "[DateTimeLastMonth]":
						dt1 = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(-1);
						dt2 = DateTime.Today.AddDays(-DateTime.Today.Day);
						break;
					case "[DateTimeThisYear]":
						dt1 = DateTime.Today.AddDays(1 - DateTime.Today.DayOfYear);
						dt2 = DateTime.Today;
						break;
					case "[DateTimeLastYear]":
						dt1 = DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear).AddYears(-1);
						dt2 = DateTime.Today.AddDays(-DateTime.Now.DayOfYear);
						break;
					case "0":
						dt1 = Dtc0.SelectedDate;
						dt2 = CHelper.GetRealWeekEndByDate(Dtc0.SelectedDate);
						break;
					case "-1":
						dt1 = Dtc1.SelectedDate;
						dt2 = Dtc2.SelectedDate;
						break;
					default:
						break;
				}
				#endregion

				foreach (ActualFinances af in ActualFinances.List(ObjectId, (ObjectTypes)ObjectTypeId, dt1, dt2))
				{
					DataRow row = dt.NewRow();
					row["ActualId"] = af.ActualFinancesId;
					row["Description"] = af.Comment;
					row["ActualDate"] = af.Date;
					row["AValue"] = af.Value;
					row["LastEditorId"] = af.CreatorId;
					row["LastEditorName"] = Util.CommonHelper.GetUserStatusPureName(af.CreatorId);
					row["OutlineLevel"] = 1;
					row["ObjectTypeId"] = af.ObjectTypeId;
					row["ObjectId"] = af.ObjectId;
					if (projectSpreadSheetIsActive && rowNameHashtable.ContainsKey(af.RowId))
						row["RowId"] = rowNameHashtable[af.RowId].ToString();
					else
						row["RowId"] = string.Empty;

					if (af.BlockId.HasValue)
						row["BlockId"] = af.BlockId.Value;

					if (af.TotalApproved.HasValue)
					{
						if (dg == dgExport)
							row["TotalApproved"] = (int)af.TotalApproved.Value;
						else
							row["TotalApproved"] = CommonHelper.GetHours((int)af.TotalApproved.Value);
					}

					if (af.OwnerId.HasValue)
					{
						row["OwnerDisplayName"] = Util.CommonHelper.GetUserStatus(af.OwnerId.Value);
						if (dgExport.Visible)
							row["OwnerName"] = Util.CommonHelper.GetUserStatusAndPositionPureName(af.OwnerId.Value);
						else
							row["OwnerName"] = Util.CommonHelper.GetUserStatusPureName(af.OwnerId.Value);
					}

					dt.Rows.Add(row);
				}

			}
			/*else if(TaskId>0)
				dt = Finance.GetListActualFinancesByTask(TaskId);
			else if(IncidentId>0)
				dt = Finance.GetListActualFinancesByIncident(IncidentId);
			else if(DocumentId>0)
				dt = Finance.GetListActualFinancesByDocument(DocumentId);
			else if(EventId>0)
				dt = Finance.GetListActualFinancesByEvent(EventId);
			else if(ToDoId>0)
				dt = Finance.GetListActualFinancesByToDo(ToDoId);*/

			DataView dv = dt.DefaultView;
			dv.Sort = pc["FinAct_Sort"].ToString();

			if (pc["FinAct_PageSize"] != null)
				dg.PageSize = int.Parse(pc["FinAct_PageSize"]);

			if (pc["FinAct_Page"] != null)
				dg.CurrentPageIndex = int.Parse(pc["FinAct_Page"]);

			int pageindex = dg.CurrentPageIndex;
			int ppi = dv.Count / dg.PageSize;
			if (dv.Count % dg.PageSize == 0)
				ppi = ppi - 1;

			if (pageindex <= ppi)
			{
				dg.CurrentPageIndex = pageindex;
			}
			else
			{
				dg.CurrentPageIndex = 0;
				pc["FinAct_Page"] = "0";
			}

			dg.DataSource = dv;
			dg.DataBind();

			bool haveRights = false;
			if (ProjectId > 0)
				haveRights = Project.CanEditFinances(ProjectId);
			if (TaskId > 0)
				haveRights = Task.CanViewFinances(TaskId);
			if (IncidentId > 0)
				haveRights = Incident.CanViewFinances(IncidentId);
			if (DocumentId > 0)
				haveRights = Document.CanViewFinances(DocumentId);
			if (EventId > 0)
				haveRights = CalendarEntry.CanViewFinances(EventId);
			if (ToDoId > 0)
				haveRights = Mediachase.IBN.Business.ToDo.CanViewFinances(ToDoId);

			foreach (DataGridItem dgi in dg.Items)
			{
				if (dgi.FindControl("ibDelete") != null)
				{
					ImageButton ibDelete = (ImageButton)dgi.FindControl("ibDelete");
					ibDelete.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "')");
				}

				if (dgi.FindControl("ibEdit") != null)
				{

					ImageButton ibEdit = (ImageButton)dgi.FindControl("ibEdit");

					string link = string.Empty;
					if (ObjectId > 0 && haveRights)
						link = String.Format(CultureInfo.InvariantCulture,
							"javascript:OpenWindow(\"{0}?ObjectId={1}&ObjectTypeId={2}&ActualFinancesId={3}&btn={4}\",520,270,false);return false;",
							ResolveClientUrl("~/projects/AddFinanceActual.aspx"), 
							ObjectId, 
							ObjectTypeId, dgi.Cells[0].Text,
							Page.ClientScript.GetPostBackEventReference(RefreshButton, ""));
					ibEdit.Attributes.Add("onclick", link);
				}
			}
			//			if(!Project.CanEditFinances(ProjectId))
			//				dg.Columns[10].Visible = false;
		}
		#endregion

		#region GetObjectLink
		protected string GetObjectLink(int objectTypeId, int objectId, object blockId)
		{
			string retval = Mediachase.UI.Web.Util.CommonHelper.GetObjectLinkAndTitle(objectTypeId, objectId);

			if (blockId != DBNull.Value)
				retval += String.Format(CultureInfo.InvariantCulture, " <span class='text-readonly'>({0})</span>", GetGlobalResourceObject("IbnFramework.TimeTracking", "ExpendedTime").ToString());
			return retval;
		}
		#endregion

		#region GetObjectTitle
		protected string GetObjectTitle(int objectTypeId, int objectId, object blockId)
		{
			string retval = Mediachase.UI.Web.Util.CommonHelper.GetObjectTitle(objectTypeId, objectId);

			if (blockId != DBNull.Value)
				retval += String.Format(CultureInfo.InvariantCulture, " ({0})", GetGlobalResourceObject("IbnFramework.TimeTracking", "ExpendedTime").ToString()); ;
			return retval;
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
			this.dgAccounts.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dg_PageChange);
			this.dgAccounts.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(this.dg_PageSizeChange);
			this.dgAccounts.DeleteCommand += new DataGridCommandEventHandler(dgAccounts_DeleteCommand);
			this.dgAccounts.SortCommand += new DataGridSortCommandEventHandler(dgAccounts_SortCommand);
		}
		#endregion

		#region DataGrid_Events
		private void dg_PageChange(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			pc["FinAct_Page"] = e.NewPageIndex.ToString();
			BindDG(dgAccounts);
		}

		private void dg_PageSizeChange(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["FinAct_PageSize"] = e.NewPageSize.ToString();
			BindDG(dgAccounts);
		}

		private void dgAccounts_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int ActualId = int.Parse(e.CommandArgument.ToString());
			//Finance.DeleteActualFinances(ActualId);
			ActualFinances.Delete(ActualId);
			BindDG(dgAccounts);
		}

		void dgAccounts_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if ((pc["FinAct_Sort"] != null) && (pc["FinAct_Sort"].ToString() == (string)e.SortExpression))
				pc["FinAct_Sort"] = (string)e.SortExpression + " DESC";
			else
				pc["FinAct_Sort"] = (string)e.SortExpression;
			BindDG(dgAccounts);
		}
		#endregion

		#region Export
		protected void ExportButton_Click(object sender, EventArgs e)
		{
			dgExport.Columns[0].HeaderText = LocRM.GetString("Date");
			dgExport.Columns[1].HeaderText = LocRM.GetString("tAccount");
			dgExport.Columns[2].HeaderText = LocRM.GetString("Description");
			dgExport.Columns[3].HeaderText = LocRM.GetString("tObjTitle");
			dgExport.Columns[4].HeaderText = LocRM2.GetString("tWorkResourcesWorkTime");
			dgExport.Columns[5].HeaderText = LocRM.GetString("Actual");
			dgExport.Columns[6].HeaderText = LocRM2.GetString("tResource");

			dgExport.Visible = true;
			BindDG(dgExport);
			CommonHelper.ExportExcel(dgExport, "FinanceReport.xls", null);
		}
		#endregion

		#region PeriodList_SelectedIndexChanged
		protected void PeriodList_SelectedIndexChanged(object sender, EventArgs e)
		{
			Dtc0.Visible = false;
			Dtc1.Visible = false;
			Dtc2.Visible = false;
			DashLabel.Visible = false;

			switch (PeriodList.SelectedValue)
			{
				case "0":
					Dtc0.Visible = true;
					break;
				case "-1":
					Dtc1.Visible = true;
					Dtc2.Visible = true;
					DashLabel.Visible = true;
					break;
				default:
					break;
			}
			BindDG(dgAccounts);
		}
		#endregion


		#region Dtc_ValueChange
		protected void Dtc_ValueChange(object sender, EventArgs e)
		{
			BindDG(dgAccounts);
		}
		#endregion

		#region RefreshButton_Click
		protected void RefreshButton_Click(object sender, EventArgs e)
		{
			BindDG(dgAccounts);
		}
		#endregion
	}
}
