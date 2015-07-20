using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.UI.Web.Util;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules
{
	public partial class IncidentListNew : System.Web.UI.UserControl
	{
		private const string _className = "Issue";
		private string _viewName = "";
		private const string _placeName = "IssueListNew";
		private string _fieldSetName = Incident.IssueFieldSet.IncidentsDefault.ToString();

		public const string IssueListGroupItemKey = "IssueListNew_GroupObjectId";
		public const string IssueListViewNameKey = "IssueListNew_ViewName";
		public const string IssueListFieldSetNameKey = "IssueListNew_FieldSetName";
		public const string IssueListGroupFieldKey = "IssueListNew_GroupField";
		
		protected string AddNewViewScript = String.Empty;

		private bool _isFilterSet = false;
		private string _filterText = String.Empty;

		private bool _isGroupSet = false;
		private string _groupFieldName = Incident.AvailableGroupField.NotSet.ToString();

		protected UserLightPropertyCollection _pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		protected void Page_Load(object sender, EventArgs e)
		{
			#region PreInit Parameters
			//получить ListViewProfiles и заполнить поля _isGroupSet и _groupFieldName
			_viewName = GetViewName(_pc);
			ListViewProfile lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
			if (lvp == null)
			{
				_viewName = GetFirstAvailableListViewProfile();
				SetViewName(_pc, _viewName);
				lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
			}
			_isGroupSet = !String.IsNullOrEmpty(lvp.GroupByFieldName);
			_groupFieldName = GetGroupFieldForView(lvp);
			_fieldSetName = lvp.FieldSetName; 
			#endregion

			//для вида с группировкой показываем левую панель
			if (_isGroupSet)
				DockLeft.DefaultSize = 230;
			else
				DockLeft.DefaultSize = 0;

			#region Init MCGrid, Toolbar
			grdMain.ChangingMCGridColumnHeader += new ChangingMCGridColumnHeaderEventHandler(grdMain_ChangingMCGridColumnHeader);
			//инициализируем грид
			grdMain.ClassName = _className;
			grdMain.ViewName = _fieldSetName;
			grdMain.PlaceName = _placeName;

			ctrlGridEventUpdater.ClassName = _className;
			ctrlGridEventUpdater.ViewName = _viewName;
			ctrlGridEventUpdater.PlaceName = _placeName;
			ctrlGridEventUpdater.GridId = grdMain.GridClientContainerId;

			//инициализируем тулбар
			MainMetaToolbar.ClassName = _className;
			MainMetaToolbar.ViewName = _fieldSetName;
			MainMetaToolbar.PlaceName = _placeName; 
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

			if (Request["Export"] == "1")
				ExportGrid();
			else if (Request["Export"] == "2")
				ExportXMLTable();
			else
				BindDataGrid(!Page.IsPostBack);

			//set header text
			if (!Page.IsPostBack)
				BindBlockHeader();

			#region CommandManager PreLoad
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand(_className, _viewName, _placeName, "MC_HDM_NewViewCreated");
			cm.AddCommand(_className, _viewName, _placeName, "MC_HDM_FiltersEdit");
			cm.AddCommand(_className, _viewName, _placeName, "MC_HDM_ExportTrue");
			cm.AddCommand(_className, _viewName, _placeName, "MC_HDM_GroupResponsibilityList");
			cm.AddCommand(_className, _viewName, _placeName, "MC_HDM_ChangeStatusServer");
			cm.AddCommand(_className, _viewName, _placeName, "MC_HDM_ChangeResponsibleServer"); 
			#endregion
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			_pc["MCGrid_IssueList_" + _viewName + "_PageIndex"] = grdMain.PageIndex.ToString();

			//кнопка очистить фильтр у key-textbox
			btnClear.Visible = !String.IsNullOrEmpty(txtSearch.Text);

			//поле поиска при группировке по клиенту
			divLeftSearchContainer.Visible = (_groupFieldName == Incident.AvailableGroupField.Client.ToString());
			if (divLeftSearchContainer.Visible)
			{
				ibGroupClear.Visible = !String.IsNullOrEmpty(txtGroupSearch.Text.Trim());
				txtGroupSearch.Width = (String.IsNullOrEmpty(txtGroupSearch.Text.Trim())) ? Unit.Pixel(160) : Unit.Pixel(140); 
			}

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
				if (String.Compare(_viewName, ddFilters.SelectedValue, true) != 0)
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
				//spanFilters.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#FFF090");
				sText = _filterText;
			}
			else
			{
				if (_isFilterSet)
				{
					//spanFilters.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#FFF090");
					sText = GetGlobalResourceObject("IbnFramework.Global", "FilterIsSet").ToString();
				}
				else
				{
					//spanFilters.Style.Add(HtmlTextWriterStyle.BackgroundColor, "inherit");
					sText = GetGlobalResourceObject("IbnFramework.Global", "FilterIsNotSet").ToString();
				}
			}
			FilterIsSet.ForeColor = Color.FromArgb(0x90, 0x90, 0x90);
			FilterIsSet.Text = sText;

			//в случае, если отображаемый набор фильтров создан текущим пользователем - он может его удалить
			//генерируем imagebutton справа от набора фильтров
			ibDeleteInfo.ToolTip = GetGlobalResourceObject("IbnFramework.Incident", "_mc_DeleteView").ToString();
			ibDeleteInfo.Attributes.Add("onclick", String.Format("return confirm('{0}');", GetGlobalResourceObject("IbnFramework.Incident", "_mc_DeleteViewWarning").ToString()));
			ibDeleteInfo.Visible = Mediachase.Ibn.Core.ListViewProfile.CanDelete(_className, ddFilters.SelectedValue, String.Empty, Mediachase.IBN.Business.Security.CurrentUser.UserID);

			ibEditInfo.ToolTip = GetGlobalResourceObject("IbnFramework.Common", "Edit").ToString();
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_HDM_FiltersEdit");
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("uid", ddFilters.SelectedValue);
			string scriptEdit = cm.AddCommand(_className, _viewName, _placeName, cp);
			ibEditInfo.Attributes.Add("onclick", String.Format("javascript:{{{0};return false;}}", scriptEdit));
			ibEditInfo.Visible = Mediachase.Ibn.Core.ListViewProfile.CanDelete(_className, ddFilters.SelectedValue, String.Empty, Mediachase.IBN.Business.Security.CurrentUser.UserID);


			cp = new CommandParameters("MC_HDM_FiltersNew");
			scriptEdit = cm.AddCommand(_className, _fieldSetName, _placeName, cp);
			scriptEdit = scriptEdit.Replace("\"", "&quot;");
			AddNewViewScript = scriptEdit;

			hfFilterValue.Value = ddFilters.SelectedValue;
			
			base.OnPreRender(e);
		}

		
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			//FilterSets
			BindFilterSets();

			//заполняем дропдаун быстрого переключения по группировкам в левой панели
			if (_isGroupSet)
			{
				ddGrouping.Items.Clear();

				ddGrouping.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "GroupPrj").ToString(), Incident.AvailableGroupField.Project.ToString()));
				if (PortalConfig.GeneralAllowClientField || _groupFieldName == Incident.AvailableGroupField.Client.ToString())
					ddGrouping.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "GroupContacts").ToString(), Incident.AvailableGroupField.Client.ToString()));
				ddGrouping.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "GroupIssBox").ToString(), Incident.AvailableGroupField.IssueBox.ToString()));
				ddGrouping.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "GroupRespTitle").ToString(), Incident.AvailableGroupField.Responsible.ToString()));
			}
		}
		#endregion

		#region BindFilterSets
		private void BindFilterSets()
		{
			//Views
			//получаем набор view для текущего пользователя
			ddFilters.Items.Clear();

			ArrayList list = new ArrayList();
			ListViewProfile[] mas = ListViewProfile.GetSystemProfiles(_className, String.Empty);

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
			summaryList.Insert(0, new KeyValuePair<string, string>("-1", String.Format("[ {0} ]", GetGlobalResourceObject("IbnFramework.Incident", "SystemViews").ToString())));

			ListViewProfile[] mas2 = ListViewProfile.GetProfiles(_className, String.Empty, Mediachase.IBN.Business.Security.CurrentUser.UserID);

			List<KeyValuePair<string, string>> summaryList2 = new List<KeyValuePair<string, string>>();
			foreach (ListViewProfile lvp in mas2)
			{
				if(!list.Contains(lvp.Id))
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

			ddFilters.DataSource = summaryList;
			ddFilters.DataTextField = "Value";
			ddFilters.DataValueField = "Key";
			ddFilters.DataBind();

			foreach (ListItem liItem in ddFilters.Items)
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
		}
		#endregion

		#region BindGroupDataList
		private void BindGroupDataList()
		{
			//заполняем левый даталист для группировок
			#region GroupByProject
			if (_groupFieldName == Incident.AvailableGroupField.Project.ToString())
			{
				//собираем таблицу с проектами
				DataTable dt = new DataTable();
				dt.Columns.Add(new DataColumn("ObjectId", typeof(string)));
				dt.Columns.Add(new DataColumn("Title", typeof(string)));
				dt.Columns.Add(new DataColumn("Weight", typeof(int)));
				DataRow dr;

				//filterSet - if filterset has a ProjectId - group has only one project
				//если предустановлен набор фильтров, в котором задан фильтр по проекту 
				//- в список попадет только одна запись - соответствующая этому проекту
				int onlyPrj = 0;
				ListViewProfile lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
				FilterElementCollection fec = lvp.Filters;
				foreach (FilterElement fe in fec)
					if (fe.Source == Incident.ProjectFilterKey)
					{
						string value = fe.Value.ToString();
						if (fe.ValueIsTemplate)
							value = TemplateResolver.ResolveAll(value);
						onlyPrj = int.Parse(value);
					}

				using (IDataReader reader = Incident.GetListProjectsForIncidents())
				{
					while (reader.Read())
					{
						if (onlyPrj != 0 && (int)reader["ProjectId"] != onlyPrj)
							continue;
						dr = dt.NewRow();
						dr["ObjectId"] = reader["ProjectId"].ToString();
						dr["Title"] = String.Format("{0}"/*"{0} ({1})"*/, reader["Title"].ToString(), reader["IncidentCount"].ToString());
						dr["Weight"] = 1;
						dt.Rows.Add(dr);
					}
				}

				//добавляем запись [Вне проектов] и узнаем сколько инцидентов есть для такого случая
				//DataTable dtTest = Incident.GetListIncidentsByFilterDataTable
				//        (-1, 0, 0, 0, 0, 0, 0,
				//        0, -1, 0, 0, 0, String.Empty, 0, 0);
				//if (dtTest.Rows.Count > 0)
				//{
					if (onlyPrj == -1 || onlyPrj == 0)
					{
						dr = dt.NewRow();
						dr["ObjectId"] = -1;
						dr["Title"] = String.Format("{0}"/*"{0} ({1})"*/, GetGlobalResourceObject("IbnFramework.Incident", "GroupNoPrj").ToString()/*, dtTest.Rows.Count*/);
						dr["Weight"] = 0;
						dt.Rows.Add(dr);
					}
				//}

				//Weight нужен чтобы запись [Вне проектов] была всегда первой
				DataView dv = dt.DefaultView;
				dv.Sort = "Weight, Title";

				groupList.DataSource = dv;
				groupList.DataKeyField = "ObjectId";
				groupList.DataBind();

				//пытаемся найти и выделить последнюю запомненную группу
				int selectedIndex = 0;
				if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
				{
					foreach (DataListItem dli in groupList.Items)
					{
						LinkButton lb = (LinkButton)dli.FindControl("lbGroupItem");
						if (lb != null && lb.CommandArgument == _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey])
						{
							selectedIndex = dli.ItemIndex;
							break;
						}
					}
				}

				if (groupList.Items.Count > 0)
					groupList.SelectedIndex = selectedIndex;

				//сохраняем выделенное значением (актуально для первого раза или вариант что проект с запомненным id пропал из списка) _pc
				if (groupList.SelectedValue != null)
					_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] = groupList.SelectedValue.ToString();
				else
					_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] = "-1";
			}
			#endregion

			#region GroupByClient
			if (_groupFieldName == Incident.AvailableGroupField.Client.ToString())
			{
				//собираем таблицу с организациями и клиентами
				DataTable dt = new DataTable();
				dt.Columns.Add(new DataColumn("ObjectId", typeof(string)));
				dt.Columns.Add(new DataColumn("Title", typeof(string)));
				dt.Columns.Add(new DataColumn("Weight", typeof(int)));
				DataRow dr;

				//filterSet - if filterset has a Client - group has only one client
				//если предустановлен набор фильтров, в котором задан фильтр по клиенту 
				//- в список попадет только одна запись - соответствующая этому клиенту
				string onlyClient = "_";
				ListViewProfile lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
				FilterElementCollection fec = lvp.Filters;
				foreach (FilterElement fe in fec)
					if (fe.Source == Incident.ClientFilterKey)
					{
						string value = fe.Value.ToString();
						if (fe.ValueIsTemplate)
							value = TemplateResolver.ResolveAll(value);
						onlyClient = value;
					}

				DataTable dtClients = Incident.GetListContactsForIncidentsDataTable(txtGroupSearch.Text.Trim(), 5);
				foreach (DataRow drClient in dtClients.Rows)
				{
					dr = dt.NewRow();
					string id = String.Format("{0}_{1}",
						((int)drClient["ObjectTypeId"] == (int)ObjectTypes.Organization) ? "org" : "contact",
						drClient["ObjectId"].ToString());

					if (onlyClient != "_" && id != onlyClient)
						continue;
					dr["ObjectId"] = id;
					dr["Title"] = String.Format("{0}"/*"{0} ({1})"*/, drClient["ClientName"].ToString(), drClient["IncidentCount"].ToString());
					dr["Weight"] = 1;
					dt.Rows.Add(dr);
				}
				//using (IDataReader reader = Incident.GetListContactsForIncidents())
				//{
				//    while (reader.Read())
				//    {
				//        dr = dt.NewRow();
				//        string id = String.Format("{0}_{1}",
				//            ((int)reader["ObjectTypeId"] == (int)ObjectTypes.Organization) ? "org" : "contact",
				//            reader["ObjectId"].ToString());

				//        if (onlyClient != "_" && id != onlyClient)
				//            continue;
				//        dr["ObjectId"] = id;
				//        dr["Title"] = String.Format("{0}"/*"{0} ({1})"*/, reader["ClientName"].ToString(), reader["IncidentCount"].ToString());
				//        dt.Rows.Add(dr);
				//    }
				//}

				//добавляем запись [все] и узнаем сколько инцидентов есть для такого случая
				dr = dt.NewRow();
				dr["ObjectId"] = "_";
				dr["Title"] = String.Format("[ {0} ]",
					GetGlobalResourceObject("IbnFramework.Incident", "AllSetMale").ToString());
				dr["Weight"] = 0;
				dt.Rows.Add(dr);

				DataView dv = dt.DefaultView;
				dv.Sort = "Weight, Title";

				groupList.DataSource = dv;
				groupList.DataKeyField = "ObjectId";
				groupList.DataBind();

				//пытаемся найти и выделить последнюю запомненную группу
				int selectedIndex = 0;
				if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
				{
					foreach (DataListItem dli in groupList.Items)
					{
						LinkButton lb = (LinkButton)dli.FindControl("lbGroupItem");
						if (lb != null && lb.CommandArgument == _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey])
						{
							selectedIndex = dli.ItemIndex;
							break;
						}
					}
				}

				if (groupList.Items.Count > 0)
					groupList.SelectedIndex = selectedIndex;

				//сохраняем выделенное значением (актуально для первого раза или вариант что клиент с запомненным id пропал из списка) _pc
				if (groupList.SelectedValue != null)
					_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] = groupList.SelectedValue.ToString();
				else
					_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] = "_";
			}
			#endregion

			#region GroupByBox
			if (_groupFieldName== Incident.AvailableGroupField.IssueBox.ToString())
			{
				//собираем таблицу с папками инцидентов
				DataTable dt = new DataTable();
				dt.Columns.Add(new DataColumn("ObjectId", typeof(string)));
				dt.Columns.Add(new DataColumn("Title", typeof(string)));
				dt.Columns.Add(new DataColumn("Weight", typeof(int)));
				DataRow dr;

				//filterSet - if filterset has a IssBoxFilter - group has only one issBox
				//если предустановлен набор фильтров, в котором задан фильтр по папке 
				//- в список попадет только одна запись - соответствующая этой папке
				int onlyIssBox = 0;
				ListViewProfile lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
				FilterElementCollection fec = lvp.Filters;
				foreach (FilterElement fe in fec)
					if (fe.Source == Incident.IssueBoxFilterKey)
					{
						string value = fe.Value.ToString();
						if (fe.ValueIsTemplate)
							value = TemplateResolver.ResolveAll(value);
						onlyIssBox = int.Parse(value);
					}

				IncidentBox[] ibMas = IncidentBox.List();
				foreach (IncidentBox ib in ibMas)
				{
					if (onlyIssBox != 0 && ib.IncidentBoxId != onlyIssBox)
						continue;

					////проверяем количество инцидентов в папке - папки с 0 не берем
					//DataTable dtTest = Incident.GetListIncidentsByFilterDataTable
					//    (0, 0, 0, 0, 0, 0, 0,
					//    ib.IncidentBoxId, -1, 0, 0, 0, String.Empty, 0, 0);
					//if (dtTest.Rows.Count == 0)
					//    continue;

					dr = dt.NewRow();
					dr["ObjectId"] = ib.IncidentBoxId.ToString();
					dr["Title"] = String.Format("{0}"/*"{0} ({1})"*/, ib.Name/*, dtTest.Rows.Count*/);
					dr["Weight"] = 1;
					dt.Rows.Add(dr);
				}

				//добавляем запись [все] и узнаем сколько инцидентов есть для такого случая
				dr = dt.NewRow();
				dr["ObjectId"] = 0;
				dr["Title"] = String.Format("[ {0} ]",
					GetGlobalResourceObject("IbnFramework.Incident", "AllSet").ToString());
				dr["Weight"] = 0;
				dt.Rows.Add(dr);

				DataView dv = dt.DefaultView;
				dv.Sort = "Weight, Title";

				groupList.DataSource = dv;
				groupList.DataKeyField = "ObjectId";
				groupList.DataBind();

				//пытаемся найти и выделить последнюю запомненную группу
				int selectedIndex = 0;
				if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
				{
					foreach (DataListItem dli in groupList.Items)
					{
						LinkButton lb = (LinkButton)dli.FindControl("lbGroupItem");
						if (lb != null && lb.CommandArgument == _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey])
						{
							selectedIndex = dli.ItemIndex;
							break;
						}
					}
				}

				if (groupList.Items.Count > 0)
					groupList.SelectedIndex = selectedIndex;

				//сохраняем выделенное значением (актуально для первого раза или вариант что проект с запомненным id пропал из списка) _pc
				if (groupList.SelectedValue != null)
					_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] = groupList.SelectedValue.ToString();
				else
					_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] = dv[0]["ObjectId"].ToString();
			}
			#endregion

			#region GroupByResp
			if (_groupFieldName == Incident.AvailableGroupField.Responsible.ToString())
			{
				//собираем таблицу с папками инцидентов
				DataTable dt = new DataTable();
				dt.Columns.Add(new DataColumn("ObjectId", typeof(string)));
				dt.Columns.Add(new DataColumn("Title", typeof(string)));
				dt.Columns.Add(new DataColumn("Weight", typeof(int)));
				DataRow dr;

				//filterSet - if filterset has a ResponsibleId - group has only one responsible
				//если предустановлен набор фильтров, в котором задан фильтр по ответственному 
				//- в список попадет только одна запись - соответствующая этому ответственному
				int onlyResp = 0;
				ListViewProfile lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
				FilterElementCollection fec = lvp.Filters;
				foreach (FilterElement fe in fec)
					if (fe.Source == Incident.ResponsibleFilterKey)
					{
						string value = fe.Value.ToString();
						if (fe.ValueIsTemplate)
							value = TemplateResolver.ResolveAll(value);
						onlyResp = int.Parse(value);
					}
				DataTable dtResp = Incident.GetListIncidentResponsiblesDT();
				//DataTable dtTest = new DataTable();
				foreach (DataRow drResp in dtResp.Rows)
				{
					if (onlyResp != 0 && (int)drResp["UserId"] != onlyResp)
						continue;

					////проверяем количество инцидентов для ответственного - ответственных с 0 не берем
					//dtTest = Incident.GetListIncidentsByFilterDataTable
					//    (0, 0, 0, 0, (int)drResp["UserId"], 0, 0,
					//    0, -1, 0, 0, 0, String.Empty, 0, 0);
					//if (dtTest.Rows.Count == 0)
					//    continue;

					dr = dt.NewRow();
					dr["ObjectId"] = (int)drResp["UserId"];
					dr["Title"] = String.Format("{0}"/*"{0} ({1})"*/, drResp["UserName"].ToString()/*, dtTest.Rows.Count*/);
					dr["Weight"] = 4;
					dt.Rows.Add(dr);
				}

				//добавляем запись [нераспределенные] и узнаем сколько инцидентов есть для такого случая
				//dtTest = Incident.GetListIncidentsByFilterDataTable
				//        (0, 0, 0, 0, -1, 0, 0,
				//        0, -1, 0, 0, 0, String.Empty, 0, 0);
				//if (dtTest.Rows.Count > 0)
				//{
				if (onlyResp == -1 || onlyResp == 0)
				{
					dr = dt.NewRow();
					dr["ObjectId"] = -1;
					dr["Title"] = String.Format("{0}",
						GetGlobalResourceObject("IbnFramework.Incident", "OutOfPersonalResponsibility").ToString());
					
					dr["Weight"] = 0;
					dt.Rows.Add(dr);
				}
				//}

				//добавляем запись [не назначен] и узнаем сколько инцидентов есть для такого случая
				//dtTest = Incident.GetListIncidentsByFilterDataTable
				//        (0, 0, 0, 0, -2, 0, 0,
				//        0, -1, 0, 0, 0, String.Empty, 0, 0);
				//if (dtTest.Rows.Count > 0)
				//{
					if (onlyResp == -2 || onlyResp == -1 || onlyResp == 0)
					{
						dr = dt.NewRow();
						dr["ObjectId"] = -2;
						dr["Title"] = String.Format("<span><img alt='' src='{1}'/><span> <span>{0}</span>",
							GetGlobalResourceObject("IbnFramework.Incident", "ResponsibleNotAssigned").ToString(),
							/*dtTest.Rows.Count,*/
							Page.ResolveUrl("~/Layouts/Images/not_set.png"));
						dr["Weight"] = 1;
						dt.Rows.Add(dr);
					}
				//}

				//добавляем запись [группа - waiting] и узнаем сколько инцидентов есть для такого случая
				//dtTest = Incident.GetListIncidentsByFilterDataTable
				//        (0, 0, 0, 0, -3, 0, 0,
				//        0, -1, 0, 0, 0, String.Empty, 0, 0);
				//if (dtTest.Rows.Count > 0)
				//{
					if (onlyResp == -3 || onlyResp == -1 || onlyResp == 0)
					{
						dr = dt.NewRow();
						dr["ObjectId"] = -3;
						dr["Title"] = String.Format("<span><img alt='' src='{1}'/><span> <span>{0}</span>",
							GetGlobalResourceObject("IbnFramework.Incident", "tRespGroup").ToString(),
							Page.ResolveUrl("~/Layouts/Images/waiting.gif"));
						dr["Weight"] = 2;
						dt.Rows.Add(dr);
					}
				//}

				//добавляем запись [группа - all denied] и узнаем сколько инцидентов есть для такого случая
				//dtTest = Incident.GetListIncidentsByFilterDataTable
				//        (0, 0, 0, 0, -4, 0, 0,
				//        0, -1, 0, 0, 0, String.Empty, 0, 0);
				//if (dtTest.Rows.Count > 0)
				//{
					if (onlyResp == -4 || onlyResp == -1 || onlyResp == 0)
					{
						dr = dt.NewRow();
						dr["ObjectId"] = -4;
						dr["Title"] = String.Format("<span><img alt='' src='{1}'/><span> <span>{0}</span>",
							GetGlobalResourceObject("IbnFramework.Incident", "tRespGroup").ToString(),
							/*dtTest.Rows.Count,*/
							Page.ResolveUrl("~/Layouts/Images/red_denied.gif"));
						dr["Weight"] = 3;
						dt.Rows.Add(dr);
					}
				//}

				//Weight нужен чтобы спец записи были всегда первыми
				DataView dv = dt.DefaultView;
				dv.Sort = "Weight, Title";

				groupList.DataSource = dv;
				groupList.DataKeyField = "ObjectId";
				groupList.DataBind();

				//пытаемся найти и выделить последнюю запомненную группу
				int selectedIndex = 0;
				if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
				{
					foreach (DataListItem dli in groupList.Items)
					{
						LinkButton lb = (LinkButton)dli.FindControl("lbGroupItem");
						if (lb != null && lb.CommandArgument == _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey])
						{
							selectedIndex = dli.ItemIndex;
							break;
						}
					}
				}

				if (groupList.Items.Count > 0)
					groupList.SelectedIndex = selectedIndex;

				//сохраняем выделенное значением (актуально для первого раза или вариант что ответственный с запомненным id пропал из списка) _pc
				if (groupList.SelectedValue != null)
					_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] = groupList.SelectedValue.ToString();
				else
					_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] = dv[0]["ObjectId"].ToString();
			}
			#endregion
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			//выделяем последний запомненный набор фильтров
			CHelper.SafeSelect(ddFilters, _viewName);

			//последнее запомненное значение key-textbox
			if (_pc[Incident.KeywordFilterKey] != null)
				txtSearch.Text = _pc[Incident.KeywordFilterKey];

			//для вида с группировкой устанавливаем значение равное текущему _groupFieldName
			if (_isGroupSet)
				CHelper.SafeSelect(ddGrouping, _groupFieldName);
		}
		#endregion

		#region BindBlockHeader
		/// <summary>
		/// Binds the tool bar.
		/// </summary>
		private void BindBlockHeader()
		{
			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.Incident", "IncidentList").ToString();
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			//проверяет установлены ли какие-то значения фильтров, кроме предопределенных - только для случая [Настроить]
			_isFilterSet = false;
			//формируется строка для FilterView
			_filterText = String.Empty;
			//выбрано ли в наборе фильтров только неотвеченные
			bool isUnansweredOnly = false;
			bool isOverdueOnly = false;

			#region Variables
			int projId = 0; int iManId = 0; int iRespId = 0; int iCreatorId = 0; int iResId = 0;
			int priority_id = -1; int issbox_id = 0; int type_id = 0;
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			int state_id = 0; int severity_id = 0; int genCategory_type = 0; int issCategory_type = 0;
			#endregion

			//случай, если выбран конкретный набор фильтров
			
			//получаем лист фильтров
			ListViewProfile lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
			FilterElementCollection fec = lvp.Filters;
			foreach (FilterElement fe in fec)
			{
				string source = fe.Source;
				string value = fe.Value.ToString();
				if (fe.ValueIsTemplate)
					value = TemplateResolver.ResolveAll(value);
				//устанавливаем значения переменных
				//и формируем строку для FilterView
				#region BindVariables
				switch (source)
				{
					case Incident.ProjectFilterKey:
						//для случая группировки - добавится ниже, если добавлять и здесь - будет дважды
						if (_groupFieldName != Incident.AvailableGroupField.Project.ToString())
						{
							projId = int.Parse(value);
							AddProjectFilterText(projId);
						}
						break;
					case Incident.ManagerFilterKey:
						iManId = int.Parse(value);
						AddManagerFilterText(iManId);
						break;
					case Incident.ResponsibleFilterKey:
						//для случая группировки - добавится ниже, если добавлять и здесь - будет дважды
						if (_groupFieldName != Incident.AvailableGroupField.Responsible.ToString())
						{
							iRespId = int.Parse(value);
							AddResponsibleFilterText(iRespId);
						}
						break;
					case Incident.CreatorFilterKey:
						iCreatorId = int.Parse(value);
						AddCreatorFilterText(iCreatorId);
						break;
					case Incident.PriorityFilterKey:
						priority_id = int.Parse(value);
						AddPriorityFilterText(priority_id);
						break;
					case Incident.IssueBoxFilterKey:
						//для случая группировки - добавится ниже, если добавлять и здесь - будет дважды
						if (_groupFieldName != Incident.AvailableGroupField.IssueBox.ToString())
						{
							issbox_id = int.Parse(value);
							AddIssueBoxFilterText(issbox_id);
						}
						break;
					case Incident.TypeFilterKey:
						type_id = int.Parse(value);
						AddTypeFilterText(type_id);
						break;
					case Incident.ClientFilterKey:
						//для случая группировки - добавится ниже, если добавлять и здесь - будет дважды
						if (_groupFieldName != Incident.AvailableGroupField.Client.ToString())
						{
							Incident.GetContactId(value, out orgUid, out contactUid);
							AddClientFilterText(orgUid, contactUid);
						}
						break;
					case Incident.StatusFilterKey:
						state_id = int.Parse(value);
						AddStatusFilterText(state_id);
						break;
					case Incident.SeverityFilterKey:
						severity_id = int.Parse(value);
						AddSeverityFilterText(severity_id);
						break;
					case Incident.GenCategoryTypeFilterKey:
						genCategory_type = int.Parse(value);
						//todo - gen category filter view
						break;
					case Incident.GenCategoriesFilterKey:
						ArrayList alGenCats = new ArrayList();
						string[] mas = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas)
						{
							if (!alGenCats.Contains(int.Parse(s)))
								alGenCats.Add(int.Parse(s));
						}
						Incident.SaveGeneralCategories(alGenCats);
						//todo - gen category filter view
						break;
					case Incident.IssueCategoryTypeFilterKey:
						issCategory_type = int.Parse(value);
						//todo - gen category filter view
						break;
					case Incident.IssueCategoriesFilterKey:
						ArrayList alIssCats = new ArrayList();
						string[] masIss = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in masIss)
						{
							if (!alIssCats.Contains(int.Parse(s)))
								alIssCats.Add(int.Parse(s));
						}
						Incident.SaveIncidentCategories(alIssCats);
						//todo - gen category filter view
						break;
					case Incident.UnansweredFilterKey:
						if (bool.Parse(value))
							isUnansweredOnly = true;
						AddUnansweredFilterText();
						break;
					case Incident.OverdueFilterKey:
						if (bool.Parse(value))
							isOverdueOnly = true;
						AddOverdueFilterText();
						break;
					default:
						break;
				}
				#endregion
			}

			//добавляем к фильтру выбраное значение из левой панели, если включена группировка
			#region GroupItem
			string groupCurrent = _groupFieldName;
			if (String.IsNullOrEmpty(groupCurrent))
				groupCurrent = Incident.AvailableGroupField.NotSet.ToString();
			switch ((Incident.AvailableGroupField)Enum.Parse(typeof(Incident.AvailableGroupField), groupCurrent))
			{
				case Incident.AvailableGroupField.IssueBox:
					if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
					{
						issbox_id = int.Parse(_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]);
						AddIssueBoxFilterText(issbox_id);
					}
					else
						issbox_id = -10;
					break;
				case Incident.AvailableGroupField.Client:
					if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
					{
						Incident.GetContactId(_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey], out orgUid, out contactUid);
						AddClientFilterText(orgUid, contactUid);
					}
					else
					{
						orgUid = PrimaryKeyId.Empty;
						contactUid = PrimaryKeyId.Empty;
					}
					break;
				case Incident.AvailableGroupField.Project:
					if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
					{
						projId = int.Parse(_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]);
						AddProjectFilterText(projId);
					}
					else
						projId = -10;
					break;
				case Incident.AvailableGroupField.Responsible:
					if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
					{
						iRespId = int.Parse(_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]);
						AddResponsibleFilterText(iRespId);
					}
					else
						iRespId = -10;
					break;
				default:
					break;
			}
			#endregion
			
			DataTable dt = Incident.GetListIncidentsByFilterDataTable
						(projId, iManId, iCreatorId, iResId, iRespId, orgUid, contactUid,
						issbox_id, priority_id, type_id, state_id, severity_id, txtSearch.Text,
						genCategory_type, issCategory_type, isUnansweredOnly, isOverdueOnly);

			DataView dv = dt.DefaultView;

			grdMain.DataSource = dv;

			if (dataBind)
				grdMain.DataBind();
		}
		#endregion

		#region grdMain_ChangingMCGridColumnHeader
		void grdMain_ChangingMCGridColumnHeader(object sender, ChangingMCGridColumnHeaderEventArgs e)
		{
			if (e.FieldName == "PriorityId")
			{
				e.ControlField.HeaderText = string.Format(CultureInfo.InvariantCulture, "<span title='{0}'>!!!</span>", GetGlobalResourceObject("IbnFramework.Project", "Priority").ToString());
			}
		}
		#endregion

		#region btnSearch_Click
		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			//фильтр по key-textbox
			_pc[Incident.KeywordFilterKey] = txtSearch.Text;
			CHelper.RequireBindGrid();
		}
		#endregion

		#region btnClear_Click
		protected void btnClear_Click(object sender, ImageClickEventArgs e)
		{
			//очистка key-textbox
			txtSearch.Text = String.Empty;
			_pc[Incident.KeywordFilterKey] = txtSearch.Text;
			CHelper.RequireBindGrid();
		}
		#endregion

		#region FilterSet_Changed
		protected void lbViewChange_Click(object sender, EventArgs e)
		{
			SetViewName(_pc, ddFilters.SelectedValue);
			this.Page.Response.Redirect(this.Page.Request.RawUrl, true);
		}
		#endregion

		#region ibDeleteInfo_Click
		protected void ibDeleteInfo_Click(object sender, ImageClickEventArgs e)
		{
			//удаление созданного пользователем набора фильтров
			string sId = ddFilters.SelectedValue;
			ListViewProfile.DeleteCustomProfile(_className, sId, String.Empty);

			ListViewProfile[] mas = ListViewProfile.GetProfiles(_className, String.Empty, Mediachase.IBN.Business.Security.CurrentUser.UserID);
			if (mas.Length == 0)
				throw new Exception("ListViewProfiles are not exist!");
			_pc[IssueListViewNameKey] = mas[0].Id;
			this.Page.Response.Redirect(this.Page.Request.RawUrl, true);
		}
		#endregion

		#region Grouping_ItemCommand
		protected void groupList_ItemCommand(object source, DataListCommandEventArgs e)
		{
			//выбор группы в левой панели
			if (e.CommandName == "Group")
			{
				_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] = e.CommandArgument.ToString();
				upLeftArea.Update();
				upFilters.Update();
				if (groupList.Items.Count > 0)
					groupList.SelectedIndex = e.Item.ItemIndex;
				CHelper.RequireBindGrid();
			}
		}
		#endregion

		#region GroupView_Changed
		protected void ddGrouping_SelectedIndexChanged(object sender, EventArgs e)
		{
			//изменение groupField в дропдауне
			_groupFieldName = ddGrouping.SelectedValue;
			SetGroupFieldForView(_groupFieldName);

			upLeftArea.Update();
			upFilters.Update();
			BindGroupDataList();
			CHelper.RequireBindGrid();
		}
		#endregion

		#region Client_Group_Search_Clear_Block
		protected void ibGroupClear_Click(object sender, ImageClickEventArgs e)
		{
			txtGroupSearch.Text = String.Empty;
			upLeftArea.Update();
			upFilters.Update();
			BindGroupDataList();
			CHelper.RequireBindGrid();
		}

		protected void ibGroupSearch_Click(object sender, ImageClickEventArgs e)
		{
			upLeftArea.Update();
			upFilters.Update();
			BindGroupDataList();
			CHelper.RequireBindGrid();
		}
		#endregion

		//Export 
		#region ExportXMLTable
		private void ExportXMLTable()
		{
			Mediachase.UI.Web.Util.CommonHelper.SaveXML(GetDataViewForExport().Table, "Issues.xml");
		}
		#endregion

		#region ExportGrid
		private void ExportGrid()
		{
			Mediachase.UI.Web.Util.CommonHelper.ExportExcel(PrepareExport(), "Issues.xls", null);
		}
		#endregion

		#region PrepareExport
		private DataGrid PrepareExport()
		{
			//IncidentId, ProjectId, ProjectTitle, IncidentBoxId, IncidentBoxName, ClientName,
			//CreatorId, CreatorName, ControllerId, ControllerName, ManagerId, ManagerName,
			//ResponsibleId, ResponsibleName, IsResponsibleGroup, ResponsibleGroupState,
			//ContactUid, OrgUid, Title, CreationDate, ModifiedDate, ActualOpenDate, ActualFinishDate, 
			//ExpectedResponseDate, ExpectedResolveDate, ExpectedAssignDate,
			//TypeId, TypeName, PriorityId, PriorityName, 
			//StateId, StateName, SeverityId, Identifier, IsOverdue, IsNewMessage, CurrentResponsibleId, 
			//CanEdit, CanDelete
			dgExport.Columns[0].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "IssId").ToString();
			dgExport.Columns[1].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "IssCode").ToString();
			dgExport.Columns[2].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "IssTitle").ToString();
			dgExport.Columns[3].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "Priority").ToString();
			dgExport.Columns[4].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "Status").ToString();
			dgExport.Columns[5].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "IssResponsible").ToString();
			dgExport.Columns[6].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "Created").ToString();
			dgExport.Columns[7].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "IssUpdated").ToString();
			dgExport.Columns[8].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "ActualOpenDate").ToString();
			dgExport.Columns[9].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "ActualFinishDate").ToString();
			dgExport.Columns[10].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "Creator").ToString();
			dgExport.Columns[11].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "Manager").ToString();
			dgExport.Columns[12].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "IssClient").ToString();
			dgExport.Columns[13].HeaderText = GetGlobalResourceObject("IbnFramework.Incident", "IssIssBox").ToString();

			DataView dv = GetDataViewForExport();

			dgExport.Visible = true;
			dgExport.DataSource = dv;
			dgExport.DataBind();

			return dgExport;
		} 
		#endregion

		#region GetDataViewForExport
		private DataView GetDataViewForExport()
		{

			string variant = "0";
			if (Request["variant"] != null)
				variant = Request["variant"];
			DataTable dt = new DataTable();
			DataView dv = dt.DefaultView;

			string sortExpression = String.Empty;
			if (_pc[grdMain.GetPropertyKey(MCGrid.SortingPropertyKey)] != null)
				sortExpression = _pc[grdMain.GetPropertyKey(MCGrid.SortingPropertyKey)].ToString();

			int pageSize = 10;
			if (_pc[grdMain.GetPropertyKey(MCGrid.PageSizePropertyKey)] != null)
			{
				pageSize = Convert.ToInt32(_pc[grdMain.GetPropertyKey(MCGrid.PageSizePropertyKey)].ToString());
				if (pageSize == -1)
					pageSize = 10000;
			}
			int pageIndex = 0;
			if (_pc["MCGrid_IssueList_" + _viewName + "_PageIndex"] != null)
				pageIndex = int.Parse(_pc["MCGrid_IssueList_" + _viewName + "_PageIndex"]);

			string groupCurrent = _groupFieldName;
			if (String.IsNullOrEmpty(groupCurrent))
				groupCurrent = Incident.AvailableGroupField.NotSet.ToString();
			ListViewProfile lvp = null;
			FilterElementCollection fecAbove = null;
			switch (variant)
			{
				case "1":	//OnlyView
					lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
					fecAbove = new FilterElementCollection();
					#region GroupItem
					switch ((Incident.AvailableGroupField)Enum.Parse(typeof(Incident.AvailableGroupField), groupCurrent))
					{
						case Incident.AvailableGroupField.IssueBox:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
								fecAbove.Add(FilterElement.EqualElement(Incident.IssueBoxFilterKey, _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]));
							break;
						case Incident.AvailableGroupField.Client:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
								fecAbove.Add(FilterElement.EqualElement(Incident.ClientFilterKey, _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]));
							break;
						case Incident.AvailableGroupField.Project:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
								fecAbove.Add(FilterElement.EqualElement(Incident.ProjectFilterKey, _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]));
							break;
						case Incident.AvailableGroupField.Responsible:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
								fecAbove.Add(FilterElement.EqualElement(Incident.ResponsibleFilterKey, _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]));
							break;
						default:
							break;
					}
					#endregion

					dt = Incident.GetListIncidentsByFilterDataTable(txtSearch.Text, lvp.Filters, fecAbove);

					dv = dt.DefaultView;
					dv.Sort = sortExpression;
					break;
				case "2":	//Only Visible
					lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
					fecAbove = new FilterElementCollection();
					#region GroupItem
					switch ((Incident.AvailableGroupField)Enum.Parse(typeof(Incident.AvailableGroupField), groupCurrent))
					{
						case Incident.AvailableGroupField.IssueBox:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
								fecAbove.Add(FilterElement.EqualElement(Incident.IssueBoxFilterKey, _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]));
							break;
						case Incident.AvailableGroupField.Client:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
								fecAbove.Add(FilterElement.EqualElement(Incident.ClientFilterKey, _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]));
							break;
						case Incident.AvailableGroupField.Project:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
								fecAbove.Add(FilterElement.EqualElement(Incident.ProjectFilterKey, _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]));
							break;
						case Incident.AvailableGroupField.Responsible:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey] != null)
								fecAbove.Add(FilterElement.EqualElement(Incident.ResponsibleFilterKey, _pc[_viewName + "_" + _groupFieldName + "_" + IssueListGroupItemKey]));
							break;
						default:
							break;
					}
					#endregion

					dt = Incident.GetListIncidentsByFilterDataTable(txtSearch.Text, lvp.Filters, fecAbove);

					dv = dt.DefaultView;
					dv.Sort = sortExpression;

					DataTable dtClone = dt.Clone();
					int index = 0;
					int start = pageIndex * pageSize;
					int finish = start + pageSize;
					DataRow dr;
					foreach (DataRowView drv in dv)
					{
						if (index >= start && index < finish)
						{
							dr = dtClone.NewRow();
							dr.ItemArray = drv.Row.ItemArray;
							dtClone.Rows.Add(dr);
						}
						index++;
					}
					dt = dtClone.Copy();
					dv = dt.DefaultView;
					break;
				default:	//All
					dt = Incident.GetListIncidentsByFilterDataTable
						(0, 0, 0, 0, 0, PrimaryKeyId.Empty, PrimaryKeyId.Empty, 0, -1, 0, 0,
						0, String.Empty, 0, 0);

					dv = dt.DefaultView;
					dv.Sort = sortExpression;
					break;
			}
			return dv;
		} 
		#endregion

		//добавление строки в FilterView
		#region AddFilterString
		private void AddFilterString(string source, string value)
		{
			if (!String.IsNullOrEmpty(value))
				_filterText += String.Format("{2}{0}:&nbsp;{1}", source, value,
					(_filterText.Length == 0) ? "" : ",&nbsp;");
		}

		#region AddProjectFilterText
		private void AddProjectFilterText(int projId)
		{
			string textValue = String.Empty;
			if (projId > 0)
				textValue = Task.GetProjectTitle(projId);
			else if (projId == -1)
				textValue = GetGlobalResourceObject("IbnFramework.Incident", "GroupNoPrj").ToString();

			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "Project").ToString(), textValue);
		}
		#endregion

		#region AddManagerFilterText
		private void AddManagerFilterText(int iManId)
		{
			string textValue = String.Empty;
			if (iManId > 0)
				textValue = CommonHelper.GetUserStatusPureName(iManId);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "Manager").ToString(), textValue);
		}
		#endregion

		#region AddResponsibleFilterText
		private void AddResponsibleFilterText(int iRespId)
		{
			string textValue = String.Empty;
			if (iRespId > 0)
				textValue = CommonHelper.GetUserStatusPureName(iRespId);
			else if (iRespId == -1)
				textValue = GetGlobalResourceObject("IbnFramework.Incident", "ResponsibleNotAssigned").ToString();

			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "IssResponsible").ToString(), textValue);
		}
		#endregion

		#region AddCreatorFilterText
		private void AddCreatorFilterText(int iCreatorId)
		{
			string textValue = String.Empty;
			if (iCreatorId > 0)
				textValue = CommonHelper.GetUserStatusPureName(iCreatorId);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "Creator").ToString(), textValue);
		}
		#endregion

		#region AddPriorityFilterText
		private void AddPriorityFilterText(int priority_id)
		{
			string textValue = String.Empty;
			if (priority_id > -1)
				textValue = Mediachase.IBN.Business.Common.GetPriority(priority_id);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "Priority").ToString(), textValue);
		}
		#endregion

		#region AddIssueBoxFilterText
		private void AddIssueBoxFilterText(int issbox_id)
		{
			string textValue = String.Empty;
			if (issbox_id > 0)
				textValue = Mediachase.IBN.Business.EMail.IncidentBox.Load(issbox_id).Name;

			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "tIncBox").ToString(), textValue);
		}
		#endregion

		#region AddTypeFilterText
		private void AddTypeFilterText(int type_id)
		{
			string textValue = String.Empty;
			if (type_id > 0)
				textValue = Mediachase.IBN.Business.Common.GetIncidentType(type_id);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "Type").ToString(), textValue);
		}
		#endregion

		#region AddClientFilterText
		private void AddClientFilterText(PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			string textValue = String.Empty;
			if (contactUid != PrimaryKeyId.Empty)
				textValue = CommonHelper.GetEntityTitle((int)ObjectTypes.Contact, contactUid);
			else if (orgUid != PrimaryKeyId.Empty)
				textValue = CommonHelper.GetEntityTitle((int)ObjectTypes.Organization, orgUid);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "IssClient").ToString(), textValue);
		}
		#endregion

		#region AddStatusFilterText
		private void AddStatusFilterText(int state_id)
		{
			string textValue = String.Empty;
			if (state_id > 0)
				textValue = Incident.GetIncidentStateName(state_id);
			else if (state_id == -2)	//inactive
				textValue = GetGlobalResourceObject("IbnFramework.Incident", "Inactive").ToString();
			else if (state_id == -1)	//active
				textValue = GetGlobalResourceObject("IbnFramework.Incident", "Active").ToString();

			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "IssStatus").ToString(), textValue);
		}
		#endregion

		#region AddSeverityFilterText
		private void AddSeverityFilterText(int severity_id)
		{
			string textValue = String.Empty;
			if (severity_id > 0)
				textValue = Mediachase.IBN.Business.Common.GetIncidentSeverity(severity_id);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "Severity").ToString(), textValue);
		}
		#endregion

		#region AddUnansweredFilterText
		private void AddUnansweredFilterText()
		{
			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "OnlyUnanswered").ToString(),
								GetGlobalResourceObject("IbnFramework.Incident", "Yes").ToString());
		}
		#endregion

		#region AddOverdueFilterText
		private void AddOverdueFilterText()
		{
			AddFilterString(GetGlobalResourceObject("IbnFramework.Incident", "OnlyOverdue").ToString(),
								GetGlobalResourceObject("IbnFramework.Incident", "Yes").ToString());
		}
		#endregion
		#endregion

		#region GetGroupFieldForView
		public string GetGroupFieldForView(ListViewProfile lvp)
		{
			if (_pc[_pc[IssueListViewNameKey] + "_" + IssueListGroupFieldKey] == null)
			{
				_pc[_pc[IssueListViewNameKey] + "_" + IssueListGroupFieldKey] = lvp.GroupByFieldName;
			}

			if (!Page.IsPostBack)
				_pc[_pc[IssueListViewNameKey] + "_" + IssueListGroupFieldKey] = lvp.GroupByFieldName;

			return _pc[_pc[IssueListViewNameKey] + "_" + IssueListGroupFieldKey];
		}
		#endregion

		#region SetGroupFieldForView
		public void SetGroupFieldForView(string name)
		{
			_pc[_pc[IssueListViewNameKey] + "_" + IssueListGroupFieldKey] = name;
		}
		#endregion

		//static methods
		#region GetViewName
		public static string GetViewName(UserLightPropertyCollection pc)
		{
			if (pc[IssueListViewNameKey] == null)
				pc[IssueListViewNameKey] = GetFirstAvailableListViewProfile();

			return pc[IssueListViewNameKey];
		}
		#endregion

		#region SetViewName
		public static void SetViewName(UserLightPropertyCollection pc, string name)
		{
			pc[IssueListViewNameKey] = name;
		}
		#endregion


		#region GetFirstAvailableListViewProfile
		public static string GetFirstAvailableListViewProfile()
		{
			ListViewProfile[] mas = ListViewProfile.GetSystemProfiles(_className, String.Empty);
			if (mas.Length == 0)
				throw new Exception("ListViewProfiles are not exist!");
			return mas[0].Id;
		} 
		#endregion

		#region GetResponsible
		protected string GetResponsible(object currentResponsibleObj)
		{
			string retval = "";
			if (currentResponsibleObj != DBNull.Value)
			{
				int currentResponsibleId = (int)currentResponsibleObj;
				if (currentResponsibleId > 0)
				{
					retval = CommonHelper.GetUserStatusPureName(currentResponsibleId);
				}
				else if (currentResponsibleId == -2)	// no responsible
				{
					retval = GetGlobalResourceObject("IbnFramework.Incident", "tRespNotSet").ToString();
				}
				else if (currentResponsibleId == -3)	// group
				{
					retval = GetGlobalResourceObject("IbnFramework.Incident", "tRespGroup").ToString();
				}
				else if (currentResponsibleId == -4)	// all denied
				{
					retval = GetGlobalResourceObject("IbnFramework.Incident", "tRespGroup").ToString();
				}
			}
			return retval;
		} 
		#endregion
	}
}