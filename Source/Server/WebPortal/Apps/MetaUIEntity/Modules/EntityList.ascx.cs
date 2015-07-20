using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Clients;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Data.Meta;
using Mediachase.UI.Web.Util;

namespace Mediachase.Ibn.Web.UI.MetaUIEntity.Modules
{
	public enum ExportMode
	{
		Excel = 1,
		Xml = 2,
		Vcf = 3
	}

	#region class: ClientProfileInfo
	public class ClientProfileInfo
	{
		#region prop: Name
		private string _name;
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		} 
		#endregion

		#region prop: Url
		private string _url;
		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}
		#endregion

		#region .ctor()
		public ClientProfileInfo()
		{
		}

		public ClientProfileInfo(string name)
			: this()
		{
			this.Name = name;
		}
		public ClientProfileInfo(string name, string url)
			: this(name)
		{
			this.Url = url;
		}
		#endregion
	} 
	#endregion

	public partial class EntityList : System.Web.UI.UserControl
	{
		#region ClassName
		public string ClassName
		{
			get
			{
				if (ViewState["_className"] != null)
					return ViewState["_className"].ToString();
				else if (Request["ClassName"] != null)
					return Request["ClassName"];
				else
					return String.Empty;
			}
			set { ViewState["_className"] = value; }
		}
		#endregion

		#region ViewName
		public string ViewName
		{
			get
			{
				if (ViewState["_viewName"] != null)
					return ViewState["_viewName"].ToString();
				else if (Request["ViewName"] != null)
					return Request["ViewName"];
				else
					return String.Empty;
			}
			set { ViewState["_viewName"] = value; }
		}
		#endregion

		private const string _placeName = "EntityList";

		private string _profileName = String.Empty;

		public const string ProfileNameKey = "EntityList_ProfileName";
		public const string KeywordFilterKey = "EntityKeyword";
		public const string GroupItemKey = "GroupObjectId";

		protected string AddNewViewScript = String.Empty;

		private bool _isFilterSet = false;
		private string _filterText = String.Empty;

		private bool _isGroupSet = false;

		private Mediachase.IBN.Business.UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		private ListViewProfile lvp = null;
		private MetaClass mc = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(ClassName))
				throw new ArgumentNullException("ClassName is Required!");
			#region PreInit Parameters
			mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(ClassName);

			//получить ListViewProfiles и заполнить поле _isGroupSet
			_profileName = GetProfileName(_pc, ClassName);
			lvp = ListViewProfile.Load(ClassName, _profileName, _placeName);
			if (lvp == null)
			{
				_profileName = GetFirstAvailableListViewProfile(ClassName);
				SetProfileName(_pc, ClassName, _profileName);
				lvp = ListViewProfile.Load(ClassName, _profileName, _placeName);
			}
			_isGroupSet = !String.IsNullOrEmpty(lvp.GroupByFieldName);
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
			grdMain.ClassName = ClassName;
			grdMain.ViewName = ViewName;
			grdMain.PlaceName = _placeName;
			grdMain.ProfileName = _profileName;

			ctrlGridEventUpdater.ClassName = ClassName;
			ctrlGridEventUpdater.ViewName = ViewName;
			ctrlGridEventUpdater.PlaceName = _placeName;
			ctrlGridEventUpdater.GridId = grdMain.GridClientContainerId;

			//инициализируем тулбар
			MainMetaToolbar.ClassName = ClassName;
			MainMetaToolbar.ViewName = ViewName;
			MainMetaToolbar.PlaceName = _placeName;
			MainMetaToolbar.ToolbarMode = Mediachase.Ibn.Web.UI.MetaUI.Toolbar.MetaToolbar.Mode.ListViewUI;
			#endregion

			if (!Page.IsPostBack)
			{
				//биндим дропдауны
				BindDefaultValues();
				//устанавливаем в дропдаунах и key-textbox запомненные значения
				BindSavedValues();
				//биндим если надо группы в левой панели
				BindGroupDataList();
			}

			if (String.Compare(Request["Export"], ExportMode.Excel.ToString(), true) == 0)
				ExportExcel();
			else if (String.Compare(Request["Export"], ExportMode.Xml.ToString(), true) == 0)
				ExportXML();
			else if (String.Compare(Request["Export"], ExportMode.Vcf.ToString(), true) == 0 &&
					String.Compare(ClassName, ContactEntity.GetAssignedMetaClassName(), true) == 0)
				ExportVCF();
			else
				BindDataGrid(!Page.IsPostBack);

			//set header text
			if (!Page.IsPostBack)
				BindBlockHeader();

			#region CommandManager PreLoad
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand(ClassName, ViewName, _placeName, "MC_MUI_ProfileEdited");
			cm.AddCommand(ClassName, ViewName, _placeName, "MC_MUI_ExportTrue");
			#endregion
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			_pc["EntityList_" + ClassName + "_" + _profileName + "_PageIndex"] = grdMain.PageIndex.ToString();

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
				//если кто-то где-то изменил значение quickfilter с ролью - надо перезагрузить вид
				if (String.Compare(_profileName, ddProfiles.SelectedValue, true) != 0)
				{
					this.Page.Response.Redirect(this.Page.Request.RawUrl, true);
					return;
				}
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

			int currentUserId = Mediachase.Ibn.Data.Services.Security.CurrentUserId;

			//в случае, если отображаемый набор фильтров создан текущим пользователем - он может его удалить
			//генерируем imagebutton справа от набора фильтров
			ibDeleteInfo.ToolTip = GetGlobalResourceObject("IbnFramework.Incident", "_mc_DeleteView").ToString();
			ibDeleteInfo.Attributes.Add("onclick", String.Format("return confirm('{0}');", GetGlobalResourceObject("IbnFramework.Incident", "_mc_DeleteViewWarning").ToString()));
			ibDeleteInfo.Visible = Mediachase.Ibn.Core.ListViewProfile.CanDelete(ClassName, ddProfiles.SelectedValue, _placeName, currentUserId);

			ibEditInfo.ToolTip = GetGlobalResourceObject("IbnFramework.Common", "Edit").ToString();
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_MUI_ProfileEdit");
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("ClassName", ClassName);
			cp.AddCommandArgument("uid", ddProfiles.SelectedValue);
			string scriptEdit = cm.AddCommand(ClassName, ViewName, _placeName, cp);
			ibEditInfo.Attributes.Add("onclick", String.Format("javascript:{{{0};return false;}}", scriptEdit));
			ibEditInfo.Visible = Mediachase.Ibn.Core.ListViewProfile.CanDelete(ClassName, ddProfiles.SelectedValue, _placeName, currentUserId);


			cp = new CommandParameters("MC_MUI_ProfileNew");
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("ClassName", ClassName);
			scriptEdit = cm.AddCommand(ClassName, ViewName, _placeName, cp);
			scriptEdit = scriptEdit.Replace("\"", "&quot;");
			AddNewViewScript = scriptEdit;

			hfFilterValue.Value = ddProfiles.SelectedValue;

			// Printer Varsion
			if (CHelper.GetFromContext(CHelper.PrinterVersionKey) != null
				&& (bool)CHelper.GetFromContext(CHelper.PrinterVersionKey))
			{
				CHelper.PrintControl(PrepareExportGrid(), CHelper.GetResFileString(mc.PluralName), this.Page);
				dgExport.Visible = false;
			}

			base.OnPreRender(e);
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			//Profiles
			BindProfiles();

			//заполняем дропдаун группировки в левой панели
			if (_isGroupSet)
			{
				ddGrouping.Items.Clear();

				//group
				if (!String.IsNullOrEmpty(lvp.GroupByFieldName) && mc.Fields.Contains(lvp.GroupByFieldName))
				{
					ddGrouping.Items.Add(new ListItem(CHelper.GetResFileString(mc.Fields[lvp.GroupByFieldName].FriendlyName), lvp.GroupByFieldName));
				}
			}
		}
		#endregion

		#region BindProfiles
		private void BindProfiles()
		{
			ddProfiles.Items.Clear();

			ArrayList list = new ArrayList();
			ListViewProfile[] mas = ListViewProfile.GetSystemProfiles(ClassName, _placeName);

			List<KeyValuePair<string, string>> summaryList = new List<KeyValuePair<string, string>>();
			foreach (ListViewProfile lvp in mas)
			{
				summaryList.Add(new KeyValuePair<string, string>(lvp.Id, "  " + CHelper.GetResFileString(lvp.Name)));
				list.Add(lvp.Id);
			}

			summaryList.Sort
			   (
				  delegate(KeyValuePair<string, string> kvp1,
							   KeyValuePair<string, string> kvp2)
				  {
					  return Comparer<string>.Default.Compare(kvp1.Value, kvp2.Value);
				  }
				);
			if (mas.Length > 0)
				summaryList.Insert(0, new KeyValuePair<string, string>("-1", String.Format("[ {0} ]", GetGlobalResourceObject("IbnFramework.Incident", "SystemViews").ToString())));

			ListViewProfile[] mas2 = ListViewProfile.GetProfiles(ClassName, _placeName, Mediachase.Ibn.Data.Services.Security.CurrentUserId);

			List<KeyValuePair<string, string>> summaryList2 = new List<KeyValuePair<string, string>>();
			foreach (ListViewProfile lvp in mas2)
			{
				if (!list.Contains(lvp.Id))
					summaryList2.Add(new KeyValuePair<string, string>(lvp.Id, "  " + CHelper.GetResFileString(lvp.Name)));
			}

			summaryList2.Sort
			   (
				  delegate(KeyValuePair<string, string> kvp1,
							   KeyValuePair<string, string> kvp2)
				  {
					  return Comparer<string>.Default.Compare(kvp1.Value, kvp2.Value);
				  }
				);

			if (summaryList2.Count > 0)
			{
				summaryList.Add(new KeyValuePair<string, string>("-2", String.Format("[ {0} ]", GetGlobalResourceObject("IbnFramework.Incident", "UserViews").ToString())));
				summaryList.AddRange(summaryList2);
			}
			summaryList.Add(new KeyValuePair<string, string>("0", String.Format("[ {0} ]", GetGlobalResourceObject("IbnFramework.Incident", "NewIssueView").ToString())));

			ddProfiles.DataSource = summaryList;
			ddProfiles.DataTextField = "Value";
			ddProfiles.DataValueField = "Key";
			ddProfiles.DataBind();

			foreach (ListItem liItem in ddProfiles.Items)
			{
				if (liItem.Value == "-1" || liItem.Value == "-2")
				{
					liItem.Enabled = false;
					liItem.Attributes.Add("style", "color:#646464;");
				}
				if (liItem.Value == "0")
				{
					liItem.Attributes.Add("style", "color:#333333;font-weight:bold;");
				}
			}

			HtmlLink linkRss = (HtmlLink)this.Page.FindControl("rssLink");
			if (linkRss != null)
			{
				linkRss.Href = "";
				linkRss.Attributes.Add("title", string.Empty);
			}
		}
		#endregion

		#region BindGroupDataList
		private void BindGroupDataList()
		{
			//заполняем левый даталист для группировок
			if (!String.IsNullOrEmpty(lvp.GroupByFieldName))
			{
				string groupField = lvp.GroupByFieldName;

				if (mc != null && mc.Fields.Contains(groupField))
				{
					MetaField mf = mc.Fields[groupField];
					if (mf.IsEnum)
					{
						DataTable dt = new DataTable();
						dt.Columns.Add(new DataColumn("Id", typeof(int)));
						dt.Columns.Add(new DataColumn("Name", typeof(string)));

						foreach (MetaEnumItem item in MetaEnum.GetItems(mf.GetMetaType()))
						{
							DataRow row = dt.NewRow();
							row["Id"] = item.Handle;
							row["Name"] = CHelper.GetResFileString(item.Name);
							dt.Rows.Add(row);
						}

						DataView dv = dt.DefaultView;
						dv.Sort = "Name";

						groupList.DataSource = dv;
						groupList.DataKeyField = "Id";
						groupList.DataBind();

						//пытаемся найти и выделить последнюю запомненную группу
						int selectedIndex = 0;
						if (_pc[ViewName + "_" + groupField + "_" + GroupItemKey] != null)
						{
							foreach (DataListItem dli in groupList.Items)
							{
								LinkButton lb = (LinkButton)dli.FindControl("lbGroupItem");
								if (lb != null && lb.CommandArgument == _pc[ViewName + "_" + groupField + "_" + GroupItemKey])
								{
									selectedIndex = dli.ItemIndex;
									break;
								}
							}
						}

						if (groupList.Items.Count > 0)
							groupList.SelectedIndex = selectedIndex;

						//сохраняем выделенное значением _pc
						if (groupList.SelectedValue != null)
							_pc[ViewName + "_" + groupField + "_" + GroupItemKey] = groupList.SelectedValue.ToString();
						else
							_pc[ViewName + "_" + groupField + "_" + GroupItemKey] = "0";
					}
				}
			}
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			//выделяем последний запомненный набор фильтров
			CHelper.SafeSelect(ddProfiles, _profileName);

			//последнее запомненное значение key-textbox
			string key = ClassName + "_" + KeywordFilterKey;
			if (_pc[key] != null)
				txtSearch.Text = _pc[key];
		}
		#endregion

		#region BindBlockHeader
		/// <summary>
		/// Binds the tool bar.
		/// </summary>
		private void BindBlockHeader()
		{
			if (mc != null)
				BlockHeaderMain.Title = CHelper.GetResFileString(mc.PluralName);
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			_isFilterSet = false;
			_filterText = String.Empty;

			//добавляем к фильтру выбраное значение из левой панели, если включена группировка
			#region GroupItem
			if (_isGroupSet)
			{
				string groupCurrent = lvp.GroupByFieldName;
				if (!String.IsNullOrEmpty(groupCurrent) && mc.Fields.Contains(groupCurrent) && groupList.Items.Count > 0 && groupList.SelectedValue != null)
				{
					FilterElementCollection fec = new FilterElementCollection();
					fec.Add(FilterElement.EqualElement(groupCurrent, groupList.SelectedValue));
					grdMain.AddFilters = fec;
				}
			}
			#endregion

			grdMain.SearchKeyword = txtSearch.Text;

			if (dataBind)
				grdMain.DataBind();
		}
		#endregion

		#region grdMain_ChangingEntityGridColumnHeader
		void grdMain_ChangingEntityGridColumnHeader(object sender, ChangingEntityGridColumnHeaderEventArgs e)
		{
		}
		#endregion

		#region btnSearch_Click
		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			//фильтр по key-textbox
			_pc[ClassName + "_" + KeywordFilterKey] = txtSearch.Text;
			CHelper.RequireBindGrid();
		}
		#endregion

		#region btnClear_Click
		protected void btnClear_Click(object sender, ImageClickEventArgs e)
		{
			//очистка key-textbox
			txtSearch.Text = String.Empty;
			_pc[ClassName + "_" + KeywordFilterKey] = txtSearch.Text;
			CHelper.RequireBindGrid();
		}
		#endregion

		#region FilterSet_Changed
		protected void lbViewChange_Click(object sender, EventArgs e)
		{
			SetProfileName(_pc, ClassName, ddProfiles.SelectedValue);
			this.Page.Response.Redirect(this.Page.Request.RawUrl, true);
		}
		#endregion

		#region ibDeleteInfo_Click
		protected void ibDeleteInfo_Click(object sender, ImageClickEventArgs e)
		{
			//удаление созданного пользователем набора фильтров
			string sId = ddProfiles.SelectedValue;
			ListViewProfile.DeleteCustomProfile(ClassName, sId, _placeName);

			//ListViewProfile[] mas = ListViewProfile.GetProfiles(ClassName, _placeName, Mediachase.IBN.Business.Security.CurrentUser.UserID);
			//if (mas.Length == 0)
			//    throw new Exception("ListViewProfiles are not exist!");
			//SetProfileName(_pc, ClassName, mas[0].Id);

			SetProfileName(_pc, ClassName, GetFirstAvailableListViewProfile(ClassName));
			this.Page.Response.Redirect(this.Page.Request.RawUrl, true);
		}
		#endregion

		//Export
		#region ExportXML
		private void ExportXML()
		{
			EntityObject[] list = GetEntityObjectListForExport();

			DataTable dt = new DataTable();
			dgExport.Columns.Clear();

			ListViewProfile lvp = ListViewProfile.Load(ClassName, _profileName, _placeName);
			foreach (string fieldName in lvp.FieldSet)
			{
				MetaField field = Mediachase.Ibn.Web.UI.Controls.Util.FormController.GetMetaField(ClassName, fieldName);
				if (field == null)
					continue;
				if (field.IsReference)
				{
					dt.Columns.Add(new DataColumn(fieldName, typeof(string)));
					if (fieldName.ToLower().EndsWith("id"))
					{
						string refField = fieldName.Substring(0, fieldName.Length - 2);
						dt.Columns.Add(new DataColumn(refField, typeof(string)));
					}
				}
				else
				{
					//et 17.02.2009 - сохранение информации о типе
					Type managedType = Mediachase.Ibn.Lists.Mapping.Type2McDataType.McDataType2ManagedType(field.GetMetaType().McDataType);
					if (managedType != null)
					{
						DataColumn column = new DataColumn(fieldName, managedType);
						if (field.IsEnum)
						{
							column.MaxLength = 1024;
						}
						else
						{
							column.MaxLength = field.Attributes.GetValue<int>(McDataTypeAttribute.StringMaxLength, -1);
							column.Unique = field.Attributes.GetValue<bool>(McDataTypeAttribute.StringIsUnique, false);
						}
						//добавляем  сотoлбец в datatable
						dt.Columns.Add(column);
					}
				}
			}

			MakeDataViewFromListForXML(dt, list, null);

			Mediachase.UI.Web.Util.CommonHelper.SaveXML(dt, String.Format("{0}s.xml", ClassName));
		}

		private void MakeDataViewFromListForXML(DataTable dt, EntityObject[] list, string prefix)
		{
			DataRow dr;
			foreach (EntityObject entityObject in list)
			{
				dr = dt.NewRow();
				foreach (EntityObjectProperty entityProp in entityObject.Properties)
				{
					EntityObject agregatedEntity = entityProp.Value as EntityObject;
					if (agregatedEntity != null)	//Aggregation
					{
						MakeDataViewFromListForXML(dt, new EntityObject[] { agregatedEntity }, entityProp.Name);
					}
					else
					{
						string metaFieldName = string.IsNullOrEmpty(prefix) ? entityProp.Name : String.Format("{0}.{1}", prefix, entityProp.Name);
						if (dt.Columns.Contains(metaFieldName))
						{
							MetaField field = Mediachase.Ibn.Web.UI.Controls.Util.FormController.GetMetaField(ClassName, metaFieldName);
							object value = Mediachase.Ibn.Lists.Mapping.Type2McDataType.ConvertMcData2ManagedData(field, entityProp.Value);
							if (value != null)
							{
								dr[metaFieldName] = value;
							}
						}
					}
				}
				dt.Rows.Add(dr);
			}
		}

		#endregion

		#region ExportVCF
		private void ExportVCF()
		{
			EntityObject[] list = GetEntityObjectListForExport();
			List<PrimaryKeyId> pkeys = new List<PrimaryKeyId>();
			foreach (EntityObject eo in list)
				pkeys.Add(eo.PrimaryKeyId.Value);
			if (pkeys.Count > 0)
			{
				string str = Mediachase.Ibn.Clients.VCardUtil.Export(pkeys.ToArray());

				Response.Clear();
				Response.ContentType = "text/plain";
				Response.AddHeader("content-disposition", String.Format("attachment; filename={0}s.vcf", ClassName));

				Response.Write(str);
				Response.End();
			}
		}
		#endregion

		#region ExportExcel
		private void ExportExcel()
		{
			Mediachase.UI.Web.Util.CommonHelper.ExportExcel(PrepareExportGrid(), String.Format("{0}s.xls", ClassName), null);
		}
		#endregion

		#region PrepareExportGrid
		private DataGrid PrepareExportGrid()
		{
			EntityObject[] list = GetEntityObjectListForExport();

			DataTable dt = new DataTable();
			dgExport.Columns.Clear();

			ListViewProfile lvp = ListViewProfile.Load(ClassName, _profileName, _placeName);
			foreach (string fieldName in lvp.FieldSet)
			{
				MetaField field = Mediachase.Ibn.Web.UI.Controls.Util.FormController.GetMetaField(ClassName, fieldName);
				if (field == null)
					continue;
				BoundColumn bc = new BoundColumn();
				bc.HeaderText = CHelper.GetResFileString(field.FriendlyName);
				string dataField = fieldName;

				if (field.IsReference)
				{
					if (fieldName.ToLower().EndsWith("id"))
					{
						dataField = fieldName.Substring(0, fieldName.Length - 2);
						if (dt.Columns.Contains(dataField))
							continue;

						dt.Columns.Add(new DataColumn(dataField, typeof(string)));
					}
					else
					{
						if (dt.Columns.Contains(dataField))
							continue;

						dt.Columns.Add(new DataColumn(fieldName, typeof(string)));
					}
				}
				else
				{
					if (dt.Columns.Contains(dataField))
						continue;

					dt.Columns.Add(new DataColumn(fieldName, typeof(string)));
				}


				bc.DataField = dataField;
				dgExport.Columns.Add(bc);
			}

			MakeDataViewFromList(dt, list);

			DataView dv = dt.DefaultView;
			dgExport.Visible = true;
			dgExport.DataSource = dv;
			dgExport.DataBind();

			return dgExport;
		} 
		#endregion

		#region GetEntityObjectListForExport
		private EntityObject[] GetEntityObjectListForExport()
		{

			string variant = "0";
			if (Request["variant"] != null)
				variant = Request["variant"];

			SortingElementCollection sec = new SortingElementCollection();
			if (_pc[grdMain.GetPropertyKey(EntityGrid.SortingPropertyKey)] != null)
			{
				string sort = _pc[grdMain.GetPropertyKey(EntityGrid.SortingPropertyKey)];
				SortingElementType set = SortingElementType.Asc;
				if (sort.IndexOf(" DESC") >= 0)
				{
					sort = sort.Substring(0, sort.IndexOf(" DESC"));
					set = SortingElementType.Desc;
				}

				sec.Add(new SortingElement(sort, set));
			}

			int pageSize = 10;
			if (_pc[grdMain.GetPropertyKey(EntityGrid.PageSizePropertyKey)] != null)
			{
				pageSize = Convert.ToInt32(_pc[grdMain.GetPropertyKey(EntityGrid.PageSizePropertyKey)].ToString());
				if (pageSize == -1)
					pageSize = 10000;
			}
			int pageIndex = 0;
			if (_pc["EntityList_" + ClassName + "_" + _profileName + "_PageIndex"] != null)
				pageIndex = int.Parse(_pc["EntityList_" + ClassName + "_" + _profileName + "_PageIndex"]);

			ListViewProfile lvp = null;
			switch (variant)
			{
				case "1":	//OnlyView
					lvp = ListViewProfile.Load(ClassName, _profileName, _placeName);
					if (sec.Count == 0)
						sec = lvp.Sorting;
					return BusinessManager.List(ClassName, lvp.Filters.ToArray(), sec.ToArray());
				case "2":	//Only Visible
					lvp = ListViewProfile.Load(ClassName, _profileName, _placeName);

					FilterElementCollection fec = new FilterElementCollection(lvp.Filters.ToArray());

					if (!String.IsNullOrEmpty(txtSearch.Text))
					{
						FilterElement fe = CHelper.GetSearchFilterElementByKeyword(txtSearch.Text, ClassName);
						fec.Add(fe);
					}

					EntityObject[] list = BusinessManager.List(ClassName, fec.ToArray(), sec.ToArray());
					List<EntityObject> newList = new List<EntityObject>();
					int start = pageIndex * pageSize;
					int finish = start + pageSize;
					for (int i = start; i <= finish && i < list.Length; i++)
						newList.Add(list[i]);
					return newList.ToArray();
				case "3":	//Only Selected
					string values = Request["ids"];
					string[] selectedElements = values.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
					List<PrimaryKeyId> listPkey = new List<PrimaryKeyId>();
					foreach (string elem in selectedElements)
					{
						string id = elem.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0];
						PrimaryKeyId key = PrimaryKeyId.Parse(id);
						if (key != PrimaryKeyId.Empty)
							listPkey.Add(key);
					}
					FilterElementCollection fecPkey = new FilterElementCollection();
					foreach (PrimaryKeyId id in listPkey)
					{
						FilterElement fe = FilterElement.EqualElement(FilterElement.PrimaryKeyFieldName, id);
						fecPkey.Add(fe);
					}
					FilterElement orFilter = new OrBlockFilterElement(fecPkey.ToArray());
					fecPkey.Clear();
					fecPkey.Add(orFilter);

					return BusinessManager.List(ClassName, fecPkey.ToArray(), sec.ToArray());
				default:	//All
					return BusinessManager.List(ClassName, (new FilterElementCollection()).ToArray(), sec.ToArray());
			}
		}
		#endregion

		#region MakeDataViewFromList
		private void MakeDataViewFromList(DataTable dt, EntityObject[] list)
		{
			DataRow dr;
			foreach (EntityObject eo in list)
			{
				dr = dt.NewRow();
				foreach (EntityObjectProperty eop in eo.Properties)
				{
					if (eop.Value is EntityObject)	//Aggregation
					{
						if (eop.Value == null)
							continue;
						EntityObject aggrObj = (EntityObject)eop.Value;
						foreach (EntityObjectProperty eopAggr in aggrObj.Properties)
						{
							if (dt.Columns.Contains(String.Format("{0}.{1}", eop.Name, eopAggr.Name)))
								dr[String.Format("{0}.{1}", eop.Name, eopAggr.Name)] = (eopAggr.Value == null) ? String.Empty : GetMetaPropertyValue(eopAggr, String.Format("{0}.{1}", eop.Name, eopAggr.Name));
						}
					}
					else
					{
						if (dt.Columns.Contains(eop.Name))
							dr[eop.Name] = (eop.Value == null) ? String.Empty : GetMetaPropertyValue(eop, eop.Name);
					}
				}
				dt.Rows.Add(dr);
			}
		}
		#endregion

		#region GetMetaPropertyValue
		private string GetMetaPropertyValue(EntityObjectProperty eop, string fieldName)
		{
			MetaField field = Mediachase.Ibn.Web.UI.Controls.Util.FormController.GetMetaField(ClassName, fieldName);
			string value = String.Empty;
			if (field == null)
				return value;
			if (field.IsMultivalueEnum)
			{
				MetaFieldType type = field.GetMetaType();

				int[] idList = (int[])eop.Value;
				foreach (int id in idList)
				{
					if (value != String.Empty)
						value += "<br />";
					value += CHelper.GetResFileString(MetaEnum.GetFriendlyName(type, id));
				}
			}
			else if (field.IsEnum)
			{
				value = CHelper.GetResFileString(MetaEnum.GetFriendlyName(field.GetMetaType(), (int)eop.Value));
			}
			//else if (field.IsReference)
			//{
			//    string sReferencedClass = field.Attributes.GetValue<string>(McDataTypeAttribute.ReferenceMetaClassName);
			//    EntityObject obj = BusinessManager.Load(sReferencedClass, (PrimaryKeyId)eop.Value);
			//    MetaClass mcRef = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName(sReferencedClass);
			//    value = obj.Properties[mcRef.TitleFieldName].Value.ToString();
			//}

            //dvs: hot-fix (IBNPORTAL_ERROR-008286-27094)
            else if (field.Name == "CreatorId" || field.Name == "ModifierId")
            {
                value = CommonHelper.GetUserStatusPureName((int)eop.Value);
            }
            else
				value = eop.Value.ToString();
            
            
			return value;
		}
		#endregion

		//static methods
		#region GetProfileName
		public static string GetProfileName(UserLightPropertyCollection pc, string className)
		{
			string key = className + "_" + ProfileNameKey;

			if (pc[key] == null)
				pc[key] = GetFirstAvailableListViewProfile(className);

			return pc[key];
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
				int currentUserId = Mediachase.Ibn.Data.Services.Security.CurrentUserId;

				mas = ListViewProfile.GetProfiles(className, _placeName, currentUserId);
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

					ListViewProfile.SaveCustomProfile(className, _placeName, currentUserId, profile);
					mas = ListViewProfile.GetProfiles(className, _placeName, currentUserId);
					if (mas.Length == 0)
						throw new Exception("ListViewProfiles are not exist!");
				}
			}
			return mas[0].Id;
		}
		#endregion

		#region Grouping_ItemCommand
		protected void groupList_ItemCommand(object source, DataListCommandEventArgs e)
		{
			//выбор группы в левой панели
			if (e.CommandName == "Group")
			{
				_pc[ViewName + "_" + lvp.GroupByFieldName + "_" + GroupItemKey] = e.CommandArgument.ToString();
				upLeftArea.Update();
				upFilters.Update();
				if (groupList.Items.Count > 0)
					groupList.SelectedIndex = e.Item.ItemIndex;
				CHelper.RequireBindGrid();
			}
		}
		#endregion
	}
}