using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Xml;
using System.Resources;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.SpreadSheet;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for ProjectBSExport.
	/// </summary>
	public partial class ProjectBSExport : System.Web.UI.Page
	{

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ProjectBSExport).Assembly);

		#region Properties

		public int ProjectId
		{
			get
			{
				if(Request["ProjectId"]!=null && Request["ProjectId"]!="")
					return int.Parse(Request["ProjectId"].ToString());
				else
					return -1;
			}
		}

		public int BasePlan1
		{
			get
			{
				if(Request["BasePlan1"]!=null && Request["BasePlan1"]!="")
					return int.Parse(Request["BasePlan1"].ToString());
				else
					return 0;
			}
		}

		public int BasePlan2
		{
			get
			{
				if(Request["BasePlan2"]!=null && Request["BasePlan2"]!="")
					return int.Parse(Request["BasePlan2"].ToString());
				else
					return -2;
			}
		}

		public int FromYear
		{
			get
			{
				if(Request["FromYear"]!=null && Request["FromYear"]!="")
					return int.Parse(Request["FromYear"].ToString());
				else
					return DateTime.Now.Year;
			}
		}

		public int ToYear
		{
			get
			{
				if(Request["ToYear"]!=null && Request["ToYear"]!="")
					return int.Parse(Request["ToYear"].ToString());
				else
					return DateTime.Now.Year;
			}
		}

		public string FinanceType
		{
			get
			{
				if(Request["FinanceType"]!=null && Request["FinanceType"]!="")
					return Request["FinanceType"].ToString();
				else
					return "1";
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			XmlDocument doc = new XmlDocument();
			ArrayList PrjsIds = new ArrayList();
			PrjsIds.Add(ProjectId);
			string ProjectTitle = string.Empty;

			ProjectTitle = Project.GetProjectTitle(ProjectId);

			/*SpreadSheetDocumentType type = SpreadSheetDocumentType.WeekYear;
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
			}*/

			SpreadSheetView view = null;
			
			view = ProjectSpreadSheet.LoadView(ProjectId, BasePlan1,  
					FromYear, ToYear);
			
			Response.ContentType = "text/xml";
			
			if(BasePlan2==-2)
			{
				doc = ProjectSpreadSheet.CreateViewDocForAnalysis(view);
			}
			else
			{
				SpreadSheetView view2 = null;
				view2	= ProjectSpreadSheet.LoadView(ProjectId, 
						BasePlan2, FromYear, ToYear);
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
										sbody+="<td style='border-top:1px solid black; mso-number-format:\\@'>";
									else
										sbody+="<td style='border-top:1px solid black'>";
									sbody+=node2.InnerText;
									sbody+="</td>";
								}
							}

						}
						if(childrows!="<tr>")
							childrows+="</tr>";	
						else
							childrows = "";
					}
					if(sbody!="<tr>")
						sbody+="</tr>";
					else
						sbody = "";
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
						header1+="<tr><td rowspan='2' style='border:1px solid black' align='center' >"+ProjectTitle+"</td>";
						header2+="<tr>";
					}
					else 
						if (view.Document.DocumentType == SpreadSheetDocumentType.MonthQuarterYear)
					{
						ColSpanRate = 17;
						header1+="<tr><td rowspan='3' style='border:1px solid black' align='center' >"+ProjectTitle+"</td>";
						header2+="<tr>";
						header3+="<tr>";
					}
					else 
						if (view.Document.DocumentType == SpreadSheetDocumentType.WeekYear)
					{
						ColSpanRate = 53;
						header1+="<tr><td rowspan='2' style='border:1px solid black' align='center' >"+ProjectTitle+"</td>";
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
					header1+="<td style='border:1px solid black' align='center' >"+ProjectTitle+"</td>";
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
