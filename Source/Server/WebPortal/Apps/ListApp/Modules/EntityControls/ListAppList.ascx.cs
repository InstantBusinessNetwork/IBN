using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Clients;
using Mediachase.Ibn.Lists;

namespace Mediachase.Ibn.Web.UI.ListApp.Modules.EntityControls
{
	public partial class ListAppList : System.Web.UI.UserControl
	{
		private string _className = "ListInfo";
		private string _viewName = String.Empty;
		private const string _placeName = "EntityList";

		private string _profileName = String.Empty;

		public const string ProfileNameKey = "EntityList_ProfileName";
		public const string GroupFieldKey = "EntityList_GroupField";
		public const string GroupItemKey = "EntityList_GroupItemKey";
		public const string KeywordFilterKey = "EntityKeyword";

		private bool _isFilterSet = false;
		private string _filterText = String.Empty;

		private bool _isGroupSet = false;
		private string _groupFieldName = String.Empty;

		protected UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			#region PreInit Parameters
			//получить ListViewProfiles и заполнить поля _isGroupSet и _groupFieldName
			_profileName = GetProfileName(_pc, _className);
			ListViewProfile lvp = ListViewProfile.Load(_className, _profileName, _placeName);
			if (lvp == null)
			{
				_profileName = GetFirstAvailableListViewProfile(_className);
				SetProfileName(_pc, _className, _profileName);
				lvp = ListViewProfile.Load(_className, _profileName, _placeName);
			}
			_isGroupSet = !String.IsNullOrEmpty(lvp.GroupByFieldName);
			_groupFieldName = GetGroupFieldForProfile(lvp);
			#endregion

			//для вида с группировкой показываем левую панель
			if (_isGroupSet)
				DockLeft.DefaultSize = 230;
			else
				DockLeft.DefaultSize = 0;

			//для вида с группировкой показываем левую панель
			//if (lvp.Filters.Count > 0)
			//{
			//    trFilterText.Visible = true;
			//    DockTop.DefaultSize = 101;
			//}
			//else
			//{
				trFilterText.Visible = false;
				DockTop.DefaultSize = 74;
			//}

			#region Init MCGrid, Toolbar
			grdMain.ChangingEntityGridColumnHeader += new ChangingEntityGridColumnHeaderEventHandler(grdMain_ChangingEntityGridColumnHeader);
			//инициализируем грид
			grdMain.ClassName = _className;
			grdMain.ViewName = _viewName;
			grdMain.PlaceName = _placeName;
			grdMain.ProfileName = _profileName;

			ctrlGridEventUpdater.ClassName = _className;
			ctrlGridEventUpdater.ViewName = _viewName;
			ctrlGridEventUpdater.PlaceName = _placeName;
			ctrlGridEventUpdater.GridId = grdMain.GridClientContainerId;

			//инициализируем тулбар
			MainMetaToolbar.ClassName = _className;
			MainMetaToolbar.ViewName = _viewName;
			MainMetaToolbar.PlaceName = _placeName;
			MainMetaToolbar.ToolbarMode = Mediachase.Ibn.Web.UI.MetaUI.Toolbar.MetaToolbar.Mode.ListViewUI;
			#endregion

			if (!Page.IsPostBack)
			{
				//устанавливаем в key-textbox запомненные значения
				BindSavedValues();
				//биндим если надо группы в левой панели
				BindGroupDataList();
			}

			int folderId = GetCurrentFolderId();
			CHelper.AddToContext("ListFolderId", folderId);

			//биндим датагрид
			BindDataGrid(!Page.IsPostBack);

			//set header text
			if (!Page.IsPostBack)
				BindBlockHeader();

			#region CommandManager PreLoad
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand(_className, _viewName, _placeName, "MC_ListApp_ChangeFolderTree");
			#endregion
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			//кнопка очистить фильтр у key-textbox
			btnClear.Visible = !String.IsNullOrEmpty(txtSearch.Text);

			//стиль key-textbox
			txtSearch.BorderWidth = 1;
			txtSearch.BorderStyle = BorderStyle.Solid;
			txtSearch.Style.Add(HtmlTextWriterStyle.Padding, "2px");
			txtSearch.BorderColor = Color.FromArgb(127, 157, 185);
			txtSearch.BackColor = (!String.IsNullOrEmpty(txtSearch.Text)) ? Color.FromArgb(255, 240, 144) : Color.White;

			//если необходимо перебиндить датагрид
			if (CHelper.NeedToBindGrid())
			{
				int folderId = GetCurrentFolderId();
				CHelper.RemoveFromContext("ListFolderId");
				CHelper.AddToContext("ListFolderId", folderId);
				jsTreeExt.TreeSourceUrl = ResolveClientUrl("~/Apps/ListApp/Pages/ListFolderTreeSource.aspx?FolderId=" + folderId);
				jsTreeExt.FolderId = folderId.ToString();

				//биндим датагрид
				BindDataGrid(true);
				//обновляем UpdatePanel грида
				grdMainPanel.Update();
			}

			//собираем текст FilterView
			string sText = String.Empty;
			if (!String.IsNullOrEmpty(_filterText))
			{
				sText = _filterText;
			}
			else
			{
				if (_isFilterSet)
				{
					sText = GetGlobalResourceObject("IbnFramework.Global", "FilterIsSet").ToString();
				}
				else
				{
					sText = GetGlobalResourceObject("IbnFramework.Global", "FilterIsNotSet").ToString();
				}
			}
			FilterIsSet.ForeColor = Color.FromArgb(0x90, 0x90, 0x90);
			FilterIsSet.Text = sText;

			base.OnPreRender(e);
		}
		#endregion

		#region BindGroupDataList
		private void BindGroupDataList()
		{
			int folderId = GetCurrentFolderId();
			
			jsTreeExt.FolderId = folderId.ToString();
			jsTreeExt.IconUrl = ResolveClientUrl("~/layouts/images/folder.gif");
			jsTreeExt.RootId = "0";
			jsTreeExt.RootNodeText = GetGlobalResourceObject("IbnShell.Navigation", "tListFolders").ToString();
			jsTreeExt.TreeSourceUrl = ResolveClientUrl("~/Apps/ListApp/Pages/ListFolderTreeSource.aspx?FolderId=" + folderId);
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			//последнее запомненное значение key-textbox
			if (_pc[_className + "_" + KeywordFilterKey] != null)
				txtSearch.Text = _pc[_className + "_" + KeywordFilterKey];
		}
		#endregion

		#region BindBlockHeader
		/// <summary>
		/// Binds the tool bar.
		/// </summary>
		private void BindBlockHeader()
		{
			int ListFolderId = 1;
			int ProjectId = 0;
			string sTitle = CHelper.GetResFileString("{IbnFramework.ListInfo:tLists}");
			if (ListFolderId == -1)
			{
				sTitle = CHelper.GetResFileString("{IbnFramework.ListInfo:tPrjLists}");
			}
			else if (ProjectId <= 0)
			{
				ListFolder folder = new ListFolder(ListFolderId);
				int iParentFolderId = (folder.ParentId.HasValue) ? (int)folder.ParentId.Value : 0;
				if (iParentFolderId == 0 && folder.FolderType != ListFolderType.Project)
				{
					if (folder.FolderType == ListFolderType.Private)
						sTitle = CHelper.GetResFileString("{IbnFramework.ListInfo:tPrivateLists}");
					else
						sTitle = CHelper.GetResFileString("{IbnFramework.ListInfo:tPublicLists}");
				}
				else if (iParentFolderId == 0)
				{
					int projectId = ListFolderId;
					if (folder.ProjectId.HasValue)
						projectId = folder.ProjectId.Value;
					sTitle = Task.GetProjectTitle(projectId);
				}
			}

			BlockHeaderMain.Title = sTitle;
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			_isFilterSet = false;
			_filterText = String.Empty;

			//добавляем к фильтру выбраное значение из левой панели, если включена группировка
			#region GroupItem
			int folderId = GetCurrentFolderId();
			FilterElementCollection fec = new FilterElementCollection();
			fec.Add(FilterElement.EqualElement("FolderId", folderId));
			grdMain.AddFilters = fec;
			#endregion

			grdMain.SearchKeyword = txtSearch.Text;

			if (dataBind)
				grdMain.DataBind();
		}
		#endregion

		#region grdMain_ChangingEntityGridColumnHeader
		void grdMain_ChangingEntityGridColumnHeader(object sender, ChangingEntityGridColumnHeaderEventArgs e)
		{
			switch (e.FieldName)
			{
				case "Title":
					e.ControlField.HeaderText = GetGlobalResourceObject("IbnFramework.ListInfo", "tName").ToString();
					break;
				case "ListType":
					e.ControlField.HeaderText = GetGlobalResourceObject("IbnFramework.ListInfo", "Type").ToString();
					break;
				case "Status":
					e.ControlField.HeaderText = GetGlobalResourceObject("IbnFramework.ListInfo", "Status").ToString();
					break;
				case "CreatorId":
					e.ControlField.HeaderText = GetGlobalResourceObject("IbnFramework.ListInfo", "CreatedBy").ToString();
					break;
				case "Created":
					e.ControlField.HeaderText = GetGlobalResourceObject("IbnFramework.ListInfo", "Created").ToString();
					break;
				default:
					break;
			}
		}
		#endregion

		#region btnSearch_Click
		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			//фильтр по key-textbox
			_pc[_className + "_" + KeywordFilterKey] = txtSearch.Text;
			CHelper.RequireBindGrid();
		}
		#endregion

		#region btnClear_Click
		protected void btnClear_Click(object sender, ImageClickEventArgs e)
		{
			//очистка key-textbox
			txtSearch.Text = String.Empty;
			_pc[_className + "_" + KeywordFilterKey] = txtSearch.Text;
			CHelper.RequireBindGrid();
		}
		#endregion

		#region GetGroupFieldForProfile
		public string GetGroupFieldForProfile(ListViewProfile lvp)
		{
			if (_pc[_pc[_className + "_" + ProfileNameKey] + "_" + GroupFieldKey] == null)
			{
				_pc[_pc[_className + "_" + ProfileNameKey] + "_" + GroupFieldKey] = lvp.GroupByFieldName;
			}

			if (!Page.IsPostBack)
				_pc[_pc[_className + "_" + ProfileNameKey] + "_" + GroupFieldKey] = lvp.GroupByFieldName;

			return _pc[_pc[_className + "_" + ProfileNameKey] + "_" + GroupFieldKey];
		}
		#endregion

		#region SetGroupFieldForProfile
		public void SetGroupFieldForProfile(string name)
		{
			_pc[_pc[_className + "_" + ProfileNameKey] + "_" + GroupFieldKey] = name;
		}
		#endregion


		#region GetCurrentFolderId
		private int GetCurrentFolderId()
		{
			int pubId = (int)ListManager.GetPublicRoot().PrimaryKeyId.Value;
			if (_pc[_className + "_" + _groupFieldName + "_" + GroupItemKey] == null)
				_pc[_className + "_" + _groupFieldName + "_" + GroupItemKey] = pubId.ToString();

			int folderId = -1;
			if (!int.TryParse(_pc[_className + "_" + _groupFieldName + "_" + GroupItemKey], out folderId))
				folderId = pubId;
			if (folderId < 0)
				folderId = pubId;

			return folderId;
		} 
		#endregion

		//static methods
		#region GetProfileName
		public static string GetProfileName(UserLightPropertyCollection pc, string className)
		{
			if (pc[className + "_" + ProfileNameKey] == null)
				pc[className + "_" + ProfileNameKey] = GetFirstAvailableListViewProfile(className);

			return pc[className + "_" + ProfileNameKey];
		}
		#endregion

		#region SetProfileName
		public static void SetProfileName(UserLightPropertyCollection pc, string className, string name)
		{
			pc[className + "_" + ProfileNameKey] = name;
		}
		#endregion

		#region GetFirstAvailableListViewProfile
		public static string GetFirstAvailableListViewProfile(string className)
		{
			ListViewProfile[] mas = ListViewProfile.GetSystemProfiles(className, _placeName);
			if (mas.Length == 0)
			{
				mas = ListViewProfile.GetProfiles(className, _placeName, Mediachase.IBN.Business.Security.CurrentUser.UserID);
				if (mas.Length == 0)
				{
					ListViewProfile profile = new ListViewProfile();
					profile.Id = Guid.NewGuid().ToString();
					profile.IsPublic = true;
					profile.IsSystem = false;
					MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(className);
					profile.Name = CHelper.GetResFileString(mc.PluralName);
					profile.FieldSetName = mc.Name;
					profile.FieldSet = new List<string>();
					profile.FieldSet.Add(mc.TitleFieldName);
					profile.GroupByFieldName = String.Empty;
					profile.Filters = new FilterElementCollection();
					profile.Sorting = new SortingElementCollection();
					profile.ColumnsUI = new ColumnPropertiesCollection();
					profile.ColumnsUI.Add(new ColumnProperties(mc.TitleFieldName, "300px", String.Empty));
					ListViewProfile.SaveCustomProfile(className, _placeName, Mediachase.IBN.Business.Security.CurrentUser.UserID, profile);
					mas = ListViewProfile.GetProfiles(className, _placeName, Mediachase.IBN.Business.Security.CurrentUser.UserID);
					if (mas.Length == 0)
						throw new Exception("ListViewProfiles are not exist!");
				}
			}
			return mas[0].Id;
		}
		#endregion
	}
}