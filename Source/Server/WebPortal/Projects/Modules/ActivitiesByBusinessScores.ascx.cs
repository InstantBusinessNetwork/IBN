namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Text;
	using System.Collections;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.SpreadSheet;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for ActivitiesByBusinessScores.
	/// </summary>
	public partial class ActivitiesByBusinessScores : System.Web.UI.UserControl
	{

		#region Html variables
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ActivitiesByBusinessScores).Assembly);
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		#region Properties
		public string ProjectListData
		{
			get
			{
				if (pc["Report_ProjectListData"] != null)
					return pc["Report_ProjectListData"];
				else
					return "";
			}
			set
			{
				pc["Report_ProjectListData"] = value;
			}
		}

		public int FromYear
		{
			get
			{
				if (pc["ProjectsByBS_FromYear"] != null)
					return int.Parse(pc["ProjectsByBS_FromYear"]);
				else
					return DateTime.Now.Year;
			}
			set
			{
				pc["ProjectsByBS_FromYear"] = value.ToString();
			}
		}

		public int ToYear
		{
			get
			{
				if (pc["ProjectsByBS_ToYear"] != null)
					return int.Parse(pc["ProjectsByBS_ToYear"]);
				else
					return DateTime.Now.Year;
			}
			set
			{
				pc["ProjectsByBS_ToYear"] = value.ToString();
			}
		}

		public string ProjectListType
		{
			get
			{
				if (pc["Report_ProjectListType"] != null)
					return pc["Report_ProjectListType"];
				else
					return "All";
			}
			set
			{
				pc["Report_ProjectListType"] = value;
			}
		}

		private int BasePlan1
		{
			get
			{
				if (pc["ProjectsByBS_BasePlan1"] != null)
					return int.Parse(pc["ProjectsByBS_BasePlan1"]);
				else
					return 0;
			}
			set
			{
				pc["ProjectsByBS_BasePlan1"] = value.ToString();
			}
		}

		private int BasePlan2
		{
			get
			{
				if (pc["ProjectsByBS_BasePlan2"] != null)
					return int.Parse(pc["ProjectsByBS_BasePlan2"]);
				else
					return -2;
			}
			set
			{
				pc["ProjectsByBS_BasePlan2"] = value.ToString();
			}
		}

		private string FinanceType
		{
			get
			{
				if (pc["ProjectsByBS_FinanceType"] != null)
					return pc["ProjectsByBS_FinanceType"];
				else
					return "1";
			}
			set
			{
				pc["ProjectsByBS_FinanceType"] = value;
			}
		}

		private bool Reverse
		{
			get
			{
				if (pc["ProjectsByBS_Reverse"] != null)
					return bool.Parse(pc["ProjectsByBS_Reverse"]);
				else
					return false;
			}
			set
			{
				pc["ProjectsByBS_Reverse"] = value.ToString();
			}
		}

		#endregion


		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/dhtmlXGrid.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/dhtmlGrid/dhtmlXCommon.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/dhtmlGrid/dhtmlXGrid.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/dhtmlGrid/dhtmlXGridCell.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/dhtmlGrid/dhtmlXTreeGrid.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/dhtmlGrid/dhtmlXMovingSelection.js");

			// Put user code to initialize the page here
			if (!Page.IsPostBack)
			{
				BindInitialValues();
			}
			BindTitles();
			GenerateScript();
		}

		private void BindInitialValues()
		{
			ddPrjGroup.DataSource = ProjectGroup.GetProjectGroups();
			ddPrjGroup.DataTextField = "Title";
			ddPrjGroup.DataValueField = "ProjectGroupId";
			ddPrjGroup.DataBind();
			ddPrjGroup.Items.Insert(0, new ListItem(LocRM.GetString("AllFem"), "0"));
			ddPrjGroup.Items.Add(new ListItem(LocRM.GetString("tCustom"), "-1"));
			switch (ProjectListType)
			{
				case "All":
					if (ddPrjGroup.Items.FindByValue("0") != null)
						ddPrjGroup.SelectedValue = "0";
					break;
				case "Custom":
					if (ddPrjGroup.Items.FindByValue("-1") != null)
						ddPrjGroup.SelectedValue = "-1";
					break;
				case "Portfolio":
					if (ddPrjGroup.Items.FindByValue(ProjectListData) != null)
						ddPrjGroup.SelectedValue = ProjectListData;
					break;
			}


			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));

			dt.Rows.Add(new object[] { 1, LocRM.GetString("tWeekYear") });
			dt.Rows.Add(new object[] { 2, LocRM.GetString("tMonthQuarterYear") });
			dt.Rows.Add(new object[] { 3, LocRM.GetString("tQuarterYear") });
			dt.Rows.Add(new object[] { 4, LocRM.GetString("tYear") });
			dt.Rows.Add(new object[] { 5, LocRM.GetString("tTotalSum") });

			ddFinanceType.DataSource = dt;
			ddFinanceType.DataTextField = "Name";
			ddFinanceType.DataValueField = "Id";
			ddFinanceType.DataBind();
			ddFinanceType.SelectedValue = FinanceType;

			DataTable dt1 = new DataTable();
			dt1.Columns.Add(new DataColumn("Id", typeof(int)));
			dt1.Columns.Add(new DataColumn("Name", typeof(string)));
			foreach (BasePlanSlot bps in BasePlanSlot.List())
			{
				DataRow dr = dt1.NewRow();
				dr["Id"] = bps.BasePlanSlotId;
				dr["Name"] = bps.Name;
				dt1.Rows.Add(dr);
			}
			dt1.Rows.Add(new object[] { "-1", LocRM.GetString("tFact") });
			dt1.Rows.Add(new object[] { "0", LocRM.GetString("tCurrent") });
			DataView dv = dt1.DefaultView;
			dv.Sort = "Id ASC";
			ddBasePlan1.DataSource = dv;
			ddBasePlan1.DataTextField = "Name";
			ddBasePlan1.DataValueField = "Id";
			ddBasePlan1.DataBind();
			ddBasePlan1.SelectedValue = BasePlan1.ToString();

			dt1.Rows.Add(new object[] { "-2", LocRM.GetString("tNotSelected") });
			dv = dt1.DefaultView;
			dv.Sort = "Id ASC";
			ddBasePlan2.DataSource = dv;
			ddBasePlan2.DataTextField = "Name";
			ddBasePlan2.DataValueField = "Id";
			ddBasePlan2.DataBind();
			ddBasePlan2.SelectedValue = BasePlan2.ToString();

			tbFromYear.Text = FromYear.ToString();
			tbToYear.Text = ToYear.ToString();


			ddGroupBy.Items.Clear();
			ddGroupBy.Items.Add(new ListItem(LocRM.GetString("tByProject"), "false"));
			ddGroupBy.Items.Add(new ListItem(LocRM.GetString("tByBusinessScore"), "true"));
			ddGroupBy.SelectedValue = Reverse.ToString().ToLower();
		}

		private void BindTitles()
		{
			secHeader.Title = LocRM.GetString("tPrjsByBusScores");
			btnApplyFilter.Text = LocRM.GetString("Apply");
			btnApplyFilter.CustomImage = this.Page.ResolveUrl("~/layouts/images/accept.gif");
			secHeader.AddLink("<img alt='' src='../Layouts/Images/icons/xlsexport.gif'/> " + LocRM.GetString("ExcelExport"), "../Projects/ProjectBSExcelExport.aspx");
		}

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
			this.btnApplyFilter.ServerClick += new EventHandler(btnApplyFilter_ServerClick);
		}
		#endregion

		#region btnApplyFilter_ServerClick
		private void btnApplyFilter_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			switch (ddPrjGroup.SelectedValue)
			{
				case "0":
					ProjectListType = "All";
					break;
				case "-1":
					ProjectListType = "Custom";
					break;
				default:
					ProjectListType = "Portfolio";
					ProjectListData = ddPrjGroup.SelectedValue;
					break;
			}

			BasePlan1 = int.Parse(ddBasePlan1.SelectedValue);
			BasePlan2 = int.Parse(ddBasePlan2.SelectedValue);
			FromYear = int.Parse(tbFromYear.Text);
			ToYear = int.Parse(tbToYear.Text);
			FinanceType = ddFinanceType.SelectedValue;
			Reverse = bool.Parse(ddGroupBy.SelectedValue);
			Response.Redirect("ProjectsByBusinessScores.aspx");
		}
		#endregion

		#region GenerateScript
		private void GenerateScript()
		{
			ArrayList ProjectIds = new ArrayList();
			switch (ProjectListType)
			{
				case "All":
					using (IDataReader reader = Project.GetListProjects())
					{
						while (reader.Read())
						{
							ProjectIds.Add(int.Parse(reader["ProjectId"].ToString()));
						}
					}
					break;
				case "Portfolio":
					DataTable dt = Project.GetListProjectGroupedByPortfolio(int.Parse(ProjectListData), 0, 0);
					if (dt != null && dt.Rows.Count > 0)
					{
						foreach (DataRow dr in dt.Rows)
						{
							int prid = int.Parse(dr["ProjectId"].ToString());
							if (prid > 0)
								ProjectIds.Add(prid);
						}
					}
					break;
				case "Custom":
					if (ProjectListData != null && ProjectListData.Length > 0)
					{
						string[] Ids = ProjectListData.Split(';');
						if (Ids != null && Ids.Length > 0)
						{
							for (int i = 0; i < Ids.Length; i++)
							{
								ProjectIds.Add(int.Parse(Ids[i]));
							}
						}
					}
					break;
				default:
					break;
			}
			SpreadSheetDocumentType type = SpreadSheetDocumentType.Total;
			switch (FinanceType)
			{
				case "1":
					type = SpreadSheetDocumentType.WeekYear;
					break;
				case "2":
					type = SpreadSheetDocumentType.MonthQuarterYear;
					break;
				case "3":
					type = SpreadSheetDocumentType.QuarterYear;
					break;
				case "4":
					type = SpreadSheetDocumentType.Year;
					break;
				case "5":
					type = SpreadSheetDocumentType.Total;
					break;
			}

			SpreadSheetView view = null;
			if (!Reverse)
				view = ProjectSpreadSheet.CompareProjects(ProjectIds, type, BasePlan1, FromYear, ToYear);
			else
				view = ProjectSpreadSheet.CompareProjectsReverse(ProjectIds, type, BasePlan1, FromYear, ToYear);
			SpreadSheetView view2;
			if (BasePlan2 != -2)
			{
				if (!Reverse)
					view2 = ProjectSpreadSheet.CompareProjects(ProjectIds, type, BasePlan2, FromYear, ToYear);
				else
					view2 = ProjectSpreadSheet.CompareProjectsReverse(ProjectIds, type, BasePlan2, FromYear, ToYear);
			}


			int colCount = view.Columns.Length;
			//int maxWidth = 950;

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<script type='text/javascript'>");
			sb.AppendLine("//<![CDATA[");
			sb.AppendLine("if (mygrid2 == null)");
			sb.AppendLine("	mygrid2 = new dhtmlXGridObject('gridbox');");
			//sb.Append("");


			#region Bind: Header Caption

			if (view.Document.DocumentType != SpreadSheetDocumentType.Total && view.Document.DocumentType != SpreadSheetDocumentType.Year)
			{
				sb.AppendFormat("mygrid2.setHeader(\"{0}", LocRM.GetString("tProjects"));
				int YearCounter = ToYear - FromYear + 1;
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
							sb.AppendFormat(",{0}", FromYear + yearCount);
						else if (((i - yearCount + 1) % (ColSpanRate - 1) == 0) && (i - yearCount + 1 != 0))
							sb.AppendFormat(",{0}", LocRM.GetString("tYear"));
						else
							sb.Append(",#cspan");
					}
					else
					{
						if (i % (ColSpanRate) == 0)
							sb.AppendFormat(",{0}", FromYear + yearCount);
						else if (((i - yearCount + 1) % (ColSpanRate - 1) == 0) && (i - yearCount + 1 != 0))
							sb.AppendFormat(",{0}", LocRM.GetString("tYear"));
						else
							sb.Append(",#cspan");
					}
					if (i % (ColSpanRate) == 0)
						yearCount++;
				}
				//}
				sb.AppendFormat(",{0}\");", LocRM.GetString("tTotalSum"));
				#endregion

				#region Quartals
				if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
				{
					sb.Append("mygrid2.attachHeader([\"#rspan\"");
					for (int i = 0; i < view.Columns.Length - 1; i++)
					{
						int counter = i % ColSpanRate;

						if (counter % (RowSpanRate) == 0 && counter != 16)
							sb.AppendFormat(",\"{0} {1}\"", LocRM.GetString("tQuarterYear"), /*(((i + 1) / RowSpanRate) % 4) + 1*/ (counter / 4) + 1);
						else
							if ((counter + 1) % RowSpanRate == 0)
								sb.AppendFormat(", \"{0}\"", LocRM.GetString("tQuarterTotal"));
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
				sb.AppendFormat("mygrid2.attachHeader([\"#rspan\", \"{0}\"", ((Column)view.Columns[0]).Name);
				int _counter = 0;
				for (int i = 1; i < view.Columns.Length - 1; i++)
				{

					if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
					{
						if (i % ColSpanRate == 0) _counter++;
						if ((i + 1 - _counter) % RowSpanRate == 0)
							sb.Append(",\"#rspan\"");
						else
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
				sb.AppendFormat("mygrid2.setHeader(\"{0}", LocRM.GetString("tProjects"));

				for (int i = 0; i < view.Columns.Length; i++)
				{
					sb.AppendFormat(",{0}", ((Column)view.Columns[i]).Name);
				}
				sb.Append("\");");
			}
			#endregion

			#region Bind: Header Width
			sb.AppendFormat("mygrid2.setImagePath('{0}');", ResolveUrl("~/Layouts/Images/dhtmlGrid/imgs"));

			sb.Append("mygrid2.setInitWidths(\"100");
			for (int i = 0; i < view.Columns.Length; i++)
			{
				sb.AppendFormat(",{0}", 60/*(int)(maxWidth / view.Columns.Length)*/);
			}
			sb.Append("\");");
			#endregion

			#region Bind: Header Align
			sb.Append("mygrid2.setColAlign(\"left");
			for (int i = 0; i < view.Columns.Length; i++)
			{
				sb.Append(",right");
			}
			sb.Append("\");");
			#endregion

			#region Bind: Cols CanEdit
			sb.Append("mygrid2.setColTypes(\"tree");

			for (int i = 0; i < colCount; i++)
			{
				sb.Append(",ro");
			}
			sb.Append("\");");
			#endregion

			#region Bind: Cols Color
			sb.Append("mygrid2.setColumnColor(\"#E1ECFC");
			for (int i = 1; i < colCount; i++)
			{
				sb.Append(",white");
			}
			sb.Append("\");");
			#endregion


			sb.AppendLine("mygrid2.enableKeyboardSupport(false);mygrid2.setOnLoadingEnd('EnableApplyButton');mygrid2.enableEditEvents(false,false,false);mygrid2.init();");

			if (ddBasePlan2.SelectedValue != "-2")
			{
				sb.AppendLine("qstring2='&compare=1';");
				sb.AppendFormat("var xmlurl='{0}?ProjectFinanceCompare=1'+qstring2;", ResolveUrl("~/Modules/XmlForTreeView.aspx"));
				sb.AppendLine("mygrid2.loadXML(xmlurl);");
				sb.AppendLine("compareMode=1;");
			}
			else
			{
				sb.AppendFormat("var xmlurl='{0}?ProjectFinanceCompare=1';", ResolveUrl("~/Modules/XmlForTreeView.aspx"));
				sb.AppendLine("mygrid2.loadXML(xmlurl);");

				sb.AppendLine("compareMode=0;");
			}

			sb.AppendLine("//]]>");
			sb.AppendLine("</script>");
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), sb.ToString());
		}
		#endregion
	}



}
