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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class ProjectList : System.Web.UI.UserControl
	{
		private const string _className = "Project";
		private string _viewName = "";
		private const string _placeName = "ProjectList";
		private string _fieldSetName = Project.ProjectFieldSet.ProjectsDefault.ToString();

		public const string ProjectListGroupItemKey = "ProjectList_GroupObjectId";
		public const string ProjectListViewNameKey = "ProjectList_ViewName";
		public const string ProjectListFieldSetNameKey = "ProjectList_FieldSetName";
		public const string ProjectListGroupFieldKey = "ProjectList_GroupField";

		protected string AddNewViewScript = String.Empty;

		private bool _isFilterSet = false;
		private string _filterText = String.Empty;

		private bool _isGroupSet = false;
		private string _groupFieldName = Project.AvailableGroupField.NotSet.ToString();

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
			else
			{
				if (Request["Export"] == "2")
					ExportXMLTable();
				else
				{
					//биндим датагрид
					BindDataGrid(!Page.IsPostBack);
				}
			}

			//set header text
			if (!Page.IsPostBack)
				BindBlockHeader();

			#region CommandManager PreLoad
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			cm.AddCommand(_className, _viewName, _placeName, "MC_PM_NewViewCreated");
			cm.AddCommand(_className, _viewName, _placeName, "MC_PM_FiltersEdit");
			cm.AddCommand(_className, _viewName, _placeName, "MC_PM_ExportTrue");
			#endregion
		}

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			_pc["MCGrid_ProjectList_" + _viewName + "_PageIndex"] = grdMain.PageIndex.ToString();

			//кнопка очистить фильтр у key-textbox
			btnClear.Visible = !String.IsNullOrEmpty(txtSearch.Text);

			//поле поиска при группировке по клиенту
			divLeftSearchContainer.Visible = (_groupFieldName == Project.AvailableGroupField.Client.ToString());
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

			//в случае, если отображаемый набор фильтров создан текущим пользователем - он может его удалить
			//генерируем imagebutton справа от набора фильтров
			ibDeleteInfo.ToolTip = GetGlobalResourceObject("IbnFramework.Project", "_mc_DeleteView").ToString();
			ibDeleteInfo.Attributes.Add("onclick", String.Format("return confirm('{0}');", GetGlobalResourceObject("IbnFramework.Project", "_mc_DeleteViewWarning").ToString()));
			ibDeleteInfo.Visible = Mediachase.Ibn.Core.ListViewProfile.CanDelete(_className, ddFilters.SelectedValue, String.Empty, Mediachase.IBN.Business.Security.CurrentUser.UserID);

			ibEditInfo.ToolTip = GetGlobalResourceObject("IbnFramework.Common", "Edit").ToString();
			CommandManager cm = CommandManager.GetCurrent(this.Page);
			CommandParameters cp = new CommandParameters("MC_PM_FiltersEdit");
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("uid", ddFilters.SelectedValue);
			string scriptEdit = cm.AddCommand(_className, _viewName, _placeName, cp);
			ibEditInfo.Attributes.Add("onclick", String.Format("javascript:{{{0};return false;}}", scriptEdit));
			ibEditInfo.Visible = Mediachase.Ibn.Core.ListViewProfile.CanDelete(_className, ddFilters.SelectedValue, String.Empty, Mediachase.IBN.Business.Security.CurrentUser.UserID);


			cp = new CommandParameters("MC_PM_FiltersNew");
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
			//ListViewProfiles
			BindListViewProfiles();

			//заполняем дропдаун быстрого переключения по группировкам в левой панели
			if (_isGroupSet)
			{
				ddGrouping.Items.Clear();

				ddGrouping.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Project", "GroupByPortfolio").ToString(), Project.AvailableGroupField.Portfolio.ToString()));
				if (PortalConfig.GeneralAllowClientField || _groupFieldName == Project.AvailableGroupField.Client.ToString())
					ddGrouping.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Project", "GroupByContacts").ToString(), Project.AvailableGroupField.Client.ToString()));
			}
		}
		#endregion

		#region BindListViewProfiles
		private void BindListViewProfiles()
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
			summaryList.Insert(0, new KeyValuePair<string, string>("-1", String.Format("[ {0} ]", GetGlobalResourceObject("IbnFramework.Project", "SystemViews").ToString())));

			ListViewProfile[] mas2 = ListViewProfile.GetProfiles(_className, String.Empty, Mediachase.IBN.Business.Security.CurrentUser.UserID);

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
				summaryList.Add(new KeyValuePair<string, string>("-2", String.Format("[ {0} ]", GetGlobalResourceObject("IbnFramework.Project", "UserViews").ToString())));
				summaryList.AddRange(summaryList2);
			}
			summaryList.Add(new KeyValuePair<string, string>("0", String.Format("[ {0} ]", GetGlobalResourceObject("IbnFramework.Project", "NewProjectView").ToString())));

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
		
		#region BindSavedValues
		private void BindSavedValues()
		{
			//выделяем последний запомненный набор фильтров
			CHelper.SafeSelect(ddFilters, _viewName);

			//последнее запомненное значение key-textbox
			if (_pc[Project.KeywordFilterKey] != null)
				txtSearch.Text = _pc[Project.KeywordFilterKey];

			//для вида с группировкой устанавливаем значение равное текущему _groupFieldName
			if (_isGroupSet)
				CHelper.SafeSelect(ddGrouping, _groupFieldName);
		}
		#endregion

		#region BindGroupDataList
		private void BindGroupDataList()
		{
			//заполняем левый даталист для группировок
			#region GroupByPortfolio
			if (_groupFieldName == Project.AvailableGroupField.Portfolio.ToString())
			{
				lblGroupName.Text = GetGlobalResourceObject("IbnFramework.Project", "GroupPortfolio").ToString();
				//собираем таблицу с группами проектов
				DataTable dt = new DataTable();
				dt.Columns.Add(new DataColumn("ObjectId", typeof(string)));
				dt.Columns.Add(new DataColumn("Title", typeof(string)));
				dt.Columns.Add(new DataColumn("Weight", typeof(int)));
				DataRow dr;

				//filterSet - if filterset has a PortfolioId - group has only one portfolio
				//если предустановлен набор фильтров, в котором задан фильтр по portfolio 
				//- в список попадет только одна запись - соответствующая этому portfolio
				int onlyPort = 0;
				ListViewProfile lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
				//TODO:
				//FilterElementCollection fec = lvp.Filters;
				//foreach (FilterElement fe in fec)
				//    if (fe.Source == Project.PortfolioTypeFilterKey)
				//    {
				//        string value = fe.Value.ToString();
				//        if (fe.ValueIsTemplate)
				//            value = TemplateResolver.ResolveAll(value);
				//        onlyPrj = int.Parse(value);
				//    }

				using (IDataReader reader = ProjectGroup.GetProjectGroups())
				{
					while (reader.Read())
					{
						if (onlyPort != 0 && (int)reader["ProjectGroupId"] != onlyPort)
							continue;
						dr = dt.NewRow();
						dr["ObjectId"] = reader["ProjectGroupId"].ToString();
						dr["Title"] = String.Format("{0}"/*"{0} ({1})"*/, reader["Title"].ToString());
						dr["Weight"] = 1;
						dt.Rows.Add(dr);
					}
				}

				//добавляем запись [Любая] и узнаем сколько инцидентов есть для такого случая
				if (onlyPort == 0)
				{
					dr = dt.NewRow();
					dr["ObjectId"] = 0;
					dr["Title"] = String.Format("[ {0} ]", GetGlobalResourceObject("IbnFramework.Project", "Any").ToString());
					dr["Weight"] = 0;
					dt.Rows.Add(dr);
				}

				//Weight нужен чтобы запись [Любая] была всегда первой
				DataView dv = dt.DefaultView;
				dv.Sort = "Weight, Title";

				groupList.DataSource = dv;
				groupList.DataKeyField = "ObjectId";
				groupList.DataBind();

				//пытаемся найти и выделить последнюю запомненную группу
				int selectedIndex = 0;
				if (_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] != null)
				{
					foreach (DataListItem dli in groupList.Items)
					{
						LinkButton lb = (LinkButton)dli.FindControl("lbGroupItem");
						if (lb != null && lb.CommandArgument == _pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey])
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
					_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] = groupList.SelectedValue.ToString();
				else
					_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] = "0";
			}
			#endregion

			#region GroupByClient
			if (_groupFieldName == Project.AvailableGroupField.Client.ToString())
			{
				lblGroupName.Text = GetGlobalResourceObject("IbnFramework.Project", "GroupContacts").ToString();

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
					if (fe.Source == Project.ClientFilterKey)
					{
						string value = fe.Value.ToString();
						if (fe.ValueIsTemplate)
							value = TemplateResolver.ResolveAll(value);
						onlyClient = value;
					}

				DataTable dtClients = Project.GetListContactsForProjectsDataTable(txtGroupSearch.Text.Trim(), 5);
				foreach (DataRow drClient in dtClients.Rows)
				{
					dr = dt.NewRow();
					string id = String.Format("{0}_{1}",
						((int)drClient["ObjectTypeId"] == (int)ObjectTypes.Organization) ? "org" : "contact",
						drClient["ObjectId"].ToString());

					if (onlyClient != "_" && id != onlyClient)
						continue;
					dr["ObjectId"] = id;
					dr["Title"] = String.Format("{0}", drClient["ClientName"].ToString());
					dr["Weight"] = 1;
					dt.Rows.Add(dr);
				}
				
				//добавляем запись [все] и узнаем сколько проектов есть для такого случая
				dr = dt.NewRow();
				dr["ObjectId"] = "_";
				dr["Title"] = String.Format("[ {0} ]",
					GetGlobalResourceObject("IbnFramework.Project", "AnyMale").ToString());
				dr["Weight"] = 0;
				dt.Rows.Add(dr);

				DataView dv = dt.DefaultView;
				dv.Sort = "Weight, Title";

				groupList.DataSource = dv;
				groupList.DataKeyField = "ObjectId";
				groupList.DataBind();

				//пытаемся найти и выделить последнюю запомненную группу
				int selectedIndex = 0;
				if (_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] != null)
				{
					foreach (DataListItem dli in groupList.Items)
					{
						LinkButton lb = (LinkButton)dli.FindControl("lbGroupItem");
						if (lb != null && lb.CommandArgument == _pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey])
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
					_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] = groupList.SelectedValue.ToString();
				else
					_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] = "_";
			}
			#endregion
		}
		#endregion

		#region BindBlockHeader
		/// <summary>
		/// Binds the tool bar.
		/// </summary>
		private void BindBlockHeader()
		{
			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.Project", "ProjectList").ToString();
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid(bool dataBind)
		{
			//проверяет установлены ли какие-то значения фильтров, кроме предопределенных - только для случая [Настроить]
			_isFilterSet = false;
			//формируется строка для FilterView
			_filterText = String.Empty;
			//выбрано ли в наборе фильтров только с участием
			bool isParticipationOnly = false;

			#region Variables
			int state = 0; int status_id = 0; int phase_id = 0; int type_id = 0; int priority_id = -1;
			int sdc = 0; DateTime startdate = DateTime.Now; int fdc = 0; DateTime finishdate = DateTime.Now;
			int manager_id = 0; int exemanager_id = 0; int prjGroup_type = 0; int genCategory_type = 0;
			int prjCategory_type = 0;
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
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
					case Project.StatusFilterKey:
						int val = int.Parse(value);
						if (val > 0)
						{
							status_id = int.Parse(value);
							AddStatusFilterText(status_id);
						}
						else if (val < 0)
						{
							state = Math.Abs(val);
							AddStatusFilterText(val);
						}
						break;
					case Project.PhaseFilterKey:
						phase_id = int.Parse(value);
						AddPhaseFilterText(phase_id);
						break;
					case Project.TypeFilterKey:
						type_id = int.Parse(value);
						AddTypeFilterText(type_id);
						break;
					case Project.PriorityFilterKey:
						priority_id = int.Parse(value);
						AddPriorityFilterText(priority_id);
						break;
					case Project.StartDateFilterKey:
						switch (fe.Type)
						{
							case FilterElementType.GreaterOrEqual:
								sdc = 1;
								startdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.LessOrEqual:
								sdc = 2;
								startdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.Equal:
								sdc = 3;
								startdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							default:
								break;
						}
						AddStartDateFilterText(sdc, startdate);
						break;
					case Project.EndDateFilterKey:
						switch (fe.Type)
						{
							case FilterElementType.GreaterOrEqual:
								fdc = 1;
								finishdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.LessOrEqual:
								fdc = 2;
								finishdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							case FilterElementType.Equal:
								fdc = 3;
								finishdate = DateTime.Parse(value, CultureInfo.InvariantCulture);
								break;
							default:
								break;
						}
						AddEndDateFilterText(fdc, finishdate);
						break;
					case Project.ManagerFilterKey:
						manager_id = int.Parse(value);
						AddManagerFilterText(manager_id);
						break;
					case Project.ExecManagerFilterKey:
						exemanager_id = int.Parse(value);
						AddExecManagerFilterText(exemanager_id);
						break;
					case Project.ClientFilterKey:
						//для случая группировки - добавится ниже, если добавлять и здесь - будет дважды
						if (_groupFieldName != Project.AvailableGroupField.Client.ToString())
						{
							Incident.GetContactId(value, out orgUid, out contactUid);
							AddClientFilterText(orgUid, contactUid);
						}
						break;
					case Project.GenCategoryTypeFilterKey:
						genCategory_type = int.Parse(value);
						//todo - gen category filter view
						break;
					case Project.GenCategoriesFilterKey:
						ArrayList alGenCats = new ArrayList();
						string[] mas = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in mas)
						{
							if (!alGenCats.Contains(int.Parse(s)))
								alGenCats.Add(int.Parse(s));
						}
						Project.SaveGeneralCategories(alGenCats);
						//todo - gen category filter view
						break;
					case Project.ProjectCategoryTypeFilterKey:
						prjCategory_type = int.Parse(value);
						//todo - gen category filter view
						break;
					case Project.ProjectCategoriesFilterKey:
						ArrayList alPrjCats = new ArrayList();
						string[] masPrj = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
						foreach (string s in masPrj)
						{
							if (!alPrjCats.Contains(int.Parse(s)))
								alPrjCats.Add(int.Parse(s));
						}
						Project.SaveProjectCategories(alPrjCats);
						//todo - gen category filter view
						break;
					case Project.PortfolioTypeFilterKey:
						if (_groupFieldName != Project.AvailableGroupField.Portfolio.ToString())
						{
							prjGroup_type = int.Parse(value);
							//todo - gen category filter view
						}
						break;
					case Project.PortfoliosFilterKey:
						if (_groupFieldName != Project.AvailableGroupField.Portfolio.ToString())
						{
							ArrayList alPrjGroups = new ArrayList();
							string[] masPrjGrp = value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
							foreach (string s in masPrjGrp)
							{
								if (!alPrjGroups.Contains(int.Parse(s)))
									alPrjGroups.Add(int.Parse(s));
							}
							Project.SaveProjectGroups(alPrjGroups);
							//todo - gen category filter view
						}
						break;
					case Project.MyParticipationOnlyFilterKey:
						if (bool.Parse(value))
							isParticipationOnly = true;
						AddMyParticipationOnlyFilterText();
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
				groupCurrent = Project.AvailableGroupField.NotSet.ToString();
			switch ((Project.AvailableGroupField)Enum.Parse(typeof(Project.AvailableGroupField), groupCurrent))
			{
				case Project.AvailableGroupField.Portfolio:
					if (_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] != null)
					{
						int port_id = int.Parse(_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey]);
						if (port_id > 0)
						{
							prjGroup_type = 1;
							ArrayList alPrjGroups = new ArrayList();
							alPrjGroups.Add(port_id);
							Project.SaveProjectGroups(alPrjGroups);
							AddPortfolioFilterText(port_id);
						}
					}
					break;
				case Project.AvailableGroupField.Client:
					if (_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] != null)
					{
						Incident.GetContactId(_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey], out orgUid, out contactUid);
						AddClientFilterText(orgUid, contactUid);
					}
					else
					{
						orgUid = PrimaryKeyId.Empty;
						contactUid = PrimaryKeyId.Empty;
					}
					break;
				default:
					break;
			}
			#endregion

			DataTable dt = Project.GetListProjectsByFilterDataTable(
				txtSearch.Text,
				state,
				status_id,
				type_id,
				priority_id,
				contactUid, orgUid,
				sdc, startdate,
				fdc, finishdate,
				manager_id,
				exemanager_id,
				genCategory_type,
				prjCategory_type,
				prjGroup_type,
				phase_id,
				isParticipationOnly);

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
				//e.ControlField.HeaderText = String.Format("<img width='16' height='16' src='{0}' title='{1}'>",
				//    this.Page.ResolveClientUrl("~/layouts/images/PriorityHeader.gif"),
				//    GetGlobalResourceObject("IbnFramework.Project", "Priority").ToString());
			}
		}
		#endregion

		#region btnSearch_Click
		protected void btnSearch_Click(object sender, ImageClickEventArgs e)
		{
			//фильтр по key-textbox
			_pc[Project.KeywordFilterKey] = txtSearch.Text;
			CHelper.RequireBindGrid();
		}
		#endregion

		#region btnClear_Click
		protected void btnClear_Click(object sender, ImageClickEventArgs e)
		{
			//очистка key-textbox
			txtSearch.Text = String.Empty;
			_pc[Project.KeywordFilterKey] = txtSearch.Text;
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
			_pc[ProjectListViewNameKey] = mas[0].Id;
			this.Page.Response.Redirect(this.Page.Request.RawUrl, true);
		}
		#endregion

		#region Grouping_ItemCommand
		protected void groupList_ItemCommand(object source, DataListCommandEventArgs e)
		{
			//выбор группы в левой панели
			if (e.CommandName == "Group")
			{
				_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] = e.CommandArgument.ToString();
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
			Mediachase.UI.Web.Util.CommonHelper.SaveXML(GetDataViewForExport().Table, "Projects.xml");
		}
		#endregion

		#region ExportGrid
		private void ExportGrid()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", Assembly.GetExecutingAssembly());
			int i = 0;
			dgExport.Columns[i++].HeaderText = "#";
			dgExport.Columns[i++].HeaderText = LocRM.GetString("Title");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("PercentCompleted");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("Status");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("Priority");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("StartDate");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("FinishDate");
			dgExport.Columns[i++].HeaderText = LocRM.GetString("Manager");

			DataView dv = GetDataViewForExport();

			dgExport.Visible = true;
			dgExport.DataSource = dv;
			dgExport.DataBind();
			Mediachase.UI.Web.Util.CommonHelper.ExportExcel(dgExport, "Projects.xls", null);
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
			if (_pc["MCGrid_ProjectList_" + _viewName + "_PageIndex"] != null)
				pageIndex = int.Parse(_pc["MCGrid_ProjectList_" + _viewName + "_PageIndex"]);

			string groupCurrent = _groupFieldName;
			if (String.IsNullOrEmpty(groupCurrent))
				groupCurrent = Project.AvailableGroupField.NotSet.ToString();
			ListViewProfile lvp = null;
			FilterElementCollection fecAbove = null;
			switch (variant)
			{
				case "1":	//OnlyView
					lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
					fecAbove = new FilterElementCollection();
					#region GroupItem
					switch ((Project.AvailableGroupField)Enum.Parse(typeof(Project.AvailableGroupField), groupCurrent))
					{
						case Project.AvailableGroupField.Portfolio:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] != null)
							{
								string port_id = _pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey];
								if (port_id != "0")
								{
									fecAbove.Add(FilterElement.EqualElement(Project.PortfolioTypeFilterKey, "1"));
									ArrayList alPrjGrps = new ArrayList();
									alPrjGrps.Add(port_id);
									fecAbove.Add(FilterElement.EqualElement(Project.PortfoliosFilterKey, String.Join(";", (string[])alPrjGrps.ToArray(typeof(string)))));
								}
							}
							break;
						case Project.AvailableGroupField.Client:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] != null)
								fecAbove.Add(FilterElement.EqualElement(Project.ClientFilterKey, _pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey]));
							break;
						default:
							break;
					}
					#endregion

					dt = Project.GetListProjectsByFilterDataTable(txtSearch.Text, lvp.Filters, fecAbove);

					dv = dt.DefaultView;
					dv.Sort = sortExpression;
					break;
				case "2":	//Only Visible
					lvp = ListViewProfile.Load(_className, _viewName, String.Empty);
					fecAbove = new FilterElementCollection();
					#region GroupItem
					switch ((Project.AvailableGroupField)Enum.Parse(typeof(Project.AvailableGroupField), groupCurrent))
					{
						case Project.AvailableGroupField.Portfolio:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] != null)
							{
								string port_id = _pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey];
								if (port_id != "0")
								{
									fecAbove.Add(FilterElement.EqualElement(Project.PortfolioTypeFilterKey, "1"));
									ArrayList alPrjGrps = new ArrayList();
									alPrjGrps.Add(port_id);
									fecAbove.Add(FilterElement.EqualElement(Project.PortfoliosFilterKey, String.Join(";", (string[])alPrjGrps.ToArray(typeof(string)))));
								}
							}
							break;
						case Project.AvailableGroupField.Client:
							if (_pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey] != null)
								fecAbove.Add(FilterElement.EqualElement(Project.ClientFilterKey, _pc[_viewName + "_" + _groupFieldName + "_" + ProjectListGroupItemKey]));
							break;
						default:
							break;
					}
					#endregion

					dt = Project.GetListProjectsByFilterDataTable(txtSearch.Text, lvp.Filters, fecAbove);

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
					dt = Project.GetListProjectsByFilterDataTable
						(String.Empty, 0, 0, 0, -1, PrimaryKeyId.Empty, PrimaryKeyId.Empty, 0, DateTime.Now, 0, DateTime.Now,
						0, 0, 0, 0, 0, 0, false);

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

		#region AddStatusFilterText
		private void AddStatusFilterText(int state_id)
		{
			string textValue = String.Empty;
			if (state_id > 0)
				textValue = Project.GetProjectStatusName(state_id);
			else if (state_id == -1)
				textValue = GetGlobalResourceObject("IbnFramework.Incident", "Active").ToString();
			else if (state_id == -2)
				textValue = GetGlobalResourceObject("IbnFramework.Incident", "Inactive").ToString();
			
			AddFilterString(GetGlobalResourceObject("IbnFramework.Project", "Status").ToString(), textValue);
		}
		#endregion

		#region AddPhaseFilterText
		private void AddPhaseFilterText(int phase_id)
		{
			string textValue = String.Empty;
			if (phase_id > 0)
				textValue = Project.GetProjectPhaseName(phase_id);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Project", "Phase").ToString(), textValue);
		}
		#endregion

		#region AddTypeFilterText
		private void AddTypeFilterText(int type_id)
		{
			string textValue = String.Empty;
			if (type_id > 0)
				textValue = Project.GetProjectTypeName(type_id);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Project", "Type").ToString(), textValue);
		}
		#endregion

		#region AddPriorityFilterText
		private void AddPriorityFilterText(int priority_id)
		{
			string textValue = String.Empty;
			if (priority_id > -1)
				textValue = Mediachase.IBN.Business.Common.GetPriority(priority_id);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Project", "Priority").ToString(), textValue);
		}
		#endregion

		#region AddStartDateFilterText
		private void AddStartDateFilterText(int sdc, DateTime startdate)
		{
			string textValue = String.Empty;
			if (sdc == 1)
			{
				_filterText += String.Format("{2}{0} {3}&nbsp;{1}",
					GetGlobalResourceObject("IbnFramework.Project", "StartDate").ToString(),
					startdate.ToShortDateString(),
					(_filterText.Length == 0) ? "" : ",&nbsp;",
					GetGlobalResourceObject("IbnFramework.Project", "GEType").ToString());
			}
			else if (sdc == 2)
			{
				_filterText += String.Format("{2}{0} {3}&nbsp;{1}",
					GetGlobalResourceObject("IbnFramework.Project", "StartDate").ToString(),
					startdate.ToShortDateString(),
					(_filterText.Length == 0) ? "" : ",&nbsp;",
					GetGlobalResourceObject("IbnFramework.Project", "LEType").ToString());
			}
			else if (sdc == 3)
			{
				_filterText += String.Format("{2}{0} {3}&nbsp;{1}",
					GetGlobalResourceObject("IbnFramework.Project", "StartDate").ToString(),
					startdate.ToShortDateString(),
					(_filterText.Length == 0) ? "" : ",&nbsp;",
					GetGlobalResourceObject("IbnFramework.Project", "EType").ToString());
			}
		}
		#endregion

		#region AddEndDateFilterText
		private void AddEndDateFilterText(int fdc, DateTime finishdate)
		{
			string textValue = String.Empty;
			if (fdc == 1)
			{
				_filterText += String.Format("{2}{0} {3}&nbsp;{1}",
					GetGlobalResourceObject("IbnFramework.Project", "EndDate").ToString(),
					finishdate.ToShortDateString(),
					(_filterText.Length == 0) ? "" : ",&nbsp;",
					GetGlobalResourceObject("IbnFramework.Project", "GEType").ToString());
			}
			else if (fdc == 2)
			{
				_filterText += String.Format("{2}{0} {3}&nbsp;{1}",
					GetGlobalResourceObject("IbnFramework.Project", "EndDate").ToString(),
					finishdate.ToShortDateString(),
					(_filterText.Length == 0) ? "" : ",&nbsp;", 
					GetGlobalResourceObject("IbnFramework.Project", "LEType").ToString());
			}
			else if (fdc == 3)
			{
				_filterText += String.Format("{2}{0} {3}&nbsp;{1}",
					GetGlobalResourceObject("IbnFramework.Project", "EndDate").ToString(),
					finishdate.ToShortDateString(),
					(_filterText.Length == 0) ? "" : ",&nbsp;",
					GetGlobalResourceObject("IbnFramework.Project", "EType").ToString());
			}
		}
		#endregion

		#region AddManagerFilterText
		private void AddManagerFilterText(int iManId)
		{
			string textValue = String.Empty;
			if (iManId > 0)
				textValue = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName(iManId);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Project", "Manager").ToString(), textValue);
		}
		#endregion

		#region AddExecManagerFilterText
		private void AddExecManagerFilterText(int iExecManId)
		{
			string textValue = String.Empty;
			if (iExecManId > 0)
				textValue = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName(iExecManId);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Project", "ExecManager").ToString(), textValue);
		}
		#endregion

		#region AddClientFilterText
		private void AddClientFilterText(PrimaryKeyId orgUid, PrimaryKeyId contactUid)
		{
			string textValue = String.Empty;
			if (contactUid != PrimaryKeyId.Empty)
				textValue = Mediachase.UI.Web.Util.CommonHelper.GetEntityTitle((int)ObjectTypes.Contact, contactUid);
			else if (orgUid != PrimaryKeyId.Empty)
				textValue = Mediachase.UI.Web.Util.CommonHelper.GetEntityTitle((int)ObjectTypes.Organization, orgUid);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Project", "Client").ToString(), textValue);
		}
		#endregion

		#region AddMyParticipationOnlyFilterText
		private void AddMyParticipationOnlyFilterText()
		{
			AddFilterString(GetGlobalResourceObject("IbnFramework.Project", "OnlyMyParticipation").ToString(),
								GetGlobalResourceObject("IbnFramework.Project", "Yes").ToString());
		}
		#endregion

		#region AddPortfolioFilterText
		private void AddPortfolioFilterText(int prjgrp_id)
		{
			string textValue = String.Empty;
			if (prjgrp_id > 0)
				textValue = ProjectGroup.GetProjectGroupTitle(prjgrp_id);

			AddFilterString(GetGlobalResourceObject("IbnFramework.Project", "Portfolio").ToString(), textValue);
		}
		#endregion

		#endregion


		#region GetGroupFieldForView
		public string GetGroupFieldForView(ListViewProfile lvp)
		{
			if (_pc[_pc[ProjectListViewNameKey] + "_" + ProjectListGroupFieldKey] == null)
			{
				_pc[_pc[ProjectListViewNameKey] + "_" + ProjectListGroupFieldKey] = lvp.GroupByFieldName;
			}

			if (!Page.IsPostBack)
				_pc[_pc[ProjectListViewNameKey] + "_" + ProjectListGroupFieldKey] = lvp.GroupByFieldName;

			return _pc[_pc[ProjectListViewNameKey] + "_" + ProjectListGroupFieldKey];
		}
		#endregion

		#region SetGroupFieldForView
		public void SetGroupFieldForView(string name)
		{
			_pc[_pc[ProjectListViewNameKey] + "_" + ProjectListGroupFieldKey] = name;
		}
		#endregion

		//static methods
		#region GetViewName
		public static string GetViewName(UserLightPropertyCollection pc)
		{
			if (pc[ProjectListViewNameKey] == null)
				pc[ProjectListViewNameKey] = GetFirstAvailableListViewProfile();

			return pc[ProjectListViewNameKey];
		}
		#endregion

		#region SetViewName
		public static void SetViewName(UserLightPropertyCollection pc, string name)
		{
			pc[ProjectListViewNameKey] = name;
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
	}
}