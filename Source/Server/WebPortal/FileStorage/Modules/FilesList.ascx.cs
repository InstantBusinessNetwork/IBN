using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using ComponentArt.Web.UI;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.UI.Web.Modules;
using Mediachase.IBN.Business.WebDAV.Common;


namespace Mediachase.UI.Web.FileStorage.Modules
{
	/// <summary>
	///		Summary description for FilesList.
	/// </summary>
	public partial class FilesList : System.Web.UI.UserControl
	{
		private UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		private IFormatProvider _culture = CultureInfo.InvariantCulture;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryList", typeof(FilesList).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryAdvanced", typeof(FilesList).Assembly);

		private BaseIbnContainer _bic;
		private Mediachase.IBN.Business.ControlSystem.FileStorage _fs;

		#region _containerKey, _containerName
		private string ck = null;
		protected string _containerKey
		{
			get
			{
				if (ck == null)
				{
					if (Request["ProjectId"] != null)
						ck = "ProjectId_" + Request["ProjectId"];
					else if (Request["IncidentId"] != null)
						ck = "IncidentId_" + Request["IncidentId"];
					else if (Request["TaskId"] != null)
						ck = "TaskId_" + Request["TaskId"];
					else if (Request["DocumentId"] != null)
						ck = "DocumentId_" + Request["DocumentId"];
					else if (Request["EventId"] != null)
						ck = "EventId_" + Request["EventId"];
					else if (Request["ToDoId"] != null)
					{
						using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetToDo(int.Parse(Request["ToDoId"]), false))
						{
							if (reader.Read())
							{
								if (reader["TaskId"] != DBNull.Value)
									ck = "TaskId_" + reader["TaskId"].ToString();
								else if (reader["DocumentId"] != DBNull.Value)
									ck = "DocumentId_" + reader["DocumentId"].ToString();
								else
									ck = "ToDoId_" + Request["ToDoId"];
							}
							else
								ck = "ToDoId_" + Request["ToDoId"];
						}
					}
					else
						ck = "Workspace";
				}
				return ck;
			}
		}

		private string _containerName
		{
			get
			{
				return "FileLibrary";
			}
		}
		#endregion

		#region _typeId
		private int _typeId
		{
			get
			{
				if (Request["TypeId"] != null)
					return int.Parse(Request["TypeId"]);
				else
					return -1;
			}
		}
		#endregion

		#region Top
		private int _top = 0;
		public int Top
		{
			get
			{
				return _top;
			}
			set
			{
				_top = value;
			}
		}
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			_bic = BaseIbnContainer.Create(_containerName, _containerKey);
			_fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)_bic.LoadControl("FileStorage");
			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				if (_pc["fl_Sort_" + _containerKey] == null)
					_pc["fl_Sort_" + _containerKey] = "sortName";

				if (_pc["fs_List_ViewStyle"] == null)
					_pc["fs_List_ViewStyle"] = "ListView";

				if (Top <= 0)
					BindDefaultValues();
				else
					trFilter.Visible = false;
			}
			if (Top <= 0)
			{
				BindSavedValues();
				BindInfoTable();
			}
			BindList();

			BindToolbar();
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (Top <= 0)
			{
				if (ddPeriod.Value == "9")
					tableDate.Style.Add("display", "block");
				else
					tableDate.Style.Add("display", "none");

				FilterTable.Visible = (_pc["ShowFilesFilter"] != null && bool.Parse(_pc["ShowFilesFilter"]));
				tblFilterInfo.Visible = !FilterTable.Visible;
			}

			if (Top > 0 && grdMain.Items.Count == 0)
				this.Parent.Visible = false;
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnApply.Value = LocRM.GetString("tApply");
			btnReset.Value = LocRM.GetString("tReset");
			lbShowFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("ShowFilter") + "' src='../Layouts/Images/scrolldown_hover.GIF' />";
			lbHideFilter.Text = "<img align='absmiddle' border='0' title='" + LocRM.GetString("HideFilter") + "' src='../Layouts/Images/scrollup_hover.GIF' />";
			lblFilterNotSet.Text = LocRM.GetString("FilterNotSet");
			btnReset2.Value = LocRM.GetString("ResetFilter");
		}
		#endregion

		#region BindInfoTable
		private void BindInfoTable()
		{
			int rowCount = 0;
			tblFilterInfoSet.Rows.Clear();

			// Type
			if (_pc["fl_ContType" + _containerKey] != null)
			{
				ListItem li = ddContType.Items.FindByValue(_pc["fl_ContType" + _containerKey]);
				if (li != null && li.Value != "0")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tContentType")), li.Text);
					rowCount++;
				}
			}

			// Period
			if (_pc["fl_ddPeriod" + _containerKey] != null)
			{
				ListItem li = ddPeriod.Items.FindByValue(_pc["fl_ddPeriod" + _containerKey]);
				if (li != null && li.Value != "0" && li.Value != "9")
				{
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tPeriod")), li.Text);
					rowCount++;
				}
				else if (li != null && li.Value == "9" && _pc["fl_Start" + _containerKey] != null && _pc["fl_End" + _containerKey] != null)
				{
					DateTime _date1 = DateTime.Parse(_pc["fl_Start" + _containerKey], _culture);
					DateTime _date2 = DateTime.Parse(_pc["fl_End" + _containerKey], _culture);
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tPeriod")), _date1.ToShortDateString() + "&nbsp;-&nbsp;" + _date2.ToShortDateString());
					rowCount++;
				}
			}

			if (rowCount > 0)
			{
				lblFilterNotSet.Visible = false;
				btnReset2.Visible = true;
			}
			else
			{
				lblFilterNotSet.Visible = true;
				btnReset2.Visible = false;
			}
		}

		private void AddRow(string filterName, string filterValue)
		{
			HtmlTableRow tr = new HtmlTableRow();
			HtmlTableCell td1 = new HtmlTableCell();
			td1.Align = "Right";
			td1.Width = "120px";
			td1.InnerHtml = filterName;
			HtmlTableCell td2 = new HtmlTableCell();
			td2.InnerHtml = filterValue;
			tr.Cells.Add(td1);
			tr.Cells.Add(td2);
			tblFilterInfoSet.Rows.Add(tr);
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			DataView dv = ContentType.GetListContentTypesDataTable().DefaultView;
			dv.Sort = "FriendlyName";

			ddContType.DataSource = dv;
			ddContType.DataTextField = "FriendlyName";
			ddContType.DataValueField = "ContentTypeId";
			ddContType.DataBind();
			ListItem lItem = new ListItem(LocRM.GetString("tAll"), "0");
			ddContType.Items.Insert(0, lItem);

			ddPeriod.Items.Clear();
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tAllTime"), "0"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tToday"), "1"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tYesterday"), "2"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tThisWeek"), "3"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tLastWeek"), "4"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tThisMonth"), "5"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tLastMonth"), "6"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tThisYear"), "7"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tLastYear"), "8"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tCustom"), "9"));
			dtcStartDate.SelectedDate = UserDateTime.UserToday.AddMonths(-1);
			dtcEndDate.SelectedDate = UserDateTime.UserNow;
			tableDate.Style.Add("display", "none");
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			//Content Type
			if (_pc["fl_ContType" + _containerKey] != null)
				Util.CommonHelper.SafeSelect(ddContType, _pc["fl_ContType" + _containerKey]);

			//Period
			if (_pc["fl_ddPeriod" + _containerKey] != null)
			{
				ddPeriod.Value = _pc["fl_ddPeriod" + _containerKey];
				if (ddPeriod.Value == "9")
				{
					DateTime startDate = new DateTime(0);
					DateTime endDate = new DateTime(0);
					if (_pc["fl_Start" + _containerKey] != null)
					{
						DateTime start = DateTime.Parse(_pc["fl_Start" + _containerKey], _culture);
						dtcStartDate.SelectedDate = start;
					}
					if (_pc["fl_End" + _containerKey] != null)
					{
						DateTime end = DateTime.Parse(_pc["fl_End" + _containerKey], _culture);
						dtcEndDate.SelectedDate = end;
					}
				}
			}
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			if (ddContType.SelectedItem != null)
				_pc["fl_ContType" + _containerKey] = ddContType.SelectedItem.Value;
			_pc["fl_ddPeriod" + _containerKey] = ddPeriod.Value;
			if (ddPeriod.Value == "9")
			{
				DateTime startDate = new DateTime(0);
				DateTime endDate = new DateTime(0);
				SetDates(ddPeriod.Value, out startDate, out endDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());
				_pc["fl_Start" + _containerKey] = startDate.ToString(_culture);
				_pc["fl_End" + _containerKey] = endDate.ToString(_culture);
			}
		}
		#endregion

		#region BindList
		private void BindList()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("ContainerKey", typeof(string)));
			dt.Columns.Add(new DataColumn("ParentFolderId", typeof(int)));
			dt.Columns.Add(new DataColumn("Icon", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("sortName", typeof(string)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));
			dt.Columns.Add(new DataColumn("ModifiedDate", typeof(string)));
			dt.Columns.Add(new DataColumn("sortModified", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("Modifier", typeof(string)));
			dt.Columns.Add(new DataColumn("sortModifier", typeof(string)));
			dt.Columns.Add(new DataColumn("ActionVS", typeof(string)));
			dt.Columns.Add(new DataColumn("Size", typeof(string)));
			dt.Columns.Add(new DataColumn("sortSize", typeof(int)));
			dt.Columns.Add(new DataColumn("ContentType", typeof(string)));
			DataRow dr;

			#region BindDataTable
			int iContTypeId = 0;
			DateTime startDate = DateTime.MinValue;
			DateTime endDate = DateTime.MinValue;
			if (Top <= 0)
			{
				iContTypeId = int.Parse(ddContType.SelectedValue);
				string sVal = ddPeriod.Value;
				if (sVal == "9")
				{
					DateTime date = DateTime.Parse(_pc["fl_Start" + _containerKey], _culture);
					startDate = date;
					date = DateTime.Parse(_pc["fl_End" + _containerKey], _culture);
					endDate = date;
				}
				else
					SetDates(sVal, out startDate, out endDate, "", "");
			}

			FileInfo[] fMas;
			if (!_containerKey.StartsWith("Workspace") && !_containerKey.StartsWith("ProjectId_"))
				fMas = _fs.GetFiles(0, true, null, iContTypeId, startDate, endDate, 0, 0);
			else if (_containerKey == "Workspace" && _typeId < 0)
			{
				fMas = Mediachase.IBN.Business.ControlSystem.FileStorage.SearchFiles(Mediachase.IBN.Business.Security.CurrentUser.UserID,
					_containerKey, 0, true, null, iContTypeId, startDate, endDate, 0, 0);
			}
			else
			{
				int projectId = -1;
				if (Request["ProjectId"] != null)
					projectId = int.Parse(Request["ProjectId"]);
				fMas = Mediachase.IBN.Business.ControlSystem.FileStorage.SearchFiles(Mediachase.IBN.Business.Security.CurrentUser.UserID, projectId,
					_typeId, null, iContTypeId, startDate, endDate, 0, 0);
			}

			foreach (FileInfo fi in fMas)
			{
				dr = dt.NewRow();
				dr["Id"] = fi.Id;
				dr["ContainerKey"] = fi.ContainerKey;
				dr["ParentFolderId"] = fi.ParentDirectory.Id;
				dr["Icon"] = ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId);

				dr["Name"] = fi.Name;
				dr["sortName"] = fi.Name;
				dr["Description"] = fi.Description;
				dr["ModifiedDate"] = fi.Modified.ToShortDateString();
				dr["sortModified"] = fi.Modified;
				dr["Modifier"] = Util.CommonHelper.GetUserStatus(fi.ModifierId);
				dr["sortModifier"] = Util.CommonHelper.GetUserStatusPureName(fi.ModifierId);
				dr["Size"] = FormatBytes((long)fi.Length);
				dr["sortSize"] = fi.Length;
				dr["ActionVS"] = String.Format("<a href=\"{0}\"><img width='16' height='16' align='absmiddle' border='0' src='{1}' title='{2}'/></a>",
					String.Format("javascript:ViewFile('{0}','{1}',{2});", _containerName, fi.ContainerKey, fi.Id),
					ResolveUrl("~/layouts/images/icon-search.gif"),
					LocRM.GetString("tViewDetails"));
				dr["ContentType"] = fi.FileBinaryContentType;
				dt.Rows.Add(dr);
			}
			#endregion

			DataView dv = dt.DefaultView;
			if (Top > 0)
				dv.Sort = "sortModified DESC";
			else
				dv.Sort = _pc["fl_Sort_" + _containerKey].ToString();

			if (_pc["fs_List_ViewStyle"] == "ListView" || Top > 0)
			{
				#region ListView
				grdMain.Visible = true;
				grdDetails.Visible = false;

				int i = 4;
				grdMain.Columns[i++].HeaderText = LocRM.GetString("tName");
				grdMain.Columns[i++].HeaderText = LocRM.GetString("tLocation");
				grdMain.Columns[i++].HeaderText = LocRM.GetString("UpdatedBy");
				grdMain.Columns[i++].HeaderText = LocRM.GetString("UpdatedDate");
				grdMain.Columns[i++].HeaderText = LocRM.GetString("Size");

				foreach (DataGridColumn dgc in grdMain.Columns)
				{
					if (dgc.SortExpression == _pc["fl_Sort_" + _containerKey].ToString())
						dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
							ResolveUrl("~/layouts/images/upbtnF.jpg"));
					else if (dgc.SortExpression + " DESC" == _pc["fl_Sort_" + _containerKey].ToString())
						dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
							ResolveUrl("~/layouts/images/downbtnF.jpg"));
				}

				if (_pc["fl_PageSize_" + _containerKey] == null)
					_pc["fl_PageSize_" + _containerKey] = "10";
				grdMain.PageSize = int.Parse(_pc["fl_PageSize_" + _containerKey].ToString());

				if (_pc["fl_Page_" + _containerKey] == null)
					_pc["fl_Page_" + _containerKey] = "0";
				int pageIndex = int.Parse(_pc["fl_Page_" + _containerKey].ToString());
				int ppi = dv.Count / grdMain.PageSize;
				if (dv.Count % grdMain.PageSize == 0)
					ppi = ppi - 1;
				if (pageIndex <= ppi)
				{
					grdMain.CurrentPageIndex = pageIndex;
				}
				else
				{
					grdMain.CurrentPageIndex = 0;
					_pc["fl_Page_" + _containerKey] = "0";
				}

				grdMain.DataSource = dv;
				if (Top > 0)
				{
					grdMain.LayoutFixed = false;
					grdMain.Columns[5].Visible = false;
//					grdMain.Columns[6].ItemStyle.Width = Unit.Pixel(0);
					grdMain.Columns[7].ItemStyle.Width = Unit.Pixel(75);	// Created
					grdMain.Columns[grdMain.Columns.Count - 1].Visible = false;
					grdMain.Columns[grdMain.Columns.Count - 2].Visible = false;
					grdMain.Columns[grdMain.Columns.Count - 3].Visible = false;
				}
				else
					grdMain.Columns[6].Visible = false;

				grdMain.DataBind();

				if (Top > 0)
				{
					grdMain.PageSize = Top;
					grdMain.AllowPaging = false;
					grdMain.PagerStyle.Visible = false;
					grdMain.ShowHeader = false;
					for (i = 0; i < grdMain.Columns.Count; i++)
						grdMain.Columns[i].ItemStyle.CssClass = "ibn-propertysheet";
					grdMain.CellPadding = 2;
				}
				else
					foreach (DataGridItem dgi in grdMain.Items)
					{
						ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
						if (ib != null)
						{
							ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "');");
							ib.ToolTip = LocRM.GetString("tDelDoc");
						}
					}
				#endregion
			}
			else if (_pc["fs_List_ViewStyle"] == "DetailsView")
			{
				#region DetailsView
				grdMain.Visible = false;
				grdDetails.Visible = true;

				if (_pc["fl_PageSize_" + _containerKey] == null)
					_pc["fl_PageSize_" + _containerKey] = "10";
				grdDetails.PageSize = int.Parse(_pc["fl_PageSize_" + _containerKey].ToString());

				if (_pc["fl_Page_" + _containerKey] == null)
					_pc["fl_Page_" + _containerKey] = "0";
				int pageIndex = int.Parse(_pc["fl_Page_" + _containerKey].ToString());
				int ppi = dv.Count / grdDetails.PageSize;
				if (dv.Count % grdDetails.PageSize == 0)
					ppi = ppi - 1;
				if (pageIndex <= ppi)
				{
					grdDetails.CurrentPageIndex = pageIndex;
				}
				else
				{
					grdDetails.CurrentPageIndex = 0;
					_pc["fl_Page_" + _containerKey] = "0";
				}

				grdDetails.DataSource = dv;
				if (Top > 0)
				{
					grdDetails.LayoutFixed = false;
					//grdDetails.Columns[5].Visible = false;  -- Location
					//grdDetails.Columns[6].ItemStyle.Width = Unit.Pixel(0);  -- Modifier
					//grdDetails.Columns[7].ItemStyle.Width = Unit.Pixel(0);  -- Modified
					grdDetails.Columns[grdDetails.Columns.Count - 1].Visible = false;
					grdDetails.Columns[grdDetails.Columns.Count - 2].Visible = false;
					grdDetails.Columns[grdDetails.Columns.Count - 3].Visible = false;
				}
				//else
				//    grdDetails.Columns[6].Visible = false; -- Modifier

				grdDetails.DataBind();

				if (Top > 0)
				{
					grdDetails.PageSize = Top;
					grdDetails.AllowPaging = false;
					grdDetails.PagerStyle.Visible = false;
					grdDetails.ShowHeader = false;
					for (int i = 0; i < grdDetails.Columns.Count; i++)
						grdDetails.Columns[i].ItemStyle.CssClass = "ibn-propertysheet";
					grdDetails.CellPadding = 2;
				}
				else
					foreach (DataGridItem dgi in grdDetails.Items)
					{
						ImageButton ib = (ImageButton)dgi.FindControl("ibDelete2");
						if (ib != null)
						{
							ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("Warning") + "');");
							ib.ToolTip = LocRM.GetString("tDelDoc");
						}
					}
				#endregion
			}
		}
		#endregion

		#region Protected DG Params
		#region CanUserEdit
		protected string CanUserEdit(int id, string containerKey, int parentFolderId)
		{
			string retVal = "";
			if (Mediachase.IBN.Business.ControlSystem.FileStorage.CanUserWrite(
				Mediachase.IBN.Business.Security.CurrentUser.UserID,
				containerKey, parentFolderId))
				retVal = String.Format("<a href=\"{0}\"><img width='16' height='16' align='absmiddle' border='0' src='{1}' title='{2}'/></a>",
					String.Format("javascript:ShowWizard('{0}?FileId={1}&ContainerKey={2}&ContainerName={3}', 470, 200);", ResolveUrl("~/FileStorage/FileEdit.aspx"), id, containerKey, _containerName),
					ResolveUrl("~/layouts/images/edit.gif"),
					LocRM.GetString("tEditDoc"));
			return retVal;
		}
		#endregion

		#region CanDelete
		protected bool CanDelete(string containerKey, int parentFolderId)
		{
			return Mediachase.IBN.Business.ControlSystem.FileStorage.CanUserWrite(
				Mediachase.IBN.Business.Security.CurrentUser.UserID,
				containerKey, parentFolderId);
		}
		#endregion

		#region GetLink
		/// <summary>
		/// Gets the link.
		/// </summary>
		/// <param name="containerKey">The container key.</param>
		/// <param name="fileId">The file id.</param>
		/// <param name="name">The name.</param>
		/// <param name="contentType">Type of the content.</param>
		/// <param name="parentFolderId">The parent folder id.</param>
		/// <returns></returns>
		protected string GetLink(string containerKey, int fileId, string name, string contentType, int parentFolderId)
		{
			if (containerKey == null)
				throw new ArgumentNullException("containerKey");

			string link = "";
			if (contentType.ToLower().IndexOf("url") >= 0)
				link = Util.CommonHelper.GetLinkText(_fs, fileId);
			if (link == "")
				link = Util.CommonHelper.GetAbsoluteDownloadFilePath(fileId, name, _containerName, containerKey);

			string sNameLocked = Util.CommonHelper.GetLockerText(link);
			if (String.IsNullOrEmpty(sNameLocked) &&
				!Mediachase.IBN.Business.ControlSystem.FileStorage.CanUserWrite(Mediachase.IBN.Business.Security.CurrentUser.UserID, containerKey, parentFolderId))
				sNameLocked = String.Concat("<span style='color:#858585;font-size:7pt;'>[", LocRM.GetString("ReadOnly"), "]</span>");

			link = String.Format("<a href=\"{0}\" title='{3}'{2}>{1}</a> {4}",
				link,
				Util.CommonHelper.GetShortFileName(name, 40),
				Mediachase.IBN.Business.Common.OpenInNewWindow(contentType) ? " target='_blank'" : "",
				name,
				sNameLocked
				);
			return link;
		}
		#endregion
		#endregion

		#region BindToolbar - Menu
		private void BindToolbar()
		{
			if (Top > 0 && this.Parent is Mediachase.UI.Web.Common.Modules.LatestFiles)
			{
				((IToolbarLight)this.Parent).GetToolBar().AddRightLink(
					String.Format("<img src='{0}' border='0' width='16' height='16' align='absmiddle' />&nbsp;{1}",
					ResolveUrl("~/Layouts/Images/icon-search.gif"), LocRM.GetString("tViewAll")), GetCurrentLink() + "&Tab=FileLibrary");
				return;
			}

			if (this.Parent.Parent is IToolbarLight || this.Parent is IToolbarLight)
			{
				BlockHeaderLightWithMenu secHeader;
				if (this.Parent.Parent is IToolbarLight)
					secHeader = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();
				else
					secHeader = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent).GetToolBar();

				secHeader.ActionsMenu.Items.Clear();
				secHeader.ClearRightItems();

				#region Menu Items
				ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
				topMenuItem.Text = LocRM2.GetString("tView");
				topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
				topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
				topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
				topMenuItem.LookId = "TopItemLook";

				ComponentArt.Web.UI.MenuItem subItem;
				string sCurrentView = _pc["fs_List_ViewStyle"];

				subItem = new ComponentArt.Web.UI.MenuItem();
				if (sCurrentView == "ListView")
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewTable, "");
				subItem.Text = LocRM2.GetString("tListView");
				topMenuItem.Items.Add(subItem);

				subItem = new ComponentArt.Web.UI.MenuItem();
				if (sCurrentView == "DetailsView")
				{
					subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
					subItem.Look.LeftIconWidth = Unit.Pixel(16);
					subItem.Look.LeftIconHeight = Unit.Pixel(16);
				}
				subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(lbChangeViewDet, "");
				subItem.Text = LocRM2.GetString("tDetailsView");
				topMenuItem.Items.Add(subItem);
				#endregion

				secHeader.ActionsMenu.Items.Add(topMenuItem);

				if (_containerKey == "Workspace")
					secHeader.AddText(LocRM.GetString("tFilesList"));
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
			this.lbChangeViewTable.Click += new EventHandler(lbChangeViewTable_Click);
			this.lbChangeViewDet.Click += new EventHandler(lbChangeViewDet_Click);
			this.grdMain.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.grdMain.DeleteCommand += new DataGridCommandEventHandler(grdMain_DeleteCommand);
			this.grdMain.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			this.grdMain.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdMain_PageSizeChanged);
			this.grdMain.ItemDataBound += new DataGridItemEventHandler(grdMain_ItemDataBound);

			this.grdDetails.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.grdDetails.DeleteCommand += new DataGridCommandEventHandler(grdMain_DeleteCommand);
			this.grdDetails.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			this.grdDetails.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdMain_PageSizeChanged);
			this.grdDetails.ItemDataBound += new DataGridItemEventHandler(grdDetails_ItemDataBound);
		}
		#endregion

		#region DataGrid Events
		private void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (_pc["fl_Sort_" + _containerKey].ToString() == (string)e.SortExpression)
				_pc["fl_Sort_" + _containerKey] = (string)e.SortExpression + " DESC";
			else
				_pc["fl_Sort_" + _containerKey] = (string)e.SortExpression;

			BindList();
		}

		private void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			_pc["fl_Page_" + _containerKey] = e.NewPageIndex.ToString();

			BindList();
		}

		private void grdMain_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			_pc["fl_PageSize_" + _containerKey] = e.NewPageSize.ToString();

			BindList();
		}

		private void grdMain_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int delId = int.Parse(e.Item.Cells[0].Text);
			try
			{
				_fs.DeleteFile(delId);
			}
			catch { }
			Response.Redirect(GetCurrentLink());
		}
		#endregion

		#region grdMain_ItemDataBound
		private void grdMain_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (Top > 0)
				return;
			else
			{
				DataGridItem item = e.Item;
				if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.EditItem)
				{
					string containerKey1 = item.Cells[1].Text;
					BaseIbnContainer bic = BaseIbnContainer.Create("FileLibrary", containerKey1);
					Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
					DirectoryInfo di = fs.GetDirectory(int.Parse(item.Cells[2].Text));
					string parentName = "";
					string parentLink = "";
					Util.CommonHelper.GetParentContainer(containerKey1, di, out parentName, out parentLink);
					item.Cells[5].Text = parentLink;
				}
			}
		}
		#endregion

		#region grdDetails_ItemDataBound
		protected void grdDetails_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				foreach (Control c in e.Item.Controls[4].Controls)
				{
					if (c is LinkButton)
					{
						LinkButton lb = (LinkButton)c;
						switch (lb.CommandArgument)
						{
							case "sortName":
								lb.Text = LocRM.GetString("tName");
								break;
							case "ContainerKey":
								lb.Text = LocRM.GetString("tLocation");
								break;
							case "sortModifier":
								lb.Text = LocRM.GetString("UpdatedBy");
								break;
							case "sortModified":
								lb.Text = LocRM.GetString("UpdatedDate");
								break;
							case "sortSize":
								lb.Text = LocRM.GetString("Size");
								break;
							default:
								break;
						}
						if (lb.CommandArgument == _pc["fl_Sort_" + _containerKey].ToString())
							lb.Text += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
								ResolveUrl("~/layouts/images/upbtnF.jpg"));
						else if (lb.CommandArgument + " DESC" == _pc["fl_Sort_" + _containerKey].ToString())
							lb.Text += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
								ResolveUrl("~/layouts/images/downbtnF.jpg"));
					}
				}
			}


			if (Top > 0)
				return;
			else
			{
				DataGridItem item = e.Item;
				if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.EditItem)
				{
					string containerKey1 = item.Cells[1].Text;
					BaseIbnContainer bic = BaseIbnContainer.Create("FileLibrary", containerKey1);
					Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
					DirectoryInfo di = fs.GetDirectory(int.Parse(item.Cells[2].Text));
					string parentName = "";
					string parentLink = "";
					Util.CommonHelper.GetParentContainer(containerKey1, di, out parentName, out parentLink);
					Label lbl = (Label)item.Cells[4].FindControl("lblLocation");
					if (lbl != null)
						lbl.Text = parentLink;
				}
			}
		}
		#endregion

		#region Apply-Reset
		protected void Apply_ServerClick(object sender, System.EventArgs e)
		{
			SaveValues();
			BindSavedValues();
			BindInfoTable();
			BindList();
		}

		protected void Reset_ServerClick(object sender, System.EventArgs e)
		{
			BindDefaultValues();
			SaveValues();
			BindInfoTable();
			BindList();
		}
		#endregion

		#region Show - Hide
		protected void lbHideFilter_Click(object sender, System.EventArgs e)
		{
			_pc["ShowFilesFilter"] = "False";
			BindDefaultValues();
			BindSavedValues();
			BindList();
		}

		protected void lbShowFilter_Click(object sender, System.EventArgs e)
		{
			_pc["ShowFilesFilter"] = "True";
			BindDefaultValues();
			BindSavedValues();
			BindList();
		}
		#endregion

		#region Change View
		private void lbChangeViewTable_Click(object sender, EventArgs e)
		{
			_pc["fs_List_ViewStyle"] = "ListView";
			Response.Redirect(GetCurrentLink());
		}

		private void lbChangeViewDet_Click(object sender, EventArgs e)
		{
			_pc["fs_List_ViewStyle"] = "DetailsView";
			Response.Redirect(GetCurrentLink());
		}
		#endregion

		#region GetCurrentLink
		private string GetCurrentLink()
		{
			string sPath = HttpContext.Current.Request.Url.LocalPath;
			if (Request["ProjectId"] != null)
				sPath += String.Format("?ProjectId={0}", Request["ProjectId"]);
			else if (Request["IncidentId"] != null)
				sPath += String.Format("?IncidentId={0}", Request["IncidentId"]);
			else if (Request["TaskId"] != null)
				sPath += String.Format("?TaskId={0}", Request["TaskId"]);
			else if (Request["ToDoId"] != null)
				sPath += String.Format("?ToDoId={0}", Request["ToDoId"]);
			else if (Request["EventId"] != null)
				sPath += String.Format("?EventId={0}", Request["EventId"]);
			else if (Request["DocumentId"] != null)
				sPath += String.Format("?DocumentId={0}", Request["DocumentId"]);
			if (_typeId >= 0)
			{
				if (sPath.IndexOf("?") >= 0)
					sPath += "&TypeId=" + _typeId;
				else
					sPath += "?TypeId=" + _typeId;
			}
			return sPath;
		}
		#endregion

		#region Help Strings
		#region FormatBytes
		string FormatBytes(long bytes)
		{
			const double ONE_KB = 1024;
			const double ONE_MB = ONE_KB * 1024;
			const double ONE_GB = ONE_MB * 1024;
			const double ONE_TB = ONE_GB * 1024;
			const double ONE_PB = ONE_TB * 1024;
			const double ONE_EB = ONE_PB * 1024;
			const double ONE_ZB = ONE_EB * 1024;
			const double ONE_YB = ONE_ZB * 1024;

			if ((double)bytes <= 999)
				return bytes.ToString() + " bytes";
			else if ((double)bytes <= ONE_KB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_KB) + " KB";
			else if ((double)bytes <= ONE_MB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_MB) + " MB";
			else if ((double)bytes <= ONE_GB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_GB) + " GB";
			else if ((double)bytes <= ONE_TB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_TB) + " TB";
			else if ((double)bytes <= ONE_PB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_PB) + " PB";
			else if ((double)bytes <= ONE_EB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_EB) + " EB";
			else if ((double)bytes <= ONE_ZB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_ZB) + " ZB";
			else
				return ThreeNonZeroDigits((double)bytes / ONE_YB) + " YB";
		}
		#endregion

		#region ThreeNonZeroDigits
		string ThreeNonZeroDigits(double value)
		{
			if (value >= 100)
				return ((int)value).ToString();
			else if (value >= 10)
				return value.ToString("0.0");
			else
				return value.ToString("0.00");
		}
		#endregion 



		#endregion

		#region SetDates
		private void SetDates(string value, out DateTime start, out DateTime finish, string fromCustom, string toCustom)
		{
			switch (value)
			{
				case "1":	//Today
					start = UserDateTime.UserToday;
					finish = UserDateTime.UserNow;
					break;
				case "2":	//Yesterday
					start = UserDateTime.UserToday.AddDays(-1);
					finish = UserDateTime.UserToday;
					break;
				case "3":	//ThisWeek
					start = UserDateTime.UserToday.AddDays(1 - (int)DateTime.Now.DayOfWeek);
					finish = UserDateTime.UserNow;
					break;
				case "4":	//LastWeek
					start = UserDateTime.UserToday.AddDays(1 - (int)DateTime.Now.DayOfWeek - 7);
					finish = UserDateTime.UserToday.AddDays(1 - (int)DateTime.Now.DayOfWeek);
					break;
				case "5":	//ThisMonth
					start = UserDateTime.UserToday.AddDays(1 - DateTime.Now.Day);
					finish = UserDateTime.UserNow;
					break;
				case "6":	//LastMonth
					start = UserDateTime.UserToday.AddDays(1 - DateTime.Now.Day).AddMonths(-1);
					finish = UserDateTime.UserToday.AddDays(1 - DateTime.Now.Day);
					break;
				case "7":	//ThisYear
					start = UserDateTime.UserToday.AddDays(1 - DateTime.Now.DayOfYear);
					finish = UserDateTime.UserNow;
					break;
				case "8":	//LastYear
					start = UserDateTime.UserToday.AddDays(1 - DateTime.Now.DayOfYear).AddYears(-1);
					finish = UserDateTime.UserToday.AddDays(1 - DateTime.Now.DayOfYear);
					break;
				case "9":	//Custom
					try
					{
						start = DateTime.Parse(fromCustom);
					}
					catch
					{
						start = Report.DefaultStartDate;
					}
					try
					{
						finish = DateTime.Parse(toCustom).AddSeconds(24 * 60 * 60 - 1);
					}
					catch
					{
						finish = Report.DefaultEndDate;
					}
					break;
				default:
					start = Report.DefaultStartDate;
					finish = Report.DefaultEndDate;
					break;
			}
		}
		#endregion
	}
}
