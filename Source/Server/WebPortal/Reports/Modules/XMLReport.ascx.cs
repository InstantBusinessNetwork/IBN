namespace Mediachase.UI.Web.Reports.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.IO;
	using System.Resources;
	using System.Text;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Xml;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.Reports;
	using Mediachase.SQLQueryCreator;
	using Mediachase.UI.Web.Util;
	using Mediachase.UI.Web.Wizards.Modules;
	using Mediachase.Ibn;
	using Mediachase.Ibn.Web.Interfaces;
	
	/// <summary>
	///		Summary description for XMLReport.
	/// </summary>
	public partial class XMLReport : System.Web.UI.UserControl, IWizardControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strXMLReport", System.Reflection.Assembly.GetExecutingAssembly());
		ArrayList _subtitles = new ArrayList();
		ArrayList _steps = new ArrayList();
		private int _stepCount = 7;
		protected QObject _qItem = null;
		protected UserLightPropertyCollection _pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolBars();
			if (!Page.IsPostBack)
				BindStep1();
		}

		#region BindToolBars
		private void BindToolBars()
		{
			if (_pc["Cust_Rep_valMode"].ToString() == "Temp")
				secElements.AddText(LocRM.GetString("tLgdMainPage1"));
			else
				secElements.AddText(LocRM.GetString("tLgdMainPage"));
			secSelField.AddText(LocRM.GetString("tSelectFields"));
			secGroup.AddText(LocRM.GetString("tSelectGroups"));
			secFilterFields.AddText(LocRM.GetString("tFields"));
			secFilterFieldInfo.AddText(LocRM.GetString("tFilterLegend"));
			secFilterValues.AddText(LocRM.GetString("tFilterValue"));
			secSort.AddText(LocRM.GetString("tSortLegend"));
			secSettings.AddText(LocRM.GetString("tSettings"));
			secPreview.AddText(LocRM.GetString("tPreview"));
			secChoice.AddText(LocRM.GetString("tCanCreateOnTemplate"));
		}
		#endregion

		#region InitializeVariables
		private void InitializeVariables()
		{
			bool fl = true;
			if (Request["Type"] != null)
			{
				if (Request["Type"].ToString() == "Inc")
					Elem.Value = "0";
				else if (Request["Type"].ToString() == "Prj")
					Elem.Value = "1";
				else if (Request["Type"].ToString() == "ToDo")
					Elem.Value = "2";
				else if (Request["Type"].ToString() == "CalEntr")
					Elem.Value = "3";
				else if (Request["Type"].ToString() == "Docs")
					Elem.Value = "4";
				else if (Request["Type"].ToString() == "Usr")
					Elem.Value = "5";
				else if (Request["Type"].ToString() == "Tsks")
					Elem.Value = "6";
				else if (Request["Type"].ToString() == "PrjGrp")
					Elem.Value = "7";
				else
					fl = false;
			}
			else
				fl = false;
			if (!Page.IsPostBack && Request["Mode"] != null)
			{
				_pc["Cust_Rep_valMode"] = Request["Mode"].ToString();
				if (_pc["Cust_Rep_valMode"].ToString() != "Both")
					fsChoice.Visible = false;
			}
			if (!fl)
			{
				Elem.Value = "NULL";
				if (_pc["Cust_Rep_valMode"] != null && _pc["Cust_Rep_valMode"].ToString() == "Temp")
				{
					_subtitles.Add(LocRM.GetString("s1SubTitle1"));
					_subtitles.Add(LocRM.GetString("s2SubTitle1"));
					_subtitles.Add(LocRM.GetString("s3SubTitle1"));
					_subtitles.Add(LocRM.GetString("s4SubTitle1"));
					_subtitles.Add(LocRM.GetString("s5SubTitle1"));
					_subtitles.Add(LocRM.GetString("s6SubTitle1"));
					_subtitles.Add(LocRM.GetString("s7SubTitle1"));
					_subtitles.Add(LocRM.GetString("s8SubTitle1"));
				}
				else
				{
					_subtitles.Add(LocRM.GetString("s1SubTitle"));
					_subtitles.Add(LocRM.GetString("s2SubTitle"));
					_subtitles.Add(LocRM.GetString("s3SubTitle"));
					_subtitles.Add(LocRM.GetString("s4SubTitle"));
					_subtitles.Add(LocRM.GetString("s5SubTitle"));
					_subtitles.Add(LocRM.GetString("s6SubTitle"));
					_subtitles.Add(LocRM.GetString("s7SubTitle"));
					_subtitles.Add(LocRM.GetString("s8SubTitle"));
				}
				_steps.Add(step1);
				_steps.Add(step2);
				_steps.Add(step3);
				_steps.Add(step4);
				_steps.Add(step5);
				_steps.Add(step6);
				_steps.Add(step7);
				_steps.Add(step8);
			}
			else
			{
				_stepCount = 6;
				step1.Visible = false;
				if (_pc["Cust_Rep_valMode"] != null && _pc["Cust_Rep_valMode"].ToString() == "Temp")
				{
					_subtitles.Add(LocRM.GetString("s2SubTitle1"));
					_subtitles.Add(LocRM.GetString("s3SubTitle1"));
					_subtitles.Add(LocRM.GetString("s4SubTitle1"));
					_subtitles.Add(LocRM.GetString("s5SubTitle1"));
					_subtitles.Add(LocRM.GetString("s6SubTitle1"));
					_subtitles.Add(LocRM.GetString("s7SubTitle1"));
					_subtitles.Add(LocRM.GetString("s8SubTitle1"));
				}
				else
				{
					_subtitles.Add(LocRM.GetString("s2SubTitle"));
					_subtitles.Add(LocRM.GetString("s3SubTitle"));
					_subtitles.Add(LocRM.GetString("s4SubTitle"));
					_subtitles.Add(LocRM.GetString("s5SubTitle"));
					_subtitles.Add(LocRM.GetString("s6SubTitle"));
					_subtitles.Add(LocRM.GetString("s7SubTitle"));
					_subtitles.Add(LocRM.GetString("s8SubTitle1"));
				}
				_steps.Add(step2);
				_steps.Add(step3);
				_steps.Add(step4);
				_steps.Add(step5);
				_steps.Add(step6);
				_steps.Add(step7);
				_steps.Add(step8);
			}
		}
		#endregion

		#region BindStep1
		private void BindStep1()
		{
			pastStep.Value = "1";
			int correction = -1;
			if (Configuration.HelpDeskEnabled)
				rbReportItem.Items.Add(new ListItem(" " + LocRM.GetString("tIncidents"), "0"));
			if (Configuration.ProjectManagementEnabled &&
				Security.IsUserInGroup(InternalSecureGroups.ProjectManager))
			{
				rbReportItem.Items.Add(new ListItem(" " + LocRM.GetString("tProjects"), "1"));
				correction = 0;
			}
			else if (Elem.Value != "NULL" && Elem.Value == "0")
				correction = 0;
			rbReportItem.Items.Add(new ListItem(" " + LocRM.GetString("tToDo"), "2"));
			rbReportItem.Items.Add(new ListItem(" " + LocRM.GetString("tCalEntries"), "3"));
			rbReportItem.Items.Add(new ListItem(" " + LocRM.GetString("tDocuments"), "4"));
			rbReportItem.Items.Add(new ListItem(" " + LocRM.GetString("tUsers"), "5"));
			if (Configuration.ProjectManagementEnabled)
			{
				rbReportItem.Items.Add(new ListItem(" " + LocRM.GetString("tTasks"), "6"));
				rbReportItem.Items.Add(new ListItem(" " + LocRM.GetString("tPrjGrps"), "7"));
			}

			if (Elem.Value == "NULL")
				rbReportItem.SelectedIndex = 0;
			else
				rbReportItem.SelectedIndex = int.Parse(Elem.Value) + correction;

			cbOnlyForMe.Text = " " + LocRM.GetString("tOnlyForMe");
			cbShowEmptyItems.Text = " " + LocRM.GetString("tShowEmptyGroupItem");
			cbGenerateNow.Text = " " + LocRM.GetString("tGenerateNow");
			cbGenerateNow.Checked = true;

			lblSelected.Text = LocRM.GetString("tSelected") + ":";
			lblAvailable.Text = LocRM.GetString("tAvailable") + ":";

			btnAddOneGr.Attributes.Add("onclick", "MoveOne(" + lbAvailableFields.ClientID + "," + lbSelectedFields.ClientID + "); SaveFields(); return false;");
			btnAddAllGr.Attributes.Add("onclick", "MoveAll(" + lbAvailableFields.ClientID + "," + lbSelectedFields.ClientID + "); SaveFields(); return false;");
			btnRemoveOneGr.Attributes.Add("onclick", "MoveOne(" + lbSelectedFields.ClientID + "," + lbAvailableFields.ClientID + "); SaveFields(); return false;");
			btnRemoveAllGr.Attributes.Add("onclick", "MoveAll(" + lbSelectedFields.ClientID + "," + lbAvailableFields.ClientID + "); SaveFields();return false;");

			lbAvailableFields.Attributes.Add("ondblclick", "MoveOne(" + lbAvailableFields.ClientID + "," + lbSelectedFields.ClientID + "); SaveFields(); return false;");
			lbSelectedFields.Attributes.Add("ondblclick", "MoveOne(" + lbSelectedFields.ClientID + "," + lbAvailableFields.ClientID + "); SaveFields(); return false;");
			lbFields.Attributes.Add("ondblclick", "MoveSort();return false;");

			rbAscDesc.Items.Add(new ListItem(LocRM.GetString("tAsc"), "1"));
			rbAscDesc.Items.Add(new ListItem(LocRM.GetString("tDesc"), "0"));
			rbAscDesc.SelectedIndex = 0;

			rblGrouptype.Items.Add(new ListItem(LocRM.GetString("tOldType"), "0"));
			rblGrouptype.Items.Add(new ListItem(LocRM.GetString("tNewType"), "1"));
			rblGrouptype.SelectedIndex = 1;

			btnPreview.Text = LocRM.GetString("tViewReport");
		}
		#endregion

		private void ShowStep(int step)
		{
			for (int i = 0; i <= _stepCount; i++)
				((Panel)_steps[i]).Visible = false;

			((Panel)_steps[step - 1]).Visible = true;

			if (step == 1 && Elem.Value == "NULL")
			{
				pastStep.Value = "1";
				ResultXML.Value = "";
				ViewState["FiltersTable"] = null;
				ViewState["SortFields"] = null;
				if (_pc["Cust_Rep_valMode"].ToString() == "Both")
				{
					ddTemplates.DataSource = Report.GetReportTemplate();
					ddTemplates.DataValueField = "TemplateId";
					ddTemplates.DataTextField = "Name";
					ddTemplates.DataBind();
					if (ddTemplates.Items.Count == 0)
						fsChoice.Visible = false;
				}
			}

			#region Step 2 - Fields
			if ((step == 2 && Elem.Value == "NULL") || (step == 1 && Elem.Value != "NULL"))
			{
				if (valMode.Value == "Both1" && ddTemplates.SelectedItem != null)
				{
					int reportTemplateId = int.Parse(ddTemplates.SelectedValue);
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
								  "<script language='javascript'>" +
								  "try {window.opener.top.frames['right'].location.href='../Reports/TemplateEdit.aspx?Generate=1&TemplateId=" + reportTemplateId.ToString() + "';}" +
								  "catch (e){}window.close();</script>");
				}
				lbAvailableFields.Items.Clear();
				BindItemCollections(lbAvailableFields.Items, rbReportItem.SelectedItem.Value, 0);
				lbSelectedFields.Items.Clear();

				string sFields = iFields.Value;
				ArrayList alFields = new ArrayList();
				while (sFields.Length > 0)
				{
					alFields.Add(sFields.Substring(0, sFields.IndexOf(",")));
					sFields = sFields.Remove(0, sFields.IndexOf(",") + 1);
				}
				foreach (string i in alFields)
				{
					ListItem lItm = lbAvailableFields.Items.FindByValue(i);
					if (lItm != null)
					{
						lbAvailableFields.Items.Remove(lItm);
						lbSelectedFields.Items.Add(lItm);
					}
				}
				pastStep.Value = "2";
			}
			#endregion

			#region Step 3 - Grouping
			if ((step == 3 && Elem.Value == "NULL") || (step == 2 && Elem.Value != "NULL"))
			{
				if (pastStep.Value == "2")
				{
					ddGroup1.Items.Clear();
					ddGroup2.Items.Clear();
					ddGroup1.Items.Add(new ListItem(" " + LocRM.GetString("tNotSet"), "01"));
					ddGroup2.Items.Add(new ListItem(" " + LocRM.GetString("tNotSet"), "02"));
					BindItemCollections(ddGroup1.Items, rbReportItem.SelectedItem.Value, 1);
					BindItemCollections(ddGroup2.Items, rbReportItem.SelectedItem.Value, 1);

					string sFields = iFields.Value;
					ArrayList alFields = new ArrayList();
					while (sFields.Length > 0)
					{
						alFields.Add(sFields.Substring(0, sFields.IndexOf(",")));
						sFields = sFields.Remove(0, sFields.IndexOf(",") + 1);
					}
					foreach (string i in alFields)
					{
						ListItem lItm1 = ddGroup1.Items.FindByValue(i);
						if (lItm1 != null)
							ddGroup1.Items.Remove(lItm1);
						ListItem lItm2 = ddGroup2.Items.FindByValue(i);
						if (lItm2 != null)
							ddGroup2.Items.Remove(lItm2);
					}

					string sGroups = iGroups.Value;
					ArrayList alGroups = new ArrayList();
					while (sGroups.Length > 0)
					{
						alGroups.Add(sGroups.Substring(0, sGroups.IndexOf(",")));
						sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
					}
					foreach (string i in alGroups)
					{
						if (ddGroup1.SelectedItem.Value == "01")
						{
							ListItem lItm1 = ddGroup1.Items.FindByValue(i);
							if (lItm1 != null)
							{
								ddGroup1.SelectedItem.Selected = false;
								lItm1.Selected = true;
							}
							continue;
						}
						else
						{
							ListItem lItm2 = ddGroup2.Items.FindByValue(i);
							if (lItm2 != null)
							{
								ddGroup2.SelectedItem.Selected = false;
								lItm2.Selected = true;
							}
						}
					}
				}
				pastStep.Value = "3";
				pastCommand.Value = "";
			}
			#endregion

			#region Step 4 - Filtering
			if ((step == 4 && Elem.Value == "NULL") || (step == 3 && Elem.Value != "NULL"))
			{
				iGroups.Value = "";
				if (ddGroup1.SelectedItem.Value != "01")
					iGroups.Value += ddGroup1.SelectedItem.Value + ",";
				if (ddGroup2.SelectedItem.Value != "02")
					iGroups.Value += ddGroup2.SelectedItem.Value + ",";

				DataTable dtFilters = new DataTable();
				if (ViewState["FiltersTable"] == null || !(ViewState["FiltersTable"] is DataTable))
				{
					dtFilters.Columns.Add(new DataColumn("IsChecked", typeof(bool)));
					dtFilters.Columns.Add(new DataColumn("FieldName", typeof(string)));
					dtFilters.Columns.Add(new DataColumn("FriendlyName", typeof(string)));
					DataRow dr;
					if (_qItem != null)
					{
						foreach (QField fd in _qItem.Fields)
							if ((fd.UsingType & QFieldUsingType.Filter) == QFieldUsingType.Filter)
							{
								dr = dtFilters.NewRow();
								dr["IsChecked"] = false;
								dr["FieldName"] = fd.Name;
								dr["FriendlyName"] = fd.FriendlyName;
								dtFilters.Rows.Add(dr);
							}
						ViewState["FiltersTable"] = dtFilters;
					}
				}
				else
					dtFilters = (DataTable)ViewState["FiltersTable"];
				DataView dvFilters = dtFilters.DefaultView;
				dvFilters.Sort = "FriendlyName";
				dlFilterFields.DataSource = dvFilters;
				dlFilterFields.DataBind();
				dlFilterFields.SelectedIndex = 0;
				LinkButton lb = (LinkButton)dlFilterFields.Items[0].FindControl("lbField");
				if (lb != null)
					BindFilter(lb.CommandName);
				pastStep.Value = "4";
			}
			#endregion

			#region Step 5 - SortStep
			if ((step == 5 && Elem.Value == "NULL") || (step == 4 && Elem.Value != "NULL"))
			{
				BindFilter("");

				btnAdd.Text = LocRM.GetString("tAdd");
				if (ViewState["SortFields"] == null || !(ViewState["SortFields"] is DataTable))
				{
					DataTable dt = new DataTable();
					dt.Columns.Add(new DataColumn("Field", typeof(string)));
					dt.Columns.Add(new DataColumn("FieldText", typeof(string)));
					dt.Columns.Add(new DataColumn("SortDirect", typeof(int))); //1 - asc, 0 - desc
					ViewState["SortFields"] = dt;
				}

				BindFields();
				BindDGFields();

				pastStep.Value = "5";
			}
			#endregion

			#region Step 6 - Settings
			if ((step == 6 && Elem.Value == "NULL") || (step == 5 && Elem.Value != "NULL"))
			{
				switch (rbReportItem.SelectedItem.Value)
				{
					case "0":	//Incident
						lblReportName.Text = LocRM.GetString("tIncRep");
						break;
					case "1":	//Project
						lblReportName.Text = LocRM.GetString("tPrjRep");
						break;
					case "2":	//ToDo`s
						lblReportName.Text = LocRM.GetString("tTdRep");
						break;
					case "3":	//Calendar Entries
						lblReportName.Text = LocRM.GetString("tCalEntrRep");
						break;
					case "4":	//Documents
						lblReportName.Text = LocRM.GetString("tAstRep");
						break;
					case "5":	//Users
						lblReportName.Text = LocRM.GetString("tUsrRep");
						break;
					case "6":	//Tasks
						lblReportName.Text = LocRM.GetString("tTkRep");
						break;
					case "7":	//Portfolios
						lblReportName.Text = LocRM.GetString("tPrjGrpRep");
						break;
					default:
						break;
				}
				lblReportName.Text += " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");

				if (iGroups.Value.Length > 1)
					trRadioGroup.Visible = true;
				else
					trRadioGroup.Visible = false;

				if (rblGrouptype.SelectedIndex == 1)
					imgType.ImageUrl = "~/layouts/images/pp1.gif";
				else
					imgType.ImageUrl = "~/layouts/images/pp2.gif";
				if (_pc["Cust_Rep_valMode"].ToString() != "Temp")
				{
					trTemplateSetsRow.Visible = false;
				}

				IBNReportTemplate repTemp = null;
				if (ResultXML.Value != "")
				{
					repTemp = IBNReportTemplate.Load(ResultXML.Value);
				}
				if (repTemp == null)
					repTemp = new IBNReportTemplate();

				#region Report Fields
				string sFields = iFields.Value;
				repTemp.Fields.Clear();
				while (sFields.Length > 0)
				{
					string sF = sFields.Substring(0, sFields.IndexOf(","));
					sFields = sFields.Remove(0, sFields.IndexOf(",") + 1);
					repTemp.Fields.Add(new FieldInfo(sF, _qItem.Fields[sF].DataType.ToString()));
				}
				#endregion

				#region Report Group Fields
				string sGroups = iGroups.Value;
				repTemp.Groups.Clear();
				while (sGroups.Length > 0)
				{
					string sG = sGroups.Substring(0, sGroups.IndexOf(","));
					sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
					repTemp.Groups.Add(new FieldInfo(sG, _qItem.Fields[sG].DataType.ToString()));
				}
				#endregion

				#region SortFields
				DataTable dt = (DataTable)ViewState["SortFields"];
				repTemp.Sorting.Clear();
				if (dt.Rows.Count > 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						SortInfo si = new SortInfo();
						string sS = dr["Field"].ToString();
						si.FieldName = sS;
						si.DataType = _qItem.Fields[sS].DataType.ToString();
						if (dr["SortDirect"].ToString() == "0")
							si.SortDirection = Mediachase.IBN.Business.Reports.SortDirection.DESC;
						else
							si.SortDirection = Mediachase.IBN.Business.Reports.SortDirection.ASC;
						repTemp.Sorting.Add(si);
					}
				}
				#endregion

				ResultXML.Value = repTemp.CreateXMLTemplate().InnerXml;

				pastStep.Value = "6";
			}
			#endregion

			#region Step 7 - Preview
			if ((step == 7 && Elem.Value == "NULL") || (step == 6 && Elem.Value != "NULL"))
			{
				IBNReportTemplate repTemp = null;
				if (ResultXML.Value != "")
				{
					repTemp = IBNReportTemplate.Load(ResultXML.Value);
				}
				if (repTemp == null)
					repTemp = new IBNReportTemplate();

				string sFilter = MakeFilterText(repTemp);

				#region Report Fields
				lblFields.Text = "";
				foreach (FieldInfo fi in repTemp.Fields)
					lblFields.Text += _qItem.Fields[fi.Name].FriendlyName + "<br>";
				#endregion

				#region Report Group Fields
				lblGroupFields.Text = "";
				foreach (FieldInfo fi in repTemp.Groups)
					lblGroupFields.Text += _qItem.Fields[fi.Name].FriendlyName + "<br>";
				#endregion

				#region SortFields
				DataTable dt = (DataTable)ViewState["SortFields"];
				lblSortFields.Text = "";
				if (dt.Rows.Count > 0)
					foreach (DataRow dr in dt.Rows)
						lblSortFields.Text += dr["FieldText"].ToString() + " - " + GetSortDirect((int)dr["SortDirect"]) + "<br>";
				#endregion

				txtReportTitle.Text = lblReportName.Text;
				//ResultXML.Value = repTemp.CreateXMLTemplate().InnerXml;

				if (!cbShowEmptyItems.Checked)
					sFilter += "&nbsp;&nbsp; " + LocRM.GetString("tDontShowEmptyGroupItem");

				lblFilter.Text = sFilter;
				pastStep.Value = "7";
			}
			#endregion

			#region Step 8 - Save & Redirect
			if ((step == 8 && Elem.Value == "NULL") || (step == 7 && Elem.Value != "NULL"))
			{
				IBNReportTemplate reportTemp = null;
				if (ResultXML.Value != "")
				{
					reportTemp = IBNReportTemplate.Load(ResultXML.Value);
				}
				if (reportTemp == null)
					reportTemp = new IBNReportTemplate();

				reportTemp.Version = IbnConst.VersionMajorDotMinor;
				reportTemp.Author = Security.CurrentUser.DisplayName;
				reportTemp.Created = UserDateTime.UserNow;
				reportTemp.Name = lblReportName.Text;
				if (reportTemp.Groups.Count > 0 && !cbShowEmptyItems.Checked)
					reportTemp.ShowEmptyGroup = false;
				else
					reportTemp.ShowEmptyGroup = true;
				if (rblGrouptype.SelectedIndex == 0)
					reportTemp.ViewType = "0"; //Old
				else
					reportTemp.ViewType = "1"; //New

				XmlDocument doc = new XmlDocument();

				switch (rbReportItem.SelectedValue)
				{
					case "0": //Incident
						reportTemp.ObjectName = "Incident";
						doc = Report.GetIncidentXMLReport(reportTemp,
							Security.CurrentUser.LanguageId.ToString(),
							Security.CurrentUser.TimeZoneId.ToString());
						break;
					case "1":	//Project
						reportTemp.ObjectName = "Project";
						doc = Report.GetProjectXMLReport(reportTemp,
							Security.CurrentUser.LanguageId.ToString(),
							Security.CurrentUser.TimeZoneId.ToString());
						break;
					case "2":	//ToDo`s
						reportTemp.ObjectName = "ToDo";
						doc = Report.GetToDoXMLReport(reportTemp,
							Security.CurrentUser.LanguageId.ToString(),
							Security.CurrentUser.TimeZoneId.ToString());
						break;
					case "3":	//Calendar Entries
						reportTemp.ObjectName = "Event";
						doc = Report.GetCalendarEntryXMLReport(reportTemp,
							Security.CurrentUser.LanguageId.ToString(),
							Security.CurrentUser.TimeZoneId.ToString());
						break;
					case "4":	//Documents
						reportTemp.ObjectName = "Document";
						doc = Report.GetDocumentXMLReport(reportTemp,
							Security.CurrentUser.LanguageId.ToString(),
							Security.CurrentUser.TimeZoneId.ToString());
						break;
					case "5":	//Users
						reportTemp.ObjectName = "Directory";
						doc = Report.GetUsersXMLReport(reportTemp,
							Security.CurrentUser.LanguageId.ToString(),
							Security.CurrentUser.TimeZoneId.ToString());
						break;
					case "6":	// Tasks
						reportTemp.ObjectName = "Task";
						doc = Report.GetTaskXMLReport(reportTemp,
							Security.CurrentUser.LanguageId.ToString(),
							Security.CurrentUser.TimeZoneId.ToString());
						break;
					case "7":	// Portfolios
						reportTemp.ObjectName = "Portfolio";
						doc = Report.GetPortfolioXMLReport(reportTemp,
							Security.CurrentUser.LanguageId.ToString(),
							Security.CurrentUser.TimeZoneId.ToString());
						break;
				}
				bool isTemporary = false;
				int iReportTemplateId = Report.CreateReportTemplate(txtReportTitle.Text, reportTemp.CreateXMLTemplate().InnerXml, !cbOnlyForMe.Checked, isTemporary);
				pastStep.Value = "7";
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"<script language='javascript'>" +
					string.Format("try {{window.opener.top.frames['right'].location.href='{0}';}}", this.ResolveUrl("~/Apps/ReportManagement/Pages/UserReport.aspx")) +
					"catch (e){}window.close();</script>");
			}
			#endregion

		}

		#region SortStep
		private void BindFields()
		{
			if (_qItem == null)
				BindQObject();
			string sFields = iFields.Value;
			ArrayList alFields = new ArrayList();
			while (sFields.Length > 0)
			{
				alFields.Add(sFields.Substring(0, sFields.IndexOf(",")));
				sFields = sFields.Remove(0, sFields.IndexOf(",") + 1);
			}

			lbFields.Items.Clear();
			foreach (string s_value in alFields)
			{
				QField field = _qItem.Fields[s_value];
				if ((field.UsingType & QFieldUsingType.Sort) == QFieldUsingType.Sort)
					lbFields.Items.Add(new ListItem(field.FriendlyName, field.Name));
			}
			DataTable dt = (DataTable)ViewState["SortFields"];

			foreach (string temp_s in alFields)
			{
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

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);

			if (!Page.IsPostBack)
				_pc["Cust_Rep_valMode"] = "0";

			InitializeVariables();
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgSortFields.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgSortFields_Delete);
			this.dlFilterFields.ItemCommand += new System.Web.UI.WebControls.DataListCommandEventHandler(this.dl_ItemCommand);
			this.btnPreview.Click += new EventHandler(btnPreview_Click);
		}
		#endregion

		#region Implementation of IWizardControl

		public int StepCount { get { return _stepCount; } }
		public string TopTitle
		{
			get
			{
				string sValMode = Request["Mode"].ToString();
				if (sValMode == "Temp")
					return LocRM.GetString("tTopTitle1");
				else
					return LocRM.GetString("tTopTitle");
			}
		}
		public bool ShowSteps { get { return true; } }
		public string Subtitle { get; private set; }
		public string MiddleButtonText { get; private set; }
		public string CancelText { get; private set; }

		public void SetStep(int stepNumber)
		{
			BindQObject();
			ShowStep(stepNumber);
			string tmp_title_str = "";
			if (Request["Type"] != null || stepNumber > 1)
			{
				switch (rbReportItem.SelectedItem.Value)
				{
					case "0":
						tmp_title_str = LocRM.GetString("tIncRep") + " - ";
						break;
					case "1":
						tmp_title_str = LocRM.GetString("tPrjRep") + " - ";
						break;
					case "2":
						tmp_title_str = LocRM.GetString("tTdRep") + " - ";
						break;
					case "3":
						tmp_title_str = LocRM.GetString("tCalEntrRep") + " - ";
						break;
					case "4":
						tmp_title_str = LocRM.GetString("tAstRep") + " - ";
						break;
					case "5":
						tmp_title_str = LocRM.GetString("tUsrRep") + " - ";
						break;
					case "6":
						tmp_title_str = LocRM.GetString("tTkRep") + " - ";
						break;
					case "7":
						tmp_title_str = LocRM.GetString("tPrjGrpRep") + " - ";
						break;
					default:
						break;
				}
			}
			Subtitle = tmp_title_str + (string)_subtitles[stepNumber - 1];
		}


		public string GenerateFinalStepScript()
		{
			return "window.close();";
		}

		public void CancelAction()
		{

		}
		#endregion

		#region BindQObject
		private void BindQObject()
		{
			if (rbReportItem.SelectedItem != null)
			{
				switch (rbReportItem.SelectedValue)
				{
					case "0":	//Incident
						_qItem = new QIncident();
						break;
					case "1":	//Project
						_qItem = new QProject();
						break;
					case "2":	//Todo`s
						_qItem = new QToDo();
						break;
					case "3":	//Events
						_qItem = new QCalendarEntries();
						break;
					case "4":	//Documents
						_qItem = new QDocument();
						break;
					case "5":	//Users
						_qItem = new QDirectory();
						break;
					case "6":	//Tasks
						_qItem = new QTask();
						break;
					case "7":	//Portfolios
						_qItem = new QPortfolio();
						break;
				}
			}
		}
		#endregion

		#region BindItemCollections
		private void BindItemCollections(ListItemCollection liColl, string elementType, int groupField)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("colName", typeof(string)));
			dt.Columns.Add(new DataColumn("colFrName", typeof(string)));
			DataRow dr;
			foreach (QField field in _qItem.Fields)
			{
				if (groupField == 0 && (field.UsingType & QFieldUsingType.Field) == QFieldUsingType.Field)
				{
					dr = dt.NewRow();
					dr["colName"] = field.Name;
					dr["colFrName"] = field.FriendlyName;
					dt.Rows.Add(dr);
				}
				if (groupField == 1 && (field.UsingType & QFieldUsingType.Grouping) == QFieldUsingType.Grouping)
				{
					dr = dt.NewRow();
					dr["colName"] = field.Name;
					dr["colFrName"] = field.FriendlyName;
					dt.Rows.Add(dr);
				}
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "colFrName";

			foreach (DataRowView dr1 in dv)
			{
				liColl.Add(new ListItem(dr1["colFrName"].ToString(), dr1["colName"].ToString()));
			}
		}
		#endregion

		#region BindFilter
		private void dl_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
		{
			string sName = e.CommandName;
			dlFilterFields.SelectedIndex = e.Item.ItemIndex;
			BindFilter(sName);
			string NewHref = dlFilterFields.ClientID + "_" + ((LinkButton)e.CommandSource).ClientID;
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script language='javascript'>window.location.href=window.location.href+'#" + NewHref + "';</script>");
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
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script language='javascript'>window.location.href=window.location.href+'#" + sName + "';</script>");
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
			if (_qItem == null)
				BindQObject();

			#region PastStep
			if (pastCommand.Value != "")
			{
				QField qPast = _qItem.Fields[pastCommand.Value];
				QDictionary qDicPast = _qItem.GetDictionary(qPast);
				bool isCheck = false;
				DataRow[] dr = dtFilters.Select("FieldName = '" + qPast.Name + "'");
				if (dr.Length > 0)
					isCheck = (bool)dr[0]["IsChecked"];
				FilterInfo fi = new FilterInfo();
				fi.FieldName = qPast.Name;
				fi.DataType = qPast.DataType.ToString();
				object curValue = null;

				if (qDicPast != null)
				{
					curValue = dictFltr.Value;
					if (curValue != null)
					{
						foreach (string sValue in (string[])curValue)
							fi.Values.Add(sValue);
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
				if (isCheck)
				{
					int index = repTemp.Filters.IndexOf(repTemp.Filters[fi.FieldName]);
					if (index >= 0)
					{
						repTemp.Filters.RemoveAt(index);
						repTemp.Filters.Insert(index, fi);
					}
					else
						repTemp.Filters.Add(fi);
				}
				else
				{
					int index = repTemp.Filters.IndexOf(repTemp.Filters[fi.FieldName]);
					if (index >= 0)
					{
						repTemp.Filters.RemoveAt(index);
					}
					else if (fi.Values.Count > 0)
					{
						dr[0]["IsChecked"] = true;
						repTemp.Filters.Add(fi);
					}
				}
			}
			#endregion

			#region CurrentStep
			if (command != "")
			{
				DataRow[] dr = dtFilters.Select("FieldName = '" + command + "'");
				bool isCheckCur = false;
				if (dr.Length > 0)
					isCheckCur = (bool)dr[0]["IsChecked"];

				QField qField = _qItem.Fields[command];
				QDictionary qDict = _qItem.GetDictionary(qField);

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
					if (flt != null && isCheckCur)
					{
						switch (control.FilterType)
						{
							case "Dictionary":
								ArrayList alValue = new ArrayList();
								foreach (string sValue in flt.Values)
									alValue.Add(sValue);
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
					else if (flt == null && isCheckCur)
					{
						control.Value = null;
						FilterInfo fi = new FilterInfo();
						fi.FieldName = qField.Name;
						fi.DataType = qField.DataType.ToString();
						repTemp.Filters.Add(fi);
					}
					else if (flt != null && !isCheckCur)
					{
						control.Value = null;
						int index = repTemp.Filters.IndexOf(flt);
						repTemp.Filters.RemoveAt(index);
					}
				}
			}
			#endregion

			ResultXML.Value = repTemp.CreateXMLTemplate().InnerXml;
			ViewState["FiltersTable"] = dtFilters;

			DataView dvFilters = dtFilters.DefaultView;
			dvFilters.Sort = "FriendlyName";
			dlFilterFields.DataSource = dvFilters;
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
			foreach (FilterInfo fi in repTemp.Filters)
			{
				QField qTemp = _qItem.Fields[fi.FieldName];
				QDictionary qDTemp = _qItem.GetDictionary(qTemp);
				if (qDTemp != null)
				{
					retval += qTemp.FriendlyName + "&nbsp;=&nbsp;";
					string sqlCommand = qDTemp.GetSQLQuery(Security.CurrentUser.LanguageId);
					using (IDataReader reader = Report.GetQDictionary(sqlCommand))
					{
						ArrayList alDicVal = new ArrayList();
						foreach (string sValue in fi.Values)
						{
							alDicVal.Add(sValue);
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
										retval += "&nbsp;=&nbsp;<font color='red'>" + LocRM.GetString("tToday") + "</font><br>";
										break;
									case "2":
										retval += "&nbsp;=&nbsp;<font color='red'>" + LocRM.GetString("tYesterday") + "</font><br>";
										break;
									case "3":
										retval += "&nbsp;=&nbsp;<font color='red'>" + LocRM.GetString("tThisWeek") + "</font><br>";
										break;
									case "4":
										retval += "&nbsp;=&nbsp;<font color='red'>" + LocRM.GetString("tLastWeek") + "</font><br>";
										break;
									case "5":
										retval += "&nbsp;=&nbsp;<font color='red'>" + LocRM.GetString("tThisMonth") + "</font><br>";
										break;
									case "6":
										retval += "&nbsp;=&nbsp;<font color='red'>" + LocRM.GetString("tLastMonth") + "</font><br>";
										break;
									case "7":
										retval += "&nbsp;=&nbsp;<font color='red'>" + LocRM.GetString("tThisYear") + "</font><br>";
										break;
									case "8":
										retval += "&nbsp;=&nbsp;<font color='red'>" + LocRM.GetString("tLastYear") + "</font><br>";
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

		private void btnPreview_Click(object sender, EventArgs e)
		{
			IBNReportTemplate reportTemp = null;

			#region Create ReportTemp
			if (ResultXML.Value != "")
			{
				reportTemp = IBNReportTemplate.Load(ResultXML.Value);
			}
			if (reportTemp == null)
				reportTemp = new IBNReportTemplate();

			reportTemp.Version = IbnConst.VersionMajorDotMinor;
			reportTemp.Author = Security.CurrentUser.DisplayName;
			reportTemp.Created = UserDateTime.UserNow;
			reportTemp.Name = lblReportName.Text;
			if (reportTemp.Groups.Count > 0 && !cbShowEmptyItems.Checked)
				reportTemp.ShowEmptyGroup = false;
			else
				reportTemp.ShowEmptyGroup = true;
			if (rblGrouptype.SelectedIndex == 0)
				reportTemp.ViewType = "0"; //Old
			else
				reportTemp.ViewType = "1"; //New

			XmlDocument doc = new XmlDocument();

			switch (rbReportItem.SelectedValue)
			{
				case "0": //Incident
					reportTemp.ObjectName = "Incident";
					doc = Report.GetIncidentXMLReport(reportTemp,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "1":	//Project
					reportTemp.ObjectName = "Project";
					doc = Report.GetProjectXMLReport(reportTemp,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "2":	//ToDo`s
					reportTemp.ObjectName = "ToDo";
					doc = Report.GetToDoXMLReport(reportTemp,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "3":	//Calendar Entries
					reportTemp.ObjectName = "Event";
					doc = Report.GetCalendarEntryXMLReport(reportTemp,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "4":	//Documents
					reportTemp.ObjectName = "Document";
					doc = Report.GetDocumentXMLReport(reportTemp,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "5":	//Users
					reportTemp.ObjectName = "Directory";
					doc = Report.GetUsersXMLReport(reportTemp,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "6":	// Tasks
					reportTemp.ObjectName = "Task";
					doc = Report.GetTaskXMLReport(reportTemp,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
				case "7":	// Portfolios
					reportTemp.ObjectName = "Portfolio";
					doc = Report.GetPortfolioXMLReport(reportTemp,
						Security.CurrentUser.LanguageId.ToString(),
						Security.CurrentUser.TimeZoneId.ToString());
					break;
			}
			#endregion

			int iReportTemplateId = Report.CreateReportTemplate(lblReportName.Text,
				reportTemp.CreateXMLTemplate().InnerXml, !cbOnlyForMe.Checked, true);
			int iReportId = Report.CreateReportByTemplate(iReportTemplateId, doc.InnerXml);
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script language='javascript'>OpenWindow('../Reports/XMLReportOutput.aspx?Mode=true&Refresh=-1&ReportId=" + iReportId.ToString() + "',screen.width,screen.height,true);</script>");
		}
	}
}
