using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Text;
using System.Xml;
using System.Resources;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for ProjectBSExcelExport.
	/// </summary>
	public partial class ProjectBSExcelExport : System.Web.UI.Page
	{
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ProjectBSExcelExport).Assembly);

		#region Properties
		public string ProjectListData
		{
			get
			{
				if(pc["Report_ProjectListData"]!=null)
					return pc["Report_ProjectListData"];
				else
					return "";
			}
		}

		public int FromYear
		{
			get
			{
				if(pc["ProjectsByBS_FromYear"]!=null)
					return int.Parse(pc["ProjectsByBS_FromYear"]);
				else
					return DateTime.Now.Year;
			}
		}

		public int ToYear
		{
			get
			{
				if(pc["ProjectsByBS_ToYear"]!=null)
					return int.Parse(pc["ProjectsByBS_ToYear"]);
				else
					return DateTime.Now.Year;
			}
		}

		public string ProjectListType
		{
			get
			{
				if(pc["Report_ProjectListType"]!=null)
					return pc["Report_ProjectListType"];
				else
					return "All";
			}
		}

		private int BasePlan1
		{
			get
			{
				if(pc["ProjectsByBS_BasePlan1"]!=null)
					return int.Parse(pc["ProjectsByBS_BasePlan1"]);
				else
					return 0;
			}
		}

		private int BasePlan2
		{
			get
			{
				if(pc["ProjectsByBS_BasePlan2"]!=null)
					return int.Parse(pc["ProjectsByBS_BasePlan2"]);
				else
					return -2;
			}
		}

		private string FinanceType
		{
			get
			{
				if(pc["ProjectsByBS_FinanceType"]!=null)
					return pc["ProjectsByBS_FinanceType"];
				else
					return "1";
			}
		}

		private bool Reverse
		{
			get
			{
				if(pc["ProjectsByBS_Reverse"]!=null)
					return bool.Parse(pc["ProjectsByBS_Reverse"]);
				else
					return false;
			}
		}

		#endregion


		private ArrayList LoadProjectList()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;

			ArrayList projectList = new ArrayList();

			//UserLightPropertyCollection pc =  Security.CurrentUser.Properties;

			string strProjectListType = pc["Report_ProjectListType"];

			if(strProjectListType==null)
				strProjectListType = "All";

			if(strProjectListType=="Custom")
			{
				string strProjectList = pc["Report_ProjectListData"];

				if(strProjectList!=null)
				{
					foreach(string strItem in strProjectList.Split(';'))
					{
						string strPrjId = strItem.Trim();

						if(strPrjId!=string.Empty)
						{
							projectList.Add(int.Parse(strPrjId));
						}
					}
				}
			}
			else if (strProjectListType=="All")
			{
				using(IDataReader reader = Project.GetListProjects())
				{
					while(reader.Read())
					{
						projectList.Add((int)reader["ProjectId"]);
					}
				}
			}
			else if (strProjectListType=="Portfolio")
			{
				string strPortfolioId = pc["Report_ProjectListData"];

				DataTable dt = Project.GetListProjectGroupedByPortfolio(int.Parse(strPortfolioId), 0, 0);

				foreach(DataRow row in dt.Rows)
				{
					int prid = int.Parse(row["ProjectId"].ToString());
					if(prid>0)
						projectList.Add(prid);
				}
			}

			return projectList;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			XmlDocument doc = new XmlDocument();
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;

			ArrayList PrIds = LoadProjectList();
			
			SpreadSheetDocumentType type = SpreadSheetDocumentType.WeekYear;
			switch(FinanceType)
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
			if(!Reverse)
				view = ProjectSpreadSheet.CompareProjects(PrIds, type, BasePlan1,  
					FromYear, ToYear);
			else
				view = ProjectSpreadSheet.CompareProjectsReverse(PrIds, type, BasePlan1,  
					FromYear, ToYear);
			Response.ContentType = "text/xml";
			
			if(BasePlan2==-2)
			{
				doc = ProjectSpreadSheet.CreateViewDocForAnalysis(view);
			}
			else
			{
				SpreadSheetView view2 = null;
				if(!Reverse)
					view2	= ProjectSpreadSheet.CompareProjects(PrIds, type, 
						BasePlan2, FromYear, ToYear);
				else
					view2	= ProjectSpreadSheet.CompareProjectsReverse(PrIds, type, 
						BasePlan2,  FromYear, ToYear);

				doc = ProjectSpreadSheet.CreateViewCompareDocForAnalysis(view, view2);
			}
			if(doc!=null)
			{
				HttpContext.Current.Response.Clear();
				HttpContext.Current.Response.Charset = "utf-8";
				HttpContext.Current.Response.AddHeader("Content-Type","application/octet-stream");
				HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=BusinessScores.xls");
				string Header = "<html><head><meta http-equiv=\"Content-Type\" content=\"application/octet-stream; charset=utf-8\"></head><body><table>";
				string Footer = "</table></body></html>";

				#region BindTableBody
				string body = string.Empty;
				string sbody = string.Empty;
				string childrows = string.Empty;;
				XmlNode ParentNode = doc.DocumentElement;
				foreach(XmlNode node in ParentNode.ChildNodes)
				{
					sbody="<tr>";
					childrows="<tr>";
					foreach(XmlNode node2 in node.ChildNodes)
					{
						foreach(XmlNode cnode in node2.ChildNodes)
						{
							if(node2.Name=="row")
							{
								if(BasePlan2 != -2)
									childrows+="<td style='mso-number-format:\\@'>";
								else childrows+="<td>";
								childrows+=cnode.InnerText;
								childrows+="</td>";
							}
							else
							{
								if(node2.Name=="cell")
								{
									if(BasePlan2 != -2)
										sbody+="<td style='border-top:1px solid black; mso-number-format:\\@'><b>";
									else
										sbody+="<td style='border-top:1px solid black'><b>";
									sbody+=node2.InnerText;
									sbody+="<b></td>";
								}
							}
						}
						childrows+="</tr>";	
					}
					sbody+="</tr>";
					sbody+=childrows;
					body+=sbody;
				}
				#endregion

				#region BindTableHeader
				string header1 = string.Empty;
				string header2 = string.Empty;
				string header3 = string.Empty;

				if (view.Document.DocumentType != SpreadSheetDocumentType.Total && view.Document.DocumentType != SpreadSheetDocumentType.Year)
				{
					int YearCounter = ToYear - FromYear + 1;
					int ColSpanRate = -1;

					#region Year
					if (view.Document.DocumentType == SpreadSheetDocumentType.QuarterYear)
					{
						ColSpanRate = 5;
						header1+="<tr><td rowspan='2' style='border:1px solid black' align='center' >"+LocRM.GetString("tProjects")+"</td>";
						header2+="<tr>";
					}
					else 
						if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
						{
							ColSpanRate = 17;
							header1+="<tr><td rowspan='3' style='border:1px solid black' align='center' >"+LocRM.GetString("tProjects")+"</td>";
							header2+="<tr>";
							header3+="<tr>";
						}
					else 
						if (view.Document.DocumentType == SpreadSheetDocumentType.WeekYear)
						{
							ColSpanRate = 53;
							header1+="<tr><td rowspan='2' style='border:1px solid black' align='center' >"+LocRM.GetString("tProjects")+"</td>";
							header2+="<tr>";
						}

					int RowSpanRate = (view.Columns.Length - YearCounter) / (YearCounter* 4);
					//int RowSpanRate = 45;
					int yearCount = 0;
					int tmp = 0;
					int tmp2 = 0;
					for (int i = 0; i < view.Columns.Length - 1; i++)
					{
						if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
						{
							if (i % (ColSpanRate)== 0)
							{
								tmp = FromYear + yearCount;
								tmp2 = ColSpanRate - 1;
								header1+="<td colspan='"+tmp2.ToString()+"' style='border:1px solid black' align='center' >"+tmp.ToString()+"</td>";
							}
							else 
								if ( ((i - yearCount + 1) % (ColSpanRate - 1)== 0) && (i - yearCount + 1 != 0))
								{
									header1+="<td rowspan='3' style='border:1px solid black' align='center' >"+LocRM.GetString("tYear")+"</td>";
								}
						}
						else//Week/Year or Quarter/Year
						{
							if (i % (ColSpanRate)== 0)
							{
								tmp = FromYear + yearCount;
								tmp2 = ColSpanRate - 1;
								header1+="<td colspan='"+tmp2.ToString()+"' style='border:1px solid black' align='center' >"+tmp.ToString()+"</td>";
							}
							else 
								if ( ((i - yearCount + 1) % (ColSpanRate - 1)== 0) && (i - yearCount + 1 != 0))
								{	
									header1+="<td rowspan='2' style='border:1px solid black' align='center' >"+LocRM.GetString("tYear")+"</td>";
								}
						}
						if (i % (ColSpanRate) == 0)
							yearCount++;
					}
					if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
					{
						header1+="<td rowspan='3' style='border:1px solid black' align='center' >"+LocRM.GetString("tTotalSum")+"</td></tr>";
					}
					else
					{
						header1+="<td rowspan='2' style='border:1px solid black' align='center' >"+LocRM.GetString("tTotalSum")+"</td></tr>";
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
								tmp = (counter / 4) +1;
								header2+="<td colspan='3' style='border:1px solid black' align='center' >"+LocRM.GetString("tQuarterYear")+" "+tmp.ToString()+"</td>";
							}
							else
								if ((counter + 1) % RowSpanRate == 0)
								{
									header2+="<td rowspan='2' style='border:1px solid black' align='center' >"+LocRM.GetString("tQuarterTotal")+"</td>";
								}
							else
								if (counter == 16)
								{
									//header2+="<td></td>";
								}
						}
						header2+="</tr>";
					}
					#endregion
					
					#region BindServerData
					
					//sb.AppendFormat("mygrid2.attachHeader([\"#rspan\", \"{0}\"",((Column)view.Columns[0]).Name);
					int _counter = 0;
					//header3+="<td>"+((Column)view.Columns[0]).Name+"</td>";
					for (int i = 0; i < view.Columns.Length - 1; i++)
					{

						if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
						{
							if (i % ColSpanRate == 0 && i>0) _counter++;
							if ((i + 1 - _counter) % RowSpanRate == 0 && i>0)
							{}
							else
								
								if ((i + 1) % ColSpanRate == 0 && i>0)
								{}
							else
							{
								header3+="<td style='border:1px solid black' align='center' >"+((Column)view.Columns[i]).Name+"</td>";	
							}
						}
						
						else
						{
							if ((i + 1)  % ColSpanRate == 0)
							{	}
							else
							{
								header3+="<td style='border:1px solid black' align='center' >"+((Column)view.Columns[i]).Name+"</td>";
							}
						}
					}
					header3+="</tr>";
					#endregion
					body=body.Insert(0, header1+header2+header3);
				}
				else
				{
					header1 = "<tr>";
					header1+="<td style='border:1px solid black' align='center' >"+LocRM.GetString("tProjects")+"</td>";
					for(int i = 0;i<view.Columns.Length;i++)
					{
						header1+="<td style='border:1px solid black' align='center' >"+((Column)view.Columns[i]).Name+"</td>";
					}
					header1+="</tr>";
					body = body.Insert(0,header1);
				}
				#endregion
				HttpContext.Current.Response.Write(String.Concat(Header,body,Footer));
				HttpContext.Current.Response.End();
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
