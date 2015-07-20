using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.UI.Web.Modules;
using System.Web.UI;
using Mediachase.IBN.Business.WebDAV.Common;


namespace Mediachase.UI.Web.FileStorage.Modules
{
	/// <summary>
	///		Summary description for FileSearch.
	/// </summary>
	public partial class FileSearch : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strSearch", typeof(FileSearch).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryAdvanced", typeof(FileSearch).Assembly);

		private UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		private string _sort = "sortName";

		private BaseIbnContainer _bic;
		private Mediachase.IBN.Business.ControlSystem.FileStorage _fs;

		#region _containerKey, _containerName
		private string _containerKey
		{
			get
			{
				if (Request["ProjectId"] != null)
					return "ProjectId_" + Request["ProjectId"];
				else if (Request["IncidentId"] != null)
					return "IncidentId_" + Request["IncidentId"];
				else if (Request["TaskId"] != null)
					return "TaskId_" + Request["TaskId"];
				else if (Request["DocumentId"] != null)
					return "DocumentId_" + Request["DocumentId"];
				else if (Request["EventId"] != null)
					return "EventId_" + Request["EventId"];
				else if (Request["ToDoId"] != null)
				{
					using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetToDo(int.Parse(Request["ToDoId"]), false))
					{
						if (reader.Read())
						{
							if (reader["TaskId"] != DBNull.Value)
								return "TaskId_" + reader["TaskId"].ToString();
							else if (reader["DocumentId"] != DBNull.Value)
								return "DocumentId_" + reader["DocumentId"].ToString();
							else
								return "ToDoId_" + Request["ToDoId"];
						}
						else
							return "ToDoId_" + Request["ToDoId"];
					}
				}
				else
					return "Workspace";
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

		protected void Page_Load(object sender, System.EventArgs e)
		{
			_bic = BaseIbnContainer.Create(_containerName, _containerKey);
			_fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)_bic.LoadControl("FileStorage");

			if (!Page.IsPostBack)
			{
				BindLists();
				ViewState["fls_Sort"] = _sort;

				if (_pc["fs_Search_ViewStyle"] == null)
					_pc["fs_Search_ViewStyle"] = "ListView";
			}
			else
			{
				BindSavedValues();
				BindDG();
			}
		}

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			BindToolbar();

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "<script type='text/javascript'>" +
					  "setTimeout(\"LoginFocusElement('" + tbSerchStr.ClientID + "')\", 0);</script>");
		}
		#endregion

		#region BindToolbar - Menu
		private void BindToolbar()
		{
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
				string sCurrentView = _pc["fs_Search_ViewStyle"];

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
					secHeader.AddText(LocRM.GetString("tbTitle"));

				secHeader.EnsureRender();
			}
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			//Content Type
			if (ViewState["fS_CType"] != null)
				Util.CommonHelper.SafeSelect(ddType, ViewState["fS_CType"].ToString());

			//Search String
			if (ViewState["SearchStr"] != null)
				tbSerchStr.Text = ViewState["SearchStr"].ToString();

		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			if (ddType.SelectedItem != null)
				ViewState["fS_CType"] = ddType.SelectedValue;

			ViewState["SearchStr"] = tbSerchStr.Text;
		}
		#endregion

		#region BindLists
		private void BindLists()
		{
			btnSearch.Text = LocRM.GetString("tFindNow");

			ddType.Items.Clear();
			DataView dv = ContentType.GetListContentTypesDataTable().DefaultView;
			dv.Sort = "FriendlyName";

			ddType.DataSource = dv;
			ddType.DataTextField = "FriendlyName";
			ddType.DataValueField = "ContentTypeId";
			ddType.DataBind();
			ListItem lItem = new ListItem(LocRM.GetString("tAll"), "0");
			ddType.Items.Insert(0, lItem);

			ddKM1.Items.Clear();
			ddKM1.Items.Add(new ListItem("Kb", "0"));
			ddKM1.Items.Add(new ListItem("Mb", "1"));

			ddKM2.Items.Clear();
			ddKM2.Items.Add(new ListItem("Kb", "0"));
			ddKM2.Items.Add(new ListItem("Mb", "1"));

			lblFrom.Text = LocRM.GetString("tFrom") + ":";
			lblTo.Text = LocRM.GetString("tTo") + ":";
			lblfromSize.Text = LocRM.GetString("tFrom") + ":";
			lbltoSize.Text = LocRM.GetString("tTo") + ":";

			ddModified.Items.Add(new ListItem(LocRM.GetString("tAny"), "0"));
			ddModified.Items.Add(new ListItem(LocRM.GetString("tToday"), "1"));
			ddModified.Items.Add(new ListItem(LocRM.GetString("tYesterday"), "2"));
			ddModified.Items.Add(new ListItem(LocRM.GetString("tThisWeek"), "3"));
			ddModified.Items.Add(new ListItem(LocRM.GetString("tLastWeek"), "4"));
			ddModified.Items.Add(new ListItem(LocRM.GetString("tThisMonth"), "5"));
			ddModified.Items.Add(new ListItem(LocRM.GetString("tLastMonth"), "6"));
			ddModified.Items.Add(new ListItem(LocRM.GetString("tThisYear"), "7"));
			ddModified.Items.Add(new ListItem(LocRM.GetString("tLastYear"), "8"));
			ddModified.Items.Add(new ListItem(LocRM.GetString("tCustom"), "9"));
			dtcStartDate.SelectedDate = UserDateTime.UserNow.AddMonths(-1);
			dtcEndDate.SelectedDate = UserDateTime.UserNow;
			tableDateFrom.Style.Add("display", "none");
			trToDate.Style.Add("display", "none");
			tdDateTo.Style.Add("display", "none");

			ddSize.Items.Add(new ListItem(LocRM.GetString("tAll"), "0"));
			ddSize.Items.Add(new ListItem(LocRM.GetString("tCustom"), "3"));
			fromSize.Text = "0";
			ddKM1.SelectedIndex = 0;
			toSize.Text = "10";
			ddKM2.SelectedIndex = 1;
			toSize.Style.Add("display", "none");
			ddKM2.Style.Add("display", "none");
			tableSizeFrom.Style.Add("display", "none");
			tdSizeTo.Style.Add("display", "none");
		}
		#endregion

		#region BindDG()
		private void BindDG()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Icon", typeof(string)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("sortName", typeof(string)));
			dt.Columns.Add(new DataColumn("ModifiedDate", typeof(string)));
			dt.Columns.Add(new DataColumn("sortModified", typeof(DateTime)));
			dt.Columns.Add(new DataColumn("Modifier", typeof(string)));
			dt.Columns.Add(new DataColumn("sortModifier", typeof(string)));
			dt.Columns.Add(new DataColumn("ActionView", typeof(string)));
			dt.Columns.Add(new DataColumn("ActionJump", typeof(string)));
			dt.Columns.Add(new DataColumn("Size", typeof(string)));
			dt.Columns.Add(new DataColumn("sortSize", typeof(int)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));
			dt.Columns.Add(new DataColumn("ContentType", typeof(string)));
			DataRow dr;

			#region BindDataTable
			int iContType = int.Parse(ddType.SelectedValue);
			int iFromSize = 0;
			int iToSize = 0;
			DateTime startDate = DateTime.MinValue;
			DateTime endDate = DateTime.MinValue;

			if (ddSize.SelectedIndex == 0)
			{
				tableSizeFrom.Style.Add("display", "none");
				tdSizeTo.Style.Add("display", "none");
				toSize.Style.Add("display", "none");
				ddKM2.Style.Add("display", "none");
			}
			else
			{
				tableSizeFrom.Style.Add("display", "block");
				tdSizeTo.Style.Add("display", "block");
				toSize.Style.Add("display", "block");
				ddKM2.Style.Add("display", "block");

				if (fromSize.Text == "")
					iFromSize = 0;
				else
					iFromSize = int.Parse(fromSize.Text);
				if (toSize.Text == "")
					iToSize = 0;
				else
					iToSize = int.Parse(toSize.Text);

				if (ddKM1.SelectedItem.Value == "1")
					iFromSize *= 1024 * 1024;
				else
					iFromSize *= 1024;

				if (iToSize != int.MaxValue)
				{
					if (ddKM2.SelectedItem.Value == "1")
						iToSize *= 1024 * 1024;
					else
						iToSize *= 1024;
				}
			}

			if (ddModified.Value != "9")
			{
				tableDateFrom.Style.Add("display", "none");
				tdDateTo.Style.Add("display", "none");
				trToDate.Style.Add("display", "none");
			}
			else
			{
				tableDateFrom.Style.Add("display", "block");
				tdDateTo.Style.Add("display", "block");
				trToDate.Style.Add("display", "block");
			}

			string sVal = ddModified.Value;
			SetDates(sVal, out startDate, out endDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());

			if (iFromSize > iToSize)
			{
				lblValid.Text = "*";
				return;
			}
			else
				lblValid.Text = "";

			string sKeyword = tbSerchStr.Text;

			int typeId = -1;
			if (Request["TypeId"] != null)
				typeId = int.Parse(Request["TypeId"]);

			FileInfo[] fMas;
			if (!_containerKey.StartsWith("Workspace") && !_containerKey.StartsWith("ProjectId_"))
				fMas = _fs.GetFiles(0, true, (sKeyword.Length > 0) ? sKeyword : null, iContType, startDate, endDate, iFromSize, iToSize);
			else if (_containerKey == "Workspace" && typeId < 0)
			{
				fMas = Mediachase.IBN.Business.ControlSystem.FileStorage.SearchFiles(Mediachase.IBN.Business.Security.CurrentUser.UserID,
					_containerKey, 0, true, (sKeyword.Length > 0) ? sKeyword : null, iContType, startDate, endDate, iFromSize, iToSize);
			}
			else
			{
				int projectId = -1;
				if (Request["ProjectId"] != null)
					projectId = int.Parse(Request["ProjectId"]);
				fMas = Mediachase.IBN.Business.ControlSystem.FileStorage.SearchFiles(Mediachase.IBN.Business.Security.CurrentUser.UserID, projectId,
					typeId, (sKeyword.Length > 0) ? sKeyword : null, iContType, startDate, endDate, iFromSize, iToSize);
			}

			foreach (FileInfo fi in fMas)
			{
				dr = dt.NewRow();
				dr["Id"] = fi.Id;
				dr["Icon"] = ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + fi.FileBinaryContentTypeId);

				dr["Name"] = fi.Name;
				dr["sortName"] = fi.Name;
				dr["Description"] = fi.Description;
				dr["ModifiedDate"] = fi.Modified.ToShortDateString();
				dr["sortModified"] = fi.Modified;
				dr["Modifier"] = Util.CommonHelper.GetUserStatus(fi.ModifierId);
				dr["sortModifier"] = Util.CommonHelper.GetUserStatusPureName(fi.ModifierId);
				dr["ActionView"] = String.Format("<a href=\"{0}\" title='{2}'><img width='16' height='16' align='absmiddle' border='0' src='{1}'/></a>",
					String.Format("javascript:ViewFile('{0}','{1}',{2});", _containerName, fi.ContainerKey, fi.Id),
					ResolveUrl("~/layouts/images/icon-search.gif"),
					LocRM.GetString("tViewDetails"));
				dr["ActionJump"] = String.Format("<a href=\"{0}\"><img width='16' height='16' align='absmiddle' border='0' src='{1}' title='{2}'/></a>",
					GetCurrentLink(fi.ParentDirectoryId),
					ResolveUrl("~/layouts/images/FOLDER.gif"),
					LocRM.GetString("tToDir"));
				dr["Size"] = FormatBytes((long)fi.Length);
				dr["sortSize"] = fi.Length;
				dr["ContentType"] = fi.FileBinaryContentType;
				dt.Rows.Add(dr);
			}
			#endregion

			DataView dv = dt.DefaultView;
			dv.Sort = ViewState["fls_Sort"].ToString();

			if (_pc["fs_Search_ViewStyle"] == "ListView")
			{
				#region ListView
				grdMain.Visible = true;
				grdDetails.Visible = false;

				int i = 2;
				grdMain.Columns[i++].HeaderText = LocRM.GetString("Title");
				grdMain.Columns[i++].HeaderText = LocRM.GetString("UpdBy");
				grdMain.Columns[i++].HeaderText = LocRM.GetString("UpdDate");
				grdMain.Columns[i++].HeaderText = LocRM.GetString("Size");

				foreach (DataGridColumn dgc in grdMain.Columns)
				{
					if (dgc.SortExpression == ViewState["fls_Sort"].ToString())
						dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
							ResolveUrl("~/layouts/images/upbtnF.jpg"));
					else if (dgc.SortExpression + " DESC" == ViewState["fls_Sort"].ToString())
						dgc.HeaderText += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
							ResolveUrl("~/layouts/images/downbtnF.jpg"));
				}

				if (ViewState["fls_PSize"] == null)
					ViewState["fls_PSize"] = "10";
				grdMain.PageSize = int.Parse(ViewState["fls_PSize"].ToString());

				if (ViewState["fls_Page"] == null)
					ViewState["fls_Page"] = "0";
				int pageIndex = int.Parse(ViewState["fls_Page"].ToString());
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
					ViewState["fls_Page"] = "0";
				}

				if (_containerKey.StartsWith("Workspace") || _containerKey.StartsWith("ProjectId_"))
					grdMain.Columns[grdMain.Columns.Count - 1].Visible = false;

				grdMain.DataSource = dv;
				grdMain.DataBind();
				#endregion
			}
			else if (_pc["fs_Search_ViewStyle"] == "DetailsView")
			{
				#region DetailsView
				grdMain.Visible = false;
				grdDetails.Visible = true;

				if (ViewState["fls_PSize"] == null)
					ViewState["fls_PSize"] = "10";
				grdDetails.PageSize = int.Parse(ViewState["fls_PSize"].ToString());

				if (ViewState["fls_Page"] == null)
					ViewState["fls_Page"] = "0";
				int pageIndex = int.Parse(ViewState["fls_Page"].ToString());
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
					ViewState["fls_Page"] = "0";
				}

				if (_containerKey.StartsWith("Workspace") || _containerKey.StartsWith("ProjectId_"))
					grdDetails.Columns[grdDetails.Columns.Count - 1].Visible = false;

				grdDetails.DataSource = dv;
				grdDetails.DataBind();
				#endregion
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
			this.CustomValidator1.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidator);
			this.grdMain.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.grdMain.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			this.grdMain.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdMain_PageSizeChanged);

			this.grdDetails.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.grdDetails.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
			this.grdDetails.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(grdMain_PageSizeChanged);
			this.grdDetails.ItemDataBound += new DataGridItemEventHandler(grdDetails_ItemDataBound);
		}

		
		#endregion

		#region DataGrid Events
		private void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (ViewState["fls_Sort"].ToString() == (string)e.SortExpression)
				ViewState["fls_Sort"] = (string)e.SortExpression + " DESC";
			else
				ViewState["fls_Sort"] = (string)e.SortExpression;
			BindDG();
		}

		private void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			ViewState["fls_Page"] = e.NewPageIndex.ToString();
			BindDG();
		}

		private void grdMain_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			ViewState["fls_PSize"] = e.NewPageSize.ToString();
			BindDG();
		}

		private void grdDetails_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				foreach (Control c in e.Item.Controls[2].Controls)
				{
					if (c is LinkButton)
					{
						LinkButton lb = (LinkButton)c;
						switch (lb.CommandArgument)
						{
							case "sortName":
								lb.Text = LocRM.GetString("Title");
								break;
							case "sortModifier":
								lb.Text = LocRM.GetString("UpdBy");
								break;
							case "sortModified":
								lb.Text = LocRM.GetString("UpdDate");
								break;
							case "sortSize":
								lb.Text = LocRM.GetString("Size");
								break;
							default:
								break;
						}
						if (lb.CommandArgument == ViewState["fls_Sort"].ToString())
							lb.Text += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
								ResolveClientUrl("~/layouts/images/upbtnF.jpg"));
						else if (lb.CommandArgument + " DESC" == ViewState["fls_Sort"].ToString())
							lb.Text += String.Format("&nbsp;<img border='0' align='absmiddle' width='9px' height='5px' src='{0}'/>",
								ResolveClientUrl("~/layouts/images/downbtnF.jpg"));
					}
				}
			}
		}
		#endregion

		#region DateValidator
		private void CustomValidator(Object sender, ServerValidateEventArgs args)
		{
			if (dtcEndDate.SelectedDate < dtcStartDate.SelectedDate)
			{
				CustomValidator1.ErrorMessage = LocRM.GetString("tWrongDate");
				args.IsValid = false;
			}
			else
				args.IsValid = true;
		}
		#endregion

		#region Search
		protected void btnSearch_Click(object sender, System.EventArgs e)
		{
			SaveValues();
			BindSavedValues();
			BindDG();
		}
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

		#region GetCurrentLink
		private string GetCurrentLink(int iFolderId)
		{
			string sPath = HttpContext.Current.Request.Url.LocalPath;
			if (Request["ProjectId"] != null)
				sPath += String.Format("?ProjectId={0}&FolderId={1}&SubTab=0", Request["ProjectId"], iFolderId);
			else if (Request["IncidentId"] != null)
				sPath += String.Format("?IncidentId={0}&FolderId={1}&SubTab=0", Request["IncidentId"], iFolderId);
			else if (Request["TaskId"] != null)
				sPath += String.Format("?TaskId={0}&FolderId={1}&SubTab=0", Request["TaskId"], iFolderId);
			else if (Request["ToDoId"] != null)
				sPath += String.Format("?ToDoId={0}&FolderId={1}&SubTab=0", Request["ToDoId"], iFolderId);
			else if (Request["EventId"] != null)
				sPath += String.Format("?EventId={0}&FolderId={1}&SubTab=0", Request["EventId"], iFolderId);
			else if (Request["DocumentId"] != null)
				sPath += String.Format("?DocumentId={0}&FolderId={1}&SubTab=0", Request["DocumentId"], iFolderId);
			else
				sPath += String.Format("?FolderId={0}&SubTab=0", iFolderId);
			return sPath;
		}
		#endregion

		#region Help Strings
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

		#region Change View
		private void lbChangeViewTable_Click(object sender, EventArgs e)
		{
			_pc["fs_Search_ViewStyle"] = "ListView";
			BindDG();
		}

		private void lbChangeViewDet_Click(object sender, EventArgs e)
		{
			_pc["fs_Search_ViewStyle"] = "DetailsView";
			BindDG();
		}
		#endregion

		#region GetLink
		protected string GetLink(int fileId, string name, string contentType)
		{
			string link = "";
			if (contentType.ToLower().IndexOf("url") >= 0)
				link = Util.CommonHelper.GetLinkText(_fs, fileId);
			if (link == "")
				link = Util.CommonHelper.GetAbsoluteDownloadFilePath(fileId, name, _containerName, _containerKey);

			string sNameLocked = Util.CommonHelper.GetLockerText(link);

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
	}
}
