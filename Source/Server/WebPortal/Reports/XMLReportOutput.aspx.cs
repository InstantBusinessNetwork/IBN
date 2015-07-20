using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Text;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.Reports;
using Mediachase.UI.Web.Util;
using System.Resources;
using Mediachase.SQLQueryCreator;
using Mediachase.Ibn;
using Mediachase.Ibn.Web.UI;
using System.Text.RegularExpressions;
using System.Globalization;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Reports
{
	/// <summary>
	/// Summary description for XMLReportOutput.
	/// </summary>
	public partial class XMLReportOutput : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strXMLReport", typeof(XMLReportOutput).Assembly);
		protected string sTitle = String.Format("{0} | {1} {2}", "Xml Report", IbnConst.ProductFamily, IbnConst.VersionMajorDotMinor);
		protected int ReportId
		{
			get
			{
				try
				{
					return int.Parse(Request["ReportId"].ToString());
				}
				catch
				{
					return -1;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (Request["Refresh"] != null && Request["Refresh"] == "1")
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					string.Format("try {{window.moveTo(0,0);window.resizeTo(screen.availWidth,screen.availHeight);window.opener.top.frames['right'].location.href='{0}';}}", this.ResolveUrl("~/Apps/ReportManagement/Pages/UserReport.aspx")) +
					"catch (e){}", true);
			if (!Page.IsPostBack)
			{
				if (Request["Mode"] != null && bool.Parse(Request["Mode"].ToString()))
				{
					int iTemplateId = -1;
					using (IDataReader reader = Report.GetReport(ReportId))
					{
						if (reader.Read())
						{
							iTemplateId = (int)reader["TemplateId"];

						}
					}
					if (iTemplateId > 0)
						using (IDataReader rdr = Report.GetReportTemplate(iTemplateId))
						{
							if (rdr.Read())
							{
								txtTemplateTitle.Text = rdr["Name"].ToString();
							}
						}
					//					btnSaveVis.Visible = true;		--[2006/01/17]
					btnSaveVis.Visible = false;
				}
				else
					btnSaveVis.Visible = false;
			}
			if (ReportId == -1)
			{
				return;
			}

			if (!Page.IsPostBack && (Request["Export"] == null || Request["Export"] != "2"))
			{
				byte[] bit_data = null;
				using (IDataReader reader_BLOB = Report.GetReportBinaryData(ReportId))
				{
					if (reader_BLOB.Read())
					{
						bit_data = (byte[])reader_BLOB["ReportData"];
					}
				}
				XmlDocument doc = new XmlDocument();
				doc.InnerXml = System.Text.Encoding.UTF8.GetString(bit_data);
				
				IBNReportTemplate repTemplate = null;
				using (IDataReader reader = Report.GetReport(ReportId))
				{
					if (reader.Read())
					{
						_header.ReportCreated = (DateTime)reader["ReportCreated"];
						_header.ReportCreator = CommonHelper.GetUserStatusPureName((int)reader["ReportCreator"]);
						XmlDocument temp = new XmlDocument();
						temp.InnerXml = "<IBNReportTemplate>" + doc.SelectSingleNode("Report/IBNReportTemplate").InnerXml + "</IBNReportTemplate>";
						repTemplate = new IBNReportTemplate(temp);
						_header.Title = HttpUtility.HtmlDecode(repTemplate.Name);
					}
				}

				#region Filters

				QObject qItem = null;
				switch (repTemplate.ObjectName)
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
					case "Document":		//Documents
						qItem = new QDocument();
						break;
					case "Directory":	//Users
						qItem = new QDirectory();
						break;
					case "Task":		//Tasks
						qItem = new QTask();
						break;
					case "Portfolio":		//Portfolios
						qItem = new QPortfolio();
						break;
					default:
						break;
				}
				_header.Filter = MakeFilterText(repTemplate, qItem);
				#endregion

				_header.BtnPrintVisible = false;
				bool ShowEmpty = repTemplate.ShowEmptyGroup;
				int GroupCount = repTemplate.Groups.Count;
				string ViewType = repTemplate.ViewType;

				string sPath = "";
				if (GroupCount == 0)
					sPath = HttpRuntime.AppDomainAppPath + @"Reports\GroupWithout.xslt";
				else if (GroupCount == 1 && ShowEmpty)
				{
					if (ViewType == "0")
					{
						sPath = HttpRuntime.AppDomainAppPath + @"Reports\GroupOne.xslt";
					}
					else
					{
						sPath = HttpRuntime.AppDomainAppPath + @"Reports\GroupOneCollapse.xslt";
					}
				}
				else if (GroupCount == 1 && !ShowEmpty)
				{
					if (ViewType == "0")
					{
						sPath = HttpRuntime.AppDomainAppPath + @"Reports\GroupOneNoEmpty.xslt";
					}
					else
					{
						sPath = HttpRuntime.AppDomainAppPath + @"Reports\GroupOneNoEmptyCollapse.xslt";
					}
				}
				else if (GroupCount == 2 && ShowEmpty)
				{
					if (ViewType == "0")
					{
						sPath = HttpRuntime.AppDomainAppPath + @"Reports\GroupTwo.xslt";
					}
					else
					{
						sPath = HttpRuntime.AppDomainAppPath + @"Reports\GroupTwoCollapse.xslt";
					}
				}
				else if (GroupCount == 2 && !ShowEmpty)
				{
					if (ViewType == "0")
					{
						sPath = HttpRuntime.AppDomainAppPath + @"Reports\GroupTwoNoEmpty.xslt";
					}
					else
					{
						sPath = HttpRuntime.AppDomainAppPath + @"Reports\GroupTwoNoEmptyCollapse.xslt";
					}
				}
				XslCompiledTransform _transform = new XslCompiledTransform();
				XsltSettings set = new XsltSettings(true, true);

				_transform.Load(sPath, set, null);

				StringWriter sw = new StringWriter();
				XmlTextWriter w = new XmlTextWriter(sw);
				_transform.Transform(doc, w);
				w.Close();
				lblXML.Text = HttpUtility.HtmlDecode(sw.ToString());
			}

			if (Request["Export"] != null && Request["Export"] == "2")
			{
				ExportXML();
			}
			if (Request["Export"] != null && Request["Export"] == "1")
			{
				ExportExcel();
			}
			btnSave.Text = LocRM.GetString("tSave");
			btnSaveVis.Value = LocRM.GetString("tSaveAsTemplate");
			btnExcel.Text = LocRM.GetString("Export");
			btnXML.Text = LocRM.GetString("XMLExport");
			lgdSaveTemplate.InnerText = LocRM.GetString("tSaveTemplate");
			cbSaveResult.Text = LocRM.GetString("tSaveResult");
			cbOnlyForMe.Text = LocRM.GetString("tOnlyForMe");
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
			this.btnSave.Click += new EventHandler(btnSave_Click);
		}
		#endregion

		protected void btnExcel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Reports/XMLReportOutput.aspx?Export=1&ReportId=" + ReportId.ToString());
		}

		protected void btnXML_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("~/Reports/XMLReportOutput.aspx?Export=2&ReportId=" + ReportId.ToString());
		}

		private static string RestoreUniversalDate(string originalText)
		{
			//originalText = "sasa>01.02.1999</td>";
			CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;

			string currentCultureDateFormat = currentCulture.DateTimeFormat.ShortDatePattern;
			currentCultureDateFormat = Regex.Replace(currentCultureDateFormat, @"\w+", @"\d{0,4}");

			string pattern = string.Format(@">(?<date>{0})</td>", currentCultureDateFormat);

			string newText = Regex.Replace(originalText, pattern, RestoreUniversalDate);

			return newText;
		}

		private static string RestoreUniversalDate(Match match)
		{
			DateTime date; 
			if(DateTime.TryParse(match.Groups["date"].Value, out date))
			{
				return ">" + date.ToString("yyyy-MM-dd") + "</td>";
			}

			return match.Value;
		}


		private void ExportExcel()
		{
			HttpResponse Response = HttpContext.Current.Response;
			Response.Clear();
			Response.Charset = "utf-8";
			Response.AddHeader("Content-Type", "application/octet-stream");
			Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", "Custom_Report.xls"));
			string Header = "<html><head><meta http-equiv=\"Content-Type\" content=\"application/octet-stream; charset=utf-8\"></head><body>";
			Header += "<table width='100%'><tr><td><font color='#0000CC' size='4' face='Verdana, Arial, Helvetica, sans-serif'>" + _header.Title + "</font></td></tr></table>";
			string Footer = "</body></html>";

			string currentCultureDateFormat = System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern;
			currentCultureDateFormat = Regex.Replace(currentCultureDateFormat, @"\w", @"\d");

			string str = lblXML.Text;
			str = RestoreUniversalDate(str);

			string body = HttpUtility.HtmlDecode(str);
			while (body.IndexOf("<img src=\"../layouts/images/minus.gif\" border=\"0\" />") >= 0 ||
					body.IndexOf("<img src=\"../layouts/images/plus.gif\" border=\"0\" />") >= 0 ||
					body.IndexOf("<img src=\"../layouts/images/spacer.gif\" width=\"5px\" align=\"middle\" border=\"0\" />") >= 0)
			{
				int j = body.IndexOf("<img src=\"../layouts/images/minus.gif\" border=\"0\" />");
				if (j >= 0)
					body = body.Remove(j, "<img src=\"../layouts/images/minus.gif\" border=\"0\" />".Length);
				j = body.IndexOf("<img src=\"../layouts/images/plus.gif\" border=\"0\" />");
				if (j >= 0)
					body = body.Remove(j, "<img src=\"../layouts/images/plus.gif\" border=\"0\" />".Length);
				j = body.IndexOf("<img src=\"../layouts/images/spacer.gif\" width=\"5px\" align=\"middle\" border=\"0\" />");
				if (j >= 0)
					body = body.Remove(j, "<img src=\"../layouts/images/spacer.gif\" width=\"5px\" align=\"middle\" border=\"0\" />".Length);
			}
			int i = body.IndexOf("<?xml version=\"1.0\" encoding=\"utf-16\"?>");
			if (i >= 0)
				body = body.Remove(i, 39);
			Response.Write(String.Concat(Header, body, Footer));
			Response.End();
		}

		private void ExportXML()
		{
			byte[] bit_data = null;
			using (IDataReader reader = Report.GetReportBinaryData(ReportId))
			{
				if (reader.Read())
				{
					bit_data = (byte[])reader["ReportData"];
				}
			}
			XmlDocument doc = new XmlDocument();
			doc.InnerXml = System.Text.Encoding.UTF8.GetString(bit_data);
			HttpResponse Response = HttpContext.Current.Response;
			Response.Clear();
			Response.Charset = "utf-8";
			Response.AddHeader("Content-Type", "application/octet-stream");
			Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", "Custom_Report.xml"));
			Response.Write(doc.InnerXml);
			Response.End();
		}

		#region MakeFilterText
		protected string MakeFilterText(IBNReportTemplate repTemp, QObject qItem)
		{
			string retval = "";
			foreach (FilterInfo fi in repTemp.Filters)
			{
				QField qTemp = qItem.Fields[fi.FieldName];
				if (qTemp == null)
					continue;
				QDictionary qDTemp = qItem.GetDictionary(qTemp);
				if (qDTemp != null)
				{
					if (fi.Values.Count > 0)
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
				}
				else
				{
					switch (qTemp.DataType)
					{
						case DbType.Decimal:
						case DbType.Int32:
							if (fi.Values.Count > 0)
							{
								retval += qTemp.FriendlyName;
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
							break;
						case DbType.DateTime:
						case DbType.Date:
							if (fi.Values.Count > 0)
							{
								retval += qTemp.FriendlyName;
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
							break;
						case DbType.String:
							if (fi.Values.Count > 0)
								retval += qTemp.FriendlyName + "&nbsp;=&nbsp;<font color='red'>" + fi.Values[0] + "</font><br>";
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

		private void btnSave_Click(object sender, EventArgs e)
		{
			using (IDataReader reader = Report.GetReport(ReportId))
			{
				if (reader.Read())
				{
					int iTemplateId = (int)reader["TemplateId"];
					using (IDataReader rdr = Report.GetReportTemplate(iTemplateId))
					{
						if (rdr.Read())
						{
							string sName = (txtTemplateTitle.Text.Length == 0) ? rdr["Name"].ToString() : txtTemplateTitle.Text;
							string sXML = rdr["TemplateXML"].ToString();
							Report.UpdateReportTemplate(iTemplateId, sName, sXML, !cbOnlyForMe.Checked, false);
						}
					}
				}
			}
			if (!cbSaveResult.Checked)
				Report.DeleteReportItem(ReportId);
			if (!(Request["Refresh"] != null && Request["Refresh"] == "-1"))
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				  string.Format("try{{window.opener.top.frames['right'].location.href='{0}';}}catch (e){{;}} window.close();", this.ResolveUrl("~/Apps/ReportManagement/Pages/UserReport.aspx")), true);
		}
	}
}
