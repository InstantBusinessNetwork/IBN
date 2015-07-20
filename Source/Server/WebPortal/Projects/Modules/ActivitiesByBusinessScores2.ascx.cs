using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.UI.Web.Util;
using System.Resources;
using System.Xml;

namespace Mediachase.UI.Web.Projects.Modules
{
	public partial class ActivitiesByBusinessScores2 : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ActivitiesByBusinessScores2).Assembly);
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


		protected void Page_Init(object sender, EventArgs e)
		{
			this.btnApplyFilter.ServerClick += new EventHandler(btnApplyFilter_ServerClick);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			BindTitles();
			if (!Page.IsPostBack)
			{
				BindInitialValues();
				BindFinanceTable();
			}
		}

		#region Initial filter values
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
		#endregion

		#region Titles binding
		private void BindTitles()
		{
			secHeader.Title = LocRM.GetString("tPrjsByBusScores");
			btnApplyFilter.Text = LocRM.GetString("Apply");
			btnApplyFilter.CustomImage = this.Page.ResolveUrl("~/layouts/images/accept.gif");
			secHeader.AddLink("<img alt='' src='../Layouts/Images/icons/xlsexport.gif'/> " + LocRM.GetString("ExcelExport"), "../Projects/ProjectBSExcelExport.aspx");
		}
		#endregion

		#region Load projects list
		private ArrayList LoadProjectList()
		{
			ArrayList projectList = new ArrayList();

			string strProjectListType = pc["Report_ProjectListType"];

			if (strProjectListType == null)
				strProjectListType = "All";

			if (strProjectListType == "Custom")
			{
				string strProjectList = pc["Report_ProjectListData"];

				if (strProjectList != null)
				{
					foreach (string strItem in strProjectList.Split(';'))
					{
						string strPrjId = strItem.Trim();

						if (strPrjId != string.Empty)
						{
							projectList.Add(int.Parse(strPrjId));
						}
					}
				}
			}
			else if (strProjectListType == "All")
			{
				using (IDataReader reader = Project.GetListProjects())
				{
					while (reader.Read())
					{
						projectList.Add((int)reader["ProjectId"]);
					}
				}
			}
			else if (strProjectListType == "Portfolio")
			{
				string strPortfolioId = pc["Report_ProjectListData"];

				DataTable dt = Project.GetListProjectGroupedByPortfolio(int.Parse(strPortfolioId), 0, 0);

				foreach (DataRow row in dt.Rows)
				{
					int prid = int.Parse(row["ProjectId"].ToString());
					if (prid > 0)
						projectList.Add(prid);
				}
			}

			return projectList;
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

			//Response.Redirect("ProjectsByBusinessScores.aspx");
			BindFinanceTable();
		}
		#endregion

		#region Binding finance table
		private void BindFinanceTable()
		{
			XmlDocument doc = new XmlDocument();
			ArrayList PrIds = LoadProjectList();

			SpreadSheetDocumentType type = SpreadSheetDocumentType.WeekYear;
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
				view = ProjectSpreadSheet.CompareProjects(PrIds, type, BasePlan1,
					FromYear, ToYear);
			else
				view = ProjectSpreadSheet.CompareProjectsReverse(PrIds, type, BasePlan1,
					FromYear, ToYear);

			if (BasePlan2 == -2)
			{
				doc = ProjectSpreadSheet.CreateViewDocForAnalysis(view);
			}
			else
			{
				SpreadSheetView view2 = null;
				if (!Reverse)
					view2 = ProjectSpreadSheet.CompareProjects(PrIds, type,
						BasePlan2, FromYear, ToYear);
				else
					view2 = ProjectSpreadSheet.CompareProjectsReverse(PrIds, type,
						BasePlan2, FromYear, ToYear);

				doc = ProjectSpreadSheet.CreateViewCompareDocForAnalysis(view, view2);
			}

			if (doc != null)
			{
				#region BindTableHeader
				HtmlTableRow rheader1 = new HtmlTableRow();
				HtmlTableRow rheader2 = new HtmlTableRow();
				HtmlTableRow rheader3 = new HtmlTableRow();
				if (view.Document.DocumentType != SpreadSheetDocumentType.Total && view.Document.DocumentType != SpreadSheetDocumentType.Year)
				{
					int YearCounter = ToYear - FromYear + 1;
					int ColSpanRate = -1;

					#region Year
					if (view.Document.DocumentType == SpreadSheetDocumentType.QuarterYear)
					{
						ColSpanRate = 5;
						HtmlTableCell c1 = new HtmlTableCell();
						c1.RowSpan = 2;
						c1.InnerText = LocRM.GetString("tProjects");
						rheader1.Cells.Add(c1);
					}
					else
						if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
						{
							ColSpanRate = 17;
							HtmlTableCell c1 = new HtmlTableCell();
							c1.RowSpan = 3;
							c1.InnerText = LocRM.GetString("tProjects");
							rheader1.Cells.Add(c1);
						}
						else
							if (view.Document.DocumentType == SpreadSheetDocumentType.WeekYear)
							{
								ColSpanRate = 53;
								HtmlTableCell c1 = new HtmlTableCell();
								c1.RowSpan = 2;
								c1.InnerText = LocRM.GetString("tProjects");
								rheader1.Cells.Add(c1);
							}

					int RowSpanRate = (view.Columns.Length - YearCounter) / (YearCounter * 4);
					int yearCount = 0;
					int tmp = 0;
					int tmp2 = 0;
					for (int i = 0; i < view.Columns.Length - 1; i++)
					{
						if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
						{
							if (i % (ColSpanRate) == 0)
							{
								tmp = FromYear + yearCount;
								tmp2 = ColSpanRate - 1;
								HtmlTableCell c1 = new HtmlTableCell();
								c1.Align = "center";
								c1.ColSpan = tmp2;
								c1.InnerHtml = tmp.ToString();
								rheader1.Cells.Add(c1);
							}
							else
								if (((i - yearCount + 1) % (ColSpanRate - 1) == 0) && (i - yearCount + 1 != 0))
								{
									HtmlTableCell c1 = new HtmlTableCell();
									c1.RowSpan = 3;
									c1.Align = "center";
									c1.InnerHtml = LocRM.GetString("tYear");
									rheader1.Cells.Add(c1);
								}
						}
						else//Week/Year or Quarter/Year
						{
							if (i % (ColSpanRate) == 0)
							{
								tmp = FromYear + yearCount;
								tmp2 = ColSpanRate - 1;
								HtmlTableCell c1 = new HtmlTableCell();
								c1.ColSpan = tmp2;
								c1.Align = "center";
								c1.InnerHtml = tmp.ToString();
								rheader1.Cells.Add(c1);
							}
							else
								if (((i - yearCount + 1) % (ColSpanRate - 1) == 0) && (i - yearCount + 1 != 0))
								{
									HtmlTableCell c1 = new HtmlTableCell();
									c1.RowSpan = 2;
									c1.InnerHtml = LocRM.GetString("tYear");
									c1.Align = "center";
									rheader1.Cells.Add(c1);
								}
						}
						if (i % (ColSpanRate) == 0)
							yearCount++;
					}
					if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
					{
						HtmlTableCell c1 = new HtmlTableCell();
						c1.RowSpan = 3;
						c1.InnerHtml = LocRM.GetString("tTotalSum");
						c1.Align = "center";
						rheader1.Cells.Add(c1);
					}
					else
					{
						HtmlTableCell c1 = new HtmlTableCell();
						c1.RowSpan = 2;
						c1.InnerHtml = LocRM.GetString("tTotalSum");
						c1.Align = "center";
						rheader1.Cells.Add(c1);
					}
					#endregion

					#region Quartals
					if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
					{
						for (int i = 0; i < view.Columns.Length - 1; i++)
						{
							int counter = i % ColSpanRate;

							if (counter % (RowSpanRate) == 0 && counter != 16)
							{
								tmp = (counter / 4) + 1;
								HtmlTableCell c1 = new HtmlTableCell();
								c1.ColSpan = 3;
								c1.Align = "center";
								c1.InnerHtml = LocRM.GetString("tQuarterYear") + " " + tmp.ToString();
								rheader2.Cells.Add(c1);
							}
							else
								if ((counter + 1) % RowSpanRate == 0)
								{
									HtmlTableCell c1 = new HtmlTableCell();
									c1.RowSpan = 2;
									c1.InnerHtml = LocRM.GetString("tQuarterTotal");
									c1.Align = "center";
									rheader2.Cells.Add(c1);
								}
								else
									if (counter == 16)
									{
									}
						}
					}
					#endregion

					#region BindServerData
					int _counter = 0;
					for (int i = 0; i < view.Columns.Length - 1; i++)
					{

						if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
						{
							if (i % ColSpanRate == 0 && i > 0) _counter++;
							if ((i + 1 - _counter) % RowSpanRate == 0 && i > 0)
							{ }
							else

								if ((i + 1) % ColSpanRate == 0 && i > 0)
								{ }
								else
								{
									HtmlTableCell c1 = new HtmlTableCell();
									c1.InnerHtml = ((Column)view.Columns[i]).Name;
									c1.Align = "center";
									rheader3.Cells.Add(c1);
								}
						}

						else
						{
							if ((i + 1) % ColSpanRate == 0)
							{ }
							else
							{
								HtmlTableCell c1 = new HtmlTableCell();
								c1.InnerHtml = ((Column)view.Columns[i]).Name;
								c1.Align = "center";
								rheader3.Cells.Add(c1);
							}
						}
					}
					#endregion
					rheader1.Attributes.Add("class", "gridheader");
					tGrid.Rows.Add(rheader1);
					rheader2.Attributes.Add("class", "gridheader");
					if (rheader2.Cells.Count > 0)
						tGrid.Rows.Add(rheader2);
					rheader3.Attributes.Add("class", "gridheader");
					if (rheader3.Cells.Count > 0)
						tGrid.Rows.Add(rheader3);

				}
				else
				{
					rheader1 = new HtmlTableRow();
					HtmlTableCell c1 = new HtmlTableCell();
					c1.InnerHtml = LocRM.GetString("tProjects");
					rheader1.Cells.Add(c1);
					for (int i = 0; i < view.Columns.Length; i++)
					{
						c1 = new HtmlTableCell();
						c1.InnerHtml = ((Column)view.Columns[i]).Name;
						c1.Align = "center";
						rheader1.Cells.Add(c1);
					}
					rheader1.Attributes.Add("class", "gridheader");
					tGrid.Rows.Add(rheader1);
				}

				#endregion

				#region BindTableBody
				XmlNode ParentNode = doc.DocumentElement;

				foreach (XmlNode node in ParentNode.ChildNodes)
				{
					HtmlTableRow rbody = new HtmlTableRow();
					ArrayList alcr = new ArrayList();
					foreach (XmlNode node2 in node.ChildNodes)
					{
						HtmlTableRow crbody = new HtmlTableRow();
						foreach (XmlNode cnode in node2.ChildNodes)
						{

							if (node2.Name == "row")
							{
								HtmlTableCell c1 = new HtmlTableCell();
								c1.Align = "right";
								c1.InnerHtml = cnode.InnerText;
								crbody.Cells.Add(c1);

							}
							else
							{
								if (node2.Name == "cell")
								{
									HtmlTableCell c2 = new HtmlTableCell();
									c2.InnerHtml = "<b>" + node2.InnerText + "</b>";
									rbody.Cells.Add(c2);
								}
							}
						}
						alcr.Add(crbody);
						crbody = new HtmlTableRow();
					}
					if (rbody.Cells.Count > 0)
						tGrid.Rows.Add(rbody);
					for (int i = 0; i < alcr.Count; i++)
					{
						if (((HtmlTableRow)alcr[i]).Cells.Count > 0)
							tGrid.Rows.Add((HtmlTableRow)alcr[i]);
					}
					for (int i = 0; i < tGrid.Rows.Count; i++)
					{
						if (tGrid.Rows[i].Cells.Count > 0)
							tGrid.Rows[i].Cells[0].Attributes.Add("class", "gridheader");
					}
				}
				#endregion
			}
		}
		#endregion
	}
}