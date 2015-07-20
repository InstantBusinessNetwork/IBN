namespace Mediachase.UI.Web.Reports.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Resources;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Xml;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using Mediachase.UI.Web.Util;
	using Mediachase.SQLQueryCreator;
	using Mediachase.IBN.Business.Reports;
	using Mediachase.Ibn.Web.Interfaces;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for TemplateEdit.
	/// </summary>
	public partial class TemplateEdit : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strXMLReport", System.Reflection.Assembly.GetExecutingAssembly());
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region Tab
		private string tab = "";
		private string Tab
		{
			get
			{
				return tab;
			}
			set
			{
				tab = value;
			}
		}
		#endregion

		#region TemplateId
		private int TemplateId
		{
			get
			{
				try
				{
					return int.Parse(Request["TemplateId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region generate
		private bool generate
		{
			get
			{
				if (Request["Generate"] != null && Request["Generate"].ToString() == "1")
					return true;
				else
					return false;
			}
		}
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterScript(Page, "~/Scripts/List2List.js");

			tblFields.Visible = false;
			tblGroupFields.Visible = false;
			tblFilters.Visible = false;
			tblFiltersGenerate.Visible = false;
			tblSorts.Visible = false;
			if (Request["Tab"] != null)
				tab = Request["Tab"].ToString();
			BindTabs();
			BindToolbars();
			if (!Page.IsPostBack)
			{
				ViewState["CurrentTab"] = pc["Temp_CurrentTab"];
				ApplyLocalization();
				BindCurrentTab();
			}
			else if (generate)
			{
				XmlDocument temp = new XmlDocument();
				using (IDataReader reader = Report.GetReportTemplate(TemplateId))
				{
					if (reader.Read())
					{
						// O.R. [2009-06-05]: If report name contains "&" then we got the exception
						//temp.InnerXml = HttpUtility.HtmlDecode(reader["TemplateXML"].ToString());
						temp.InnerXml = reader["TemplateXML"].ToString();
					}
				}
				IBNReportTemplate repTemplate = new IBNReportTemplate(temp);
				QObject qItem = GetQObject(repTemplate);
				BindGenerateFilters(repTemplate, qItem);
			}
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (repName.Text != "")
				secHeader.Title += " - " + repName.Text;
		}
		#endregion

		#region BindToolbars
		private void BindToolbars()
		{
			btnSave.Text = LocRM.GetString("tSave");
			switch (Tab)
			{
				case "Fields":
					secHeader.Title = LocRM.GetString("tFields");
					tblFields.Visible = true;
					break;
				case "Groups":
					secHeader.Title = LocRM.GetString("s6GroupFields");
					tblGroupFields.Visible = true;
					break;
				case "Filters":
					secHeader.Title = LocRM.GetString("tFilters");
					if (generate)
					{
						tblFiltersGenerate.Visible = true;
						btnSave.Text = LocRM.GetString("tGenerate");
						tblButton.Attributes.Add("width", "400px");
						tblButton.Attributes.Add("align", "left");
					}
					else
						tblFilters.Visible = true;
					break;
				case "Sorts":
					secHeader.Title = LocRM.GetString("s6SortFields");
					tblSorts.Visible = true;
					break;
				default:
					break;
			}
			secHeader.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tCustomReports"), this.ResolveUrl("~/Apps/ReportManagement/Pages/UserReport.aspx"));
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			secRepName.AddText(LocRM.GetString("s6ReportName"));
			secRepFields.AddText(LocRM.GetString("tSelectFields"));
			secRepGroup.AddText(LocRM.GetString("tSelectGroups"));
			secRepFilterFields.AddText(LocRM.GetString("tFields"));
			secFilterFieldInfo.AddText(LocRM.GetString("tFilterLegend"));
			secFilterValues.AddText(LocRM.GetString("tFilterValue"));
			secSort.AddText(LocRM.GetString("tSortLegend"));
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblSelected.Text = LocRM.GetString("tSelected") + ":";
			lblAvailable.Text = LocRM.GetString("tAvailable") + ":";

			btnAddOneGr.Attributes.Add("onclick", "MoveOne(" + lbAvailableFields.ClientID + "," + lbSelectedFields.ClientID + "); SaveFields(); return false;");
			btnAddAllGr.Attributes.Add("onclick", "MoveAll(" + lbAvailableFields.ClientID + "," + lbSelectedFields.ClientID + "); SaveFields(); return false;");
			btnRemoveOneGr.Attributes.Add("onclick", "MoveOne(" + lbSelectedFields.ClientID + "," + lbAvailableFields.ClientID + "); SaveFields(); return false;");
			btnRemoveAllGr.Attributes.Add("onclick", "MoveAll(" + lbSelectedFields.ClientID + "," + lbAvailableFields.ClientID + "); SaveFields();return false;");

			lbAvailableFields.Attributes.Add("ondblclick", "MoveOne(" + lbAvailableFields.ClientID + "," + lbSelectedFields.ClientID + "); SaveFields(); return false;");
			lbSelectedFields.Attributes.Add("ondblclick", "MoveOne(" + lbSelectedFields.ClientID + "," + lbAvailableFields.ClientID + "); SaveFields(); return false;");

			rbAscDesc.Items.Add(new ListItem(LocRM.GetString("tAsc"), "1"));
			rbAscDesc.Items.Add(new ListItem(LocRM.GetString("tDesc"), "0"));
			rbAscDesc.SelectedIndex = 0;

			btnAdd.Text = LocRM.GetString("tAdd");
			lbFields.Attributes.Add("ondblclick", "MoveSort();return false;");
		}
		#endregion

		#region BindTabs
		private void BindTabs()
		{

			if (generate)
				pc["Temp_CurrentTab"] = "Filters";
			if (Tab != "")
			{
				if (Tab == "Fields" || Tab == "Groups" || Tab == "Filters" || Tab == "Sorts")
					pc["Temp_CurrentTab"] = Tab;
			}
			else if (ViewState["CurrentTab"] != null)
				pc["Temp_CurrentTab"] = ViewState["CurrentTab"].ToString();
			else if (pc["Temp_CurrentTab"] == null)
				pc["Temp_CurrentTab"] = "Filters";
			Tab = pc["Temp_CurrentTab"];

			if (!generate)
			{
				ctrlTopTab.AddTab(LocRM.GetString("tFields"), "Fields");
				ctrlTopTab.AddTab(LocRM.GetString("s6GroupFields"), "Groups");
			}
			ctrlTopTab.AddTab(LocRM.GetString("tFilters"), "Filters");
			if (!generate)
				ctrlTopTab.AddTab(LocRM.GetString("s6SortFields"), "Sorts");

			ctrlTopTab.TabWidth = "150px";

			ctrlTopTab.SelectItem(Tab);
		}
		#endregion

		#region BindCurrentTab
		private void BindCurrentTab()
		{

			ViewState["SortFields"] = null;
			iFields.Value = "";
			ViewState["FiltersTable"] = null;
			ResultXML.Value = "";
			pastCommand.Value = "";
			changedCheck.Value = "";

			XmlDocument temp = new XmlDocument();
			using (IDataReader reader = Report.GetReportTemplate(TemplateId))
			{
				if (reader.Read())
				{
					// O.R. [2009-06-05]: If report name contains "&" then we got the exception
					//temp.InnerXml = HttpUtility.HtmlDecode(reader["TemplateXML"].ToString());
					temp.InnerXml = reader["TemplateXML"].ToString();
					repName.Text = reader["Name"].ToString();
				}
			}
			IBNReportTemplate repTemplate = new IBNReportTemplate(temp);
			QObject qItem = GetQObject(repTemplate);
			switch (Tab)
			{
				case "Fields":
					lbAvailableFields.Items.Clear();
					foreach (QField _Field in qItem.Fields)
					{
						if ((_Field.UsingType & QFieldUsingType.Field) == QFieldUsingType.Field)
							lbAvailableFields.Items.Add(new ListItem(_Field.FriendlyName, _Field.Name));
					}
					lbSelectedFields.Items.Clear();

					foreach (FieldInfo fi in repTemplate.Fields)
					{
						ListItem lItm = lbAvailableFields.Items.FindByValue(fi.Name);
						if (lItm != null)
						{
							lbAvailableFields.Items.Remove(lItm);
							lbSelectedFields.Items.Add(lItm);
						}
					}
					break;
				case "Groups":
					ddGroup1.Items.Clear();
					ddGroup2.Items.Clear();
					ddGroup1.Items.Add(new ListItem(" " + LocRM.GetString("tNotSet"), "01"));
					ddGroup2.Items.Add(new ListItem(" " + LocRM.GetString("tNotSet"), "02"));
					foreach (QField _Field in qItem.Fields)
						if ((_Field.UsingType & QFieldUsingType.Grouping) == QFieldUsingType.Grouping)
						{
							ddGroup1.Items.Add(new ListItem(_Field.FriendlyName, _Field.Name));
							ddGroup2.Items.Add(new ListItem(_Field.FriendlyName, _Field.Name));
						}
					foreach (FieldInfo fi in repTemplate.Fields)
					{
						ListItem lItm1 = ddGroup1.Items.FindByValue(fi.Name);
						if (lItm1 != null)
							ddGroup1.Items.Remove(lItm1);
						ListItem lItm2 = ddGroup2.Items.FindByValue(fi.Name);
						if (lItm2 != null)
							ddGroup2.Items.Remove(lItm2);
					}
					foreach (FieldInfo gi in repTemplate.Groups)
					{
						if (ddGroup1.SelectedItem.Value == "01")
						{
							ListItem lItm1 = ddGroup1.Items.FindByValue(gi.Name);
							if (lItm1 != null)
							{
								ddGroup1.SelectedItem.Selected = false;
								lItm1.Selected = true;
							}
							continue;
						}
						else
						{
							ListItem lItm2 = ddGroup2.Items.FindByValue(gi.Name);
							if (lItm2 != null)
							{
								ddGroup2.SelectedItem.Selected = false;
								lItm2.Selected = true;
							}
						}
					}
					break;
				case "Filters":
					if (!generate)
					{
						DataTable dtFilters = new DataTable();
						dtFilters.Columns.Add(new DataColumn("IsChecked", typeof(bool)));
						dtFilters.Columns.Add(new DataColumn("FieldName", typeof(string)));
						dtFilters.Columns.Add(new DataColumn("FriendlyName", typeof(string)));
						DataRow dr;
						foreach (QField fd in qItem.Fields)
							if ((fd.UsingType & QFieldUsingType.Filter) == QFieldUsingType.Filter)
							{
								dr = dtFilters.NewRow();
								if (repTemplate.Filters[fd.Name] != null)
									dr["IsChecked"] = true;
								else
									dr["IsChecked"] = false;
								dr["FieldName"] = fd.Name;
								dr["FriendlyName"] = fd.FriendlyName;
								dtFilters.Rows.Add(dr);
							}
						ViewState["FiltersTable"] = dtFilters;
						ResultXML.Value = repTemplate.CreateXMLTemplate().InnerXml;
						dlFilterFields.DataSource = dtFilters.DefaultView;
						dlFilterFields.DataBind();
						dlFilterFields.SelectedIndex = 0;
						LinkButton lb = (LinkButton)dlFilterFields.Items[0].FindControl("lbField");
						if (lb != null)
							BindFilter(lb.CommandName);
					}
					else
						BindGenerateFilters(repTemplate, qItem);
					break;
				case "Sorts":
					DataTable dt = new DataTable();
					dt.Columns.Add(new DataColumn("Field", typeof(string)));
					dt.Columns.Add(new DataColumn("FieldText", typeof(string)));
					dt.Columns.Add(new DataColumn("SortDirect", typeof(int))); //1 - asc, 0 - desc
					foreach (SortInfo si in repTemplate.Sorting)
					{
						DataRow s_dr = dt.NewRow();
						s_dr["Field"] = si.FieldName;
						s_dr["FieldText"] = qItem.Fields[si.FieldName].FriendlyName;
						if (si.SortDirection == Mediachase.IBN.Business.Reports.SortDirection.ASC)
							s_dr["SortDirect"] = "1";
						else
							s_dr["SortDirect"] = "0";
						dt.Rows.Add(s_dr);
					}
					ViewState["SortFields"] = dt;
					BindFields();
					BindDGFields();
					break;
				default:
					break;
			}
		}
		#endregion

		#region GetQObject
		private QObject GetQObject(IBNReportTemplate repTemp)
		{
			QObject qItem = null;
			switch (repTemp.ObjectName)
			{
				case "Incident":	//Incident
					qItem = new QIncident();
					break;
				case "Project":		//Project
					qItem = new QProject();
					break;
				case "ToDo":		//ToDo`s
					qItem = new QToDo();
					break;
				case "Event":		//Calendar Entries
					qItem = new QCalendarEntries();
					break;
				case "Document":	//Documents
					qItem = new QDocument();
					break;
				case "Directory":	//Users
					qItem = new QDirectory();
					break;
				case "Task":		//Tasks
					qItem = new QTask();
					break;
				case "Portfolio":	//Portfolios
					qItem = new QPortfolio();
					break;
				default:
					break;
			}
			return qItem;
		}
		#endregion

		#region BindGenerateFilters
		private void BindGenerateFilters(IBNReportTemplate repTemplate, QObject qItem)
		{
			tblName.Visible = false;
			foreach (FilterInfo fi in repTemplate.Filters)
			{
				HtmlTableRow row = new HtmlTableRow();
				HtmlTableCell cellValue = new HtmlTableCell();
				cellValue.Attributes.Add("width", "100%");

				QField qField = qItem.Fields[fi.FieldName];
				if (qField == null)
					continue;
				QDictionary qDict = qItem.GetDictionary(qField);

				System.Web.UI.UserControl control = null;
				IDataReader reader = null;

				if (qDict != null)
				{
					control = (System.Web.UI.UserControl)Page.LoadControl("~/Reports/Modules/FilterControls/DictionaryFilter.ascx");
					string sqlCommand = qDict.GetSQLQuery(Security.CurrentUser.LanguageId);
					reader = Report.GetQDictionary(sqlCommand);
					((Mediachase.UI.Web.Reports.Modules.DictionaryFilter)control).tdTitle.Attributes.Add("width", "150px");
				}
				else
				{
					switch (qField.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
							control = (System.Web.UI.UserControl)Page.LoadControl("~/Reports/Modules/FilterControls/IntFilter.ascx");
							((Mediachase.UI.Web.Reports.Modules.IntFilter)control).tdTitle.Attributes.Add("width", "150px");
							break;
						case DbType.DateTime:
						case DbType.Date:
							control = (System.Web.UI.UserControl)Page.LoadControl("~/Reports/Modules/FilterControls/DateFilter.ascx");
							((Mediachase.UI.Web.Reports.Modules.DateFilter)control).tdTitle.Attributes.Add("width", "150px");
							break;
						case DbType.String:
							control = (System.Web.UI.UserControl)Page.LoadControl("~/Reports/Modules/FilterControls/StringFilter.ascx");
							((Mediachase.UI.Web.Reports.Modules.StringFilter)control).tdTitle.Attributes.Add("width", "150px");
							break;
						case DbType.Time:
							control = (System.Web.UI.UserControl)Page.LoadControl("~/Reports/Modules/FilterControls/TimeFilter.ascx");
							((Mediachase.UI.Web.Reports.Modules.TimeFilter)control).tdTitle.Attributes.Add("width", "150px");
							break;
					}
				}
				if (control != null)
				{
					cellValue.Controls.Add(control);
				}
				row.Cells.Add(cellValue);
				tblFiltersValues.Rows.Add(row);
				if (control != null)
				{
					((IFilterControl)control).InitControl(reader);
					((IFilterControl)control).FilterField = qField.Name;
					((IFilterControl)control).FilterTitle = qField.FriendlyName;
					if (fi.Values != null)
					{
						switch (((IFilterControl)control).FilterType)
						{
							case "Dictionary":
								ArrayList alValue = new ArrayList();
								foreach (string _s in fi.Values)
									alValue.Add(_s);
								((IFilterControl)control).Value = alValue.ToArray(typeof(string));
								break;
							case "Int":
								if (fi.Values.Count > 0)
								{
									IntFilterValue ifValue = new IntFilterValue();
									ifValue.TypeValue = fi.Values[0];
									ifValue.FirstValue = fi.Values[1];
									ifValue.SecondValue = fi.Values[2];
									((IFilterControl)control).Value = ifValue;
								}
								break;
							case "DateTime":
								if (fi.Values.Count > 0)
								{
									DateFilterValue dtValues = new DateFilterValue();
									dtValues.TypeValue = fi.Values[0];
									dtValues.FirstValue = fi.Values[1];
									dtValues.SecondValue = fi.Values[2];
									((IFilterControl)control).Value = dtValues;
								}
								break;
							case "String":
								if (fi.Values.Count > 0)
									((IFilterControl)control).Value = fi.Values[0];
								break;
							case "Time":
								if (fi.Values.Count > 0)
								{
									TimeFilterValue tfValue = new TimeFilterValue();
									tfValue.TypeValue = fi.Values[0];
									tfValue.FirstValue = fi.Values[1];
									tfValue.SecondValue = fi.Values[2];
									((IFilterControl)control).Value = tfValue;
								}
								break;
						}
					}
					else if (fi.Values == null)
						((IFilterControl)control).Value = null;
				}
			}
			if (repTemplate.Filters.Count == 0)
			{
				XmlDocument doc = GetReportDoc(repTemplate);
				int iReportId = Report.CreateReportByTemplate(TemplateId, doc.InnerXml);
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				  "OpenWindow('../Reports/XMLReportOutput.aspx?ReportId=" +
							iReportId.ToString() + "',screen.width,screen.height,true);window.location.href='../Reports/ReportHistory.aspx?TemplateId=" + TemplateId.ToString() + "';",
				  true);
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
			this.dgSortFields.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSortFields_Delete);
			this.dlFilterFields.ItemCommand += new System.Web.UI.WebControls.DataListCommandEventHandler(this.dl_ItemCommand);
		}
		#endregion

		#region SortStep
		private void BindFields()
		{
			XmlDocument temp = new XmlDocument();
			using (IDataReader reader = Report.GetReportTemplate(TemplateId))
			{
				if (reader.Read())
				{
					// O.R. [2009-06-05]: If report name contains "&" then we got the exception
					//temp.InnerXml = HttpUtility.HtmlDecode(reader["TemplateXML"].ToString());
					temp.InnerXml = reader["TemplateXML"].ToString();
				}
			}
			IBNReportTemplate repTemplate = new IBNReportTemplate(temp);
			QObject qItem = GetQObject(repTemplate);
			lbFields.Items.Clear();
			foreach (FieldInfo fi in repTemplate.Fields)
			{
				QField _Field = qItem.Fields[fi.Name];
				if ((_Field.UsingType & QFieldUsingType.Sort) == QFieldUsingType.Sort)
					lbFields.Items.Add(new ListItem(_Field.FriendlyName, _Field.Name));
			}

			DataTable dt = (DataTable)ViewState["SortFields"];

			foreach (SortInfo si in repTemplate.Sorting)
			{
				string temp_s = si.FieldName;
				DataRow[] dr = dt.Select("Field = '" + temp_s + "'");
				if (dr.Length != 0)
				{
					ListItem lItm = lbFields.Items.FindByValue(temp_s);
					if (lItm != null)
						lbFields.Items.Remove(lItm);
				}
			}
		}
		private void BindDGFields()
		{
			dgSortFields.Columns[1].HeaderText = LocRM.GetString("tField");
			dgSortFields.Columns[2].HeaderText = LocRM.GetString("tSortDirect");

			DataTable dt = (DataTable)ViewState["SortFields"];
			dgSortFields.DataSource = dt.DefaultView;
			dgSortFields.DataBind();

			foreach (DataGridItem dgi in dgSortFields.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				ib.Attributes.Add("onclick", "return confirm('" + LocRM.GetString("tWarning") + "')");
				ib.ToolTip = LocRM.GetString("tDelete");
			}
		}

		protected string GetSortDirect(int direct)
		{
			if (direct == 1)
				return LocRM.GetString("tAsc");
			else
				return LocRM.GetString("tDesc");
		}

		private void dgSortFields_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{

			string sField = e.Item.Cells[0].Text;
			DataTable dt = (DataTable)ViewState["SortFields"];
			DataRow[] dr = dt.Select("Field = '" + sField + "'");
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["SortFields"] = dt;

			BindFields();
			BindDGFields();
		}

		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			DataTable dt = (DataTable)ViewState["SortFields"];
			foreach (ListItem li in lbFields.Items)
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					dr["Field"] = li.Value;
					dr["FieldText"] = li.Text;
					if (rbAscDesc.SelectedItem.Value == "0")
						dr["SortDirect"] = 0;
					else
						dr["SortDirect"] = 1;
					dt.Rows.Add(dr);
				}

			ViewState["SortFields"] = dt;

			BindFields();
			BindDGFields();
		}
		#endregion

		#region BindFilter
		private void dl_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
		{
			string sName = e.CommandName;
			dlFilterFields.SelectedIndex = e.Item.ItemIndex;
			BindFilter(sName);
			string NewHref = dlFilterFields.ClientID + "_" + ((LinkButton)e.CommandSource).ClientID;
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "window.location.href=window.location.href+'#" + NewHref + "';", true);
		}

		protected void lbFilterCheck_Click(object sender, System.EventArgs e)
		{
			string sName = changedCheck.Value;
			DataTable dtFilters = (DataTable)ViewState["FiltersTable"];
			string sFieldName = "";
			foreach (DataListItem item in dlFilterFields.Items)
			{
				LinkButton lbCur = (LinkButton)item.FindControl("lbField");
				if (lbCur != null && lbCur.ClientID == sName)
				{
					sFieldName = lbCur.CommandName;
					break;
				}
			}

			DataRow[] dr = dtFilters.Select("FieldName = '" + sFieldName + "'");
			if (dr.Length > 0)
			{
				if ((bool)dr[0]["IsChecked"])
					dr[0]["IsChecked"] = false;
				else
					dr[0]["IsChecked"] = true;
			}
			ViewState["FiltersTable"] = dtFilters;
			BindFilter(sFieldName);
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "window.location.href=window.location.href+'#" + sName + "';", true);
		}

		private void BindFilter(string command)
		{
			DataTable dtFilters = (DataTable)ViewState["FiltersTable"];

			IBNReportTemplate repTemp = null;
			if (ResultXML.Value != "")
			{
				repTemp = IBNReportTemplate.Load(ResultXML.Value);
			}
			if (repTemp == null)
				repTemp = new IBNReportTemplate();

			QObject qItem = GetQObject(repTemp);

			#region PastStep
			if (pastCommand.Value != "")
			{
				QField qPast = qItem.Fields[pastCommand.Value];
				if (qPast != null)
				{
					QDictionary qDicPast = qItem.GetDictionary(qPast);
					bool _IsCheck = false;
					DataRow[] dr = dtFilters.Select("FieldName = '" + qPast.Name + "'");
					if (dr.Length > 0)
						_IsCheck = (bool)dr[0]["IsChecked"];
					FilterInfo fi = new FilterInfo();
					fi.FieldName = qPast.Name;
					fi.DataType = qPast.DataType.ToString();
					object curValue = null;

					if (qDicPast != null)
					{
						curValue = dictFltr.Value;
						if (curValue != null)
						{
							foreach (string _s in (string[])curValue)
								fi.Values.Add(_s);
						}
					}
					else
					{
						switch (qPast.DataType)
						{
							case DbType.Decimal:
							case DbType.Int32:
								curValue = intFltr.Value;
								if (curValue != null)
								{
									fi.Values.Add(((IntFilterValue)curValue).TypeValue);
									fi.Values.Add(((IntFilterValue)curValue).FirstValue);
									fi.Values.Add(((IntFilterValue)curValue).SecondValue);
								}
								break;
							case DbType.DateTime:
							case DbType.Date:
								curValue = dtFltr.Value;
								if (curValue != null)
								{
									fi.Values.Add(((DateFilterValue)curValue).TypeValue);
									fi.Values.Add(((DateFilterValue)curValue).FirstValue);
									fi.Values.Add(((DateFilterValue)curValue).SecondValue);
								}
								break;
							case DbType.String:
								curValue = strFltr.Value;
								if (curValue != null)
									fi.Values.Add(curValue.ToString());
								break;
							case DbType.Time:
								curValue = timeFltr.Value;
								if (curValue != null)
								{
									fi.Values.Add(((TimeFilterValue)curValue).TypeValue);
									fi.Values.Add(((TimeFilterValue)curValue).FirstValue);
									fi.Values.Add(((TimeFilterValue)curValue).SecondValue);
								}
								break;
						}
					}
					if (_IsCheck)
					{
						int _index = repTemp.Filters.IndexOf(repTemp.Filters[fi.FieldName]);
						if (_index >= 0)
						{
							repTemp.Filters.RemoveAt(_index);
							repTemp.Filters.Insert(_index, fi);
						}
						else
							repTemp.Filters.Add(fi);
					}
					else
					{
						int _index = repTemp.Filters.IndexOf(repTemp.Filters[fi.FieldName]);
						if (_index >= 0)
						{
							repTemp.Filters.RemoveAt(_index);
						}
						else if (fi.Values.Count > 0)
						{
							dr[0]["IsChecked"] = true;
							repTemp.Filters.Add(fi);
						}
					}
				}
			}
			#endregion

			#region CurrentStep
			if (command != "")
			{
				DataRow[] dr = dtFilters.Select("FieldName = '" + command + "'");
				bool _IsCheckCur = false;
				if (dr.Length > 0)
					_IsCheckCur = (bool)dr[0]["IsChecked"];

				QField qField = qItem.Fields[command];
				if (qField != null)
				{
					QDictionary qDict = qItem.GetDictionary(qField);

					dictFltr.Visible = false;
					intFltr.Visible = false;
					dtFltr.Visible = false;
					strFltr.Visible = false;
					timeFltr.Visible = false;
					IFilterControl control = null;
					IDataReader reader = null;

					if (qDict != null)
					{
						control = dictFltr;
						string sqlCommand = qDict.GetSQLQuery(Security.CurrentUser.LanguageId);
						reader = Report.GetQDictionary(sqlCommand);
					}
					else
					{
						switch (qField.DataType)
						{
							case DbType.Decimal:
							case DbType.Int32:
								control = intFltr;
								break;
							case DbType.DateTime:
							case DbType.Date:
								control = dtFltr;
								break;
							case DbType.String:
								control = strFltr;
								break;
							case DbType.Time:
								control = timeFltr;
								break;
						}
					}
					if (control != null)
					{
						((System.Web.UI.UserControl)control).Visible = true;
						control.InitControl(reader);
						control.FilterTitle = qField.FriendlyName;
						FilterInfo flt = repTemp.Filters[command];
						if (flt != null && _IsCheckCur)
						{
							switch (control.FilterType)
							{
								case "Dictionary":
									ArrayList alValue = new ArrayList();
									foreach (string _s in flt.Values)
										alValue.Add(_s);
									dictFltr.Value = alValue.ToArray(typeof(string));
									break;
								case "Int":
									if (flt.Values.Count > 0)
									{
										IntFilterValue ifValue = new IntFilterValue();
										ifValue.TypeValue = flt.Values[0];
										ifValue.FirstValue = flt.Values[1];
										ifValue.SecondValue = flt.Values[2];
										intFltr.Value = ifValue;
									}
									break;
								case "DateTime":
									if (flt.Values.Count > 0)
									{
										DateFilterValue dtValues = new DateFilterValue();
										dtValues.TypeValue = flt.Values[0];
										dtValues.FirstValue = flt.Values[1];
										dtValues.SecondValue = flt.Values[2];
										dtFltr.Value = dtValues;
									}
									break;
								case "String":
									if (flt.Values.Count > 0)
										strFltr.Value = flt.Values[0];
									break;
								case "Time":
									if (flt.Values.Count > 0)
									{
										TimeFilterValue tfValue = new TimeFilterValue();
										tfValue.TypeValue = flt.Values[0];
										tfValue.FirstValue = flt.Values[1];
										tfValue.SecondValue = flt.Values[2];
										timeFltr.Value = tfValue;
									}
									break;
							}
						}
						else if (flt == null && _IsCheckCur)
						{
							control.Value = null;
							FilterInfo fi = new FilterInfo();
							fi.FieldName = qField.Name;
							fi.DataType = qField.DataType.ToString();
							repTemp.Filters.Add(fi);
						}
						else if (flt != null && !_IsCheckCur)
						{
							control.Value = null;
							int _index = repTemp.Filters.IndexOf(flt);
							repTemp.Filters.RemoveAt(_index);
						}
					}
				}
			}
			#endregion

			ResultXML.Value = repTemp.CreateXMLTemplate().InnerXml;
			ViewState["FiltersTable"] = dtFilters;
			dlFilterFields.DataSource = dtFilters.DefaultView;
			dlFilterFields.DataBind();
			dlFilterFields.SelectedIndex = -1;
			foreach (DataListItem liItem in dlFilterFields.Items)
			{
				dlFilterFields.SelectedIndex++;
				LinkButton lb = (LinkButton)liItem.FindControl("lbField");
				if (lb != null && lb.CommandName == command)
					break;
			}

			lblCurrentFilter.Text = MakeFilterText(repTemp);

			pastCommand.Value = command;
		}
		#endregion

		#region MakeFilterText
		protected string MakeFilterText(IBNReportTemplate repTemp)
		{
			string retval = "";
			QObject qItem = GetQObject(repTemp);
			foreach (FilterInfo fi in repTemp.Filters)
			{
				QField qTemp = qItem.Fields[fi.FieldName];
				if (qTemp == null)
					continue;
				QDictionary qDTemp = qItem.GetDictionary(qTemp);
				if (qDTemp != null)
				{
					retval += qTemp.FriendlyName + "&nbsp;=&nbsp;";
					string sqlCommand = qDTemp.GetSQLQuery(Security.CurrentUser.LanguageId);
					using (IDataReader reader = Report.GetQDictionary(sqlCommand))
					{
						ArrayList alDicVal = new ArrayList();
						foreach (string _s in fi.Values)
						{
							alDicVal.Add(_s);
						}
						while (reader.Read())
						{
							if (alDicVal.Contains(reader["Id"].ToString()))
							{
								retval += "<font color='red'>" + CommonHelper.GetResFileString(reader["Value"].ToString()) + "</font>,&nbsp;";
							}
						}
					}
					retval = retval.Remove(retval.Length - 7, 7) + "<br>";
				}
				else
				{
					switch (qTemp.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
							retval += qTemp.FriendlyName;
							if (fi.Values.Count > 0)
							{
								switch (fi.Values[0])
								{
									case "0":
										retval += "&nbsp;=<font color='red'>&nbsp;" + fi.Values[1] + "</font><br>";
										break;
									case "1":
										retval += "&nbsp;&gt;<font color='red'>&nbsp;" + fi.Values[1] + "</font><br>";
										break;
									case "2":
										retval += "&nbsp;&lt;<font color='red'>&nbsp;" + fi.Values[1] + "</font><br>";
										break;
									case "3":
										retval += "&nbsp;<font color='red'>" + LocRM.GetString("tBetween") + "&nbsp;" + fi.Values[1] + "&nbsp;-&nbsp;" + fi.Values[2] + "</font><br>";
										break;
								}
							}
							else
								retval += "<br>";
							break;
						case DbType.DateTime:
						case DbType.Date:
							retval += qTemp.FriendlyName;
							if (fi.Values.Count > 0)
							{
								switch (fi.Values[0])
								{
									case "1":
										retval += "&nbsp;=<font color='red'>&nbsp;" + LocRM.GetString("tToday") + "</font><br>";
										break;
									case "2":
										retval += "&nbsp;=<font color='red'>&nbsp;" + LocRM.GetString("tYesterday") + "</font><br>";
										break;
									case "3":
										retval += "&nbsp;=<font color='red'>&nbsp;" + LocRM.GetString("tThisWeek") + "</font><br>";
										break;
									case "4":
										retval += "&nbsp;=<font color='red'>&nbsp;" + LocRM.GetString("tLastWeek") + "</font><br>";
										break;
									case "5":
										retval += "&nbsp;=<font color='red'>&nbsp;" + LocRM.GetString("tThisMonth") + "</font><br>";
										break;
									case "6":
										retval += "&nbsp;=<font color='red'>&nbsp;" + LocRM.GetString("tLastMonth") + "</font><br>";
										break;
									case "7":
										retval += "&nbsp;=<font color='red'>&nbsp;" + LocRM.GetString("tThisYear") + "</font><br>";
										break;
									case "8":
										retval += "&nbsp;=<font color='red'>&nbsp;" + LocRM.GetString("tLastYear") + "</font><br>";
										break;
									case "9":
										if (DateTime.Parse(fi.Values[1]) == DateTime.MinValue)
										{
											retval += "&nbsp;<font color='red'>" + LocRM.GetString("tLess") + "&nbsp;" + DateTime.Parse(fi.Values[2]).ToShortDateString() + "</font><br>";
										}
										else if (DateTime.Parse(fi.Values[2]) >= DateTime.MaxValue.Date)
										{
											retval += "&nbsp;<font color='red'>" + LocRM.GetString("tGreater") + "&nbsp;" + DateTime.Parse(fi.Values[1]).ToShortDateString() + "</font><br>";
										}
										else
											retval += "&nbsp;<font color='red'>" + LocRM.GetString("tBetween") + "&nbsp;" + DateTime.Parse(fi.Values[1]).ToShortDateString() + "&nbsp;-&nbsp;" + DateTime.Parse(fi.Values[2]).ToShortDateString() + "</font><br>";
										break;
								}
							}
							else
								retval += "<br>";
							break;
						case DbType.String:
							if (fi.Values.Count > 0)
							{
								retval += qTemp.FriendlyName + "&nbsp;=&nbsp;<font color='red'>" + fi.Values[0] + "</font><br>";
							}
							else
								retval += qTemp.FriendlyName + "<br>";
							break;
						case DbType.Time:
							retval += qTemp.FriendlyName;
							if (fi.Values.Count > 0)
							{
								switch (fi.Values[0])
								{
									case "0":
										retval += "&nbsp;=<font color='red'>&nbsp;" + CommonHelper.GetHours(int.Parse(fi.Values[1])) + "</font><br>";
										break;
									case "1":
										retval += "&nbsp;&gt;<font color='red'>&nbsp;" + CommonHelper.GetHours(int.Parse(fi.Values[1])) + "</font><br>";
										break;
									case "2":
										retval += "&nbsp;&lt;<font color='red'>&nbsp;" + CommonHelper.GetHours(int.Parse(fi.Values[1])) + "</font><br>";
										break;
									case "3":
										retval += "&nbsp;<font color='red'>" + LocRM.GetString("tBetween") + "&nbsp;" + CommonHelper.GetHours(int.Parse(fi.Values[1])) + "&nbsp;-&nbsp;" + CommonHelper.GetHours(int.Parse(fi.Values[2])) + "</font><br>";
										break;
								}
							}
							else
								retval += "<br>";
							break;
					}
				}
			}
			return retval;
		}
		#endregion

		#region Save-Update
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			XmlDocument temp = new XmlDocument();
			bool isGlobal = true;
			using (IDataReader reader = Report.GetReportTemplate(TemplateId))
			{
				if (reader.Read())
				{
					// O.R. [2009-06-05]: If report name contains "&" then we got the exception
					//temp.InnerXml = HttpUtility.HtmlDecode(reader["TemplateXML"].ToString());
					temp.InnerXml = reader["TemplateXML"].ToString();
					isGlobal = (bool)reader["IsGlobal"];
				}
			}
			IBNReportTemplate repTemplate = new IBNReportTemplate(temp);
			QObject qItem = GetQObject(repTemplate);
			switch (Tab)
			{
				case "Fields":
					string sFields = iFields.Value;
					ArrayList alFields = new ArrayList();
					while (sFields.Length > 0)
					{
						alFields.Add(sFields.Substring(0, sFields.IndexOf(",")));
						sFields = sFields.Remove(0, sFields.IndexOf(",") + 1);
					}
					repTemplate.Fields.Clear();
					foreach (string s in alFields)
					{
						repTemplate.Fields.Add(new FieldInfo(s, qItem.Fields[s].DataType.ToString()));
						if (repTemplate.Groups[s] != null)
							repTemplate.Groups.RemoveAt(repTemplate.Groups.IndexOf(repTemplate.Groups[s]));
					}
					ArrayList alSort = new ArrayList();
					foreach (SortInfo si in repTemplate.Sorting)
						alSort.Add(si.FieldName);
					foreach (string s in alSort)
					{
						if (repTemplate.Fields[s] == null)
							repTemplate.Sorting.RemoveAt(repTemplate.Sorting.IndexOf(repTemplate.Sorting[s]));
					}
					break;
				case "Groups":
					repTemplate.Groups.Clear();
					if (ddGroup1.SelectedItem.Value != "01")
						repTemplate.Groups.Add(new FieldInfo(ddGroup1.SelectedItem.Value, qItem.Fields[ddGroup1.SelectedItem.Value].DataType.ToString()));
					if (ddGroup2.SelectedItem.Value != "02")
						repTemplate.Groups.Add(new FieldInfo(ddGroup2.SelectedItem.Value, qItem.Fields[ddGroup2.SelectedItem.Value].DataType.ToString()));
					break;
				case "Filters":
					if (!generate)
					{
						BindFilter("");
						IBNReportTemplate repTemp = IBNReportTemplate.Load(ResultXML.Value);
						if (repTemp != null)
						{
							repTemplate.Filters.Clear();
							foreach (FilterInfo fi in repTemp.Filters)
								repTemplate.Filters.Add(fi);
						}
					}
					else
					{
						foreach (HtmlTableRow row in tblFiltersValues.Rows)
						{
							HtmlTableCell cell = row.Cells[0];
							IFilterControl ctrl = (IFilterControl)cell.Controls[0];
							string FieldName = ctrl.FilterField;
							FilterInfo flt = new FilterInfo();
							flt.FieldName = FieldName;
							flt.DataType = repTemplate.Filters[FieldName].DataType;
							repTemplate.Filters.RemoveAt(repTemplate.Filters.IndexOf(repTemplate.Filters[FieldName]));
							object curValue = null;
							switch (ctrl.FilterType)
							{
								case "Dictionary":
									curValue = ctrl.Value;
									if (curValue != null)
									{
										foreach (string _s in (string[])curValue)
											flt.Values.Add(_s);
									}
									break;
								case "DateTime":
									curValue = ctrl.Value;
									if (curValue != null)
									{
										flt.Values.Add(((DateFilterValue)curValue).TypeValue);
										flt.Values.Add(((DateFilterValue)curValue).FirstValue);
										flt.Values.Add(((DateFilterValue)curValue).SecondValue);
									}
									break;
								case "Int":
									curValue = ctrl.Value;
									if (curValue != null)
									{
										flt.Values.Add(((IntFilterValue)curValue).TypeValue);
										flt.Values.Add(((IntFilterValue)curValue).FirstValue);
										flt.Values.Add(((IntFilterValue)curValue).SecondValue);
									}
									break;
								case "String":
									curValue = ctrl.Value;
									if (curValue != null)
										flt.Values.Add(curValue.ToString());
									break;
								case "Time":
									curValue = ctrl.Value;
									if (curValue != null)
									{
										flt.Values.Add(((TimeFilterValue)curValue).TypeValue);
										flt.Values.Add(((TimeFilterValue)curValue).FirstValue);
										flt.Values.Add(((TimeFilterValue)curValue).SecondValue);
									}
									break;
								default:
									break;
							}
							repTemplate.Filters.Add(flt);
						}
					}
					break;
				case "Sorts":
					DataTable dt = (DataTable)ViewState["SortFields"];
					repTemplate.Sorting.Clear();
					if (dt.Rows.Count > 0)
					{
						foreach (DataRow dr in dt.Rows)
						{
							SortInfo si = new SortInfo();
							string sS = dr["Field"].ToString();
							si.FieldName = sS;
							si.DataType = qItem.Fields[sS].DataType.ToString();
							if (dr["SortDirect"].ToString() == "0")
								si.SortDirection = Mediachase.IBN.Business.Reports.SortDirection.DESC;
							else
								si.SortDirection = Mediachase.IBN.Business.Reports.SortDirection.ASC;
							repTemplate.Sorting.Add(si);
						}
					}
					break;
				default:
					break;
			}
			if (!generate)
			{
				Report.UpdateReportTemplate(TemplateId, repName.Text, repTemplate.CreateXMLTemplate().InnerXml, isGlobal, false);
				BindCurrentTab();
			}
			else
			{
				XmlDocument doc = GetReportDoc(repTemplate);
				int iReportId = Report.CreateReportByTemplate(TemplateId, doc.InnerXml);
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				  "OpenWindow('../Reports/XMLReportOutput.aspx?ReportId=" +
							iReportId.ToString() + "',screen.width,screen.height,true);window.location.href='../Reports/ReportHistory.aspx?TemplateId=" + TemplateId.ToString() + "';",
				  true);
			}
		}
		#endregion

		#region GetReportDoc
		private XmlDocument GetReportDoc(IBNReportTemplate repTemplate)
		{
			XmlDocument doc = new XmlDocument();
			switch (repTemplate.ObjectName)
			{
				case "Incident":
					doc = Report.GetIncidentXMLReport(repTemplate,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "Project":
					doc = Report.GetProjectXMLReport(repTemplate,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "ToDo":
					doc = Report.GetToDoXMLReport(repTemplate,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "Event":
					doc = Report.GetCalendarEntryXMLReport(repTemplate,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "Document":
					doc = Report.GetDocumentXMLReport(repTemplate,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "Directory":
					doc = Report.GetUsersXMLReport(repTemplate,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "Task":
					doc = Report.GetTaskXMLReport(repTemplate,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "Portfolio":
					doc = Report.GetPortfolioXMLReport(repTemplate,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				default:
					break;
			}
			return doc;
		}
		#endregion

	}
}
