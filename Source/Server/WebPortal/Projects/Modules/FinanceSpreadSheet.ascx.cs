namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Resources;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.SpreadSheet;
	using Mediachase.UI.Web.Modules;

	using ComponentArt.Web.UI;
	using System.Globalization;



	/// <summary>
	///		Summary description for FinancesSpreadSheet.
	/// </summary>
	public partial class FinanceSpreadSheet : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectFinances", typeof(FinanceSpreadSheet).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(FinanceSpreadSheet).Assembly);

		protected System.Web.UI.WebControls.Label lblDescription;
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		private int projType;
		
		#region ProjectId
		private int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		#region Bind: User Values
		private void BindUserValues()
		{
			if (pc["SpreadSheet_From" + ProjectId.ToString()] != null)
				tbFrom.Text = pc["SpreadSheet_From" + ProjectId.ToString()].ToString();
			if (pc["SpreadSheet_To" + ProjectId.ToString()] != null)
				tbTo.Text = pc["SpreadSheet_To" + ProjectId.ToString()].ToString();
			if (pc["SpreadSheet_dd1" + ProjectId.ToString()] != null && ddPrimarySheet.Items.FindByValue(pc["SpreadSheet_dd1" + ProjectId.ToString()].ToString()) != null)
				ddPrimarySheet.SelectedValue = pc["SpreadSheet_dd1" + ProjectId.ToString()].ToString();
			if (pc["SpreadSheet_dd2" + ProjectId.ToString()] != null && ddSecondarySheet.Items.FindByValue(pc["SpreadSheet_dd2" + ProjectId.ToString()].ToString()) != null)
				ddSecondarySheet.SelectedValue = pc["SpreadSheet_dd2" + ProjectId.ToString()].ToString();
		}
		#endregion

		#region Set: User Values
		private void SetUserValues()
		{
			pc["SpreadSheet_From" + ProjectId.ToString()] = tbFrom.Text;
			pc["SpreadSheet_To" + ProjectId.ToString()] = tbTo.Text;
			pc["SpreadSheet_dd1" + ProjectId.ToString()] = ddPrimarySheet.SelectedValue;
			pc["SpreadSheet_dd2" + ProjectId.ToString()] = ddSecondarySheet.SelectedValue;
			pc["SpreadSheet_DocumentType" + ProjectId.ToString()] = projType.ToString();
		}
		#endregion

		#region ApplyLocalization
		void ApplyLocalization()
		{
			cvMinYear.ErrorMessage = LocRM.GetString("YearLimit");
			cvMaxYear.ErrorMessage = LocRM.GetString("YearLimit");
			btnApplyFilter.Text = LocRM.GetString("Apply");
			btnCancel.Text = LocRM.GetString("tCancel");
			btnActivate.Text = LocRM.GetString("Activate");
			btnDeactivateFinance.Text = LocRM.GetString("ReactivateText");
		}
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			divGrid.Visible = false;
			divActivate.Visible = false;

			BindHeader();
			ApplyLocalization();

			btnDeactivate.Click += new EventHandler(btnDeactivate_Click);
			btnDeactivateFinance.ServerClick += new EventHandler(btnDeactivateFinance_ServerClick);
			btnDeactivateFinance.Attributes.Add("onclick", String.Format("if(!confirm('{0}')) return false;", LocRM.GetString("ReactivateMsg")));
			btnApplyFilter.ServerClick += new EventHandler(btnApplyFilter_ServerClick);
			btnDeactivateFinance.CustomImage = this.Page.ResolveUrl("~/layouts/images/card-delete.gif");
			btnApplyFilter.CustomImage = this.Page.ResolveUrl("~/layouts/images/accept.gif");
			btnActivate.CustomImage = this.Page.ResolveUrl("~/layouts/images/accept.gif");
			btnCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");

			if (!IsPostBack)
				BindDropDowns();

			if (ProjectSpreadSheet.IsActive(int.Parse(this.Request["ProjectId"])))
			{
				divGrid.Visible = true;
				if (!IsPostBack)
				{
					GenerateScript();
				}
				// Put user code to initialize the page here
				AddOnkeydownAttribute();
				//AddOnMouseClickListener();
			}
			else
			{
				divActivate.Visible = true;

			}
			BindToolbar();
		}

		private void BindToolbar()
		{
			if (this.Parent.Parent is IToolbarLight)
			{
				bool projectSpreadSheetIsActive = ProjectSpreadSheet.IsActive(int.Parse(this.Request["ProjectId"]));
				if (projectSpreadSheetIsActive)
				{
					BlockHeaderLightWithMenu secHeaderLight = (BlockHeaderLightWithMenu)((IToolbarLight)this.Parent.Parent).GetToolBar();
					secHeaderLight.AddRightLink("<img alt='' src='../Layouts/Images/icons/xlsexport.gif'/> " + LocRM2.GetString("ExcelExport"), "../Projects/ProjectBSExport.aspx?" +
						"ProjectId=" + ProjectId.ToString() + "&BasePlan1=" + pc["SpreadSheet_dd1" + ProjectId.ToString()] +
						"&BasePlan2=" + pc["SpreadSheet_dd2" + ProjectId.ToString()] + "&FromYear=" + pc["SpreadSheet_From" + ProjectId.ToString()] +
						"&ToYear=" + pc["SpreadSheet_To" + ProjectId.ToString()] + "&FinanceType=" + pc["SpreadSheet_DocumentType" + ProjectId.ToString()]);
				}
			}
		}


		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			SetUserValues();
		}
		#endregion

		#region BindDropDowns
		void BindDropDowns()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));

			dt.Rows.Add(new object[] { 1, LocRM.GetString("tWeekYear") });
			dt.Rows.Add(new object[] { 2, LocRM.GetString("tMonthQuarterYear") });
			dt.Rows.Add(new object[] { 3, LocRM.GetString("tQuarterYear") });
			dt.Rows.Add(new object[] { 4, LocRM.GetString("tYear") });
			dt.Rows.Add(new object[] { 5, LocRM.GetString("tTotal") });

			ArrayList list = new ArrayList(SpreadSheetTemplateInfo.List());
			list.Sort(new SortComparer());
			ddTemplate.DataSource = list; //SpreadSheetTemplateInfo.List();
			ddTemplate.DataTextField = "Name";
			ddTemplate.DataValueField = "FileName";
			ddTemplate.DataBind();

			ddDocType.DataSource = dt;
			ddDocType.DataTextField = "Name";
			ddDocType.DataValueField = "Id";
			ddDocType.DataBind();

			ddDocType.SelectedValue = "2";

			DataTable dt2 = new DataTable();
			dt2.Columns.Add(new DataColumn("Id", typeof(int)));
			dt2.Columns.Add(new DataColumn("Name", typeof(string)));

			foreach (BasePlan bp in BasePlan.List(ProjectId))
			{
				DataRow row = dt2.NewRow();
				row["Id"] = bp.BasePlanSlotId;
				row["Name"] = String.Format("{0} ({1}: {2})", BasePlanSlot.Load(bp.BasePlanSlotId).Name, LocRM.GetString("LastSaved"), bp.Created);
				dt2.Rows.Add(row);
			}
			dt2.Rows.Add(new object[] { -1, LocRM.GetString("Fact") });
			dt2.Rows.Add(new object[] { 0, LocRM.GetString("Current") });

			DataView dv = dt2.DefaultView;
			dv.Sort = "Id ASC";


			ddPrimarySheet.DataSource = dv;
			ddPrimarySheet.DataTextField = "Name";
			ddPrimarySheet.DataValueField = "Id";
			ddPrimarySheet.DataBind();
			ddPrimarySheet.SelectedValue = "0";

			dt2.Rows.Add(new object[] { -2, LocRM.GetString("NotSelected") });

			ddSecondarySheet.DataSource = dv;
			ddSecondarySheet.DataTextField = "Name";
			ddSecondarySheet.DataValueField = "Id";
			ddSecondarySheet.DataBind();
			ddSecondarySheet.SelectedValue = "-2";

			tbFrom.Text = DateTime.Now.Year.ToString();
			tbTo.Text = DateTime.Now.Year.ToString();

			BindUserValues();
		}
		#endregion

		#region Bind: Header
		void BindHeader()
		{
			//secHeader.AddText(LocRM.GetString("tPrjFinance"));
			//activateSettings.AddText(LocRM.GetString("ActivateSettings"));

		}
		#endregion

		#region GenerateScaleUpDownScript
		private void GenerateScaleUpDownScript(SpreadSheetView view)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<script language=JavaScript>");
			sb.Append("function GridScaleUpDown2(newlength2) {  popup_show(); ");
			//Clear TD selection
			sb.Append("var obj = document.getElementById('span-link-month'); if (obj != null) obj.className='dhtmlLink_Passive'; ");
			sb.Append("obj = document.getElementById('span-link-quartal'); if (obj != null) obj.className='dhtmlLink_Passive'; ");
			sb.Append("obj = document.getElementById('span-link-year'); if (obj != null) obj.className='dhtmlLink_Passive'; ");
			sb.Append("obj = document.getElementById('span-link-week'); if (obj != null) obj.className='dhtmlLink_Passive'; ");
			sb.Append("obj = document.getElementById('span-link-project'); if (obj != null) obj.className='dhtmlLink_Passive'; ");

			sb.Append("newColumnLength = newlength2; setTimeout(\"GridScaleUpDown(newColumnLength);\", 100); }");
			sb.Append("function GridScaleUpDown(newlength) { ");

			sb.Append("if (gridScale != null && gridScale != 'undefined') { ");

			#region ShowHideToalForProject
			sb.Append("if (gridScale == -1) {");
			for (int i = 2; i <= view.Columns.Length; i++)
			{
				sb.AppendFormat("mygrid.setColWidth({0},newlength); ", i);
			}
			sb.Append("popup_hide(); return;");
			sb.Append("}");
			#endregion

			#region MonthQuarterYear
			if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
			{
				//Month <-> Quartal
				sb.Append("if((gridScale == 0 && newlength == 0) || (gridScale == 1 && newlength > 0)) { ");
				for (int i = 2; i < view.Columns.Length; i++)
				{
					int counter = (i - 2) % 17;
					if ((counter + 1) % 4 == 0 || counter > 14) continue;
					sb.AppendFormat("mygrid.setColWidth({0},newlength);", i);
				}
				sb.Append("if (newlength == 0) {gridScale++; } else {gridScale--; } popup_hide(); return;}"); // gridScale == 0

				//Quartal <-> Year
				sb.Append("if ((gridScale == 1 && newlength == 0) || (gridScale == 2 && newlength > 0)) {");
				for (int i = 2; i < view.Columns.Length; i++)
				{
					int counter = (i - 2) % 17;
					if ((counter + 1) % 4 == 0 || counter < 15)
						sb.AppendFormat("mygrid.setColWidth({0},newlength); ", i);
				}
				sb.Append("if (newlength == 0) {gridScale++; } else {gridScale=0; } popup_hide(); return;}"); //gridScale == 1


			}
			#endregion

			#region WeekYear
			if (view.Document.DocumentType == SpreadSheetDocumentType.WeekYear)
			{
				sb.Append("if ((gridScale == 0 && newlength == 0) || (gridScale == 1 && newlength > 0)) {");
				for (int i = 2; i < view.Columns.Length; i++)
				{
					int counter = (i - 2) % 53;
					if (counter < 52)
						sb.AppendFormat("mygrid.setColWidth({0},newlength); ", i);
				}
				sb.Append("if (newlength == 0) {gridScale++; } else {gridScale--; }  popup_hide(); return;}");
			}
			#endregion

			#region QuarterYear
			if (view.Document.DocumentType == SpreadSheetDocumentType.QuarterYear)
			{
				sb.Append("if ((gridScale == 0 && newlength == 0) || (gridScale == 1 && newlength > 0)) {");
				for (int i = 2; i < view.Columns.Length; i++)
				{
					int counter = (i - 2) % 5;
					if (counter < 4)
						sb.AppendFormat("mygrid.setColWidth({0},newlength); ", i);
				}
				sb.Append("if (newlength == 0) {gridScale++; } else {gridScale--; } popup_hide(); return;}");
			}
			#endregion



			sb.Append("popup_hide();} "); // gridScale != null

			sb.Append("}"); // function
			sb.Append("</script>");

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), sb.ToString());
		}
		#endregion

		#region GenerateScript
		void GenerateScript()
		{
			SpreadSheetView view = ProjectSpreadSheet.LoadView(int.Parse(this.Request["ProjectId"]), 0, int.Parse(tbFrom.Text), int.Parse(tbTo.Text));
			projType = (int)view.Document.DocumentType;

			int colCount = view.Columns.Length;
			int YearFrom = int.Parse(tbFrom.Text);
			int YearTo = int.Parse(tbTo.Text);
			StringBuilder sb = new StringBuilder();
			sb.Append("<script type='text/javascript'>");
			sb.Append("if (mygrid == null) mygrid = new dhtmlXGridObject('gridbox');");

			#region Bind: Header Caption
			if (view.Document.DocumentType != SpreadSheetDocumentType.Total && view.Document.DocumentType != SpreadSheetDocumentType.Year)
			{
				// Generate procedure for scale grid
				GenerateScaleUpDownScript(view);

				#region Generate Html for scaling
				StringBuilder scaleHtml = new StringBuilder();
				scaleHtml.Append("<div style='margin: 2px; padding: 2px;'>");
				scaleHtml.Append("<table class='text' cellspacing='2' cellpadding='0'><tr>");
				scaleHtml.AppendFormat("<td valign='top'><img alt='' src='{0}' border='0'/>&nbsp;</td>", ResolveUrl("~/Layouts/Images/zoomin.gif"));
				string disableKeyboard = ((bool)(ddPrimarySheet.SelectedValue != "0")).ToString().ToLower();
				if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
				{
					scaleHtml.AppendFormat("<td id='span-link-month' class='dhtmlLink_Active' onclick='gridScale=-1; GridScaleUpDown2(110); DisableSelection = {1}; this.className=\"dhtmlLink_Active\";'>&nbsp;{0}&nbsp;</td><td class='ibn-separator'>|</td>", LocRM.GetString("tMonthQuarterYear"), disableKeyboard);
					scaleHtml.AppendFormat("<td id='span-link-quartal' class='dhtmlLink_Passive' onclick='gridScale=2; GridScaleUpDown(110); gridScale=0; GridScaleUpDown2(0); DisableSelection = true; this.className=\"dhtmlLink_Active\";'>&nbsp;{0}&nbsp;</td><td class='ibn-separator'>|</td>", LocRM.GetString("tQuarterYear"));
					scaleHtml.AppendFormat("<td id='span-link-year' class='dhtmlLink_Passive' onclick='gridScale=-1; GridScaleUpDown(110); gridScale=1; GridScaleUpDown2(0); DisableSelection = true; this.className=\"dhtmlLink_Active\"; '>&nbsp;{0}&nbsp;</td><td class='ibn-separator'>|</td>", LocRM.GetString("tYear"));
					scaleHtml.AppendFormat("<td id='span-link-project' class='dhtmlLink_Passive' onclick='gridScale=-1; GridScaleUpDown2(0); DisableSelection = true; this.className=\"dhtmlLink_Active\"; '>&nbsp;{0}&nbsp;</td>", LocRM.GetString("tTotal"));
				}
				else
					if (view.Document.DocumentType == SpreadSheetDocumentType.WeekYear)
					{
						scaleHtml.AppendFormat("<td id='span-link-week' class='dhtmlLink_Active' onclick='gridScale=-1; GridScaleUpDown2(110); DisableSelection = {1}; this.className=\"dhtmlLink_Active\";'>&nbsp;{0}&nbsp;</td><td class='ibn-separator'>|</td>", LocRM.GetString("tWeekYear"), disableKeyboard);
						scaleHtml.AppendFormat("<td id='span-link-year' class='dhtmlLink_Passive' onclick='gridScale=-1; GridScaleUpDown(110); gridScale=0; GridScaleUpDown2(0); DisableSelection = true; this.className=\"dhtmlLink_Active\"; '>&nbsp;{0}&nbsp;</td><td class='ibn-separator'>|</td>", LocRM.GetString("tYear"));
						scaleHtml.AppendFormat("<td id='span-link-project' class='dhtmlLink_Passive' onclick='gridScale=-1; GridScaleUpDown2(0); DisableSelection = true; this.className=\"dhtmlLink_Active\"; '>&nbsp;{0}&nbsp;</td>", LocRM.GetString("tTotal"));
					}
					else
						if (view.Document.DocumentType == SpreadSheetDocumentType.QuarterYear)
						{
							scaleHtml.AppendFormat("<td id='span-link-quartal' class='dhtmlLink_Active' onclick='gridScale=-1; GridScaleUpDown2(110); DisableSelection = {1}; this.className=\"dhtmlLink_Active\";'>&nbsp;{0}&nbsp;</td><td class='ibn-separator'>|</td>", LocRM.GetString("tQuarterYear"), disableKeyboard);
							scaleHtml.AppendFormat("<td id='span-link-year' class='dhtmlLink_Passive' onclick='gridScale=-1; GridScaleUpDown(110); gridScale=0; GridScaleUpDown2(0); DisableSelection = true; this.className=\"dhtmlLink_Active\"; '>&nbsp;{0}&nbsp;</td><td class='ibn-separator'>|</td>", LocRM.GetString("tYear"));
							scaleHtml.AppendFormat("<td id='span-link-project' class='dhtmlLink_Passive' onclick='gridScale=-1; GridScaleUpDown2(0); DisableSelection = true; this.className=\"dhtmlLink_Active\"; '>&nbsp;{0}&nbsp;</td>", LocRM.GetString("tTotal"));
						}
				scaleHtml.Append("</tr></table>");
				scaleHtml.Append("</div>");
				#endregion

				//Absolute panel for scale up/down finance grid
				panelScale.InnerHtml = scaleHtml.ToString();

				sb.AppendFormat("mygrid.setHeader(\"{0},&nbsp;", LocRM.GetString("tGridHeader1"));
				int YearCounter = YearTo - YearFrom + 1;
				int ColSpanRate = -1;

				#region Year
				if (view.Document.DocumentType == SpreadSheetDocumentType.QuarterYear)
					ColSpanRate = 5;
				else if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
					ColSpanRate = 17;
				else if (view.Document.DocumentType == SpreadSheetDocumentType.WeekYear)
					ColSpanRate = 53;

				int RowSpanRate = (view.Columns.Length - YearCounter) / (YearCounter * 4);
				//int RowSpanRate = 45;
				int yearCount = 0;

				for (int i = 0; i < view.Columns.Length - 1; i++)
				{
					if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
					{
						if (i % (ColSpanRate) == 0)
							sb.AppendFormat(",{0}", YearFrom + yearCount);
						else if (((i - yearCount + 1) % (ColSpanRate - 1) == 0) && (i - yearCount + 1 != 0))
							sb.AppendFormat(",{0} {1}", LocRM.GetString("tYearTotal"), YearFrom + yearCount - 1);
						else
							sb.Append(",#cspan");
					}
					else
					{
						if (i % (ColSpanRate) == 0)
							sb.AppendFormat(",{0}", YearFrom + yearCount);
						else if (((i - yearCount + 1) % (ColSpanRate - 1) == 0) && (i - yearCount + 1 != 0))
							sb.AppendFormat(",{0} {1}", LocRM.GetString("tYearTotal"), YearFrom + yearCount - 1);
						else
							sb.Append(",#cspan");
					}
					if (i % (ColSpanRate) == 0)
						yearCount++;
				}
				//}
				sb.AppendFormat(",{0}\");", LocRM.GetString("tProjectTotal"));
				#endregion

				#region Quartals
				if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
				{
					sb.Append("mygrid.attachHeader([\"#rspan\", \"#rspan\"");
					for (int i = 0; i < view.Columns.Length - 1; i++)
					{
						int counter = i % ColSpanRate;

						if (counter % (RowSpanRate) == 0 && counter != 16)
							sb.AppendFormat(",\"{0} {1}\"", LocRM.GetString("tQuarterYear"), /*(((i + 1) / RowSpanRate) % 4) + 1*/ (counter / 4) + 1);
						else
							if ((counter + 1) % RowSpanRate == 0)
								sb.AppendFormat(", \"{0}{1}\"", LocRM.GetString("tQuartalTotal"), (counter / 4) + 1);
							else
								if (counter == 16)
									sb.Append(",\"#rspan\"");

								else
									sb.Append(",\"#cspan\"");
					}
					sb.Append(",\"#rspan\"]);");
				}
				#endregion

				#region BindServerData
				sb.AppendFormat("mygrid.attachHeader([\"#rspan\",\"#rspan\", \"{0}\"", ((Column)view.Columns[0]).Name);
				int _counter = 0;
				for (int i = 1; i < view.Columns.Length - 1; i++)
				{

					if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
					{
						if (i % ColSpanRate == 0) _counter++;
						// Total for quartal
						if ((i + 1 - _counter) % RowSpanRate == 0)
							sb.Append(",\"#rspan\"");
						else
							//Total for year
							if ((i + 1) % ColSpanRate == 0)
								sb.Append(",\"#rspan\"");
							else
								sb.AppendFormat(",\" {0}\"", ((Column)view.Columns[i]).Name);
					}
					else
					{
						if ((i + 1) % ColSpanRate == 0)
							sb.Append(",\"#rspan\"");
						else
							sb.AppendFormat(",\" {0}\"", ((Column)view.Columns[i]).Name);
					}
				}
				sb.Append(",\"#rspan\"]);");
				#endregion
			}
			else
			{
				//for scales: Year, Project Total
				sb.AppendFormat("mygrid.setHeader(\"{1},&nbsp;, {0}", ((Column)view.Columns[0]).Name, LocRM.GetString("tGridHeader1"));

				for (int i = 1; i < view.Columns.Length; i++)
				{
					sb.AppendFormat(", {0}", ((Column)view.Columns[i]).Name);
				}
				sb.Append("\");");
			}
			#endregion

			#region Bind: Header Width
			sb.AppendFormat("mygrid.setImagePath('{0}');", ResolveUrl("~/Layouts/Images/dhtmlGrid/imgs"));

			sb.AppendFormat("mygrid.setInitWidths(\"120,25,{0}", 110/*(int)(maxWidth / view.Columns.Length)*/);
			for (int i = 1; i < view.Columns.Length; i++)
			{
				sb.AppendFormat(",{0}", 110/*(int)(maxWidth / view.Columns.Length)*/);
			}
			sb.Append("\");");
			#endregion

			#region Bind: Header Align
			sb.Append("mygrid.setColAlign(\"left,left,right");
			for (int i = 1; i < view.Columns.Length; i++)
			{
				sb.Append(",right");
			}
			sb.Append("\");");
			#endregion

			#region Bind: Cols CanEdit

			if (ddPrimarySheet.SelectedValue != "0")
				sb.Append("mygrid.setColTypes(\"tree,ro,ro");
			else
				sb.Append("mygrid.setColTypes(\"tree,ro,ed");
			for (int i = 1; i < colCount; i++)
			{
				if (ddPrimarySheet.SelectedValue != "0")
					sb.Append(",ro");
				else
					sb.Append(",ed");
			}
			sb.Append("\");");
			#endregion

			#region Bind: Cols Color
			sb.Append("mygrid.setColumnColor(\"#E1ECFC,#E1ECFC");
			for (int i = 1; i < colCount; i++)
			{
				sb.Append(",white");
			}
			sb.Append("\");");
			#endregion

			sb.Append("gridScale = 0;");
			sb.Append(" mygrid.enableKeyboardSupport(false); mygrid.enableDragAndDrop(true); mygrid.setDragBehavior('sibling'); mygrid.init(); ");
			sb.Append("mygrid.setDragHandler(drag_f); mygrid.setDropHandler(drop_f);");


			if (ddSecondarySheet.SelectedValue != "-2")
			{
				sb.AppendFormat("qstring2='&BasePlanSlotId1={0}&BasePlanSlotId2={1}&FromYear={2}&ToYear={3}&compare=1';", ddPrimarySheet.SelectedValue, ddSecondarySheet.SelectedValue, tbFrom.Text, tbTo.Text);
				sb.AppendFormat("var xmlurl = '{0}?ProjectId={1}&ProjectFinance=1'+qstring2;", ResolveUrl("~/Modules/XmlForTreeView.aspx"), ProjectId);
				sb.AppendFormat("mygrid.loadXML(xmlurl);", ResolveUrl("~/Modules/XmlForTreeView.aspx"), ProjectId);
				//sb.Append("function({HighlightCell(null, -1, mygrid.getRowId(0), 2);});");
				sb.Append("compareMode = 1;");
			}
			else
			{
				sb.AppendFormat("qstring2='&BasePlanSlotId1={0}&BasePlanSlotId2={1}&FromYear={2}&ToYear={3}';", ddPrimarySheet.SelectedValue, ddSecondarySheet.SelectedValue, tbFrom.Text, tbTo.Text);
				sb.AppendFormat("var xmlurl = '{0}?ProjectId={1}&ProjectFinance=1'+qstring2;", ResolveUrl("~/Modules/XmlForTreeView.aspx"), ProjectId);
				sb.AppendFormat("mygrid.loadXML(xmlurl);", ResolveUrl("~/Modules/XmlForTreeView.aspx"), ProjectId);
				//sb.Append("function({HighlightCell(null, -1, mygrid.getRowId(0), 2);});");
				sb.Append("compareMode = 0;");
			}
			//sb.Append("mygrid.attachHeader([\"#rspan\", \"#rspan\", \"TEST MF\", \"#cspan\", \"T2\", \"_\", \"_\", \"T3\", \"_\", \"_\", \"_\", \"_\", \"_\", \"_\", \"_\", \"_\", \"_\", \"_\", \"_\", \"_\"]);");
			//sb.Append("mygrid.setSizes();");

			if (ddPrimarySheet.SelectedValue == "0")
			{
				sb.Append("DisableSelection = false;");
				sb.Append(" setTimeout(\"HighlightCell(null,-1,mygrid.getRowId(0), 2)\", 500); ");
			}
			else
			{
				sb.Append("DisableSelection = true;");
			}
			sb.Append("mygrid.setOnEditCellHandler(GridCellEdit); ");

			//DV: Fix bug with new doctype
			sb.Append("window.setTimeout(function () ");
			sb.Append("{ if (document.getElementById('gridbox')) { ");
			sb.Append("	document.getElementById('gridbox').style.height = '399px'; ");
			sb.Append("	window.setTimeout(function() { document.getElementById('gridbox').style.height = '400px'; }, 1000);");
			sb.Append("	}");
			sb.Append("}, 100);");

			sb.Append("</script>");

			//Page.RegisterClientScriptBlock(); 
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), sb.ToString());
		}
		#endregion

		#region Add onkeydown attribute
		public void AddOnkeydownAttribute()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<script language=JavaScript> ");
			/*
			sb.Append(" if(document.body.getAttribute('onkeydown')==null) ");
			sb.Append("{document.body.setAttribute(\"onkeydown\",\"javascript:ProcessPressedKey(event)\");} ");
			sb.Append("else ");
			sb.Append("{document.body.setAttribute(\"onkeydown\",document.body.getAttribute('onkeydown')+\" ProcessPressedKey(event)\");} ");
			*/
			sb.Append("if (document.addEventListener) document.addEventListener(\"keydown\",new Function(\"e\",\" return ProcessPressedKey(e);\"),false); ");
			sb.Append("else if (document.attachEvent) document.attachEvent(\"onkeydown\",new Function(\"e\",\" return ProcessPressedKey(e);\")); ");
			sb.Append(" </script>");
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), sb.ToString());
		}
		#endregion

		#region AddOnMouseClickListener
		public void AddOnMouseClickListener()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<script language=JavaScript> ");
			sb.Append("if (mygrid != null) { if (mygrid.addEventListener) mygrid.addEventListener(\"click\",new Function(\"e\",\"  return ProcessMouseClick(e);\"),false); ");
			sb.Append("else if (mygrid.attachEvent) mygrid.attachEvent(\"onclick\",new Function(\"e\",\" return ProcessMouseClick(e);\")); }");
			sb.Append(" </script>");
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), sb.ToString());
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
			this.btnActivate.ServerClick += new EventHandler(btnActivate_ServerClick);
		}
		#endregion

		#region btnActivate_ServerClick
		private void btnActivate_ServerClick(object sender, EventArgs e)
		{
			ProjectSpreadSheet.Activate(int.Parse(Request["ProjectId"]), (SpreadSheetDocumentType)int.Parse(ddDocType.SelectedValue), ddTemplate.SelectedValue);

			#region tmpversion
			/*DataTable dt = Finance.GetListActualFinancesByProject(int.Parse(Request["ProjectId"]));
			if (dt.Rows.Count > 0)
			{
				SpreadSheetView view = ProjectSpreadSheet.LoadView(int.Parse(Request["ProjectId"]), 0, DateTime.Now.Year, DateTime.Now.Year);
				string[] newIds = new string[100];
				int counter = 0;
				foreach (DataRow row in dt.Rows)
				{
					#region GetBlockRowId
					string _id = string.Empty;
					foreach (Row r in view.Rows)
					{
						if (r is Block)
						{
							_id = r.Id;
							break;
						}
					}
					#endregion

					//DV: Convert finances from IBN 4.1
					newIds[counter] = Guid.NewGuid().ToString();
					view.AddBlockRow(_id, newIds[counter]);
					counter++;
					if (row == null) return;
				}
				counter = 0;
				ProjectSpreadSheet.SaveView(int.Parse(Request["ProjectId"]), 0, view);
				foreach (DataRow row in dt.Rows)
				{
					ProjectSpreadSheet.SetUserRowName(int.Parse(Request["ProjectId"]), newIds[counter], (string)row[2]);
					ActualFinances.Create((int)row["ObjectId"], (ObjectTypes)row["ObjectTypeId"], (DateTime)row["LastSavedDate"], newIds[counter], Convert.ToDouble(row["AValue"]), (string)row[2]);
					counter++;
				}
			}*/
			#endregion

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "<script language=JavaScript> window.location.href=window.location.href; </script>");
		}
		#endregion

		#region btnApplyFilter_ServerClick
		private void btnApplyFilter_ServerClick(object sender, EventArgs e)
		{
			GenerateScript();
		}
		#endregion

		#region btnDeactivate_Click
		private void btnDeactivate_Click(object sender, EventArgs e)
		{
			ProjectSpreadSheet.Deactivate(int.Parse(Request["ProjectId"]));
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "<script language=JavaScript> window.location.href=window.location.href; </script>");
		}
		#endregion

		#region btnDeactivateFinance_ServerClick
		/// <summary>
		/// Handles the ServerClick event of the btnDeactivateFinance control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnDeactivateFinance_ServerClick(object sender, EventArgs e)
		{
			ProjectSpreadSheet.Deactivate(int.Parse(Request["ProjectId"]));
			Response.Redirect(
				String.Format(CultureInfo.InvariantCulture,
				"~/Projects/ProjectView.aspx?ProjectId={0}",
				Request["ProjectId"]));
		}
		#endregion

	}

	public class SortComparer : IComparer
	{
		#region IComparer Members
		public int Compare(object x, object y)
		{
			return string.Compare(((SpreadSheetTemplateInfo)x).Name, ((SpreadSheetTemplateInfo)y).Name, true);
		}
		#endregion
	}
}
